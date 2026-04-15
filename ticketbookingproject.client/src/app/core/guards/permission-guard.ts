import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { PermissionService } from '../services/permission-service';

export const permissionGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const permission = inject(PermissionService);
  const router = inject(Router);

  const required: string = route.data['permission'];

  if (!required || permission.has(required)) return true;

  router.navigate(['/forbidden']);
  return false;
};
