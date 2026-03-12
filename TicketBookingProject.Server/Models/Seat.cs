using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Seat
{
    public int Id { get; set; }

    public int VenueId { get; set; }

    public int SectionId { get; set; }

    public string? Row { get; set; }

    public string? SeatNumber { get; set; }

    public SeatType SeatType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EventSeat> EventSeats { get; set; } = new List<EventSeat>();

    public virtual VenueSection Section { get; set; } = null!;

    public virtual Venue Venue { get; set; } = null!;
}
