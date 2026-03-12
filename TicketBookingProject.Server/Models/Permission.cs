using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string Resource { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
