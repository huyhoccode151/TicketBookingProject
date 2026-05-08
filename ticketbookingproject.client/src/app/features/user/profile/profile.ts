import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { AuthUser, ChangePasswordRequest, UpdateUserProfile, User } from '../models/user';
import { UserService } from '../services/user';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
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
  activeTab: 'home' | 'about' | 'settings' | 'orders' = 'home';
  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;
  loading: boolean = true;

  form!: FormGroup;
  submitted = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  isDialogOpen: boolean = false;

  profileForm!: FormGroup;
  isProfileDialogOpen: boolean = false;
  profileDialogConfig: ConfirmDialogConfig = { title: 'Edit Profile', message: 'Profile editing is not implemented yet.' };
  isProfileSubmitting: boolean = false;

  filters: TicketSuccessListRequest = {
    searchTemp: '',
    sortDesc: true,
    page: 1,
    pageSize: 10
  };

  route = inject(RouteService);
  private router = inject(Router);
  private userService = inject(UserService);
  private ticketService = inject(TicketService);
  private auditService = inject(AuditLogService);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);

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
    this.loadUserBookings();
    this.getMyLogs();
  }

  loadUserAuth() {
    this.userService.getUserAuth().subscribe({
      next: (res) => {
        this.userId = res.userId;
        console.log(this.userId);
        this.toast.success('User profile loaded successfully');
        this.loadUserProfile()
      },
      error: (err) => {
        this.toast.error('Failed to load user profile');
      }
    });
  }

  loadUserProfile() {
    this.userService.getUserById(this.userId).subscribe({
      next: (res) => {
        this.userAuth = res.data;
        
        console.log(res);
        this.toast.success('User profile loaded successfully');
      },
      error: (err) => {
        this.toast.error('Failed to load user profile');
      }
    });
  }

  loadUserBookings() {
    this.ticketService.getTicketsSuccessByUserId(this.filters).subscribe({
      next: (res) => {
        console.log(res);
        this.bookings = res.data.items;
        this.pageSize = res.data.pageSize;
        this.totalCount = res.data.totalCount;
        this.totalPages = res.data.totalPages;
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 1000);
        this.toast.success('User bookings loaded successfully');
      },
      error: (err) => {
        this.toast.error('Failed to load user bookings');
      }
    });
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
        this.toast.success('User audit logs loaded successfully');
      },
      error: (err) => {
        if (err.status === 403) {
          this.toast.error('You do not have permission to view audit logs');
        }
      }
    });
  }

  setTab(tab: 'home' | 'about' | 'settings' | 'orders') {
    this.activeTab = tab;
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
