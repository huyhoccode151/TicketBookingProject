import { CommonModule } from '@angular/common';
import { Component, ChangeDetectorRef, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Card } from '../../../shared/ui/card/card';
import { EventService } from '../services/event';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Subscription } from 'rxjs';
import { CategoryListItemResponse, Ticket, VenueListItemResponse } from '../models/event';
import { TableActions } from '../../../shared/ui/table-actions/table-actions';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { Router } from '@angular/router';

type ImageItem = {
  file: File;
  preview: string;
};

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Card, TableActions, ConfirmDialog],
  templateUrl: './create.html',
  styleUrls: ['./create.scss'],
})
export class Create implements OnInit {
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private eventService = inject(EventService);
  private toast = inject(ToastService);
  private subs = new Subscription();

  form!: FormGroup;
  ticketFormEdit!: FormGroup;
  ticketFormCreate!: FormGroup;

  steps = [
    { label: 'Details' },
    { label: 'Tickets' },
    { label: 'Setting' },
  ];

  currentStep = 0;
  MAX_IMAGES = 10;

  images: ImageItem[] = [];
  isDragOver = false;

  ticketsPerBooking = [''];
  categories: CategoryListItemResponse[] = [];
  venues: VenueListItemResponse[] = [];
  visibilityOpts = ['Public', 'Private', 'Invite only'];
  currencyOpts = ['USD ($)', 'VND (₫)', 'EUR (€)'];

  isDialogEditOpen = false;
  isDialogCreateOpen = false;
  isDialogDeleteOpen = false;
  ticketId = 0;
  dialogEditConfig: ConfirmDialogConfig = { title: '', message: '' }
  dialogCreateConfig: ConfirmDialogConfig = { title: '', message: '' }
  dialogDeleteConfig: ConfirmDialogConfig = { title: '', message: '' }

  tickets: Ticket[] = [
    {
      initials: 'VIP', name: 'New Small', price: 10,
      quantity: '20', maxPerUser: '2'
    },
    {
      initials: 'NOR', name: 'New Small', price: 10,
      quantity: '20', maxPerUser: '2'
    },
  ];

  ngOnInit(): void {
    this.eventService.getCategory().subscribe({
      next: (res) => {
        this.categories = res.data;
        this.toast.success('Success', 'Load categories successfully!!!');
      },
      error: () => {
        this.toast.error('Error', 'Cannot load categories');
      }
    });

    this.eventService.getVenue().subscribe({
      next: (res) => {
        this.venues = res.data;
        this.toast.success('Success', 'Load venues successfully!!!');
      },
      error: () => {
        this.toast.error('Error', 'Cannot load venues');
      }
    });

    this.form = this.fb.group({
      details: this.fb.group({
        name: ['', Validators.required],
        startTime: ['', Validators.required],
        endTime: ['', Validators.required],
        venue: ['', Validators.required],
        description: [''],
        category: ['', Validators.required]
      }),

      settings: this.fb.group({
        ticketsPerBooking: [1, Validators.required],

        startImmediately: [false],
        bookingStart: this.fb.group({
          date: [null, Validators.required],
          time: ['10:00']
        }),

        endImmediately: [false],
        bookingEnd: this.fb.group({
          date: [null, Validators.required],
          time: ['10:00']
        }),
      })
    });

    this.ticketFormEdit = this.fb.group({
      name: ['', Validators.required],
      price: [0, Validators.required],
      quantity: [0, Validators.required],
      maxPerUser: [0, Validators.required],
    });

    this.ticketFormCreate = this.fb.group({
      name: ['', Validators.required],
      price: [0, Validators.required],
      quantity: [0, Validators.required],
      maxPerUser: [0, Validators.required],
    });

    this.subs.add(
      this.form.get('details')?.valueChanges.subscribe(val => {
        if (val.startTime && val.endTime) {
          if (new Date(val.startTime) > new Date(val.endTime)) {
            this.form.get('details.endTime')?.setErrors({ invalidRange: true });
          }
          else {
            this.form.get('details.endTime')?.setErrors(null);
          }
        }
      })
    );

    this.subs.add(
      this.form.get('settings.startImmediately')!.valueChanges.subscribe(immediate => {
        const group = this.form.get('settings.bookingStart') as FormGroup;
        const dateCtrl = group.get('date');

        if (immediate) {
          group.disable({ emitEvent: false });
          group.reset({ date: null, time: '10:00' }, { emitEvent: false });
          group.updateValueAndValidity({ emitEvent: false });
        } else {
          group.enable();
          dateCtrl?.setValidators(Validators.required);
        }

        dateCtrl?.updateValueAndValidity();
      })
    );

    // Disable/enable bookingEnd based on endImmediately toggle
    this.subs.add(
      this.form.get('settings.endImmediately')!.valueChanges.subscribe(immediate => {
        const group = this.form.get('settings.bookingEnd') as FormGroup;
        const dateCtrl = group.get('date');

        if (immediate) {
          group.disable({ emitEvent: false });
          group.reset({ date: null, time: '10:00' }, { emitEvent: false });
          group.updateValueAndValidity({ emitEvent: false });
        } else {
          group.enable();
          dateCtrl?.setValidators(Validators.required);
        }

        dateCtrl?.updateValueAndValidity();
      })
    );
  }
  //create ticket
  onCreateConfirmed(): void {
    if (this.ticketFormCreate.invalid) {
      this.ticketFormCreate.markAllAsTouched();
      return;
    }

    const request = this.ticketFormCreate.value;
    request.initials = request.name;

    console.log(request);

    if (request != null) {
      this.tickets.push({
        ...request
      });
    }

    console.log(this.tickets);

    this.isDialogCreateOpen = false;
  }

  onCreateCancelled(): void {
    this.isDialogCreateOpen = false;
  }

  createTicket() {
    this.dialogCreateConfig = {
      title: 'Create ticket type',
      message: '',
      detail: ``,
      confirmText: 'Create',
      cancelText: 'Cancel',
      variant: 'success',
    }
    this.isDialogCreateOpen = true;
  }

  //edit ticket
  onConfirmed(): void {
    if (this.ticketFormEdit.invalid) {
      this.ticketFormEdit.markAllAsTouched();
      return;
    }
    const request = this.ticketFormEdit.value;

    if (this.ticketId != null) {
      this.tickets[this.ticketId] = {
        ...this.tickets[this.ticketId],
        ...request
      };
      this.tickets[this.ticketId].initials = request.Name;
    }

    console.log(this.tickets);

    this.isDialogEditOpen = false;
  }

  onCancelled(): void {
    this.isDialogEditOpen = false;
  }

  editTicket(eventId: number) {
    this.ticketId = eventId;

    let ticket = this.ticketFormEdit.patchValue(this.tickets[eventId]);

    this.dialogEditConfig = {
      title: 'Edit ticket type',
      message: '',
      detail: ``,
      confirmText: 'Edit',
      cancelText: 'Cancel',
      variant: 'info',
    }
    this.isDialogEditOpen = true;
  }

  deleteTicket(eventId: number) {
    this.ticketId = eventId;

    console.log(this.ticketId);

    this.dialogDeleteConfig = {
      title: 'Delete ticket type',
      message: 'Do you want to delete this ticket type?',
      detail: `Delete this ticket type!!!`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    }

    this.isDialogDeleteOpen = true;
  }

  onDeleteConfirmed() {
    if (!this.ticketId) this.toast.error('Couldnt found ticket type!!!');

    this.tickets.splice(this.ticketId, 1);
  }

  onDeleteCancelled() {
    this.isDialogDeleteOpen = false
  }

  viewTicket(eventId: number) {

  }

  get detailsGroup(): FormGroup {
    return this.form.get('details') as FormGroup;
  }

  get settingsGroup(): FormGroup {
    return this.form.get('settings') as FormGroup;
  }

  get isImmediate(): boolean {
    return this.form.get('settings.startImmediately')!.value;
  }

  get isDateInvalid(): boolean {
    const ctrl = this.form.get('settings.bookingStart.date')!;
    return ctrl.invalid && ctrl.touched;
  }

  get resolvedBookingStart(): string | null {
    if (this.isImmediate) return new Date().toISOString();

    const { date, time } = this.form.get('settings.bookingStart')!.value;
    if (date && time) return `${date}T${time}:00`;
    return null;
  }

  get isEndImmediate(): boolean {
    return this.form.get('settings.endImmediately')!.value;
  }

  get isEndDateInvalid(): boolean {
    const ctrl = this.form.get('settings.bookingEnd.date')!;
    return ctrl.invalid && ctrl.touched;
  }

  get resolvedBookingEnd(): string | null {
    if (this.isEndImmediate) {
      return this.form.get('details.startTime')!.value;
    }

    const { date, time } = this.form.get('settings.bookingEnd')!.value;
    if (date && time) return `${date}T${time}:00`;
    return null;
  }

  isCompleted(i: number): boolean { return i < this.currentStep; }
  isActive(i: number): boolean { return i === this.currentStep; }
  goToStep(i: number): void { if (i <= this.currentStep) this.currentStep = i; }
  nextStep(): void {
    if (this.currentStep == 0) {
      const group = this.form.get('details') as FormGroup;
      if (group.invalid) {
        group.markAllAsTouched();
        return;
      }
    }

    if (this.currentStep == 2) {
      const group = this.form.get('settings') as FormGroup;
      if (group.invalid) {
        group.markAllAsTouched();
        return;
      }
    }

    this.currentStep++;
  }

  prevStep(): void { if (this.currentStep > 0) this.currentStep--; }
  get isLastStep(): boolean { return this.currentStep === this.steps.length - 1; }

  onNextOrSubmit(): void {
    if (this.isLastStep) {
      this.onSubmit();
    } else {
      this.nextStep();
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const toDateTime = (date: string) => `${date}T00:00:00`;

    const v = this.form.value;

    const request = {
      name: v.details.name,
      description: v.details.description,
      categoryId: v.details.category,
      venueId: v.details.venue,
      activeAt: toDateTime(v.details.startTime),
      endAt: toDateTime(v.details.endTime),
      ticketsPerBooking: v.settings.ticketsPerBooking,
      saleStartAt: this.resolvedBookingStart,
      saleEndAt: this.resolvedBookingEnd,
    }

    const formData = new FormData();

    Object.keys(request).forEach(key => {
      formData.append(key, (request as Record<string, any>)[key]);
    });

    this.images.forEach(img => formData.append('posters', img.file));

    this.tickets.forEach((t, index) => {
      formData.append(`TicketTypes[${index}].Name`, t.name);
      formData.append(`TicketTypes[${index}].Price`, t.price.toString());
      formData.append(`TicketTypes[${index}].Quantity`, t.quantity.toString());
      formData.append(`TicketTypes[${index}].MaxPerUser`, t.maxPerUser.toString());
    });


    console.log('FormData:', formData);
    console.log('Details:', v.details);
    console.log('Settings:', v.settings);
    console.log('Tickets:', this.tickets);

    this.eventService.createEvent(formData).subscribe({
      next: () => {
        this.form.reset();
        this.router.navigate(['/events']);
        this.toast.success('Edited User', 'Edited user successfully!!!');
      }
    });

    alert('Event created: ' + v.details.name);
  }

  //IMAGE
  onFileSelect(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files) return;

    this.readFiles(input.files);
    input.value = '';
  }

  readFiles(files: FileList) {
    const remaining = this.MAX_IMAGES - this.images.length;

    Array.from(files).slice(0, remaining)
      .forEach(file => {
        if (!file.type.startsWith('image/')) return;
        if (file.size > 10 * 1024 * 1024) return;

        const reader = new FileReader();
        reader.onload = () => {
          this.images.push({
            file,
            preview: reader.result as string
          });
          this.cdr.detectChanges();
        };
        reader.readAsDataURL(file);
      });
  }

  removeImage(index: number) {
    this.images.splice(index, 1);
  }

  onDragOver(e: DragEvent) {
    e.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave() {
    this.isDragOver = false;
  }

  onDrop(e: DragEvent) {
    e.preventDefault();
    this.isDragOver = false;

    if (e.dataTransfer?.files) {
      this.readFiles(e.dataTransfer.files);
    }
  }

  getError(controlName: string, errorField: string): string {
    const control = this.form.get(controlName);

    if (!control || !control.errors || !(control.touched || control.dirty)) {
      return '';
    }

    if (control.errors['required']) return `${errorField} is required`;
    if (control.errors['minlength']) return 'Minimum 3 characters';

    return 'Invalid field';
  }

  getInvalid(controlName: string, errorField: string): string {
    const control = this.ticketFormEdit.get(controlName);

    if (!control || !control.errors || !(control.touched || control.dirty)) {
      return '';
    }

    if (control.errors['required']) return `${errorField} is required`;
    if (control.errors['minlength']) return 'Minimum 3 characters';

    return 'Invalid field';
  }

  getCreateInvalid(controlName: string, errorField: string): string {
    const control = this.ticketFormCreate.get(controlName);

    if (!control || !control.errors || !(control.touched || control.dirty)) {
      return '';
    }

    if (control.errors['required']) return `${errorField} is required`;
    if (control.errors['minlength']) return 'Minimum 3 characters';

    return 'Invalid field';
  }

  //this.form.get('settings.startImmediately')!.valueChanges.subscribe(immediate => {
  //  const group = this.form.get('settings.startImmediately') as FormGroup;

  //  if (immediate) {
  //    group.disable();
  //    group.get('date')?.clearValidators();
  //  }

  //  group.get('date')?.updateValueAndValidity();
  //});
}
