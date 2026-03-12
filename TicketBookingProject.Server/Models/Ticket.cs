using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int EventSeatId { get; set; }

    public string QrCode { get; set; } = null!;

    public TicketStatus Status { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public int? CheckedInBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User? CheckedInByNavigation { get; set; }

    public virtual EventSeat EventSeat { get; set; } = null!;
}
