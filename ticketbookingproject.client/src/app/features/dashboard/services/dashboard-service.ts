import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient, HttpParams } from '@angular/common/http';
import { EventTrendingResponse, RecentBookingListRequest, TicketWithEventType, TotalVenueResponse } from '../models/dashboard';
import { AdminBookingListItemResponse } from '../../booking/models/booking';
import { ApiResponse } from '../../user/models/paged-result';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private baseUrl = environment.api;
  private api = environment.apiUrl + '/Event';
  private bookApi = environment.apiUrl + '/Booking';
  private userApi = environment.apiUrl + '/User';
  private cateApi = environment.apiUrl + '/Category';
  private venueApi = environment.apiUrl + '/Venue';
  private paymentApi = environment.apiUrl + '/Payment';

  private http = inject(HttpClient);

  getRevenueOverTime() {
    return this.http.get<ApiResponse<TotalVenueResponse[]>>(this.paymentApi + '/total-revenue');
  }

  getCateWithTicNum() {
    return this.http.get<ApiResponse<TicketWithEventType[]>>(this.cateApi + '/cate-with-tic');
  }

  getEventTrending() {
    return this.http.get<ApiResponse<EventTrendingResponse[]>>(this.api + '/event-trending');
  }

  getEventName(req?: string) {
    const params = new HttpParams()
      .set('req', req ?? '');
    return this.http.get<ApiResponse<string[]>>(this.api + '/event-name', { params });
  }

  getUserName(req?: string) {
    const params = new HttpParams()
      .set('req', req ?? '');
    return this.http.get<ApiResponse<string[]>>(this.userApi + '/name', { params });
  }

  getRecentBooking(req?: RecentBookingListRequest) {
    let params = new HttpParams()
      .set('UserName', req != null ? req.userName : '')
      .set('EventName', req != null ? req.eventName : '');

    if (req != null && req.dateFrom) {
      params = params.set('DateFrom', req != null ? req.dateFrom.toString() : '');
    }

    if (req != null && req.dateTo) {
      params = params.set('DateTo', req != null ? req.dateTo.toString() : '');
    }

    return this.http.get<ApiResponse<AdminBookingListItemResponse[]>>(this.bookApi + '/recent-booking', { params });
  }
}
