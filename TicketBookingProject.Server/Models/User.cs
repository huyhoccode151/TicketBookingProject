using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class User : IEntities, IDelEntity
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Password { get; set; }

    public int AccessFailedCount { get; set; }

    public Gender? Gender { get; set; }

    public UserStatus Status { get; set; }

    public LoginType LoginType { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<EventSeatLog> EventSeatLogs { get; set; } = new List<EventSeatLog>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual ICollection<SeatHold> SeatHolds { get; set; } = new List<SeatHold>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
