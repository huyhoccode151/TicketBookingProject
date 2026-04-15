using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class BookingDetail : IEntities
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int? EventSeatId { get; set; }

    public long Price { get; set; }

    public int Quantity { get; set; } = 1;

    public int TicketTypeId { get; set; }

    public TicketType TicketType { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual EventSeat? EventSeat { get; set; } = null!;
}
