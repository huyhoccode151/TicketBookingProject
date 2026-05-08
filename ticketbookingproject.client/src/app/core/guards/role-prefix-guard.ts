// core/guards/role-prefix.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../services/permission-service';

export const rolePrefixGuard: CanActivateFn = (route) => {
  const permission = inject(PermissionService);
  const router = inject(Router);

  const urlSlug = route.paramMap.get('roleSlug');
  const userSlug = permission.getRoleSlug();

  if (!userSlug) {
    return router.createUrlTree(['/login']);
  }

  // User gõ slug của role khác → đá về slug đúng của họ
  if (urlSlug !== userSlug) {
    return router.createUrlTree(['/forbidden']);
  }

  return true;
};
