import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

export interface BookingSettings {
  startImmediately: boolean;
  bookingDate: string | null;
  bookingTime: string | null;
}

@Component({
  selector: 'app-event-setting',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './test.html',
  styleUrls: ['./test.scss']
})
export class EventSettingComponent implements OnInit, OnDestroy {
  form!: FormGroup;

  private subs = new Subscription();

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      startImmediately: [false],
      bookingStart: this.fb.group({
        date: [null, Validators.required],
        time: ['10:00']
      })
    });

    // React to toggle changes
    this.subs.add(
      this.form.get('startImmediately')!.valueChanges.subscribe((immediate: boolean) => {
        const bookingStart = this.form.get('bookingStart')!;
        if (immediate) {
          bookingStart.disable();
          bookingStart.reset({ date: null, time: '10:00' });
        } else {
          bookingStart.enable();
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  // ─── Convenience getters ────────────────────────────────────────────────────

  get isImmediate(): boolean {
    return this.form.get('startImmediately')!.value;
  }

  get isDateInvalid(): boolean {
    const ctrl = this.form.get('bookingStart.date')!;
    return ctrl.invalid && ctrl.touched;
  }

  /**
   * Returns the resolved booking start:
   * - 'now'                     → toggle ON
   * - 'YYYY-MM-DDTHH:mm:00'    → toggle OFF + date set
   * - null                      → toggle OFF, date not yet chosen
   */
  get resolvedBookingStart(): string | null {
    if (this.isImmediate) return 'now';

    const { date, time } = this.form.get('bookingStart')!.value;
    if (date && time) return `${date}T${time}:00`;
    return null;
  }

  /** Snapshot of current settings — call this before submitting */
  getSettings(): BookingSettings {
    const { startImmediately, bookingStart } = this.form.getRawValue();
    return {
      startImmediately,
      bookingDate: startImmediately ? null : bookingStart.date,
      bookingTime: startImmediately ? null : bookingStart.time
    };
  }

  /** Marks all fields touched to trigger validation display */
  submit(): void {
    if (!this.isImmediate) {
      this.form.get('bookingStart')!.markAllAsTouched();
    }

    if (this.form.valid || this.isImmediate) {
      console.log('Settings:', this.getSettings());
    }
  }
}
