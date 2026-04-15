import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { BookingTicketDetails } from '../../payment/models/booking_payment';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { HttpClient, HttpParams } from '@angular/common/http';
import { AdminBookingListItemResponse, AdminBookingListRequest } from '../models/booking';

@Injectable({
  providedIn: 'root',
})
export class BookingService {
  private baseUrl = environment.apiUrl;
  private api = this.baseUrl + '/Booking';

  private http = inject(HttpClient);

  getBookingById(id: number) {
    return this.http.get<ApiResponse<BookingTicketDetails>>(`${this.api}/${id}`);
  }

  getListBooking(req: AdminBookingListRequest) {
    const params = new HttpParams()
      .set('Search', req.searchTemp)
      .set('Status', req.status)
      .set('Page', req.page)
      .set('PageSize', req.pageSize)
      .set('DateFrom', req.dateFrom)
      .set('DateTo', req.dateTo)
      .set('SortDesc', req.sortDesc);

    return this.http.get<ApiResponse<PagedResult<AdminBookingListItemResponse>>>(this.api, { params });
  }
}
