using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Event : IEntities
{
    public int Id { get; set; }

    public int OrganizerId { get; set; }

    public int VenueId { get; set; }

    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public EventStatus Status { get; set; }

    public DateTime? ActiveAt { get; set; }

    public DateTime? EndAt { get; set; }

    public DateTime? SaleStartAt { get; set; }

    public DateTime? SaleEndAt { get; set; }

    public int MaxTicketsPerBooking { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<EventPoster> EventPosters { get; set; } = new List<EventPoster>();

    public virtual ICollection<EventSeat> EventSeats { get; set; } = new List<EventSeat>();

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

    public virtual Venue Venue { get; set; } = null!;
}
