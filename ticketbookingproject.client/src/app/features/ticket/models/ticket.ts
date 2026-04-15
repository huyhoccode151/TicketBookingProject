export interface Ticket {
  id: number;
  addressDetails: string;
  province: string;
  eventName: string;
  eventActiveAt: string;
  venueName: string;
  ticketTypeName: string;
  seatLabel: string;
  imageUrl: string;
  price: number;
  qrCode: string;
  isCheckedIn: boolean;
  status: boolean;
  statusLabel: string;
  createdAt: Date;
}

export interface TicketBookingListItemResponse {
  id: number;
  sectionName: string | null;
  row: string | null;
  seatNumber: string | null;
  seatType: number | null;
  ticketTypeName: string;
  price: number;
  qrCode: string;
  status: number;
  statusLabel: string;
  createdAt: string;
}

export interface BookingTicketListItemResponse {
  id: number;
  eventName: string;
  imageUrl: string;
  venueName: string;
  eventActiveAt: string;
  seatCount: number;
  totalAmount: number;
  status: number;
  statusLabel: string;
  createdAt: string;
  tickets: TicketBookingListItemResponse[];
}

export interface TicketListRequest {
  searchTemp: string;
  status: string;
  sortDesc: boolean;
  page: number;
  pageSize: number;
}
