import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { PermissionService } from '../services/permission-service';
import { AuthService } from '../services/auth.service';

export const permissionGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const permission = inject(PermissionService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }

  const required: string = route.data['permission'];

  if (!required || permission.has(required)) return true;

  router.navigate(['/forbidden']);
  return false;
};
