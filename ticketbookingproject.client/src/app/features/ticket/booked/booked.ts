import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { Ticket } from '../models/ticket';
import { CommonModule } from '@angular/common';
import { TicketService } from '../services/ticket-service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Loader } from '../../../shared/ui/loader/loader';

@Component({
  selector: 'app-booked',
  standalone: true,
  imports: [CommonModule, Loader],
  templateUrl: './booked.html',
  styleUrls: ['./booked.scss'],
})
export class Booked implements OnInit {
  tickets!: Ticket[];
  bookingId!: number;
  loading: boolean = true;

  private ticketService = inject(TicketService);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.bookingId = Number(this.route.snapshot.paramMap.get('bookingId'));

    this.ticketService.getTicketsByBookingId(this.bookingId).subscribe({
      next: (res) => {
        this.tickets = res.data;
        console.log(res);

        setTimeout(() => {
          this.loading = false,
          this.cdr.markForCheck();
        }, 1000);
        this.toast.info('Show tickets', 'Show tickets successfully!!!');
      },
      error: () => {
        this.toast.error('Show tickets', 'Show tickets successfully!!!');
      }
    });
  }

  currentIndex = 0;

  nextTicket() {
    if (this.currentIndex < this.tickets.length - 1) {
      this.currentIndex++;
    }
  }

  prevTicket() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    }
  }

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
}
