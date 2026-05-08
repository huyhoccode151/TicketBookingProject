import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environments';
import { PermissionService } from '../../../core/services/permission-service';
import { ToastService } from '../../../shared/ui/toast/toast.service';
import { RouteService } from '../../../core/services/route.service';

declare var google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  showPassword: boolean = false;
  isLoading: boolean = false;
  errorMessage: string = '';
  submitted: boolean = false;
  private tokenClient: any;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) { }
  private toast = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);
  private permissionService = inject(PermissionService);
  route = inject(RouteService);

  ngOnInit(): void {
    this.buildForm();
  }

  ngAfterViewInit(): void {
    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.signInByGoogle(response.credential)
    });

    google.accounts.id.renderButton(
      document.getElementById("buttonDiv"),
      { theme: "outline", size: "large" }
    );

    google.accounts.id.prompt();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get usernameControl(): AbstractControl {
    return this.loginForm.get('username')!;
  }

  get passwordControl(): AbstractControl {
    return this.loginForm.get('password')!;
  }

  private buildForm(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    this.submitted = true;

    this.isLoading = true;
    this.errorMessage = '';

    const { username, password } = this.loginForm.value;

    this.authService
      .login(username, password)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.submitted = false;

          const token = localStorage.getItem('access_token');

          const payload = JSON.parse(atob(token!.split('.')[1]));
          const roles: string = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? [];
          console.log(roles);
          

          console.log('Login successful', roles);
          this.toast.success("Login successfully");

          this.permissionService.load();

          const slug = this.permissionService.getRoleSlug();
          const isManage = this.permissionService.hasAny(
            'user:manage', 'event:manage', 'booking:manage',
            'payment:manage', 'role:manage', 'permission:manage'
          );
          console.log('isManage:', isManage, 'slug:', slug);

          if (roles === 'customer' && !isManage)
          {
            this.router.navigate(this.route.customerEvents());
            return;
          }
          else if (isManage) {
            this.router.navigate(this.route.dashboard());
            console.log('Navigating to dashboard for role:', slug);
            return;
          }

          if (this.permissionService.has('ticket:scan')) {
            this.router.navigate(this.route.ticketScan());
            return;
          }

          this.router.navigate(this.route.customerEvents());
          return;
        },
        error: (err) => {
          this.isLoading = false;

          if (err.status === 400 && err.error.errors || err.status === 400 && err.error?.message) {
            const serverError = err.error?.errors ?? err.error?.message;
            console.log(serverError, 'dfsaf');
            for (const field in serverError) {

              let controlName = field.toLowerCase();

              const control = this.loginForm.get(controlName);

              if (control) {
                control.setErrors({ serverError: serverError[field][0] });
              }
            }

            this.toast.error('Validation failed. Please check your input.');
          } else {
            this.errorMessage = err?.error?.message ?? 'An error occured. Please try again.';
            this.toast.error('SignUp failed!!!');
          }
        },
      });
  }

    signInWithGoogle(): void {
      //google.accounts.id.prompt();
      const googleBtn = document.querySelector("#buttonDiv div[role=button]") as HTMLElement;
      googleBtn?.click();
    }

  signInByGoogle(response: string): void {
    this.authService.loginWithGoogle(response).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.submitted = false;

        const token = localStorage.getItem('access_token');

        const payload = JSON.parse(atob(token!.split('.')[1]));
        const permissions: string[] = payload.permission ?? [];

        console.log('Login successful', permissions); console.log('Login successful', permissions);

        if (permissions.includes('user:manage')) {
          this.router.navigate(this.route.dashboard()); // admin
        } else if (permissions.includes('event:manage')) {
          this.router.navigate(this.route.dashboard()); // organizer
        } else {
          this.router.navigate(this.route.customerEvents()); // customer
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message ?? 'An error occurred during login. Please try again.';
        console.log(this.errorMessage);
        console.log("dskfjh", err?.error?.message);
        this.cdr.markForCheck();
      },
    });
  }

  signInWithFacebook(): void {
    this.authService.loginWithFacebook();
  }
}
