import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { first, Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { passwordMatchValidator } from '../../user/services/user';
import { ToastService } from '../../../shared/ui/toast/toast.service';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './sign-up.html',
  styleUrls: ['./sign-up.scss'],
})
export class SignUp implements OnInit, OnDestroy {
  signUpForm!: FormGroup;
  showPassword: boolean = false;
  isLoading: boolean = false;
  errorMessage: string = '';
  submitted: boolean = false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
  ) { }
  private router = inject(Router);
  private toast = inject(ToastService);

  ngOnInit(): void {
    this.buildForm();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get usernameControl(): AbstractControl {
    return this.signUpForm.get('username')!;
  }

  get passwordControl(): AbstractControl {
    return this.signUpForm.get('password')!;
  }

  get confirmPasswordControl(): AbstractControl {
    return this.signUpForm.get('confirmpassword')!;
  }

  get emailControl(): AbstractControl {
    return this.signUpForm.get('email')!;
  }

  get firstnameControl(): AbstractControl {
    return this.signUpForm.get('firstname')!;
  }

  get lastnameControl(): AbstractControl {
    return this.signUpForm.get('lastname')!;
  }

  private buildForm(): void {
    this.signUpForm = this.fb.group({
      firstname: ['', [Validators.required, Validators.maxLength(50)]],
      lastname: ['', [Validators.required, Validators.maxLength(50)]],
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmpassword: ['', Validators.required],
    },
      {
        validators: passwordMatchValidator
      });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    this.submitted = true;
    console.log(this.signUpForm.value);
    this.isLoading = true;
    this.errorMessage = '';

    const { firstname, lastname, username, email, password, confirmpassword } = this.signUpForm.value;
    this.authService
      .signup(username, email, password, firstname, lastname, confirmpassword)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/login']);
          this.toast.success('SignUp successfully!!!');
          console.log("Sign up successful");
        },
        error: (err) => {
          this.isLoading = false;

          if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
            const serverError = err.error?.errors ?? err.error?.message;
            console.log(serverError, 'dfsaf');
            for (const field in serverError) {

              let controlName = field.toLowerCase();

              const control = this.signUpForm.get(controlName);

              if (control) {
                control.setErrors({ serverError: serverError[field][0] });
              }
            }

            this.toast.error('Validation failed. Please check your input.');
          } else {
            this.errorMessage = err?.error?.message ?? 'An error occured. Please try again.';
            this.toast.error('SignUp failed!!!');
          }
        }
      });
  }
}
