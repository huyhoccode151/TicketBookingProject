import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environments';
import { ListUIActionRequest, UIActionRequest, UIActionResponse } from '../models/ui-action';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';

@Injectable({ providedIn: 'root' })
export class UIActionService {
  private baseUrl = environment.apiUrl;
  private apiUrl = this.baseUrl + '/UiAction';

  private http = inject(HttpClient);

  getList(req: ListUIActionRequest) {
    let params = new HttpParams()
      .set('Page', req.page?.toString() || '1')
      .set('PageSize', req.pageSize?.toString() || '10');

    if (req.keyword) params = params.set('Keyword', req.keyword);
    if (req.actionType) params = params.set('ActionType', req.actionType);
    if (req.isActive != null) params = params.set('IsActive', req.isActive.toString());

    return this.http.get<ApiResponse<PagedResult<UIActionResponse>>>(this.apiUrl + '/list', { params });
  }

  getById(id: number) {
    return this.http.get<ApiResponse<UIActionResponse>>(this.apiUrl + '/' + id);
  }

  create(req: UIActionRequest) {
    return this.http.post<ApiResponse<UIActionResponse>>(this.apiUrl, req);
  }

  update(id: number, req: UIActionRequest) {
    return this.http.put<ApiResponse<UIActionResponse>>(this.apiUrl + '/' + id, req);
  }

  delete(id: number) {
    return this.http.delete<ApiResponse<boolean>>(this.apiUrl + '/' + id);
  }
}
