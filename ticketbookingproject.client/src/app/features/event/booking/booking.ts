import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CreateBooking, EventPoster, TicketType, TicketVM } from '../models/event';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../services/event';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { environment } from '../../../../environments/environments';
import { catchError, forkJoin, of, retry } from 'rxjs';


@Component({
  selector: 'app-booking',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './booking.html',
  styleUrls: ['./booking.scss'],
})
export class Booking implements OnInit {
  eventId!: number;
  images: EventPoster[] = [{ imageUrl: 'https://picsum.photos/1200/630/?image=375' } as any];
  tickets: TicketVM[] = [];
  selectedBookings: CreateBooking[] = [];
  baseUrl = environment.api;
  isSubmitting = false;

  serviceFeeRate = 0.02;
  currentIndex = 0;

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(EventService);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadEventDetails();
  }

  loadEventDetails() {

    forkJoin({
      posters: this.eventService.getEventPoster(this.eventId).pipe(catchError(() => { this.toast.error('Load events image failed!!!'); return of(null); })),
      tickettypes: this.eventService.getTicketTypes(this.eventId).pipe(catchError(() => { this.toast.error('Load ticket types failed!!!'); return of(null); }))
    }).subscribe(({ posters, tickettypes }) => {
      if (posters) {
        const postersRes = posters.data;
        this.images = postersRes?.length ? postersRes.map((p: any) => ({
          id: p.id,
          imageUrl: p.imageUrl,
          imageType: p.imageType,
          isPrimary: p.isPrimary,
        })) : [{ imageUrl: 'https://picsum.photos/1200/630/?image=375' } as any];
      }

      if (tickettypes) {
        const data = tickettypes.data;
        this.tickets = data.map((t: any) => ({
          ...t,
          selectedCount: 0
        }));
      }

      this.cdr.detectChanges();
    });
  }

  // Tính phần trăm đã bán để làm Progress Bar
  getSoldPercent(ticket: TicketType): number {
    return (ticket.soldQuantity / ticket.quantity) * 100;
  }

  get selectedTickets() {
    return this.tickets.filter(t => t.selectedCount > 0);
  }

  get subtotal() {
    return this.selectedTickets.reduce((acc, t) => acc + (t.price * t.selectedCount), 0);
  }

  updateQuantity(ticket: TicketVM, amount: number) {
    const nextValue = ticket.selectedCount + amount;
    if (nextValue >= 0 && nextValue <= ticket.availableQuantity && nextValue <= ticket.maxPerUser) {
      ticket.selectedCount = nextValue;
    }
  }

  createBooking() {
    if (this.isSubmitting) return;

    if (this.selectedTickets.length == 0) {
      this.toast.error("Booking Ticket", "Please choose at least one ticket");
      return;
    }

    this.isSubmitting = true;
    this.selectedBookings = this.selectedTickets.map(t => ({
      id: t.id,
      quantity: t.selectedCount
    }));

    this.eventService.createBooking(this.selectedBookings).subscribe({
      next: (res) => {
        const data = res.data;
        console.log('Booking created:', data);

        this.toast.success('Direct to payment...');

        setTimeout(() => {
          window.location.href = `events/${this.eventId}/bookings/${data.id}/payment`;
        }, 1500);
      },
      error: (err) => {
        console.error(err);
        this.toast.error('Errors, please try again later!!!');
        this.isSubmitting = false;
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

  changeImage(step: number) {
    this.currentIndex = (this.currentIndex + step + this.images.length) % this.images.length;
  }

  setIndex(index: number) {
    this.currentIndex = index;
  }
}
