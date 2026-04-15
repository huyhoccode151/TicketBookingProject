export interface Event {
  id: number,
  name: string,
  categoryName: string,
  venueName: string,
  province: string,
  organizeName: string,
  ticketQuantity: number,
  ticketSold: number,
  status: string,
  activeAt: Date,
  endAt: Date,
  saleStart: Date,
  saleEnd: Date,
  minPrice: number,
  maxPrice: number,
  thumbnailUrl: string,
  onSale: boolean,
}
export interface Ticket {
  initials: string;
  name: string;
  price: number;
  quantity: string;
  maxPerUser: string;
}

export interface CreateEvent {
  name: string,
  description: string,
  venueId: number,
  categoryId: number,
  activeAt: Date,
  endAt: Date,
  saleStartAt: Date,
  saleEndAt: Date,
  maxTicketPerBooking: number,

}


export interface VenueListItemResponse {
  id: number;
  name: string;
  province: string;
  addressDetail: string;
  capacity: number;
  sectionCount: number;
}
export interface CategoryListItemResponse {
  id: number;
  name: string;
  description: string;
}

export interface EventCategory {
  id: number;
  name: string;
  description?: string;
}

export interface EventVenue {
  id: number;
  name: string;
  province: string;
  addressDetail: string;
  latitude: number;
  longitude: number;
}

export interface EventOrganizer {
  id: number;
  username: string;
  firstname: string;
  lastname: string;
}

export interface EventPoster {
  id: number;
  imageUrl: string;
  imageType: number;
  isPrimary: boolean;
}

export interface TicketType {
  id: number;
  name: string;
  price: number;
  quantity: number;
  soldQuantity: number;
  availableQuantity: number;
  maxPerUser: number;
  status: number;
  statusLabel: string;
}

export interface TicketVM extends TicketType {
  selectedCount: number;
}

export interface CreateBooking {
  id: number;
  quantity: number;
}

export interface BookingResponse {
  id: number;
  userId: number;
  eventId: number;
  status: string;
  createdAt: Date;
  details: BookingDetailResponse[];
}

export interface BookingDetailResponse {
  eventseatId: number;
  ticketTypeId: number;
  price: number;
}


export type EventStatus = 'Draft' | 'Published' | 'Cancelled';

export class EventDetailResponse {
  id: number;
  name: string;
  description: string;

  category?: EventCategory;
  venue?: EventVenue;
  organizer?: EventOrganizer;

  status: string;
  statusLabel: string;

  activeAt: Date;
  endAt: Date;
  saleStartAt: Date;
  saleEndAt: Date;

  maxTicketsPerBooking: number;

  posters: EventPoster[];
  ticketTypes: TicketType[];

  createdAt: Date;
  updatedAt: Date;

  constructor(init?: Partial<EventDetailResponse>) {
    this.id = init?.id ?? 0;
    this.name = init?.name ?? '';
    this.description = init?.description ?? '';

    this.category = init?.category;
    this.venue = init?.venue;
    this.organizer = init?.organizer;

    this.status = init?.status ?? 'Draft';
    this.statusLabel = init?.statusLabel ?? '';

    this.activeAt = init?.activeAt ? new Date(init.activeAt) : new Date();
    this.endAt = init?.endAt ? new Date(init.endAt) : new Date();
    this.saleStartAt = init?.saleStartAt ? new Date(init.saleStartAt) : new Date();
    this.saleEndAt = init?.saleEndAt ? new Date(init.saleEndAt) : new Date();

    this.maxTicketsPerBooking = init?.maxTicketsPerBooking ?? 0;

    this.posters = init?.posters ?? [];
    this.ticketTypes = init?.ticketTypes ?? [];

    this.createdAt = init?.createdAt ? new Date(init.createdAt) : new Date();
    this.updatedAt = init?.updatedAt ? new Date(init.updatedAt) : new Date();
  }

  static fromJson(obj: any): EventDetailResponse {
    if (!obj) throw new Error('Invalid event data');

    return new EventDetailResponse({
      id: obj.id,
      name: obj.name,
      description: obj.description,

      category: obj.category,
      venue: obj.venue,
      organizer: obj.organizer,

      status: obj.status, // string rồi → giữ nguyên
      statusLabel: obj.statusLabel,

      activeAt: obj.activeAt,
      endAt: obj.endAt,
      saleStartAt: obj.saleStartAt,
      saleEndAt: obj.saleEndAt,

      maxTicketsPerBooking: obj.maxTicketsPerBooking,

      posters: Array.isArray(obj.posters) ? obj.posters : [],
      ticketTypes: Array.isArray(obj.ticketTypes) ? obj.ticketTypes : [],

      createdAt: obj.createdAt,
      updatedAt: obj.updatedAt,
    });
  }

  // Helper cực hữu ích
  get primaryPoster(): string | null {
    return this.posters?.find(p => p.isPrimary)?.imageUrl || null;
  }

  get organizerName(): string {
    return `${this.organizer?.firstname ?? ''} ${this.organizer?.lastname ?? ''}`.trim();
  }

  get isOnSale(): boolean {
    const now = new Date();
    return now >= this.saleStartAt && now <= this.saleEndAt;
  }

  get isUpcoming(): boolean {
    return new Date() < this.activeAt;
  }
}
