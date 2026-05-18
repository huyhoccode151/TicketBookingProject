import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { EventService } from '../services/event';
import { Router } from '@angular/router';
import { Event } from '../models/event';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Loader } from '../../../shared/ui/loader/loader';
import { RouteService } from '../../../core/services/route.service';
import { NgSelectModule } from '@ng-select/ng-select';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-index-cus',
  standalone: true,
  imports: [CommonModule, FormsModule, Loader, NgSelectModule, ConfirmDialog],
  templateUrl: './index-cus.html',
  styleUrls: ['./index-cus.scss'],
})
export class IndexCus {
  loading = true;
  events: Event[] = [];
  categoryOptions: { label: string; value: string }[] = [];
  venueOptions: { label: string; value: string }[] = [];
  categories: string[] = [];
  selectedCategories: string[] = [];
  currentEventId: number = 0;
  currentEventName: string = '';
  isOpenDialog = false;
  subscribeDialogConfig: ConfirmDialogConfig = { title: '', message: '' };
  isOpenUnSubscribeDialog = false;
  unSubscribeDialogConfig: ConfirmDialogConfig = { title: '', message: '' };

  searchTemp = '';
  page = 1;
  pageSize = 12; // 4-column grid × 3 rows
  totalCount = 0;
  totalPages = 1;
  datePreset = '';
  dateFrom: string = '';
  dateTo: string = '';
  onSale = false;

  filter = { venue: '', category: '', status: 'published' };

  datePresets = [
    { label: 'All', value: '' },
    { label: 'Today', value: 'Today' },
    { label: 'Tomorrow', value: 'Tomorrow' },
    { label: 'This Week', value: 'ThisWeek' },
    { label: 'This Weekend', value: 'ThisWeekend' },
    { label: 'Next Weekend', value: 'NextWeekend' },
    { label: 'This Month', value: 'ThisMonth' },
    { label: 'Next Month', value: 'NextMonth' },
    { label: 'This Year', value: 'ThisYear' },
    { label: 'Next Year', value: 'NextYear' },
  ];

  private router = inject(Router);
  private eventService = inject(EventService);
  private cdr = inject(ChangeDetectorRef);
  private toast = inject(ToastService);
  route = inject(RouteService);

  ngOnInit() {
    this.loadEvent();
    this.loadCategory();
    this.loadVenue();
  }

  loadEvent(append = false) {
    this.eventService
      .getEvent(
        this.page,
        this.pageSize,
        this.searchTemp,
        this.filter.venue,
        this.selectedCategories,
        this.filter.status,
        this.datePreset,
        this.onSale,
        this.dateFrom,
        this.dateTo,
      )
      .subscribe({
        next: (res) => {
          this.events = append
            ? [...this.events, ...res.data.items]
            : res.data.items;
          this.pageSize = res.data.pageSize;
          this.totalCount = res.data.totalCount;
          this.totalPages = res.data.totalPages;

          setTimeout(() => {
            this.loading = false;
            this.cdr.markForCheck();
          }, 500);
        },
        error: (err) => {
          this.toast.error('Delete User', 'Delete Failed!!!');
        }
      });
  }

  loadCategory() {
    this.eventService.getCategory().subscribe((res) => {
      this.categoryOptions = res.data.map((item: any) => ({
        label: item.name,
        value: item.name,
      }));
      this.categories = res.data.map((item) => item.name);
      this.cdr.markForCheck();
    });
  }

  loadVenue() {
    this.eventService.getVenue().subscribe((res) => {
      this.venueOptions = res.data.map((item: any) => ({
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

  onDatePreset(preset: string) {
    this.datePreset = preset;
    this.page = 1;
    this.loadEvent();
  }

  /** Append next page results without resetting the list */
  loadMore() {
    this.page++;
    this.loadEvent(true);
  }

  clearFilters() {
    this.searchTemp = '';
    this.filter = { venue: '', category: '', status: 'published' };
    this.datePreset = '';
    this.onSale = false;
    this.page = 1;
    this.loadEvent();
  }

  viewEvent(eventId: number) {
    this.router.navigate(this.route.customerEventShow(eventId));
  }

  getRemainingTickets(event: Event): number {
    return (event.ticketQuantity ?? 0) - (event.ticketSold ?? 0);
  }

  // ── Thumbnail helper ─────────────────────────────────────────────────────
  readonly baseUrl = 'http://localhost:5220';

  getThumbnail(url?: string): string {
    if (!url) return '';
    return url.startsWith('http') ? url : this.baseUrl + url;
  }

  onCategoryFilterChange(value: string[]) {
    this.selectedCategories = value ? value : [];
    this.page = 1;
    this.loadEvent();
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
    this.isOpenUnSubscribeDialog = false;
    this.isOpenDialog = false;
  }
}
