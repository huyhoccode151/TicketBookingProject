import { Routes } from '@angular/router';
import { authGuard } from './core/guards/Auth.guard';
import { Shell } from './layout/shell/shell';
import { permissionGuard } from './core/guards/permission-guard';
import { UserHeader } from './layout/user-header/user-header';
import { rolePrefixGuard } from './core/guards/role-prefix-guard';

export const APP_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    // Public: login, register...
    path: '',
    loadChildren: () =>
      import('./features/auth/Auth.routes').then((m) => m.AUTH_ROUTES),
  },


  // ─── STANDALONE ROUTES ───────────────────────────────────────────────────

  // Cần permission: ticket:scan (permissionGuard tự check login luôn)
  {
    path: 'ticket-scan',
    canActivate: [permissionGuard],
    data: { permission: 'ticket:scan' },
    loadComponent: () =>
      import('./features/ticket/scan/scan').then(m => m.Scan),
  },

  {
    path: 'forbidden',
    loadComponent: () =>
      import('./features/forbidden/forbidden').then(m => m.Forbidden)
  },


  // ─── PUBLIC / CUSTOMER (UserHeader) ──────────────────────────────────────
  {
    path: '',
    component: UserHeader,
    children: [
      // Public — không cần login
      {
        path: 'booking-events-ticket-online',
        loadComponent: () =>
          import('./features/event/index-cus/index-cus').then(m => m.IndexCus),
      },
      {
        path: 'events/show/:id',
        loadComponent: () =>
          import('./features/event/show/show').then(m => m.Show)
      },

      // Cần login — dùng authGuard vì không cần permission cụ thể
      {
        path: 'events/:id/bookings',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/event/booking/booking').then(m => m.Booking),
      },
      {
        path: 'events/:eventId/bookings/:bookingId/payment',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/payment/pre-pay/pre-pay').then(m => m.PrePay),
      },
      {
        path: 'vnpay-return',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/payment/vnpay-return/vnpay-return').then(m => m.VnpayReturn)
      },
      {
        path: 'ticket-booked/:bookingId',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/ticket/booked/booked').then(m => m.Booked)
      },
      {
        path: 'my-booking',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/ticket/user-booking/user-booking').then(m => m.UserBooking),
      },
      {
        path: 'profile',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/user/profile/profile').then(m => m.Profile),
      },
      {
        path: 'change-password',
        loadComponent: () =>
          import('./features/user/change-password/change-password').then(m => m.ChangePassword),
      },
    ]
  },

  // ─── ADMIN / STAFF SHELL ─────────────────────────────────────────────────
  {
    path: ':roleSlug',
    component: Shell,
    canActivate: [authGuard, rolePrefixGuard], // chỉ cần login là vào Shell
    children: [

      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'my-booking',
        loadComponent: () =>
          import('./features/ticket/user-booking/user-booking').then(m => m.UserBooking)
      },
      {
        path: 'forbidden',
        loadComponent: () =>
          import('./features/forbidden/forbidden').then(m => m.Forbidden)
      },
      {
        path: 'test',
        loadComponent: () =>
          import('./features/test/test').then(m => m.EventSettingComponent)
      },

      // Cần permission: user:manage
      {
        path: 'users',
        canActivate: [permissionGuard],
        data: { permission: 'user:manage' },
        loadComponent: () =>
          import('./features/user/index/index').then(m => m.Index),
      },
      {
        path: 'users/create',
        canActivate: [permissionGuard],
        data: { permission: 'user:manage' },
        loadComponent: () =>
          import('./features/user/create/create').then(m => m.Create)
      },
      {
        path: 'users/edit/:id',
        canActivate: [permissionGuard],
        data: { permission: 'user:manage' },
        loadComponent: () =>
          import('./features/user/edit/edit').then(m => m.Edit)
      },
      {
        path: 'audit-logs',
        canActivate: [permissionGuard],
        data: { permission: 'user:manage' },
        loadComponent: () =>
          import('./features/auditlog/index/index').then(m => m.Index),
      },

      // Cần permission: event:manage
      {
        path: 'events',
        canActivate: [permissionGuard],
        data: { permission: 'event:manage' },
        loadComponent: () =>
          import('./features/event/index').then(m => m.Index),
      },
      {
        path: 'events/create',
        canActivate: [permissionGuard],
        data: { permission: 'event:manage' },
        loadComponent: () =>
          import('./features/event/create/create').then(m => m.Create)
      },
      {
        path: 'events/edit/:id',
        canActivate: [permissionGuard],
        data: { permission: 'event:manage' },
        loadComponent: () =>
          import('./features/event/edit/edit').then(m => m.Edit)
      },

      // Cần permission: booking:manage
      {
        path: 'bookings',
        canActivate: [permissionGuard],
        data: { permission: 'booking:manage' },
        loadComponent: () =>
          import('./features/booking/index/index').then(m => m.Index)
      },

      // Cần permission: payment:manage
      {
        path: 'payments',
        canActivate: [permissionGuard],
        data: { permission: 'payment:manage' },
        loadComponent: () =>
          import('./features/payment/index/index').then(m => m.Index)
      },

      // Cần permission: role:manage
      {
        path: 'roles',
        canActivate: [permissionGuard],
        data: { permission: 'role:manage' },
        loadComponent: () =>
          import('./features/role/index/index').then(m => m.Index)
      },

      {
        path: 'ui-actions',
        canActivate: [permissionGuard],
        data: { permission: 'ui-action:manage' },
        loadComponent: () =>
          import('./features/ui-action/index/index').then(m => m.Index)
      },

      // Cần permission: permission:manage
      {
        path: 'permissions',
        canActivate: [permissionGuard],
        data: { permission: 'permission:manage' },
        loadComponent: () =>
          import('./features/permission/index/index').then(m => m.Index)
      },
      {
        path: 'venues',
        canActivate: [permissionGuard],
        data: { permission: 'venue:manage' },
        loadComponent: () =>
          import('./features/venue/index/index').then(m => m.Index)
      },
      {
        path: 'venues/create',
        canActivate: [permissionGuard],
        data: { permission: 'venue:create' },
        loadComponent: () =>
          import('./features/venue/create/create').then(m => m.Create)
      },
      {
        path: 'venues/edit/:id',
        canActivate: [permissionGuard],
        data: { permission: 'venue:update' },
        loadComponent: () =>
          import('./features/venue/edit/edit').then(m => m.Edit)
      }
    ]
  },

  {
    path: '**',
    redirectTo: 'login',
  }
];
