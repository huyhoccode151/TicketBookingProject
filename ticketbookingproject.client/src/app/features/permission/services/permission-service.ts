import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreatePermissionDto, Permission, Role, TogglePermissionDto } from '../models/permission';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { environment } from '../../../../environments/environments';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private baseUrl = environment.apiUrl;
  private apiUrl = this.baseUrl + '/Permission';
  private roleUrl = this.baseUrl + '/Role';

  private http = inject(HttpClient);

  getPermissions(action?: string, resource?: string, page: number = 1, pageSize: number = 10): Observable<ApiResponse<PagedResult<Permission>>> {
    let params = new HttpParams().set('Page', page).set('PageSize', pageSize);
    if (action) params = params.set('Action', action);
    if (resource) params = params.set('Resource', resource);
    return this.http.get<ApiResponse<PagedResult<Permission>>>(this.apiUrl, { params });
  }

  createPermission(dto: CreatePermissionDto): Observable<Permission> {
    return this.http.post<Permission>(this.apiUrl, dto);
  }

  togglePermission(dto: TogglePermissionDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/toggle-role-permission`, dto);
  }

  getRoles(): Observable<ApiResponse<PagedResult<Role>>> {
    return this.http.get<ApiResponse<PagedResult<Role>>>(this.roleUrl);
  }
}
