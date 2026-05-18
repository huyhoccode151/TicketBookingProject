import { CommonModule } from '@angular/common';
import { Component, ChangeDetectorRef, OnInit, OnDestroy, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Card } from '../../../shared/ui/card/card';
import { EventService } from '../services/event';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { forkJoin, Subscription } from 'rxjs';
import { CategoryListItemResponse, Ticket, VenueListItemResponse } from '../models/event';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { Router, ActivatedRoute } from '@angular/router';
import { environment } from '../../../../environments/environments';
import { Index } from '../index';
import { RouteService } from '../../../core/services/route.service';
import { disabled } from '@angular/forms/signals';

type ImageItem = {
  file?: File;       // undefined = ảnh cũ từ server
  preview: string;
  posterId?: number; // id của poster cũ (để xoá nếu cần)
  isExisting: boolean
};

type PosterMeta = {
  posterId?: number;
  isPrimary: boolean;
};

@Component({
  selector: 'app-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Card, TableActions, ConfirmDialog],
  templateUrl: './edit.html',       // dùng lại template create.html, chỉ đổi tên nếu cần
  styleUrls: ['../create/create.scss'],
})
export class Edit implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private eventService = inject(EventService);
  private toast = inject(ToastService);
  private subs = new Subscription();

  form!: FormGroup;
  ticketFormEdit!: FormGroup;
  ticketFormCreate!: FormGroup;
  baseUrl = environment.api;
  routeNavigate = inject(RouteService);

  eventId!: number;
  isLoading = true;

  steps = [
    { label: 'Details' },
    { label: 'Tickets' },
    { label: 'Setting' },
  ];

  currentStep = 0;
  MAX_IMAGES = 10;

  isPastEvent = false;
  isPastBookingStart = false;
  isPastBookingEnd = false;

  images: ImageItem[] = [];
  isDragOver = false;
  listDeleteImageIds: number[] = [];

  categories: CategoryListItemResponse[] = [];
  venues: VenueListItemResponse[] = [];

  isDialogEditOpen = false;
  isDialogCreateOpen = false;
  isDialogDeleteOpen = false;
  ticketId = 0;
  dialogEditConfig: ConfirmDialogConfig = { title: '', message: '' };
  dialogCreateConfig: ConfirmDialogConfig = { title: '', message: '' };
  dialogDeleteConfig: ConfirmDialogConfig = { title: '', message: '' };
  errorMessage = '';
  formattedPrice = '';

  tickets: Ticket[] = [];

  ngOnInit(): void {
    // Lấy eventId từ route params, VD: /events/:id/edit
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));

    this._buildForms();
    this._setupWatchers();
    this._loadDropdowns();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  // ─── Private helpers ────────────────────────────────────────────────────────

  private _buildForms(): void {
    this.form = this.fb.group({
      details: this.fb.group({
        name: ['', Validators.required],
        startTime: ['', Validators.required],
        endTime: ['', Validators.required],
        venue: ['', Validators.required],
        description: [''],
        category: ['', Validators.required],
        images: [[], Validators.required]
      }),

      settings: this.fb.group({
        ticketsPerBooking: [1, Validators.required],

        startImmediately: [false],
        bookingStart: this.fb.group({
          date: [null, Validators.required],
          time: ['10:00'],
        }),

        endImmediately: [false],
        bookingEnd: this.fb.group({
          date: [null, Validators.required],
          time: ['10:00'],
        }),
      }),
    });

    this.ticketFormEdit = this.fb.group({
      name: ['', Validators.required],
      price: [2000, [Validators.required, Validators.min(2000)]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      maxPerUser: [1, [Validators.required, Validators.min(1)]],
    });

    this.ticketFormCreate = this.fb.group({
      name: ['', Validators.required],
      price: [2000, [Validators.required, Validators.min(2000)]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      maxPerUser: [1, [Validators.required, Validators.min(1)]],
    });
  }

  private _setupWatchers(): void {
    // Validate endTime > startTime
    this.subs.add(
      this.form.get('details')?.valueChanges.subscribe(val => {
        const minDate = new Date();
        minDate.setDate(minDate.getDate() + 3);
        const startDate = new Date(val.startTime);

        if (startDate < minDate) {
          this.form.get('details.startTime')?.setErrors({ invalidTime: true });
          this.form.get('details.startTime')?.markAsTouched();
        }

        if (startDate && val.endTime && new Date(val.startTime) >= new Date(val.endTime)) {
          this.form.get('details.endTime')?.setErrors({ invalidRange: true });
        } else {
          this.form.get('details.endTime')?.setErrors(null);
        }
      })
    );

    // bookingStart toggle
    this.subs.add(
      this.form.get('settings.startImmediately')!.valueChanges.subscribe(immediate => {
        const group = this.form.get('settings.bookingStart') as FormGroup;
        if (immediate) {
          group.disable({ emitEvent: false });
          group.reset({ date: null, time: '10:00' }, { emitEvent: false });
        } else {
          group.enable();
          group.get('date')?.setValidators(Validators.required);
        }
        group.get('date')?.updateValueAndValidity();

      })
    );

    // bookingEnd toggle
    this.subs.add(
      this.form.get('settings.endImmediately')!.valueChanges.subscribe(immediate => {
        const group = this.form.get('settings.bookingEnd') as FormGroup;
        if (immediate) {
          group.disable({ emitEvent: false });
          group.reset({ date: null, time: '10:00' }, { emitEvent: false });
        } else {
          group.enable();
          group.get('date')?.setValidators(Validators.required);
        }
        group.get('date')?.updateValueAndValidity();
      })
    );

    this.subs.add(
      this.form.get('settings')!.valueChanges.subscribe(settings => {
        if (settings.bookingStart.date && settings.bookingEnd.date) {
          const minDate = new Date();
          const start = new Date(settings.bookingStart.date);
          const end = new Date(settings.bookingEnd.date);
          const startTime = settings.bookingStart.time || '00:00';
          const endTime = settings.bookingEnd.time || '00:00';

          if (start < minDate) {
            this.form.get('settings.bookingStart.date')?.setErrors({ invalidTime: true });
            this.form.get('settings.bookingStart.date')?.markAsTouched();
          }

          if (start > end && startTime > endTime) {
            this.form.get('settings.bookingEnd.date')?.setErrors({ invalidBookingRange: true });
            this.form.get('settings.bookingEnd.date')?.markAsTouched();
          } else {
            this.form.get('settings.bookingEnd.date')?.setErrors(null);
          }

          const eventEndTime = this.form.get('details.endTime')?.value;
          if (eventEndTime) {
            const eventEnd = new Date(eventEndTime);

            const [endHour, endMin] = endTime.split(':').map(Number);
            const bookingEndDateTime = new Date(end);
            bookingEndDateTime.setHours(endHour, endMin, 0, 0);

            if (bookingEndDateTime > eventEnd) {
              this.form.get('settings.bookingEnd.date')?.setErrors({ exceedsEventEnd: true });
              this.form.get('settings.bookingEnd.date')?.markAsTouched();
            }
          }
        }
      })
    );
  }

  private _loadDropdowns(): void {
    const category$ = this.eventService.getCategory();
    const venue$ = this.eventService.getVenue();

    forkJoin({ categories: category$, venues: venue$ }).subscribe({
      next: ({ categories, venues }) => {
        this.categories = categories.data;
        this.venues = venues.data;

        this._loadEvent();
      },
      error: () => this.toast.error('Error', 'Cannot load dropdown data'),
    });
  }

  private _loadEvent(): void {
    this.eventService.getEventById(this.eventId).subscribe({
      next: res => {
        const e = res.data;
        const now = new Date();
        console.log('Loaded event:', e);

        // ── 1. Patch details ──────────────────────────────────────────────────
        this.form.patchValue({
          details: {
            name: e.name,
            description: e.description,
            category: e.category?.id,
            venue: e.venue?.id,
            startTime: this._toDateInput(e.activeAt),   // "YYYY-MM-DD"
            endTime: this._toDateInput(e.endAt),
          },
        });

        // ── 2. Patch settings ─────────────────────────────────────────────────
        const saleStartImmediate = !e.saleStartAt;
        const saleEndImmediate = !e.saleEndAt;

        // Patch toggles FIRST so the watcher disables/enables sub-groups correctly
        this.form.get('settings.startImmediately')!.setValue(saleStartImmediate);
        this.form.get('settings.endImmediately')!.setValue(saleEndImmediate);

        this.form.patchValue({
          settings: {
            ticketsPerBooking: e.maxTicketsPerBooking,
            bookingStart: saleStartImmediate ? { date: null, time: '10:00' } : {
              date: this._toDateInput(e.saleStartAt),
              time: this._toTimeInput(e.saleStartAt),
            },
            bookingEnd: saleEndImmediate ? { date: null, time: '10:00' } : {
              date: this._toDateInput(e.saleEndAt),
              time: this._toTimeInput(e.saleEndAt),
            },
          },
        });

        // ── 3. Load tickets ───────────────────────────────────────────────────
        this.tickets = e.ticketTypes.map((t: any) => ({
          id: t.id,
          initials: t.name.slice(0, 3).toUpperCase(),
          name: t.name,
          price: t.price,
          quantity: String(t.quantity),
          maxPerUser: String(t.maxPerUser),
        }));

        // ── 4. Load existing images ───────────────────────────────────────────
        this.images = e.posters.map((p: any) => ({
          preview: this.getThumbnail(p.imageUrl),
          posterId: p.id,
          isExisting: true,
        }));
        this.updateImagesControl();

        // Check startTime (activeAt)
        if (e.activeAt) {
          this.isPastEvent = new Date(e.activeAt) < now;
          if (this.isPastEvent) {
            this.form.get('details.startTime')?.disable({ emitEvent: false });
            this.form.get('details.endTime')?.disable({ emitEvent: false });
          }
        }

        // Check saleStartAt
        if (e.saleStartAt) {
          this.isPastBookingStart = new Date(e.saleStartAt) < now;
          if (this.isPastBookingStart) {
            this.form.get('settings.bookingStart')?.disable({ emitEvent: false });
            this.form.get('settings.startImmediately')?.disable({ emitEvent: false });
          }
        }

        // Check saleEndAt
        if (e.saleEndAt) {
          this.isPastBookingEnd = new Date(e.saleEndAt) < now;
          if (this.isPastBookingEnd) {
            this.form.get('settings.bookingEnd')?.disable({ emitEvent: false });
            this.form.get('settings.endImmediately')?.disable({ emitEvent: false });
          }
        }

        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Error', 'Cannot load event data');
        this.isLoading = false;
      },
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

  private _toDateInput(value: string | Date | null | undefined): string {
    if (!value) return '';
    const iso = value instanceof Date ? value.toISOString() : String(value);
    return iso.split('T')[0];
  }

  private _toTimeInput(value: string | Date | null | undefined): string {
    if (!value) return '10:00';
    const iso = value instanceof Date ? value.toISOString() : String(value);
    return iso.split('T')[1]?.slice(0, 5) ?? '10:00';
  }

  // ─── Stepper ─────────────────────────────────────────────────────────────────

  isCompleted(i: number): boolean { return i < this.currentStep; }
  isActive(i: number): boolean { return i === this.currentStep; }
  goToStep(i: number): void { if (i <= this.currentStep) this.currentStep = i; }

  nextStep(): void {
    if (this.currentStep === 0) {
      const group = this.form.get('details') as FormGroup;
      if (group.invalid) { group.markAllAsTouched(); return; }
    }

    if (this.currentStep == 1) {
      if (!this.tickets || this.tickets.length === 0) {
        this.toast.error('You must create at least one ticket!');
        return;
      }
    }

    if (this.currentStep === 2) {
      const group = this.form.get('settings') as FormGroup;
      if (group.invalid) { group.markAllAsTouched(); return; }
    }
    this.currentStep++;
  }

  prevStep(): void { if (this.currentStep > 0) this.currentStep--; }
  get isLastStep(): boolean { return this.currentStep === this.steps.length - 1; }

  onNextOrSubmit(): void {
    if (this.isLastStep) this.onSubmit();
    else this.nextStep();
  }

  // ─── Submit (UPDATE) ──────────────────────────────────────────────────────────

  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const toDateTime = (date: string) => `${date}T00:00:00`;
    const v = this.form.getRawValue();
    const posterMeta: PosterMeta[] = this.images.map((img, index) => ({
      posterId: img.isExisting ? img.posterId : undefined,
      isPrimary: index == 0,
    }));

    // UpdateEventRequest fields (chỉ gửi những gì thay đổi)
    const request: Record<string, any> = {
      name: v.details.name,
      description: v.details.description,
      categoryId: v.details.category,
      venueId: v.details.venue,
      activeAt: toDateTime(v.details.startTime),
      endAt: toDateTime(v.details.endTime),
      maxTicketsPerBooking: v.settings.ticketsPerBooking,
      saleStartAt: this.resolvedBookingStart,
      saleEndAt: this.resolvedBookingEnd,
    };

    const formData = new FormData();
    Object.keys(request).forEach(key => {
      if (request[key] != null) formData.append(key, request[key]);
    });

    // Chỉ gửi ảnh MỚI (isExisting = false)
    this.images
      .filter(img => !img.isExisting && img.file)
      .forEach(img => formData.append('posters', img.file!));

    formData.append('posterMeta', JSON.stringify(posterMeta));

    // Tickets
    this.tickets.forEach((t: any, index: number) => {
      if (t.id) formData.append(`TicketTypes[${index}].Id`, t.id.toString());
      formData.append(`TicketTypes[${index}].Name`, t.name);
      formData.append(`TicketTypes[${index}].Price`, t.price.toString());
      formData.append(`TicketTypes[${index}].Quantity`, t.quantity.toString());
      formData.append(`TicketTypes[${index}].MaxPerUser`, t.maxPerUser.toString());
    });

    this.eventService.updateEvent(this.eventId, formData).subscribe({
      next: () => {
        this.toast.success('Success', 'Event updated successfully!!!');
            this.router.navigate(this.routeNavigate.events());
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
          const serverError = err.error?.errors ?? err.error?.message;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.form.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
            console.log(controlName, 'controlName');
          }
          for (const key in serverError) {
            if (Array.isArray(serverError[key])) {
              serverError[key].forEach(msg => this.toast.error('Validation failed. Please check your input.', msg));
            }
          }
        } else {
          this.errorMessage = err?.error?.message ?? 'An error occured. Please try again.';
          this.toast.error('Validate failed!!!');
        }
      }
    });
  }

  // ─── Tickets CRUD ─────────────────────────────────────────────────────────────

  createTicket(): void {
    this.ticketFormCreate.reset({ name: 'New Ticket', price: 2000, quantity: 1, maxPerUser: 1 });
    this.dialogCreateConfig = {
      title: 'Create ticket type',
      message: '',
      confirmText: 'Create',
      cancelText: 'Cancel',
      variant: 'success',
    };
    this.isDialogCreateOpen = true;
  }

  onCreateConfirmed(): void {
    if (this.ticketFormCreate.invalid) { this.ticketFormCreate.markAllAsTouched(); return; }
    const val = this.ticketFormCreate.value;
    this.tickets.push({
      initials: val.name.slice(0, 3).toUpperCase(),
      name: val.name,
      price: val.price,
      quantity: val.quantity,
      maxPerUser: val.maxPerUser,
    });
    this.isDialogCreateOpen = false;
  }

  onCreateCancelled(): void { this.isDialogCreateOpen = false; }

  editTicket(index: number): void {
    this.ticketId = index;
    this.ticketFormEdit.patchValue(this.tickets[index]);
    this.dialogEditConfig = {
      title: 'Edit ticket type',
      message: '',
      confirmText: 'Save',
      cancelText: 'Cancel',
      variant: 'info',
    };
    this.isDialogEditOpen = true;
  }

  onConfirmed(): void {
    if (this.ticketFormEdit.invalid) { this.ticketFormEdit.markAllAsTouched(); return; }
    const val = this.ticketFormEdit.value;
    this.tickets[this.ticketId] = {
      ...this.tickets[this.ticketId],
      ...val,
      initials: val.name.slice(0, 3).toUpperCase(),
    };
    this.isDialogEditOpen = false;
  }

  onCancelled(): void { this.isDialogEditOpen = false; }

  deleteTicket(index: number): void {
    this.ticketId = index;
    this.dialogDeleteConfig = {
      title: 'Delete ticket type',
      message: 'Do you want to delete this ticket type?',
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    };
    this.isDialogDeleteOpen = true;
  }

  onDeleteConfirmed(): void {
    this.tickets.splice(this.ticketId, 1);
    this.isDialogDeleteOpen = false;
  }

  onDeleteCancelled(): void { this.isDialogDeleteOpen = false; }

  viewTicket(_index: number): void { /* navigate to ticket detail nếu cần */ }

  // ─── Images ───────────────────────────────────────────────────────────────────

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;
    this.readFiles(input.files);
    input.value = '';
  }

  readFiles(files: FileList): void {
    const remaining = this.MAX_IMAGES - this.images.length;
    Array.from(files).slice(0, remaining).forEach(file => {
      if (!file.type.startsWith('image/')) {
        this.form.get('details.images')?.setErrors({
          fileType: 'Only image files are allowed'
        });
        return;
      }

      if (!file.type.startsWith('image/') || file.size > 10 * 1024 * 1024) {
        this.form.get('details.images')?.setErrors({
          fileSize: 'Each image must be less than 10MB'
        });
        return;
      }
      const reader = new FileReader();
      reader.onload = () => {
        this.images.push({ file, preview: reader.result as string, isExisting: false });
        this.updateImagesControl();
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    });
  }

  removeImage(index: number): void {
    this.images.splice(index, 1);
    this.updateImagesControl();
  }

  updateImagesControl(): void {
    const ctrl = this.form.get('details.images');
    if (!ctrl) return;

    ctrl.setValue(this.images);

    if (this.images.length === 0) {
      ctrl.setErrors({ required: true });
    } else {
      ctrl.setErrors(null);
    }

    ctrl.updateValueAndValidity();
  }

  onDragOver(e: DragEvent): void { e.preventDefault(); this.isDragOver = true; }
  onDragLeave(): void { this.isDragOver = false; }
  onDrop(e: DragEvent): void {
    e.preventDefault();
    this.isDragOver = false;
    if (e.dataTransfer?.files) this.readFiles(e.dataTransfer.files);
  }

  // ─── Getters ──────────────────────────────────────────────────────────────────

  get isImmediate(): boolean { return this.form.get('settings.startImmediately')!.value; }
  get isEndImmediate(): boolean { return this.form.get('settings.endImmediately')!.value; }

  get isDateInvalid(): boolean {
    const ctrl = this.form.get('settings.bookingStart.date')!;
    return ctrl.invalid && ctrl.touched;
  }

  get isEndDateInvalid(): boolean {
    const ctrl = this.form.get('settings.bookingEnd.date')!;
    return ctrl.invalid && ctrl.touched;
  }

  get resolvedBookingStart(): string | null {
    if (this.isImmediate) return new Date().toISOString();
    const { date, time } = this.form.get('settings.bookingStart')!.value;
    return date && time ? `${date}T${time}:00` : null;
  }

  get resolvedBookingEnd(): string | null {
    if (this.isEndImmediate) return this.form.get('details.endTime')!.value
      ? `${this.form.get('details.endTime')!.value}T00:00:00` : null;
    const { date, time } = this.form.get('settings.bookingEnd')!.value;
    return date && time ? `${date}T${time}:00` : null;
  }

  get detailsGroup(): FormGroup { return this.form.get('details') as FormGroup; }
  get settingsGroup(): FormGroup { return this.form.get('settings') as FormGroup; }

  // ─── Validation helpers ───────────────────────────────────────────────────────

  isInvalid(controlPath: string): boolean {
    const control = this.form.get(controlPath);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  getError(controlName: string, errorField: string): string {
    const ctrl = this.form.get(controlName);
    if (!ctrl?.errors || !(ctrl.touched || ctrl.dirty)) return '';
    if (ctrl.errors['required']) return `${errorField} is required`;
    if (ctrl.errors['minlength']) return 'Minimum 3 characters';
    if (ctrl.errors['invalidRange']) return 'End date must be after start date';
    if (ctrl.errors['invalidTime']) return 'Start time must be after now';
    if (ctrl.errors['exceedsEventEnd']) return 'Booking end time must be befor event start';

    return 'Invalid field';
  }

  getInvalid(controlName: string, errorField: string): string {
    const ctrl = this.ticketFormEdit.get(controlName);
    if (!ctrl?.errors || !(ctrl.touched || ctrl.dirty)) return '';
    if (ctrl.errors['required']) return `${errorField} is required`;
    if (ctrl.errors['min']) return `${errorField} must be at least 1`;

    return 'Invalid field';
  }

  getCreateInvalid(controlName: string, errorField: string): string {
    const ctrl = this.ticketFormCreate.get(controlName);
    if (!ctrl?.errors || !(ctrl.touched || ctrl.dirty)) return '';
    if (ctrl.errors['required']) return `${errorField} is required`;
    if (ctrl.errors['min']) return `${errorField} must be at least 1`;

    return 'Invalid field';
  }

  onPriceInput(event: any) {
    let value = event.target.value.replace(/\D/g, '');

    this.ticketFormEdit.patchValue({
      price: Number(value)
    }, { emitEvent: false });

    this.formattedPrice =
      Number(value).toLocaleString('vi-VN');
  }
}
