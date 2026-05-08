import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { EventService } from '../services/event';
import { Router } from '@angular/router';
import { Event } from '../models/event';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Loader } from '../../../shared/ui/loader/loader';
import { RouteService } from '../../../core/services/route.service';
import { NgSelectModule } from '@ng-select/ng-select';

@Component({
  selector: 'app-index-cus',
  standalone: true,
  imports: [CommonModule, FormsModule, Loader, NgSelectModule],
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

  searchTemp = '';
  page = 1;
  pageSize = 12; // 4-column grid × 3 rows
  totalCount = 0;
  totalPages = 1;
  datePreset = '';
  dateFrom: Date | null = null;
  dateTo: Date | null = null;
  onSale = false;

  filter = { venue: '', category: '', status: 'published' };

  datePresets = [
    { label: 'All', value: '' },
    { label: 'Today', value: 'today' },
    { label: 'Tomorrow', value: 'tomorrow' },
    { label: 'This Week', value: 'this_week' },
    { label: 'This Weekend', value: 'this_weekend' },
    { label: 'Next Weekend', value: 'next_weekend' },
    { label: 'This Month', value: 'this_month' },
    { label: 'Next Month', value: 'next_month' },
    { label: 'This Year', value: 'this_year' },
    { label: 'Next Year', value: 'next_year' },
  ];

  private router = inject(Router);
  private eventService = inject(EventService);
  private cdr = inject(ChangeDetectorRef);
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
      .subscribe((res) => {
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
}
