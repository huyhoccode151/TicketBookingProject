import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environments';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BookingTicketListItemResponse, Ticket, TicketListRequest } from '../models/ticket';
import { ApiResponse, PagedResult } from '../../user/models/paged-result';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private baseUrl = environment.apiUrl;
  private api = this.baseUrl + '/Ticket';

  private http = inject(HttpClient);

  getTicketsByBookingId(bookingId: number) {
    return this.http.get<ApiResponse<Ticket[]>>(`${this.api}/${bookingId}`);
  }

  private mockBookings: BookingTicketListItemResponse[] = [
    {
      id: 9021,
      eventName: 'Neon Nights World Tour',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuATFr6NIqdExW-ZyKsav5fI4gC67OYBAN366KoBt0M8AWY2zieDVArIO3wpDr2Z39CYvhA3p2pck53cY86_-w_jQlhNMRXA5NleyPYn7tOp1s3tiFQllpfC2JgwHqowGNGmCwxBRWvGwfs_AOqHrMS5iUF4jxzinWLREn4yuq9xOpCmQGP96TDZEYr_gSmUHzs73uYYLZ_tFCihylOSCNogMaQPKi4XV539FLbhceeAu9bATIYdMxlSWpuwfP4FbFlOJr9z6DEII5dA',
      venueName: 'Grand Stadium, Sector A',
      eventActiveAt: '2024-10-24T20:00:00Z',
      seatCount: 3,
      totalAmount: 4500000,
      status: 1,
      statusLabel: 'Confirmed',
      createdAt: '2024-10-01T10:00:00Z',
      tickets: [
        {
          id: 9021,
          sectionName: 'Section A',
          row: '12',
          seatNumber: 'S4',
          seatType: 1,
          ticketTypeName: 'VIP GOLD',
          price: 2000000,
          qrCode: 'QR-9021',
          status: 1,
          statusLabel: 'Valid',
          createdAt: '2024-10-01T10:00:00Z'
        },
        {
          id: 8842,
          sectionName: 'Section B',
          row: '05',
          seatNumber: 'S1',
          seatType: 2,
          ticketTypeName: 'STANDARD',
          price: 1000000,
          qrCode: 'QR-8842',
          status: 2,
          statusLabel: 'Used',
          createdAt: '2024-10-01T10:05:00Z'
        },
        {
          id: 7721,
          sectionName: 'Section A',
          row: '01',
          seatNumber: 'S10',
          seatType: 1,
          ticketTypeName: 'VIP GOLD',
          price: 2000000,
          qrCode: 'QR-7721',
          status: 3,
          statusLabel: 'Cancelled',
          createdAt: '2024-10-01T10:10:00Z'
        }
      ]
    },
    {
      id: 9022,
      eventName: 'Phantom of the Opera',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBvq4x8HECea0fMoORnBriJjTO67QAg8C6dSiOWWxY7aIT9OjnBTylihqvwx9xi7Vl03R7Hdhc-93QolOCT-udSqeb9WffFpR-Qcn1TZA2Ekaa_6APMDsWjy2EzLzKkNMo1xacI_knHcYQhbzf6a9umCIsYysKpJSPQ6wSu04hYqbBGvKhXiG-8dmDwZ4gE2p1rnG0IDI99BZDuJQzeMR3fDSIRJRH_KmqtemzKB8_EupVikw35eT-3YMNaLMaqYsAEukT4bfT4awYi',
      venueName: 'The Grand Lyric Theatre',
      eventActiveAt: '2024-11-12T19:30:00Z',
      seatCount: 2,
      totalAmount: 3000000,
      status: 2,
      statusLabel: 'Pending',
      createdAt: '2024-10-05T14:00:00Z',
      tickets: []
    },
    {
      id: 9023,
      eventName: 'Championship Finals 2024',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCYLyxgYmbqhIrQ8YJywU3pj5UlQUKkvAWngqialXhhccg3dGuWqhABh6eZgdPPPLRZOp45ZjQ6w4u83IptW6JYf6gQn_C1Ppe1TK1wSnelZoz8vcVpRrt8kgZrG6xPivRxWGDW0ED0zljzzFGZ7O_slVI-SJtKIapr5q96lCPBlDf7XrZEffIgxEH_9vfFmG1TNK_bBvKxCe_RsN5VhoilLjjOG5bIrVKFKB9KBHsfB2tCKZmR5Z-e9JwjlUtaeRFc5axRX-45MV23',
      venueName: 'City Center Arena',
      eventActiveAt: '2024-12-01T18:00:00Z',
      seatCount: 1,
      totalAmount: 500000,
      status: 1,
      statusLabel: 'Confirmed',
      createdAt: '2024-11-20T09:00:00Z',
      tickets: []
    },
    {
      id: 9024,
      eventName: 'Summer Beats 2024',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCU6KyZPBI_jeL7aIV9e69MmbHl3imn5QIb4Q96_8aI1LB9gO1ycfla2PERlhUdUkW1sGAJ5BtSnUODrcqRdR8VWJ7yESmDdIveTYlq597-xmUugnLuFz93TsTYU9hDlELJd15zUYBC82CBTZ0hcI6QXhfi62pD8GQa9iAHzJs8UoWINwEUynXxgEEJys7i0pTQeFBaUi6opeFVtWMPRHCQ7rz3_py_eGoMO7NRPV2JmLPPgB8vdx8qjgUrfGxPwGkynEK4Y92EmUiz',
      venueName: 'Coastal Park',
      eventActiveAt: '2024-06-15T12:00:00Z',
      seatCount: 4,
      totalAmount: 2000000,
      status: 1,
      statusLabel: 'Confirmed',
      createdAt: '2024-05-01T11:00:00Z',
      tickets: []
    }
  ];

  getTicketsByUserId(req: TicketListRequest): Observable<ApiResponse<PagedResult<BookingTicketListItemResponse>>> {
    const params = new HttpParams()
      .set('Page', req.page)
      .set('PageSize', req.pageSize)
      .set('Search', req.searchTemp)
      .set('Status', req.status);

    return this.http.get<ApiResponse<PagedResult<BookingTicketListItemResponse>>>(this.api + '/my-bookings', { params });

    //let bookings = [...this.mockBookings];

    //// Filter by Event Name
    //if (req.eventName) {
    //  const search = req.eventName.toLowerCase();
    //  bookings = bookings.filter(b => b.eventName.toLowerCase().includes(search));
    //}

    //// Filter by Status
    //if (req.status !== undefined && req.status !== null) {
    //  bookings = bookings.filter(b => b.status === req.status);
    //}

    //// Filter by Venue Name
    //if (req.venueName) {
    //  const search = req.venueName.toLowerCase();
    //  bookings = bookings.filter(b => b.venueName.toLowerCase().includes(search));
    //}

    //// Sort
    //if (req.sortDesc) {
    //  bookings.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    //} else {
    //  bookings.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
    //}

    //// Pagination
    //const startIndex = (req.page - 1) * req.pageSize;
    //const paginatedBookings = bookings.slice(startIndex, startIndex + req.pageSize);

    //return of(paginatedBookings);
  }
}
