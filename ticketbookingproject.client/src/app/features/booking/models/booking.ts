export interface AdminBookingListRequest {
  searchTemp: string;
  status: string;
  dateFrom: string;
  dateTo: string;
  sortDesc: boolean;
  page: number;
  pageSize: number;
}

export interface AdminBookingListItemResponse {
  id: number;
  userEmail: string;
  userFullName: string;
  eventName: string;
  seatCount: number;
  totalAmount: number;
  status: number;
  statusLabel: string;
  createdAt: string;
}
