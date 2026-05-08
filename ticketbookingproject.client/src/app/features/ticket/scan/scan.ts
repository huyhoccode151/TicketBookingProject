import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Html5Qrcode } from 'html5-qrcode';
import { TicketService } from '../services/ticket-service';

@Component({
  selector: 'app-scan',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './scan.html',
  styleUrl: './scan.scss',
})
export class Scan {
  private http = inject(HttpClient);

  scanning = false;
  loading = false;
  result: any = null;
  error: string = '';
  manualCode = '';

  // dùng thư viện html5-qrcode
  private scanner: Html5Qrcode | null = null;
  private ticketService = inject(TicketService);
  private cdr = inject(ChangeDetectorRef);

  startScan() {
    this.scanner = new Html5Qrcode('qr-reader');

    this.scanner.start(
      { facingMode: 'environment' }, // camera sau
      { fps: 10, qrbox: { width: 250, height: 250 } },
      (qrCode) => {
        this.scanner?.stop();
        this.scanning = false;
        this.checkIn(qrCode);
      },
      () => { } // ignore scan errors
    );

    this.scanning = true;
  }

  submitManual() {
    if (this.manualCode.trim()) this.checkIn(this.manualCode.trim());
  }

  checkIn(qrCode: string) {
    this.loading = true;
    this.result = null;
    this.error = '';

    this.ticketService.scanTicket(qrCode).subscribe({
      next: (res) => {
        this.result = res;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.error = err.error || 'Check-in thất bại';
        this.loading = false;
        this.cdr.markForCheck();
      },
    });
  }

  reset() {
    this.result = null;
    this.error = '';
    this.manualCode = '';
  }
}
