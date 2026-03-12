using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class VenueSection
{
    public int Id { get; set; }

    public int VenueId { get; set; }

    public string? Name { get; set; }

    public int Capacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual Venue Venue { get; set; } = null!;
}
