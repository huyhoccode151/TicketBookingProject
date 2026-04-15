import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../event/services/event';
import { EventPoster, TicketVM } from '../../event/models/event';
import { environment } from '../../../../environments/environments';
import { ActivatedRoute } from '@angular/router';
import { BookingPayments } from '../services/booking-payments';
import { BookingTicketDetails, TicketSelected } from '../models/booking_payment';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { ApiResponse } from '../../user/models/paged-result';

@Component({
  selector: 'app-pre-pay',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pre-pay.html',
  styleUrls: ['./pre-pay.scss'],
})
export class PrePay implements OnInit, OnDestroy {
  eventId!: number;
  bookingId!: number;
  images: EventPoster[] = [];
  bookingInfo!: BookingTicketDetails;
  //tickets: TicketVM[] = [];
  expiresAt: string = '2026-04-08T10:15:00Z';
  eventName: string = '';
  ticketSelected: TicketSelected[] = [];
  activeTime: Date = new Date();
  venueName: string = '';
  paymentMethod: string = 'vnpay';
  totalAmount: number = 0;
  baseUrl = environment.api;

  private route = inject(ActivatedRoute);
  private eventService = inject(EventService);
  private bookingService = inject(BookingPayments);
  private toast = inject(ToastService);

  tickets = [
    { name: 'Diamond VIP Box', desc: 'Includes complimentary lounge access', price: 1800000, quantity: 2 },
    { name: 'Standard Balcony', desc: 'Level 2 seating', price: 480000, quantity: 1 }
  ];

  serviceFeePercent = 0.02;
  timeLeft: number = 599; // 9:59 tính bằng giây
  timerInterval: any;
  selectedPayment = 'card';

  get subtotal() {
    return this.tickets.reduce((acc, t) => acc + (t.price * t.quantity), 0);
  }

  get serviceFee() {
    return this.subtotal * this.serviceFeePercent;
  }

  get total() {
    return this.subtotal + this.serviceFee;
  }

  ngOnInit() {
    this.eventId = Number(this.route.snapshot.paramMap.get("eventId"));
    this.bookingId = Number(this.route.snapshot.paramMap.get("bookingId"));
    this.initTimeLeft();
    this.startTimer();
    this.getEventInfo();
    this.getBookingInfo();
  }

  getEventInfo()
  {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (res) => {
        const data = res.data;
        this.images = data.posters?.length ? data.posters.map((p: any) => ({
          id: p.id,
          imageUrl: p.imageUrl,
          imageType: p.imageType,
          isPrimary: p.isPrimary,
        })) : [{ imageUrl: 'https://picsum.photos/1200/630/?image=375' } as any];

        this.eventName = data.name;
        this.venueName = data.venue?.name ?? '';
        this.activeTime = new Date(data.activeAt);
      }
    });
  }

  getBookingInfo() {
    this.bookingService.getBookingById(this.bookingId).subscribe({
      next: (res) => {
        const data = res.data;
        console.log(data);
        this.expiresAt = data.expiresAt;
        this.totalAmount = data.totalAmount;
        this.bookingInfo = data;
        this.ticketSelected = data.details.map((p: any) => ({
          name: p.ticketTypeName,
          price: p.price,
          quantity: p.quantity,
        }));

        this.toast.success('Booking info loaded');
      }
    })
  }

  initTimeLeft(): void {
    const now = new Date().getTime();
    const exp = new Date(this.expiresAt).getTime();

    this.timeLeft = Math.max(0, Math.floor((exp - now) / 1000));
  }

  startTimer() {
    this.timerInterval = setInterval(() => {
      if (this.timeLeft > 0) this.timeLeft--;
      else clearInterval(this.timerInterval);
    }, 1000);
  }

  formatTime(seconds: number): string {
    const min = Math.floor(seconds / 60);
    const sec = seconds % 60;
    return `${min.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
  }

  ngOnDestroy() {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }

  confirmPayNow() {
    const request = {
      amount: this.totalAmount,
      bookingId: this.bookingId.toString(),
      bookingInfo: (this.bookingInfo.userId + '-' + this.bookingInfo.eventId + '-' + this.bookingId).toString(),
    };

    if (this.paymentMethod == "momo") {
      this.bookingService.paymentByMomo(request).subscribe({
        next: (res) => {
          const data = res.data;
          console.log('MOMO RESPONSE:', data);
          if (data.payUrl) window.location.href = data.payUrl;
        },
        error: (err) => {
          this.toast.error('Failed to create payment: ' + (err.error?.message ?? err.message ?? 'Unknown error'));
        }
      })
    }

    if (this.paymentMethod == "vnpay") {
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
}
