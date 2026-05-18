import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { AuthUser, ChangePasswordRequest, UpdateUserProfile, User } from '../models/user';
import { UserService } from '../services/user';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { ActivatedRoute, Router, RouterModule, RouterOutlet } from '@angular/router';
import { TicketService } from '../../ticket/services/ticket-service';
import { BookingTicketListItemResponse, TicketListRequest, TicketSuccessListRequest } from '../../ticket/models/ticket';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { Loader } from '../../../shared/ui/loader/loader';
import { environment } from '../../../../environments/environments';
import { AuditLogService } from '../../auditlog/services/audit-log';
import { AuditLog } from '../../auditlog/models/audit-log';
import { RouteService } from '../../../core/services/route.service';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { ChangePassword } from '../change-password/change-password';
import { Card } from '../../../shared/ui/card/card';
import { disabled, email } from '@angular/forms/signals';
import { BookingService } from '../../booking/services/booking-service';
import { BookingTicketDetails } from '../../payment/models/booking_payment';
import { BookingPayments } from '../../payment/services/booking-payments';
import { interval, Subscription } from 'rxjs';
import { EventService } from '../../event/services/event';
import { Event } from '../../event/models/event';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterOutlet,
    CommonModule,
    FormsModule,
    PageHeader,
    DataTable,
    TableToolbar,
    FilterSelect,
    Pagination,
    Loader,
    RouterModule,
    ConfirmDialog,
    ChangePassword,
    ReactiveFormsModule,
    Card

  ],
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss'],
})
export class Profile implements OnInit {
  userAuth: User = {} as User;
  userId!: string;
  bookings: BookingTicketListItemResponse[] = [];
  auditLogs: AuditLog[] = [];
  expandedBookingId: number | null = 9021;
  activeTab: 'home' | 'about' | 'favorite' | 'orders' = 'home';
  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;
  loading: boolean = true;
  isLoading: boolean = true;
  paymentMethod: string = 'vnpay';
  countdownDisplay: string = '--:--';
  isExpired: boolean = false;
  events: Event[] = [];
  currentEventId: number = 0;
  currentEventName: string = '';
  subscribeDialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  isOpenDialog: boolean = false;

  form!: FormGroup;
  submitted = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  isDialogOpen: boolean = false;

  profileForm!: FormGroup;
  isProfileDialogOpen: boolean = false;
  profileDialogConfig: ConfirmDialogConfig = { title: 'Edit Profile', message: 'Profile editing is not implemented yet.' };
  isProfileSubmitting: boolean = false;

  bookingInfo: BookingTicketDetails | null = null;
  unSubscribeDialogConfig: ConfirmDialogConfig = { title: '', message: '' }
  isOpenUnSubscribeDialog: boolean = false;

  filters: TicketSuccessListRequest = {
    searchTemp: '',
    sortDesc: true,
    page: 1,
    pageSize: 10
  };

  route = inject(RouteService);
  activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  private userService = inject(UserService);
  private ticketService = inject(TicketService);
  private bookingService = inject(BookingPayments);
  private auditService = inject(AuditLogService);
  private eventService = inject(EventService);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);
  private timerSub: Subscription | null = null;

  ngOnInit() {
    this.form = this.fb.group({
      password: ['', Validators.required],
      newpassword: ['', Validators.required],
      confirmpassword: ['', Validators.required],
    });

    this.profileForm = this.fb.group({
      first_name: ['', Validators.required],
      last_name: ['', Validators.required],
      username: [{ value: '', disabled: true }, Validators.required],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      gender: ['']
    });
    this.loadUserAuth();
    this.loadFavEvent();
    this.loadUserBookings();
    this.loadBookingPending();
    this.getMyLogs();
  }

  loadUserAuth() {
    this.userService.getUserAuth().subscribe({
      next: (res) => {
        this.userId = res.userId;
        console.log(this.userId);
        this.loadUserProfile()
      },
      error: (err) => {
        this.toast.error('Failed to load user profile');
      }
    });
  }

  loadFavEvent() {
    this.eventService.getFavEvent().subscribe({
      next: (res) => {
        this.events = res.data;
      },
      error: (err) => {
        if (err != null) this.toast.error("Load Fav Event", err?.errors?.error);
      }
    });
  }

  viewEvent(eventId: number) {
    this.router.navigate(this.route.customerEventShow(eventId));
  }

  loadUserProfile() {
    this.userService.getUserById(this.userId).subscribe({
      next: (res) => {
        this.userAuth = res.data;
        
        console.log(res);
      },
      error: (err) => {
        this.toast.error('Failed to load user profile');
      }
    });
  }

  loadUserBookings() {
    this.ticketService.getUpcomingTicketsByUserId().subscribe({
      next: (res) => {
        console.log(res);
        this.bookings = res.data;
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 1000);
      },
      error: (err) => {
        this.toast.error('Failed to load user bookings');
      }
    });
  }

  loadBookingPending(): void {
    this.bookingService.getMyPendingBooking().subscribe({
      next: (res) => {
        if (res.success && res.data && res.data.id !== 0 && res.data.expiresAt) {
          this.bookingInfo = res.data;
          this.startCountdown(res.data.expiresAt);
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.toast.error('Failed to load booking: ' + (err.error?.message ?? err.message ?? 'Unknown error'));
        this.isLoading = false;
      }
    });
  }

  //startCountdown(expiresAt: string): void {
  //  this.timerSub?.unsubscribe();

  //  this.timerSub = interval(1000).subscribe(() => {
  //    const now = new Date().getTime();
  //    const expiry = new Date(expiresAt).getTime();
  //    const diff = expiry - now;

  //    if (diff <= 0) {
  //      this.countdownDisplay = '00:00';
  //      this.isExpired = true;
  //      this.timerSub?.unsubscribe();
  //      return;
  //    }

  //    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
  //    const seconds = Math.floor((diff % (1000 * 60)) / 1000);
  //    this.countdownDisplay = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  //  });
  //}
  startCountdown(expiresAt: string): void {
    this.timerSub?.unsubscribe();

    // Thêm 'Z' để báo cho browser biết đây là UTC
    const expiryTime = new Date(
      expiresAt.endsWith('Z') ? expiresAt : expiresAt + 'Z'
    ).getTime();

    // Kiểm tra ngay lập tức trước khi start interval
    const now = new Date().getTime();
    if (expiryTime - now <= 0) {
      this.countdownDisplay = '00:00';
      this.isExpired = true;
      return;
    }

    this.timerSub = interval(1000).subscribe(() => {
      const diff = expiryTime - new Date().getTime();

      if (diff <= 0) {
        this.countdownDisplay = '00:00';
        this.isExpired = true;
        this.timerSub?.unsubscribe();
        return;
      }

      const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((diff % (1000 * 60)) / 1000);
      this.countdownDisplay = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
      this.cdr.markForCheck();
    });
  }

  getRemainingTickets(event: Event): number {
    return (event.ticketQuantity ?? 0) - (event.ticketSold ?? 0);
  }

  confirmPayNow(): void {
    if (!this.bookingInfo) return;

    const request = {
      amount: this.bookingInfo.totalAmount,          
      bookingId: this.bookingInfo.id.toString(),      
      bookingInfo: `${this.bookingInfo.userId}-${this.bookingInfo.eventId}-${this.bookingInfo.id}`,
    };

    if (this.paymentMethod === 'momo') {
      this.bookingService.paymentByMomo(request).subscribe({
        next: (res) => {
          const data = res.data;
          if (data.payUrl) window.location.href = data.payUrl;
        },
        error: (err) => {
          this.toast.error('Failed to create payment: ' + (err.error?.message ?? err.message ?? 'Unknown error'));
        }
      });
    }

    if (this.paymentMethod === 'vnpay') {
      this.bookingService.paymentByVnPay(request).subscribe({
        next: (res) => {
          const data = res.data;
          if (data.payUrl) window.location.href = data.payUrl;
        },
        error: (err) => {
          this.toast.error('Failed to create payment: ' + (err.error?.message ?? err.message ?? 'Unknown error'));
        }
      });
    }
  }

  countTicket(booking: BookingTicketListItemResponse) {
    return booking.tickets.length;
  }

  toggleExpand(id: number) {
    this.expandedBookingId = this.expandedBookingId === id ? null : id;
  }

  onSearch(keyword: string) {
    this.filters.searchTemp = keyword;
    this.filters.page = 1;
    this.loadUserBookings();
  }

  onPageChange(p: number) {
    this.filters.page = p;
    this.loadUserBookings();
  }

  baseUrl = environment.api;

  getThumbnail(url?: string): string {
    if (!url) return '';

    // Nếu là link ngoài (http, https)
    if (url.startsWith('http')) {
      return url;
    }

    // Nếu là link local (/uploads/...)
    return this.baseUrl + url;
  }

  getMyLogs() {
    this.auditService.getMyLogs().subscribe({
      next: (res) => {
        this.auditLogs = res.data;
        console.log(res);
      },
      error: (err) => {
        if (err.status === 403) {
          this.toast.error('You do not have permission to view audit logs');
        }
      }
    });
  }

  setTab(tab: 'home' | 'about' | 'favorite' | 'orders') {
    this.activeTab = tab;
  }

  openDialogSubscribe(id: number, EventName: string) {
    this.currentEventId = id;
    this.currentEventName = EventName;
    this.subscribeDialogConfig = {
      title: 'Subscribe Event',
      message: 'Are you sure you want to subscribe this event? You can receive noti of this event when it has any changes!!!',
      detail: `Event: ${EventName}`,
      confirmText: 'Subscribe',
      cancelText: 'Cancel',
      variant: 'info',
    }
    this.isOpenDialog = true;
  }

  subscribeNotiFromEvent() {
    this.isOpenDialog = false;
    this.eventService.subscribeEvent(this.currentEventId).subscribe({
      next: () => {
        this.toast.success("You just subscribed " + this.currentEventName);
      },
      error: (err) => {
        this.toast.error("You can't subscribe this event");
      }
    });
  }

  openDialogUnSubscribe(id: number, EventName: string) {
    this.currentEventId = id;
    this.currentEventName = EventName;
    this.unSubscribeDialogConfig = {
      title: 'UnSubscribe Event',
      message: 'Are you sure you want to unsubscribe this event? You can not receive noti of this event when it has any changes!!!',
      detail: `Event: ${EventName}`,
      confirmText: 'UnSubscribe',
      cancelText: 'Cancel',
      variant: 'warning',
    }
    this.isOpenUnSubscribeDialog = true;
  }

  unSubscribeNotiFromEvent() {
    this.isOpenUnSubscribeDialog = false;
    this.eventService.unsubscribeEvent(this.currentEventId).subscribe({
      next: () => {
        this.toast.success("You just unsubscribed " + this.currentEventName);
      },
      error: (err) => {
        this.toast.error("You can't unsubscribe this event");
      }
    });
  }

  onCancelled(): void {
    this.isOpenDialog = false;
    this.isOpenUnSubscribeDialog = false;
  }

  openChangeProfileDialog(): void {
    this.profileDialogConfig = {
      title: 'Change Profile',
      message: 'Are you surely change the profile?',
      detail: `Account: `,
      confirmText: 'Change',
      cancelText: 'Cancel',
      variant: 'info',
    }

    this.profileForm.patchValue({
      first_name: this.userAuth?.firstname ?? '',
      last_name: this.userAuth?.lastname ?? '',
      username: this.userAuth?.username ?? '',
      email: this.userAuth?.email ?? '',
      gender: this.userAuth?.gender ?? ''
    });

    this.isProfileDialogOpen = true;
  }

  onProfileUpdateCancelled(): void {
    this.isProfileDialogOpen = false;
  }

  isProfileInvalid(controlName: string) {
    return this.isProfileSubmitting && this.profileForm.get(controlName)?.invalid;
  }

  onProfileUpdateConfirmed() {
    this.isProfileSubmitting = true;
    const v = this.profileForm.value;

    const request: UpdateUserProfile = {
      firstname: v.first_name,
      lastname: v.last_name,
      gender: v.gender
    }

    console.log(request);

    this.userService.updateProfile(request).subscribe({
      next: (res) => {
        this.profileForm.reset();
        this.router.navigate(this.route.profile());
        this.loadUserProfile();
        this.isProfileDialogOpen = false;
        this.toast.success('Profile changed successfully.');
      },
      error: (err) => {
        console.log(err);
        if (err.status === 400 && err.error.errors) {
          const serverError = err.error?.errors;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.profileForm.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
          }

          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.toast.error('Change profile failed!!!');
        }

        this.toast.error('Failed to change profile');
      }
    });
  }

  openChangePasswordDialog(): void {
    this.dialogConfig = {
      title: 'Change Password',
      message: 'Are you surely change the password?',
      detail: `Account: `,
      confirmText: 'Change',
      cancelText: 'Cancel',
      variant: 'danger',
    }
    this.isDialogOpen = true;
  }

  onPasswordChangeCancelled(): void {
    this.isDialogOpen = false;
  }

  isInvalid(controlName: string) {
    return this.submitted && this.form.get(controlName)?.invalid;
  }

  viewTicket(ticketId: number) {
    return this.router.navigate(this.route.ticket(ticketId));
  }

  onPasswordChangeConfirmed() {
    this.submitted = true;

    if (this.form.invalid) return;

    const v = this.form.value;

    const request: ChangePasswordRequest = {
      currentPassword: v.password,
      newPassword: v.newpassword,
      confirmPassword: v.confirmpassword
    };

    this.userService.changePassword(request).subscribe({
      next: () => {
        this.form.reset();
        this.router.navigate(this.route.profile());
        this.isDialogOpen = false;
        this.toast.success('Password changed successfully. Password will change in your next login');
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors) {
          const serverError = err.error?.errors;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.form.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
          }

          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.toast.error('Change password failed!!!');
        }

        this.toast.error('Failed to change password');
      }
    });
  }
}
