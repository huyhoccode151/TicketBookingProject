import { Routes } from '@angular/router';
export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./login/login').then((m) => m.LoginComponent),
      title: 'Sign In - Ticket',
  },
  {
    path: 'sign-up',
    loadComponent: () =>
      import('./sign-up/sign-up').then((m) => m.SignUp),
    title: 'Sign Up - Ticket',
  },
  //{
  //  path: 'forgot-password',
  //  loadComponent: () => import('./forgot-password/forgot-password').then((m) => m.ForgotPasswordComponent),
  //  title: 'Forgot Password - Ticket'
  //}
]
