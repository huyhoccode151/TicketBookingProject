import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private permissionsSubject = new BehaviorSubject<string[]>([]);
  private roleSlug = '';
  permissions$ = this.permissionsSubject.asObservable();

  load() {
    const token = localStorage.getItem('access_token');
    const roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    if (!token) {
      this.permissionsSubject.next([]); // ✅ NEW (clear khi guest)
      return;
    }

    const payload = JSON.parse(atob(token.split('.')[1]));
    console.log('Decoded JWT Payload:', payload); // ✅ DEBUG

    this.roleSlug = payload[roleKey] ?? '';

    const raw = payload['permission'];

    const perms = Array.isArray(raw) ? raw : [raw];

    this.permissionsSubject.next(perms);
  }

  has(permission: string): boolean {
    return this.permissionsSubject.value.includes(permission);
  }

  hasAny(...perms: string[]): boolean {
    return perms.some(p => this.permissionsSubject.value.includes(p));
  }

  clear() {
    this.permissionsSubject.next([]);
  }

  getRoleSlug(): string { return this.roleSlug; }
}
