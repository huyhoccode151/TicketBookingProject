import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { Card } from '../../../shared/ui/card/card';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ConfirmDialog, ConfirmDialogConfig } from '../../../shared/ui/confirm-dialog/confirm-dialog';
import { CommonModule } from '@angular/common';
import { RouteService } from '../../../core/services/route.service';
import { Venue } from '../services/venue';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastService } from '../../../shared/ui/toast/toast.service';

@Component({
  selector: 'app-edit',
  standalone: true,
  imports: [PageHeader, Card, ReactiveFormsModule, ConfirmDialog, CommonModule],
  templateUrl: './edit.html',
  styleUrls: ['./edit.scss'],
})
export class Edit implements OnInit {
  form: FormGroup;
  submitted = false;
  venueId!: string;
  venueName: string = '';
  errorMessage: string = '';

  isDialogOpen = false;
  dialogConfig: ConfirmDialogConfig = { title: '', message: '' };

  routeNavigate = inject(RouteService);
  private cdr = inject(ChangeDetectorRef);

  constructor(
    private fb: FormBuilder,
    private venueService: Venue,
    private route: ActivatedRoute,
    private router: Router,
    private toast: ToastService,
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(255)]],
      province: ['', [Validators.required, Validators.maxLength(255)]],
      addressDetail: ['', [Validators.required, Validators.maxLength(1024)]],
      latitude: [null, [Validators.min(-90), Validators.max(90)]],
      longitude: [null, [Validators.min(-180), Validators.max(180)]],
      capacity: [0, [Validators.required, Validators.min(0)]],
    });
  }

  ngOnInit(): void {
    this.venueId = this.route.snapshot.paramMap.get('id')!;
    this.venueService.getVenueById(this.venueId).subscribe({
      next: (res) => {
        const venue = res.data;
        this.venueName = venue.name;
        this.form.patchValue({
          name: venue.name,
          province: venue.province,
          addressDetail: venue.addressDetail,
          latitude: venue.latitude ?? null,
          longitude: venue.longitude ?? null,
          capacity: venue.capacity,
        });
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Error', 'Cannot load venue');
        this.router.navigate(this.routeNavigate.venues());
      }
    });
  }

  submit() {
    this.submitted = true;
    this.errorMessage = '';

    if (this.form.invalid) return;

    const v = this.form.value;

    const request = {
      name: v.name,
      province: v.province,
      addressDetail: v.addressDetail,
      latitude: v.latitude ? parseFloat(v.latitude) : null,
      longitude: v.longitude ? parseFloat(v.longitude) : null,
      capacity: parseInt(v.capacity),
    };

    this.venueService.updateVenue(this.venueId, request).subscribe({
      next: () => {
        this.submitted = false;
        this.router.navigate(this.routeNavigate.venues());
        this.toast.success('Updated Venue', 'Venue updated successfully!');
      },
      error: (err) => {
        if (err.status === 400 && err.error?.errors) {
          const serverErrors = err.error.errors;
          for (const field in serverErrors) {
            const controlName = field.charAt(0).toLowerCase() + field.slice(1);
            const control = this.form.get(controlName);
            if (control) {
              control.setErrors({ serverError: serverErrors[field][0] });
            }
          }
          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.errorMessage = err?.error?.message ?? 'An error occurred. Please try again.';
          this.toast.error('Update failed!');
        }
      }
    });
  }

  isInvalid(field: string): boolean {
    return this.submitted && (this.form.get(field)?.invalid ?? false);
  }

  openDelete(): void {
    this.dialogConfig = {
      title: 'Delete Venue',
      message: 'Are you sure you want to delete this venue?',
      detail: `Venue: ${this.venueName}`,
      confirmText: 'Delete',
      cancelText: 'Cancel',
      variant: 'danger',
    };
    this.isDialogOpen = true;
  }

  onConfirmed(): void {
    this.isDialogOpen = false;
    this.venueService.deleteVenue(this.venueId).subscribe({
      next: () => {
        this.toast.success('Delete Venue', 'Venue deleted successfully!');
        this.router.navigate(this.routeNavigate.venues());
      },
      error: (err) => {
        const msg = err?.error?.message ?? 'Delete failed!';
        this.toast.error('Delete Venue', msg);
      }
    });
  }

  onCancelled(): void {
    this.isDialogOpen = false;
  }

  onNavigateToVenues() {
    this.router.navigate(this.routeNavigate.venues());
  }
}
