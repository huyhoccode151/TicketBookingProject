import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { BookingResponse, CategoryListItemResponse, CreateBooking, Event, EventDetailResponse, EventPoster, TicketType, TicketVM, VenueListItemResponse } from '../models/event';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class EventService {
  private baseUrl = environment.api;
  private api = environment.apiUrl + '/Event';
  private bookApi = environment.apiUrl + '/Booking';
  private cateApi = environment.apiUrl + '/Category';
  private venueApi = environment.apiUrl + '/Venue';
  constructor(private http: HttpClient) { }

  getEvent(page: number, pageSize: number, searchTemp: string, venue: string, category: string, status: string, datePreset: string, onsale: boolean, dateFrom?: Date | null, dateTo?: Date | null) {
    let params = new HttpParams()
      .set('Page', page)
      .set('PageSize', pageSize)
      .set('Search', searchTemp)
      .set('Venue', venue)
      .set('Category', category)
      .set('Status', status)
      .set('DatePreset', datePreset)
      .set('OnSaleOnly', onsale);

    if (dateFrom) {
      params = params.set('DateFrom', dateFrom.toISOString());
    }

    if (dateTo) {
      params = params.set('DateTo', dateTo.toISOString());
    }

    return this.http.get<ApiResponse<PagedResult<Event>>>(this.api, { params });
  }
  //note: careful venue & category need to set repo to active -> filterbyname

  getCategory() {
      const params = new HttpParams();

    return this.http.get<ApiResponse<CategoryListItemResponse[]>>(this.cateApi);
  }

  getVenue() {
      const params = new HttpParams();

    return this.http.get<ApiResponse<VenueListItemResponse[]>>(this.venueApi);
  }

  createEvent(formData: FormData) {
    return this.http.post(this.api, formData);
  }

  getEventById(id: number) {
    return this.http.get<ApiResponse<EventDetailResponse>>(`${this.api}/${id}`);
  }

  updateEvent(id: number, formData: FormData)
  {
    return this.http.put(`${this.api}/${id}`, formData);
  }

  deleteEvent(id: number)
  {
    return this.http.delete(`${this.api}/${id}`);
  }

  getEventPoster(id: number) {
    return this.http.get<ApiResponse<EventPoster[]>>(`${this.api}/${id}/poster`);
  }

  getTicketTypes(id: number) {
    return this.http.get<ApiResponse<TicketType[]>>(`${this.api}/${id}/ticket-types`);
  }

  getThumbnail(url?: string): string {
    if (!url) return '';

    // Nếu là link ngoài (http, https)
    if (url.startsWith('http')) {
      return url;
    }

    // Nếu là link local (/uploads/...)
    return this.baseUrl + url;
  }

  //sau này nếu phát triển thêm chọn từng ghế ngồi sẽ sửa lại đầu vào của api!!!
  createBooking(selectedBookings: CreateBooking[]) {
    return this.http.post<ApiResponse<BookingResponse>>(`${this.bookApi}`, { items: selectedBookings });
  }
}
