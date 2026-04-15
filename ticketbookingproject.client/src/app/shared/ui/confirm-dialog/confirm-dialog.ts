import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
  ChangeDetectionStrategy,
  inject
} from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

export type ConfirmVariant = 'danger' | 'warning' | 'info' | 'success';

export interface ConfirmDialogConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: ConfirmVariant;
  detail?: string;
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-dialog.html',
  styleUrls: ['./confirm-dialog.scss'],
})
export class ConfirmDialog implements OnChanges {
  @Input() isOpen = false;
  @Input() config: ConfirmDialogConfig = {
    title: 'Xác nhận',
    message: 'Bạn có chắc muốn thực hiện hành động này?',
    confirmText: 'Xác nhận',
    cancelText: 'Hủy',
    variant: 'danger',
  };

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  isAnimatingOut = false;
  isVisible = false;
  private santizer = inject(DomSanitizer);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isOpen']) {
      if (this.isOpen) {
        this.isVisible = true;
        this.isAnimatingOut = false;
      } else if (this.isVisible) {
        this.closeWithAnimation();
      }
    }
  }

  get variantIcon(): string {
    const icons: Record<ConfirmVariant, string> = {
      danger: 'delete',
      warning: 'warning',
      info: 'info',
      success: 'check_circle',
    };
    return icons[this.config.variant ?? 'danger'];
  }


  onConfirm(): void {
    this.confirmed.emit();
    this.closeWithAnimation();
  }

  onCancel(): void {
    this.cancelled.emit();
    this.closeWithAnimation();
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('dialog-backdrop')) {
      this.onCancel();
    }
  }

  private closeWithAnimation(): void {
    this.isAnimatingOut = true;
    setTimeout(() => {
      this.isVisible = false;
      this.isAnimatingOut = false;
    }, 280);
  }
}
