using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class BookingDetail
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int EventSeatId { get; set; }

    public long Price { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual EventSeat EventSeat { get; set; } = null!;
}
