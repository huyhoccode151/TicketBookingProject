import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { BookingService } from '../services/booking-service';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { AdminBookingListItemResponse, AdminBookingListRequest } from '../models/booking';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { Loader } from '../../../shared/ui/loader/loader';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { EventService } from '../../event/services/event';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [CommonModule,
    FormsModule,
    PageHeader,
    DataTable,
    TableToolbar,
    FilterSelect,
    Pagination,
    Loader,
    TableActions,
    ConfirmDialog],
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class Index {
  loading: boolean = true;
  isDialogOpen: boolean = false;
  bookings: AdminBookingListItemResponse[] = [];

  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  deleteBookingId!: number;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  eventName: string = '';
  status: string ='';
  dateFrom: string ='';
  dateTo: string ='';
  sortDesc: boolean = true;

  filters: AdminBookingListRequest = {
    searchTemp: '',
    eventName: '',
    status: '',
    dateFrom: '',
    dateTo: '',
    sortDesc: true,
    page: 1,
    pageSize: 10
  };

  eventOptions: { label: string, value: string }[] = [];

  statusOptions = [
    { label: 'Pending', value: 'pending' },
    { label: 'Confirmed', value: 'confirmed' },
    { label: 'Cancelled', value: 'cancelled' },
    { label: 'Expired', value: 'expired' }
  ];

  private toast = inject(ToastService);
  private bookingService = inject(BookingService);
  private eventService = inject(EventService);
  private cdr = inject(ChangeDetectorRef);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  ngOnInit() {
    this.loadEventNames();
    this.route.queryParams.subscribe(params => {
      if (params['eventName']) {
        this.eventName = params['eventName'];
        this.filters.eventName = params['eventName'];
        this.cdr.detectChanges();
      }
      this.loadBookings();
    });
  }

  loadEventNames() {
    this.eventService.getEventNames().subscribe({
      next: (res) => {
        this.eventOptions = res.data.map(name => ({ label: name, value: name }));
      },
      error: (err) => {
        if (err?.error?.errors) {
          this.toast.error("Loading Event Names failed!!!", err.error.errors);
        } else {
          this.toast.error("Loading Event Names failed!!!", "Unknown error");
        }
      }
    });
  }

  loadBookings() {
    //this.loading = true;
    this.filters.status = this.status;
    this.filters.dateFrom = this.dateFrom;
    this.filters.dateTo = this.dateTo;
    this.filters.eventName = this.eventName;

    this.bookingService.getListBooking(this.filters).subscribe({
      next: (res) => {
        if (res.success) {
          this.bookings = res.data.items;
          this.pageSize = res.data.pageSize;
          this.totalCount = res.data.totalCount;
          this.totalPages = res.data.totalPages;
        }
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 500);
      },
      error: (err) => {
        this.loading = false;
        this.toast.error("Loading Bookings failed!!!", err?.error?.errors || 'Unknown error');
      }
    });
  }

  onSearch(keyword: string) {
    this.filters.searchTemp = keyword;
    this.filters.page = 1;
    this.loadBookings();
  }

  onPageChange(p: number) {
    this.filters.page = p;
    this.loadBookings();
  }

  onDateChange(): void {
    if (!this.filters.dateFrom && !this.filters.dateTo) {
      this.loadBookings();
      return;
    }
    if (this.filters.dateFrom && this.filters.dateTo) {
      this.loadBookings();
    }
  }

  viewBooking(id: number) {
    //this.router.navigate(['/admin/bookings', id]);
  }

  deleteBooking(id: number) {
    this.deleteBookingId = id;
    this.dialogConfig = {
      title: 'Delete Booking',
      message: 'Are you sure you want to permanently delete this booking?',
      detail: `Booking ID: ${id}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    };
    this.isDialogOpen = true;
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.bookingService.deleteBooking(this.deleteBookingId).subscribe({
      next: (res) => {
        if (res.success) {
          this.toast.success('Delete Booking', 'Deleted Booking Successfully!!!');
          this.filters.page = 1;
          this.loadBookings();
        }
      },
      error: (err) => {
        console.error('Delete error:', err);
        this.toast.error('Delete Booking', 'Delete Failed!!!');
      }
    });
  }

  onCancelled(): void {
    this.isDialogOpen = false;
  }

  getStatusColor(status: number): string {
    switch (status) {
      case 1: return 'text-emerald-600'; // Confirmed
      case 2: return 'text-red-600';     // Cancelled
      case 3: return 'text-blue-600';    // Completed
      default: return 'text-amber-600';  // Pending (0)
    }
  }

  getStatusDotClass(status: number): string {
    switch (status) {
      case 1: return 'bg-emerald-500';
      case 2: return 'bg-red-500';
      case 3: return 'bg-blue-500';
      default: return 'bg-amber-500';
    }
  }

}
