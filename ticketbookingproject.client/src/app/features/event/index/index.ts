import { Component, inject } from '@angular/core';
import { Pagination } from '../../../shared/ui/pagination/pagination';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { StatCard } from '../../../shared/ui/stat-card/stat-card';
import { CommonModule } from '@angular/common';
import { TableToolbar } from '../../../shared/ui/table-toolbar/table-toolbar';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';
import { EventService } from '../services/event';
import { Event } from '../models/event';
import { ChangeDetectorRef } from '@angular/core';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Router, RouterModule } from '@angular/router';
import { Loader } from '../../../shared/ui/loader/loader';
import { HasPermissionDirective } from '../../../shared/directives/has-permission-directive';
import { RouteService } from '../../../core/services/route.service';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [
    Pagination,
    PageHeader,
    StatCard,
    TableToolbar,
    TableActions,
    FilterSelect,
    CommonModule,
    Loader,
    HasPermissionDirective,
    ConfirmDialog,
    RouterModule,
    FormsModule,
    NgSelectModule,

  ],
  templateUrl: './index.html',
  styleUrls: ['./index.scss'],
})
export class Index {
  loading: boolean = true;
  events: Event[] = [];
  categoryOptions: any[] = [];
  venueOptions: any[] = [];
  searchTemp = '';
  cateSearch = '';
  venueSearch = '';
  page = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;
  datePreset = '';
  dateFrom: string = '';
  dateTo: string = '';
  filter = {
    venue: '',
    category: '',
    status: '',
  }
  onSale: boolean = false;
  eventId!: number;
  isDialogOpen = false;
  isDraftDialogOpen = false;
  isConfirmDialogOpen = false;
  isCancelDialogOpen = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  dialogDraftConfig: ConfirmDialogConfig = { title: '', message: '' }
  dialogConfirmConfig: ConfirmDialogConfig = { title: '', message: '' }
  dialogCancelConfig: ConfirmDialogConfig = { title: '', message: '' }
  selectedCategories: string[] = [];
  categories: string[] = [];

  statusOptions = [
    { label: 'Draft', value: 'draft' },
    { label: 'Publish', value: 'published' },
    { label: 'On Going', value: 'ongoing' },
    { label: 'Completed', value: 'completed' },
    { label: 'Cancelled', value: 'cancelled' },
  ]

  private toast = inject(ToastService);
  private router = inject(Router);
  route = inject(RouteService);
  constructor(private eventService: EventService,
    private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadCategory();
    this.loadVenue();
    this.loadEvent();
  }

  loadEvent() {
    this.eventService.getEvent(this.page, this.pageSize, this.searchTemp, this.filter.venue, this.selectedCategories, this.filter.status, this.datePreset, this.onSale, this.dateFrom, this.dateTo)
      .subscribe((res) => {
        this.events = res.data.items;
        this.pageSize = res.data.pageSize;
        this.totalCount = res.data.totalCount;
        this.totalPages = res.data.totalPages;

        setTimeout(() => {
          this.loading = false;
          this.cdr.markForCheck();
        }, 1000);
      });
  }

  loadCategory() {
    this.eventService.getCategory().subscribe(
      (res) => {
        this.categoryOptions = res.data.map((item) => ({
          label: item.name,
          value: item.name,
        }));
        this.categories = res.data.map((item) => item.name);
        this.cdr.markForCheck();
      }
    );
  }

  loadVenue() {
    this.eventService.getVenue().subscribe((res) => {
      this.venueOptions = res.data.map((item) => ({
        label: item.name,
        value: item.name,
      }));
      this.cdr.markForCheck();
    });
  }

  onSearch(keyword: string) {
    this.searchTemp = keyword;
    this.page = 1;
    this.loadEvent();
  }

  onPageChange(p: number) {
    this.page = p;
    this.loadEvent();
  }

  editEvent(eventId: number) {
    this.router.navigate(this.route.eventsEdit(eventId));
  }

  deleteEvent(eventId: number) {
    this.eventId = eventId;
    this.dialogConfig = {
      title: 'Delete Event',
      message: 'Are you sure you want to delete this event?',
      detail: `Event ID: ${eventId}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    }
    this.isDialogOpen = true;
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.eventService.deleteEvent(this.eventId).subscribe({
      next: (res) => {
        this.toast.success('Delete User', 'Deleted User Successfully!!!');
        this.page = 1;
        this.loadEvent();
      },
      error: (err) => {
        console.log('FULL_ERRORS', err);
        console.log('VALIDATION:', err.error?.errors);
        this.toast.error('Delete User', 'Delete Failed!!!');
      }
    });
  }

  onCancelled(): void {
    this.isDialogOpen = false;
    this.isDraftDialogOpen = false;
    this.isConfirmDialogOpen = false;
    this.isCancelDialogOpen = false;
  }

  viewEvent(eventId: number) {
    this.eventService.getEventById(eventId).subscribe({
      next: (res) => {
        this.router.navigate(this.route.bookings(), { queryParams: { eventName: res.data.name } });
      }
    });
  }

  //
  baseUrl = 'http://localhost:5220';

  getThumbnail(url?: string): string {
    if (!url) return '';

    // Nếu là link ngoài (http, https)
    if (url.startsWith('http')) {
      return url;
    }

    // Nếu là link local (/uploads/...)
    return this.baseUrl + url;
  }

  openDraftEvent(eventId: number) {
    this.eventId = eventId;
    this.dialogDraftConfig = {
      title: 'Draft Event',
      message: 'Are you sure you want to draft this event?',
      detail: `Event ID: ${eventId}`,
      confirmText: 'Draft',
      cancelText: 'Cancel',
      variant: 'info',
    }
    this.isDraftDialogOpen = true;
  }

  draftEvent() {
    this.isDraftDialogOpen = false;
    this.eventService.draftEvent(this.eventId).subscribe({
      next: (res) => {
        this.toast.success('Draft Event', 'Event drafted successfully!');
        this.loadEvent();
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
          this.toast.error('Validation failed. Please check your input.', err.error?.errors || err.error?.message);
        } else {
          this.toast.error('Draft Event', 'Failed to draft event.');
        }
      }
    });
  }

  openConfirmEvent(eventId: number) {
    this.eventId = eventId;
    this.dialogConfirmConfig = {
      title: 'Confirm Event',
      message: 'Are you sure you want to confirm this event?',
      detail: `Event ID: ${eventId}`,
      confirmText: 'Confirm',
      cancelText: 'Cancel',
      variant: 'success',
    }
    this.isConfirmDialogOpen = true;
  }

  confirmEvent() {
    this.isConfirmDialogOpen = false;
    this.eventService.confirmEvent(this.eventId).subscribe({
      next: (res) => {
        this.toast.success('Confirm Event', 'Event confirmed successfully!');
        this.loadEvent();
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
          this.toast.error('Validation failed. Please check your input.', err.error?.errors || err.error?.message);
        } else {
          this.toast.error('Confirm Event', 'Failed to confirm event.');
        }
      }
    });
  }

  openCancelEvent(eventId: number) {
    this.eventId = eventId;
    this.dialogCancelConfig = {
      title: 'Cancel Event',
      message: 'Are you sure you want to cancel this event?',
      detail: `Event ID: ${eventId}`,
      confirmText: 'Cancel',
      cancelText: 'Cancel',
      variant: 'warning',
    }
    this.isCancelDialogOpen = true;
  }

  cancelEvent() {
    this.isCancelDialogOpen = false;
    this.eventService.cancelEvent(this.eventId).subscribe({
      next: (res) => {
        this.toast.success('Cancel Event', 'Event cancelled successfully!');
        this.loadEvent();
      },
      error: (err) => {
        if (err.status === 400 && err.error?.errors || err.status === 400 && err.error?.message) {
          console.log(err.error?.errors, 'dfsaf');
          this.toast.error('Validation failed. Please check your input.', err.error?.errors || err.error?.message);
        } else {
          this.toast.error('Cancel Event', 'Failed to cancel event.');
        }
      }
    });
  }

  onCategoryFilterChange(value: string[]) {
    this.selectedCategories = value ? value : [];
    this.page = 1;
    this.loadEvent();
  }

  onDateChange(): void {
    if (!this.dateFrom && !this.dateTo) {
      this.loadEvent();
      return;
    }
    if (this.dateFrom && this.dateTo) {
      this.loadEvent();
    }
  }
}
