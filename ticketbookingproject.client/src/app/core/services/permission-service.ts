import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private permissions: string[] = [];

  load() {
    const token = localStorage.getItem('access_token');
    if (!token) return;

    const payload = JSON.parse(atob(token.split('.')[1]));

    const raw = payload['permission'];

    this.permissions = Array.isArray(raw) ? raw : [raw];
  }

  has(permission: string): boolean {
    return this.permissions.includes(permission);
  }

  hasAny(...perms: string[]): boolean {
    return perms.some(p => this.permissions.includes(p));
  }
}
