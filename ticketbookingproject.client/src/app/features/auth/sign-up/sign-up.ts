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
    return this.signUpForm.get('confirm_password')!;
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
      confirm_password: ['', Validators.required],
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
    if (this.signUpForm.invalid) {
      return;
    }
    console.log(this.signUpForm.value);
    this.isLoading = true;
    this.errorMessage = '';

    const { firstname, lastname, username, email, password } = this.signUpForm.value;
    this.authService
      .signup(username, email, password, firstname, lastname)
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
          this.toast.error('SignUp failed!!!');
          this.errorMessage = err?.error?.message ?? 'An error occurred during login. Please try again.';
        }
      });
  }
}
