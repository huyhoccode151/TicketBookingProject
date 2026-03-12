using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class EventSeat
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int SeatId { get; set; }

    public int TicketTypeId { get; set; }

    public long? Price { get; set; }

    public EventSeatStatus Status { get; set; }

    public int Version { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<EventSeatLog> EventSeatLogs { get; set; } = new List<EventSeatLog>();

    public virtual Seat Seat { get; set; } = null!;

    public virtual SeatHold? SeatHold { get; set; }

    public virtual TicketType TicketType { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
