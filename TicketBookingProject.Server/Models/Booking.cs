using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Booking : IEntities
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public BookingStatus Status { get; set; }

    public long TotalAmount { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? CancelledReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<EventSeatLog> EventSeatLogs { get; set; } = new List<EventSeatLog>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual ICollection<SeatHold> SeatHolds { get; set; } = new List<SeatHold>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
