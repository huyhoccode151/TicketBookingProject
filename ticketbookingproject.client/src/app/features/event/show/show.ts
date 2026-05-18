import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventDetailResponse, EventPoster, RelatedEvents } from '../models/event';
import { EventService } from '../services/event';
import { environment } from '../../../../environments/environments';
import { Loader } from '../../../shared/ui/loader/loader';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { RouteService } from '../../../core/services/route.service';
import { FilterSelect } from '../../../shared/ui/filter-select/filter-select';

@Component({
  selector: 'app-show',
  standalone: true,
  imports: [CommonModule, Loader, FilterSelect],
  templateUrl: './show.html',
  styleUrls: ['./show.scss'],
})
export class Show {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(ToastService);
  eventService = inject(EventService);
  private cdr = inject(ChangeDetectorRef);
  routeNavigate = inject(RouteService);

  baseUrl = environment.api;

  eventId!: number;
  event!: EventDetailResponse;
  relatedEvent!: RelatedEvents[];
  images: EventPoster[] = [];
  venue: string = '';
  name: string = '';
  username: string = '';
  maxTicketsPerBooking: number = 1; 
  description: string = '';
  activeAt: Date = new Date();
  endAt: Date = new Date();
  saleStartAt: Date = new Date();
  saleEndAt: Date = new Date();
  numTake: string = '4';
  relatedLoading: boolean = true;

  bookingEnd: Date = new Date();

  currentIndex = 0;
  ticketCount = 1;
  ticketPrice = 120;

  private timer: any;
  days = '00';
  hours = '00';
  minutes = '00';
  seconds = '00';
  loading: boolean = true;

  relatedOptions = [
    { label: '4', value: '4' },
    { label: '5', value: '5' },
    { label: '6', value: '6' },
    { label: '7', value: '7' },
    { label: '8', value: '8' },
  ];

  ngOnInit() {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));

    this.loadEventDetails();
    this.loadRelatedEvent();
  }

  ngAfterViewInit() {
    this._startCountdown();
  }

  ngOnDestroy() {
    if (this.timer) clearInterval(this.timer);
  }

  loadRelatedEvent() {
    this.eventService.getRelatedEvents(this.eventId, this.numTake).subscribe({
      next: (res) => {
        this.relatedEvent = res.data;
        this.relatedLoading = false;
      },
      error: (err) => {
        if (err)
          this.toast.error(err?.error?.errors ?? "Related Event retrived failed!!!");
      }
    });
  }

  loadEventDetails() {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (res) => {
        const data = res.data;
        console.log(data);

        this.name = data.name;
        this.description = data.description;
        this.venue = data.venue?.name ?? '';
        this.username = data.organizer?.lastname ?? '';
        this.maxTicketsPerBooking = data.maxTicketsPerBooking;
        this.activeAt = new Date(data.activeAt);
        this.endAt = new Date(data.endAt);
        this.saleStartAt = new Date(data.saleStartAt);
        this.saleEndAt = new Date(data.saleEndAt);

        this.images = data.posters?.length
          ? data.posters.map((p: any) => ({
          id: p.id,
          imageUrl: p.imageUrl,
          imageType: p.imageType,
          isPrimary: p.isPrimary,
          })) : [{ imageUrl: 'https://picsum.photos/1200/630/?image=375' } as any];

        if (this.timer) {
          clearInterval(this.timer);
        }
        this._startCountdown();

        setTimeout(() => {
          this.loading = false,
            this.cdr.detectChanges();
        }, 1000);
      },
      error: () => {
        this.toast.error("Load Event Failed!!!");
        //this.router.navigate(['forbidden']);
      }
    });
  }

  getThumbnail(url?: string): string {
    if (!url) return '';

    // Nếu là link ngoài (http, https)
    if (url.startsWith('http')) {
      return url;
    }

    // Nếu là link local (/uploads/...)
    return this.baseUrl + url;
  }

  private _startCountdown(): void {
    const pad = (n: number) => String(n).padStart(2, '0');
    //const target = new Date('2026-04-24T19:00:00');
    const target = this.saleEndAt ?? new Date();
    console.log('SALE_START_AT', target);  

    const tick = () => {
      const diff = Math.max(0, target.getTime() - Date.now());
      this.days = pad(Math.floor(diff / 86400000));
      this.hours = pad(Math.floor((diff % 86400000) / 3600000));
      this.minutes = pad(Math.floor((diff % 3600000) / 60000));
      this.seconds = pad(Math.floor((diff % 60000) / 1000));
      this.cdr.markForCheck();
    };


    tick();
    console.log('TICK', this.minutes);

    this.timer = setInterval(tick, 1000);
  }

  changeImage(step: number) {
    this.currentIndex = (this.currentIndex + step + this.images.length) % this.images.length;
  }

  setIndex(index: number) {
    this.currentIndex = index;
  }

  updateTicket(amount: number) {
    this.ticketCount = Math.max(1, this.ticketCount + amount);
  }

  showRelatedEvent(id: number) {
    this.router.navigate(this.routeNavigate.customerEventShow(id));
    console.log("iii");
  }

  book() {
    this.router.navigate(this.routeNavigate.customerEventBooking(this.eventId));
  }

  get totalAmount() {
    return this.ticketCount * this.ticketPrice;
  }
}
