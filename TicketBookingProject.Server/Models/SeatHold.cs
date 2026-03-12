using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class SeatHold
{
    public int Id { get; set; }

    public int EventSeatId { get; set; }

    public int UserId { get; set; }

    public int BookingId { get; set; }

    public SeatHoldStatus Status { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual EventSeat EventSeat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
