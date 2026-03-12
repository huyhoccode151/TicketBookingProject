using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class RefreshToken : IEntities
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string? DeviceInfo { get; set; }

    public string? IpAddress { get; set; }

    public RefreshTokenStatus Status { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public bool IsActive => Status == RefreshTokenStatus.Active && ExpiresAt > DateTime.UtcNow;
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
}
