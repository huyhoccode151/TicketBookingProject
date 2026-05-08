import { inject, Injectable } from "@angular/core";
import { PermissionService } from "./permission-service";

// core/services/route.service.ts
@Injectable({ providedIn: 'root' })
export class RouteService {
  private permission = inject(PermissionService);

  private get slug() { return this.permission.getRoleSlug(); }

  // Manage
  dashboard() { return [`/${this.slug}/dashboard`]; }
  users() { return [`/${this.slug}/users`]; }
  usersCreate() { return [`/${this.slug}/users/create`]; }
  usersEdit(id: string | number) { return [`/${this.slug}/users/edit`, id]; }
  events() { return [`/${this.slug}/events`]; }
  eventsCreate() { return [`/${this.slug}/events/create`]; }
  eventsEdit(id: string | number) { return [`/${this.slug}/events/edit`, id]; }
  bookings() { return [`/${this.slug}/bookings`]; }
  payments() { return [`/${this.slug}/payments`]; }
  roles() { return [`/${this.slug}/roles`]; }
  permissions() { return [`/${this.slug}/permissions`]; }
  auditLogs() { return [`/${this.slug}/audit-logs`]; }
  myBooking() { return [`/${this.slug}/my-booking`]; }
  uiActions() { return [`/${this.slug}/ui-actions`]; }
  venues() { return [`/${this.slug}/venues`]; }
  venuesCreate() { return [`/${this.slug}/venues/create`]; }
  venuesEdit(id: string | number) { return [`/${this.slug}/venues/edit`, id]; }

  // Customer
  customerEvents() { return ['/booking-events-ticket-online']; }
  customerEventShow(id: string | number) { return ['/events/show', id]; }
  customerEventBooking(id: string | number) { return ['/events', id, 'bookings']; }
  customerMyBooking() { return ['/my-booking']; }

  // Standalone
  profile() { return ['/profile']; }
  ticketScan() { return ['/ticket-scan']; }
  changePassword() { return ['/change-password']; }
  forbidden() { return ['/forbidden']; }
  ticketBooked(id: string | number) {
    return ['/ticket-booked', id];
  }
  eventBookingPayment(eventId: string | number, bookingId: string | number) {
    return ['/events', eventId, 'bookings', bookingId, 'payment'];
  }
}
