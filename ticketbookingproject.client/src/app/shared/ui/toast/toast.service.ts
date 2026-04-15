import { Injectable, InjectionToken, Inject, Optional } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Toast, ToastType, ToastConfig, ToastPosition } from './toast.model';

export const TOAST_CONFIG = new InjectionToken<ToastConfig>('TOAST_CONFIG');

const DEFAULT_CONFIG: ToastConfig = {
  defaultDuration: 4000,
  defaultPosition: 'top-right',
  maxToasts: 5,
};

@Injectable({ providedIn: 'root' })
export class ToastService {
  private config: ToastConfig;
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  toasts$: Observable<Toast[]> = this.toastsSubject.asObservable();

  constructor(@Optional() @Inject(TOAST_CONFIG) cfg: ToastConfig) {
    this.config = { ...DEFAULT_CONFIG, ...(cfg || {}) };
  }

  private generate(): string {
    return `toast_${Date.now()}_${Math.random().toString(36).slice(2, 7)}`;
  }

  show(toast: Omit<Toast, 'id'>): string {
    const id = this.generate();
    const newToast: Toast = {
      id,
      duration: this.config.defaultDuration,
      position: this.config.defaultPosition,
      closable: true,
      progress: true,
      ...toast,
    };

    const current = this.toastsSubject.value;
    const updated =
      current.length >= this.config.maxToasts
        ? [...current.slice(1), newToast]
        : [...current, newToast];

    this.toastsSubject.next(updated);

    if (newToast.duration && newToast.duration > 0) {
      setTimeout(() => this.dismiss(id), newToast.duration);
    }

    return id;
  }

  success(title: string, message?: string, opts?: Partial<Toast>): string {
    return this.show({ type: 'success', title, message, ...opts });
  }

  error(title: string, message?: string, opts?: Partial<Toast>): string {
    return this.show({ type: 'error', title, message, duration: 0, ...opts });
  }

  warning(title: string, message?: string, opts?: Partial<Toast>): string {
    return this.show({ type: 'warning', title, message, ...opts });
  }

  info(title: string, message?: string, opts?: Partial<Toast>): string {
    return this.show({ type: 'info', title, message, ...opts });
  }

  loading(title: string, message?: string): string {
    return this.show({ type: 'loading', title, message, duration: 0, closable: false, progress: false });
  }

  /** Update an existing toast (e.g. loading → success) */
  update(id: string, patch: Partial<Omit<Toast, 'id'>>): void {
    const current = this.toastsSubject.value;
    const updated = current.map(t => (t.id === id ? { ...t, ...patch } : t));
    this.toastsSubject.next(updated);

    const patched = updated.find(t => t.id === id);
    if (patched?.duration && patched.duration > 0) {
      setTimeout(() => this.dismiss(id), patched.duration);
    }
  }

  dismiss(id: string): void {
    this.toastsSubject.next(this.toastsSubject.value.filter(t => t.id !== id));
  }

  dismissAll(): void {
    this.toastsSubject.next([]);
  }
}
