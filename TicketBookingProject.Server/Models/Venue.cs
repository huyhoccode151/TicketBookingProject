using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Venue : IEntities
{
    public int Id { get; set; }

    public string? Name { get; set; } = string.Empty;

    public string? Province { get; set; } = string.Empty;

    public string? AddressDetail { get; set; } = string.Empty;

    public decimal? Latitude { get; set; } = decimal.Zero;

    public decimal? Longitude { get; set; } = decimal.Zero;

    public int Capacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<VenueSection> VenueSections { get; set; } = new List<VenueSection>();
}
