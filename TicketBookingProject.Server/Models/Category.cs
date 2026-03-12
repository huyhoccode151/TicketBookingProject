using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
