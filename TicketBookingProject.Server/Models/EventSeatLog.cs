using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class EventSeatLog
{
    public int Id { get; set; }

    public int EventSeatId { get; set; }

    public int? BookingId { get; set; }

    public int? UserId { get; set; }

    public EventSeatStatus? OldStatus { get; set; }

    public EventSeatStatus? NewStatus { get; set; }

    public string? Action { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual EventSeat EventSeat { get; set; } = null!;

    public virtual User? User { get; set; }
}
