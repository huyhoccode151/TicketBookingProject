import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { BookingDetailResponse } from '../../event/models/event';
import { AdminPaymentListItemResponse, AdminPaymentListRequest, BookingTicketDetails, MomoPaymentRequest, MomoPaymentResponse, VnPayPaymentRequest, VnPayPaymentResponse } from '../models/booking_payment';

@Injectable({
  providedIn: 'root',
})
export class BookingPayments {
  private baseUrl = environment.apiUrl;
  private api = this.baseUrl + '/Booking';

  private http = inject(HttpClient);

  getBookingById(id: number) {
    return this.http.get<ApiResponse<BookingTicketDetails>>(`${this.api}/${id}`);
  }

  paymentByMomo(request: MomoPaymentRequest) {
    console.log('MOMO REQUEST:', request);
    return this.http.post<ApiResponse<MomoPaymentResponse>>(this.baseUrl + '/Payment/create-momo-payment', request);
  }

  paymentByVnPay(request: VnPayPaymentRequest) {
    return this.http.post<ApiResponse<VnPayPaymentResponse>>(this.baseUrl + '/Payment/create-vnpay-payment', request);
  }

  verifyVnPayPayment(params: any) {
    return this.http.get<ApiResponse<{ success: boolean, message?: string, totalAmount: number, orderId: number, transactionId: string, vnPayResponseCode: string }>>(this.baseUrl + '/Payment/vnpay-return', { params });
  }

  getListPayment(req: AdminPaymentListRequest) {
    const params = new HttpParams()
      .set('Page', req.page)
      .set('PageSize', req.pageSize)
      .set('Search', req.searchTemp)
      .set('Status', req.status)
      .set('Method', req.method)
      .set('DateFrom', req.dateFrom)
      .set('DateTo', req.dateTo);

    return this.http.get<ApiResponse<PagedResult<AdminPaymentListItemResponse>>>(this.baseUrl + '/Payment', { params });
  }

  deletePayment(id: number) {
    return this.http.delete(this.baseUrl + '/Payment/' + id);
  }
}

