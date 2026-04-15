import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AbstractControl, ValidationErrors } from '@angular/forms';
import { Observable } from 'rxjs';
import { StatUsers, User } from '../models/user';
import { ApiResponse, PagedResult } from '../models/paged-result';
import { environment } from '../../../../environments/environments';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private api = environment.apiUrl + '/User';
  constructor(private http: HttpClient) { }

  getUser(page: number, pageSize: number, searchTemp: string, role: string, status: string, loginType: string): Observable<ApiResponse<PagedResult<User>>> {
    const params = new HttpParams()
      .set('Page', page)
      .set('PageSize', pageSize)
      .set('Search', searchTemp)
      .set('LoginType', loginType)
      .set('Role', role)
      .set('Status', status);

    return this.http.get<ApiResponse<PagedResult<User>>>(this.api, { params });
  }

  createUser(request: any) {
    return this.http.post(this.api, request);
  }

  getUserById(id: string) {
    return this.http.get<ApiResponse<User>>(`${this.api}/${id}`);
  }

  editUser(id: string,request: any) {
    return this.http.patch<ApiResponse<User>>(`${this.api}/${id}`, request);
  }

  deleteUser(id: string) {
    return this.http.delete(`${this.api}/${id}`);
  }

  updatePermissions(id: string, selected: string[]) {
    return this.http.post(`${this.api}/${id}/permissions`, { permissions: selected });
  }

  getRolePermissions(id: string) {
    return this.http.get<ApiResponse<string[]>>(`${this.api}/${id}/roles`);
  }

  getUserPermissions(id: string) {
    return this.http.get<ApiResponse<string[]>>(`${this.api}/${id}/permissions`);
  }

  getStatUsers() {
    return this.http.get<ApiResponse<StatUsers>>(`${this.api}/stats`);
  }
}
export function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirm = control.get('confirm_password')?.value;

  if (!password || !confirm) return null;

  return password === confirm ? null : { passwordMismatch: true };
}
