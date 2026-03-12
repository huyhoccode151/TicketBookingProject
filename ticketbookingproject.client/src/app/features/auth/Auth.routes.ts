import { Routes } from '@angular/router';
export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./login/login').then((m) => m.LoginComponent),
      title: 'Sign In - Ticket',
  },
  //{
  //  path: 'register',
  //  loadComponent: () =>
  //    import('./register/register').then((m) => m.LoginComponent),
  //  title: 'Sign Up - Ticket',
  //},
  //{
  //  path: 'forgot-password',
  //  loadComponent: () => import('./forgot-password/forgot-password').then((m) => m.ForgotPasswordComponent),
  //  title: 'Forgot Password - Ticket'
  //}
]
