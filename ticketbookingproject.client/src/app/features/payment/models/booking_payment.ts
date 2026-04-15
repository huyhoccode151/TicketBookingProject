/**
 * BookingPayment model
 *
 * Represents payment details for a booking in the client application.
 */

export enum PaymentMethod {
  Card = 'Card',
  Paypal = 'Paypal',
  ApplePay = 'ApplePay',
  GooglePay = 'GooglePay',
  BankTransfer = 'BankTransfer',
  Cash = 'Cash',
  Other = 'Other',
}

export enum PaymentStatus {
  Pending = 'Pending',
  Processing = 'Processing',
  Paid = 'Paid',
  Failed = 'Failed',
  Refunded = 'Refunded',
  Cancelled = 'Cancelled',
}

export interface BookingPaymentDTO {
  id: string;
  bookingId: string;
  amount: number;
  currency?: string;
  method?: PaymentMethod | string;
  status?: PaymentStatus | string;
  transactionId?: string | null;
  metadata?: Record<string, string>;
  createdAt?: string; // ISO
  updatedAt?: string; // ISO
  failureReason?: string | null;
}

export class BookingPayment {
  id: string;
  bookingId: string;
  amount: number;
  currency: string;
  method: PaymentMethod | string;
  status: PaymentStatus | string;
  transactionId?: string | null;
  metadata: Record<string, string>;
  createdAt: Date;
  updatedAt: Date;
  failureReason?: string | null;

  constructor(params: {
    id: string;
    bookingId: string;
    amount: number;
    currency?: string;
    method?: PaymentMethod | string;
    status?: PaymentStatus | string;
    transactionId?: string | null;
    metadata?: Record<string, string>;
    createdAt?: Date;
    updatedAt?: Date;
    failureReason?: string | null;
  }) {
    this.id = params.id;
    this.bookingId = params.bookingId;
    this.amount = params.amount;
    this.currency = params.currency ?? 'USD';
    this.method = params.method ?? PaymentMethod.Card;
    this.status = params.status ?? PaymentStatus.Pending;
    this.transactionId = params.transactionId ?? null;
    this.metadata = params.metadata ?? {};
    this.createdAt = params.createdAt ?? new Date();
    this.updatedAt = params.updatedAt ?? new Date();
    this.failureReason = params.failureReason ?? null;
  }

  static fromDTO(dto: BookingPaymentDTO): BookingPayment {
    return new BookingPayment({
      id: dto.id,
      bookingId: dto.bookingId,
      amount: dto.amount,
      currency: dto.currency ?? 'USD',
      method: (dto.method as PaymentMethod) ?? PaymentMethod.Card,
      status: (dto.status as PaymentStatus) ?? PaymentStatus.Pending,
      transactionId: dto.transactionId ?? null,
      metadata: dto.metadata ?? {},
      createdAt: dto.createdAt ? new Date(dto.createdAt) : new Date(),
      updatedAt: dto.updatedAt ? new Date(dto.updatedAt) : new Date(),
      failureReason: dto.failureReason ?? null,
    });
  }

  toDTO(): BookingPaymentDTO {
    return {
      id: this.id,
      bookingId: this.bookingId,
      amount: this.amount,
      currency: this.currency,
      method: this.method,
      status: this.status,
      transactionId: this.transactionId ?? null,
      metadata: this.metadata,
      createdAt: this.createdAt.toISOString(),
      updatedAt: this.updatedAt.toISOString(),
      failureReason: this.failureReason ?? null,
    };
  }

  isPaid(): boolean {
    return this.status === PaymentStatus.Paid;
  }

  isPending(): boolean {
    return this.status === PaymentStatus.Pending || this.status === PaymentStatus.Processing;
  }

  markProcessing(): void {
    this.status = PaymentStatus.Processing;
    this.updatedAt = new Date();
  }

  markPaid(transactionId: string): void {
    this.status = PaymentStatus.Paid;
    this.transactionId = transactionId;
    this.failureReason = null;
    this.updatedAt = new Date();
  }

  markFailed(reason?: string): void {
    this.status = PaymentStatus.Failed;
    this.failureReason = reason ?? null;
    this.updatedAt = new Date();
  }

  markRefunded(): void {
    this.status = PaymentStatus.Refunded;
    this.updatedAt = new Date();
  }

  updateMetadata(key: string, value: string): void {
    this.metadata = { ...this.metadata, [key]: value };
    this.updatedAt = new Date();
  }

  validate(): { valid: boolean; errors: string[] } {
    const errors: string[] = [];

    if (!this.id || this.id.trim().length === 0) {
      errors.push('id is required');
    }

    if (!this.bookingId || this.bookingId.trim().length === 0) {
      errors.push('bookingId is required');
    }

    if (!Number.isFinite(this.amount) || this.amount < 0) {
      errors.push('amount must be a non-negative number');
    }

    if (!this.currency || this.currency.trim().length === 0) {
      errors.push('currency is required');
    }

    return { valid: errors.length === 0, errors };
  }

  formattedAmount(locale = 'en-US'): string {
    try {
      return new Intl.NumberFormat(locale, {
        style: 'currency',
        currency: this.currency,
      }).format(this.amount);
    } catch {
      // Fallback
      return `${this.currency} ${this.amount.toFixed(2)}`;
    }
  }
}

export interface BookingTicketDetails {
  userId: number;
  eventId: number;
  totalAmount: number;
  expiresAt: string; // hoặc Date nếu bạn parse

  details: BookingDetail[];
  seatHolds: SeatHold[];
}

export interface BookingDetail {
  eventSeatId: number | null;
  ticketTypeId: number;
  ticketTypeName: string | null;
  quantity: number;
  price: number;
}

export interface SeatHold {
  eventSeatId: number | null;
  ticketTypeId: number;
  quantity: number;
}

export interface TicketSelected {
  name: string;
  price: number;
  quantity: number;
}

export interface MomoPaymentRequest {
  amount: number;
  bookingId: string;
  bookingInfo: string;
}

export interface MomoPaymentResponse {
  payUrl: string;
}

export interface VnPayPaymentRequest {
  amount: number;
  bookingId: string;
  bookingInfo: string;
}

export interface VnPayPaymentResponse {
  payUrl: string;
}

export interface AdminPaymentListRequest {
  searchTemp: string;          // transaction id hoặc user email hoặc user name
  status: string;   
  method: string;   
  dateFrom: string;        
  dateTo: string;

  sortDesc: boolean;
  page: number;
  pageSize: number;
}

export interface AdminPaymentListItemResponse {
  id: number;
  userEmail: string;
  eventName: string;
  paymentTransaction: string;
  paymentMethodLabel: string;
  totalAmount: number;
  status: number;           // byte → number
  statusLabel: string;
  createdAt: string;        // DateTime → string
}
