using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class EventPoster : IEntities
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string? ImageUrl { get; set; }

    public ImageType ImageType { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;
}
