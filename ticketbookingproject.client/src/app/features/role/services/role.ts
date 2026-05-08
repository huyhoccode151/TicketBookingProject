import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CreateRoleRequest, ListRoleRequest, RoleListResponse, RoleResponse, UpdateRoleRequest } from '../models/role';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { Permission } from '../../permission/models/permission';

@Injectable({
  providedIn: 'root',
})
export class Role {
  private baseUrl = environment.apiUrl;
  private apiUrl = this.baseUrl + '/Role';

  private http = inject(HttpClient);

  getRoles(req: ListRoleRequest) {
    let params = new HttpParams()
      .set('Page', req.page?.toString() || '1')
      .set('PageSize', req.pageSize?.toString() || '10');

    if (req.keyword) {
      params = params.set('Keyword', req.keyword);
    }

    req.permissionNames?.forEach(p => {
      params = params.append('PermissionNames', p);
    });

    return this.http.get<ApiResponse<PagedResult<RoleListResponse>>>(this.apiUrl, { params });
  }

  getListPermissions(name: string[]) {
    let params = new HttpParams();
    name.forEach(n => {
      params = params.append('Name', n);
    });

    return this.http.get<ApiResponse<string[]>>(this.baseUrl + '/Permission/name', { params });
  }

  updateRole(id: number, names: UpdateRoleRequest) {
    return this.http.patch(this.apiUrl + '/' + id, names);
  }

  getRole(id: number) {
    return this.http.get<ApiResponse<RoleResponse>>(this.apiUrl + '/' + id);
  }

  createRole(req: CreateRoleRequest) {
    return this.http.post<ApiResponse<RoleResponse>>(this.apiUrl, req);
  }

  deleteRole(id: number) {
    return this.http.delete<ApiResponse<boolean>>(this.apiUrl + '/' + id);
  }
}
