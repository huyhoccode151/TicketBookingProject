import { Component, inject } from '@angular/core';
import { Card } from '../../../shared/ui/card/card';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChangePasswordRequest } from '../models/user';
import { UserService } from '../services/user';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { Router } from '@angular/router';
import { RouteService } from '../../../core/services/route.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [Card, ReactiveFormsModule, CommonModule],
  templateUrl: './change-password.html',
  styleUrls: ['./change-password.scss'],
})
export class ChangePassword {
  form!: FormGroup;
  submitted = false;

  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(RouteService);

  ngOnInit(): void {
    this.form = this.fb.group({
      password: ['', Validators.required],
      newpassword: ['', Validators.required],
      confirmpassword: ['', Validators.required],
    });
  }

  isInvalid(controlName: string) {
    return this.submitted && this.form.get(controlName)?.invalid;
  }

  onSubmit() {
    this.submitted = true;

    if (this.form.invalid) return;

    const v = this.form.value;

    const request: ChangePasswordRequest = {
      currentPassword: v.password,
      newPassword: v.newpassword,
      confirmPassword: v.confirmpassword
    };

    this.userService.changePassword(request).subscribe({
      next: () => {
        this.form.reset();
        this.router.navigate(this.route.profile());
        this.toast.success('Password changed successfully. Password will change in your next login');
      },
      error: (err) => {
        if (err.status === 400 && err.error.errors) {
          const serverError = err.error?.errors;
          console.log(serverError, 'dfsaf');
          for (const field in serverError) {

            let controlName = field.toLowerCase();

            const control = this.form.get(controlName);

            if (control) {
              control.setErrors({ serverError: serverError[field][0] });
            }
          }

          this.toast.error('Validation failed. Please check your input.');
        } else {
          this.toast.error('Change password failed!!!');
        }

        this.toast.error('Failed to change password');
      }
    });
  }
}
