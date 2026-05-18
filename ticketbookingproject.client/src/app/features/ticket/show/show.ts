import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { Loader } from '../../../shared/ui/loader/loader';
import { environment } from '../../../../environments/environments';
import { TicketService } from '../services/ticket-service';
import { ActivatedRoute } from '@angular/router';
import { Ticket } from '../models/ticket';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../../shared/ui/toast/toast.service';

@Component({
  selector: 'app-show',
  standalone: true,
  imports: [Loader, CommonModule],
  templateUrl: './show.html',
  styleUrls: ['./show.scss'],
})
export class Show implements OnInit {
  loading: boolean = true;
  ticketId!: number;
  ticket!: Ticket;

  private ticketService = inject(TicketService);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  private toast = inject(ToastService);

  ngOnInit() {
    this.ticketId = Number(this.route.snapshot.paramMap.get('ticketId'));

    this.loadTicket();
  }

  loadTicket() {
    
    this.ticketService.getTicketById(this.ticketId).subscribe({
      next: (res) => {
        this.ticket = res.data;
        console.log(this.ticket);
        this.toast.success("Show ticket " + this.ticket.id);
        this.cdr.markForCheck();
      }
    });
    this.loading = false;
  }

  baseUrl = environment.api;

  getThumbnail(url?: string): string {
    if (!url) return '';

    if (url.startsWith('http')) {
      return url;
    }

    // Nếu là link local (/uploads/...)
    return this.baseUrl + url;
  }
}
