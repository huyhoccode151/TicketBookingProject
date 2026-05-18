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
  ticket(id: string | number) {
    return [`/ticket`, id];
  }
  eventBookingPayment(eventId: string | number, bookingId: string | number) {
    return ['/events', eventId, 'bookings', bookingId, 'payment'];
  }


  getRoutePath(actionKey: string): string[] {

    switch (actionKey) {

      case 'nav.events.manage':
        return this.events();

      case 'nav.bookings.manage':
        return this.bookings();

      //case 'nav.tickets.manage':
      //  return ['/', this.permission.getRoleSlug(), 'tickets'];

      case 'nav.venues.manage':
        return this.venues();

      case 'nav.users.manage':
        return this.users();

      case 'nav.roles.manage':
        return this.roles();

      case 'nav.permissions.manage':
        return this.permissions();

      case 'nav.payments.manage':
        return this.payments();

      //case 'nav.refunds.manage':
      //return ['/', this.permission.getRoleSlug(), 'refunds'];

      //case 'nav.reports.manage':
      //return ['/', this.permission.getRoleSlug(), 'reports'];

      case 'nav.audit-log.manage':
        return this.auditLogs();

      default:
        return ['/'];
    }
  }
}
