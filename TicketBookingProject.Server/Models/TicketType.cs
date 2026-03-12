using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class TicketType
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string? Name { get; set; }

    public long Price { get; set; }

    public int Quantity { get; set; }

    public int SoldQuantity { get; set; }

    public int MaxPerUser { get; set; }

    public TicketTypeStatus Status { get; set; }

    public int Version { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<EventSeat> EventSeats { get; set; } = new List<EventSeat>();
}
