import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookingTicketListItemResponse, TicketListRequest } from '../models/ticket';
import { TicketService } from '../services/ticket-service';
import { Router } from '@angular/router';
import { EventService } from '../../event/services/event';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { DataTable } from '../../../shared/ui/data-table/data-table';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { CategoryListItemResponse, VenueListItemResponse } from '../../event/models/event';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { Loader } from '../../../shared/ui/loader/loader';

@Component({
  selector: 'app-user-booking',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PageHeader,
    DataTable,
    TableToolbar,
    FilterSelect,
    Pagination,
    Loader,
  ],
  templateUrl: './user-booking.html',
  styleUrls: ['./user-booking.scss'],
})
export class UserBooking {
  bookings: BookingTicketListItemResponse[] = [];
  loading: boolean = true;

  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;
  expandedBookingId: number | null = 9021;

  filters: TicketListRequest = {
    searchTemp: '',
    status: '',
    sortDesc: true,
    page: 1,
    pageSize: 10
  };

  statuses = [
    { label: 'Confirmed', value: 'confirmed' },
    { label: 'Pending', value: 'pending' },
    { label: 'Cancelled', value: 'cancelled' },
    { label: 'Expired', value: 'expired' },
    { label: 'Refunded', value: 'refunded' },
  ];

  selectedCategory = 'All Categories';
  selectedVenue = 'All Venues';
  selectedStatusValue: number | undefined = undefined;

  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private eventService = inject(EventService);
  private toast = inject(ToastService);
  private bookingService = inject(TicketService);

  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.bookingService.getTicketsByUserId(this.filters).subscribe({
      next: (res) => {
        this.bookings = res.data.items;
        this.pageSize = res.data.pageSize;
        this.totalCount = res.data.totalCount;
        this.totalPages = res.data.totalPages;
        setTimeout(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }, 1000);
      },
      error: () => {
        this.loading = false;
        this.toast.error("Loading Bookings failed!!!");
      }
    });
  }

  toggleExpand(id: number) {
    this.expandedBookingId = this.expandedBookingId === id ? null : id;
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
}
