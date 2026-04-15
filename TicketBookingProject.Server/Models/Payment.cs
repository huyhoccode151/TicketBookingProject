using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Payment : IEntities
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? PaymentTransaction { get; set; }

    public long TotalAmount { get; set; }

    public PaymentStatus Status { get; set; }

    public string? IdempotencyKey { get; set; }

    public string? MetaData { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual User User { get; set; } = null!;
}
