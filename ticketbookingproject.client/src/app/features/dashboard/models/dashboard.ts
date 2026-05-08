export interface TotalVenueRequest {
  dateFrom?: string | null;
  dateTo?: string | null;
}

export interface TotalVenueResponse {
  revenue: number;
  date: string;
}
export interface TicketWithEventType {
  eventType: string;
  stock: number;
  sold: number;
}
export interface EventTrendingResponse {
  imageUrl: string;
  eventName: string;
  sold: number;
  stock: number;
}

export interface RecentBookingListRequest {
  userName: string;
  eventName: string;
  dateFrom: string;
  dateTo: string;
}
export interface ChartPoint {
  x: number;
  y: number;
  label: string;
  originalValue: number;
}

export interface ChartSegment {
  label: string;
  value: number;
  percentage: number;
  color: string;
  dashArray: string;
  dashOffset: number;
}
