import { Component, inject, NgModule, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookingPayments } from '../services/booking-payments';
import { CommonModule, DecimalPipe } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { RouteService } from '../../../core/services/route.service';

@Component({
  selector: 'app-vnpay-return',
  standalone: true,
  imports: [DecimalPipe, CommonModule],
  templateUrl: './vnpay-return.html',
  styleUrls: ['./vnpay-return.scss'],
})
export class VnpayReturn implements OnInit {
  loading: boolean = true;
  status = '';
  isSuccess: boolean = true;
  bookingExpires: Date = new Date();
  responseCode: string = '';
  eventId: number = 0;
  amount: number = 0;
  orderId: number = 0;
  transactionId: string = '';
  date: Date = new Date();

  private route = inject(ActivatedRoute);
  private paymentService = inject(BookingPayments);
  private router = inject(Router);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);
  routeNavigate = inject(RouteService);


  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (Object.keys(params).length > 0) {
        this.verifyPayment(params);
      }
    });
  }

  verifyPayment(params: any) {
    console.log('test');
    
    //trả về vnpay return
    this.paymentService.verifyVnPayPayment(params).subscribe({
      next: (res) => {
        this.loading = false;
        const data = res.data;
        console.log(res);

        this.responseCode = data.vnPayResponseCode;
        this.amount = data.amount;
        this.orderId = data.orderId;
        this.transactionId = data.transactionId;

        if (res.data.success) {
          this.isSuccess = true;
          this.status = 'Payment successful! Thank you for your purchase.';

          setTimeout(() => this.router.navigate(this.routeNavigate.ticketBooked(this.orderId)), 5000);
        } else {
          this.isSuccess = false;
          this.status = 'Payment failed: ' + (res.message || 'Unknown error');
        }

        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.isSuccess = false;
        this.status = 'Payment failed: An error occurred while processing your payment.';
        this.getBookingId(this.orderId);
        setTimeout(() => this.router.navigate(this.routeNavigate.eventBookingPayment(this.eventId, this.orderId)), 5000);
      }
    });
  }

  getBookingId(id: number) {
    this.paymentService.getBookingById(id).subscribe({
      next: (res) => {
        this.eventId = res.data.eventId;
        this.bookingExpires = new Date(res.data.expiresAt);

        const expiresAt = new Date(this.bookingExpires);
        if (this.bookingExpires < expiresAt) {
          this.toast.error("Order has been expired, please create new order!!!");
          setTimeout(() => this.router.navigate(this.routeNavigate.customerEvents()), 5000);
        }
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getErrorMessage(code: string) {
    switch (code) {
      case '24': return 'You cancelled the trans.';
      case '51': return 'Account does not have enough credit.';
      case '00': return 'Success.';
      default: return 'Transact failed. Please check or replace by another method.';
    }
  }
}
