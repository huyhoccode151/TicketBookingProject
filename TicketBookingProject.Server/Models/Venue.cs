using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Venue : IEntities
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Province { get; set; }

    public string? AddressDetail { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int Capacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<VenueSection> VenueSections { get; set; } = new List<VenueSection>();
}
