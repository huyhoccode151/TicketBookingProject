import { Routes } from '@angular/router';
import { authGuard } from './core/guards/Auth.guard';

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
    path: '**',
    redirectTo: 'login',
  },
];
