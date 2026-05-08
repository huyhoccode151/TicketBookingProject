import { Component, inject } from '@angular/core';
import { Card } from '../../../shared/ui/card/card';
import { PageHeader } from '../../../shared/ui/page-header/page-header';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouteService } from '../../../core/services/route.service';
import { Venue } from '../services/venue';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create',
  standalone: true,
  imports: [
    Card,
    PageHeader,
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './create.html',
  styleUrls: ['./create.scss'],
})
export class Create {
  form: FormGroup;
  submitted = false;
  errorMessage: string = '';

  routeNavigate = inject(RouteService);

  constructor(
    private fb: FormBuilder,
    private venueService: Venue,
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

    this.venueService.createVenue(request).subscribe({
      next: () => {
        this.form.reset();
        this.submitted = false;
        this.router.navigate(this.routeNavigate.venues());
        this.toast.success('Created Venue', 'Venue created successfully!');
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
          this.toast.error('Create failed!');
        }
      }
    });
  }

  isInvalid(field: string): boolean {
    return this.submitted && (this.form.get(field)?.invalid ?? false);
  }

  onNavigateToVenues() {
    this.router.navigate(this.routeNavigate.venues());
  }
}
