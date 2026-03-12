using System;
using System.Collections.Generic;

namespace TicketBookingProject.Server.Models;

public partial class Refund
{
    public int Id { get; set; }

    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public long Amount { get; set; }

    public string? Reason { get; set; }

    public RefundStatus Status { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;

    public virtual User? ProcessedByNavigation { get; set; }
}
