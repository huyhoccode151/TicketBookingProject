import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient, HttpParams } from '@angular/common/http';
import { VenueDetailResponse, VenueListItemResponse } from '../models/venue';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Venue {
  private api = environment.apiUrl + '/Venue';

  constructor(private http: HttpClient) { }

  getVenues(
    page: number,
    pageSize: number,
    search: string,
    province: string
  ): Observable<ApiResponse<PagedResult<VenueListItemResponse>>> {
    const params = new HttpParams()
      .set('Page', page)
      .set('PageSize', pageSize)
      .set('Search', search)
      .set('Province', province);

    return this.http.get<ApiResponse<PagedResult<VenueListItemResponse>>>(
      `${this.api}/list-venue`,
      { params }
    );
  }

  getVenueById(id: string): Observable<ApiResponse<VenueDetailResponse>> {
    return this.http.get<ApiResponse<VenueDetailResponse>>(`${this.api}/${id}`);
  }

  createVenue(request: any): Observable<ApiResponse<VenueDetailResponse>> {
    return this.http.post<ApiResponse<VenueDetailResponse>>(this.api, request);
  }

  updateVenue(id: string, request: any): Observable<ApiResponse<VenueDetailResponse>> {
    return this.http.put<ApiResponse<VenueDetailResponse>>(`${this.api}/${id}`, request);
  }

  deleteVenue(id: string): Observable<any> {
    return this.http.delete(`${this.api}/${id}`);
  }

  getVenueNames(search?: string): Observable<ApiResponse<string[]>> {
    const params = search ? new HttpParams().set('req', search) : undefined;
    return this.http.get<ApiResponse<string[]>>(`${this.api}/names`, { params });
  }
}
