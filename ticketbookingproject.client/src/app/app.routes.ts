import { Routes } from '@angular/router';
import { authGuard } from './core/guards/Auth.guard';
import { Shell } from './layout/shell/shell';
import { permissionGuard } from './core/guards/permission-guard';

export const APP_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    // Lazy-load toàn bộ auth feature
    path: '',
    loadChildren: () =>
      import('./features/auth/Auth.routes').then((m) => m.AUTH_ROUTES),
  },
  //{
  //  path: 'dashboard',
  //  canActivate: [authGuard],
  //  loadComponent: () =>
  //    import('./features/dashboard/dashboard.component').then(
  //      (m) => m.DashboardComponent
  //    ),
  //},
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard')
            .then(m => m.Dashboard)
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./features/user/index/index')
            .then(m => m.Index),
        canActivate: [permissionGuard],
        data: {permission: 'user:manage'}
      },
      {
        path: 'users/create',
        loadComponent: () =>
          import('./features/user/create/create')
            .then(m => m.Create)
      },
      {
        path: 'users/edit/:id',
        loadComponent: () =>
          import('./features/user/edit/edit')
            .then(m => m.Edit)
      },
      {
        path: 'events',
        loadComponent: () =>
          import('./features/event/index')
            .then(m => m.Index)
      },
      {
        path: 'events/create',
        loadComponent: () =>
          import('./features/event/create/create')
            .then(m => m.Create)
      },
      {
        path: 'events/edit/:id',
        loadComponent: () =>
          import('./features/event/edit/edit')
            .then(m => m.Edit)
      },
      {
        path: 'events/show/:id',
        loadComponent: () =>
          import('./features/event/show/show')
            .then(m => m.Show)
      },
      {
        path: 'audit-logs',
        loadComponent: () =>
          import('./features/auditlog/index/index').then(m => m.Index)
      },
      {
        path: 'events/:id/bookings',
        loadComponent: () =>
          import('./features/event/booking/booking').then(m => m.Booking)
      },
      {
        path: 'events/:eventId/bookings/:bookingId/payment',
        loadComponent: () =>
          import('./features/payment/pre-pay/pre-pay').then(m => m.PrePay)
      },
      {
        path: 'vnpay-return',
        loadComponent: () =>
          import('./features/payment/vnpay-return/vnpay-return').then(m => m.VnpayReturn)
      },
      {
        path: 'ticket-booked/:bookingId',
        loadComponent: () =>
          import('./features/ticket/booked/booked').then(m => m.Booked)
      },
      {
        path: 'bookings',
        loadComponent: () =>
          import('./features/booking/index/index').then(m => m.Index)
      },
      {
        path: 'my-booking',
        loadComponent: () =>
          import('./features/ticket/user-booking/user-booking').then(m => m.UserBooking)
      },
      {
        path: 'payments',
        loadComponent: () =>
          import('./features/payment/index/index').then(m => m.Index)
      },
      {
        path: 'test',
        loadComponent: () =>
          import('./features/test/test').then(m => m.EventSettingComponent)
      },
      {
        path: 'forbidden',
        loadComponent: () =>
          import('./features/forbidden/forbidden').then(m => m.Forbidden)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login',
  }
];
