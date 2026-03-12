using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Role : IEntities
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
