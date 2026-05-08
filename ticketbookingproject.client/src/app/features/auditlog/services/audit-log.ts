import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuditLog, AuditLogQuery } from '../models/audit-log';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { environment } from '../../../../environments/environments';

/**
 * Service for interacting with the Audit Log API.
 * - Base URL assumed to be `/api/auditlogs`. Adjust if your backend uses a different route.
 */
@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private api = environment.apiUrl + '/AuditLog';

  constructor(private http: HttpClient) {}

  /**
   * Get a paginated list of audit logs.
   */
  getLogs(query?: AuditLogQuery): Observable<ApiResponse<PagedResult<AuditLog>>> {
    let params = new HttpParams();
    if (query) {
      if (query.page != null) { params = params.set('Page', String(query.page)); }
      if (query.pageSize != null) { params = params.set('PageSize', String(query.pageSize)); }
      if (query.search) { params = params.set('Search', query.search); }
      //if (query.from) { params = params.set('from', query.from); }
      //if (query.to) { params = params.set('to', query.to); }
      //if (query.userId) { params = params.set('userId', query.userId); }
      //if (query.action) { params = params.set('action', query.action); }
    }

    return this.http
      .get<ApiResponse<PagedResult<AuditLog>>>(this.api, { params })
      .pipe(catchError(this.handleError));
  }

  getMyLogs(): Observable<ApiResponse<AuditLog[]>> {
    return this.http.get<ApiResponse<AuditLog[]>>(`${this.api}/profile`).pipe(catchError(this.handleError));
  }
 
  /**
   * Simple error handler for HTTP requests.
   */
  private handleError = (error: any) => {
    // Keep this minimal; consumers can inspect the error object.
    return throwError(() => error);
  };
}
