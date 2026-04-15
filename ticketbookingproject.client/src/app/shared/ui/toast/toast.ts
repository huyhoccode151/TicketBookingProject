import {
  Component, OnInit, OnDestroy, ChangeDetectionStrategy,
  ChangeDetectorRef,
  inject,
  Sanitizer
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { Toast, ToastPosition } from './toast.model';
import { ToastService } from './toast.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.html',
  styleUrls: ['./toast.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastComponent implements OnInit, OnDestroy {
  grouped: Record<ToastPosition, Toast[]> = {
    'top-right': [], 'top-left': [], 'top-center': [],
    'bottom-right': [], 'bottom-left': [], 'bottom-center': [],
  };

  positions: ToastPosition[] = [
    'top-right', 'top-left', 'top-center',
    'bottom-right', 'bottom-left', 'bottom-center',
  ];

  private sub!: Subscription;

  private svc = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);
  private sanitizer = inject(DomSanitizer);

  ngOnInit(): void {
    this.sub = this.svc.toasts$.subscribe(toasts => {
      this.positions.forEach(p => (this.grouped[p] = []));
      toasts.forEach(t => {
        const pos = t.position ?? 'top-right';
        this.grouped[pos].push(t);
      });
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy(): void { this.sub.unsubscribe(); }

  dismiss(id: string): void { this.svc.dismiss(id); }

  trackById(_: number, t: Toast): string { return t.id; }

  icon(type: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml({
      success: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <path d="M20 6L9 17l-5-5"/></svg>`,
      error: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>`,
      warning: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <path d="M12 9v4m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/></svg>`,
      info: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>`,
      loading: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="spin">
        <path d="M21 12a9 9 0 11-6.219-8.56"/></svg>`,
    }[type] ?? '');
  }
}
