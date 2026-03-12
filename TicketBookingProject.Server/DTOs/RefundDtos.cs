using System.ComponentModel.DataAnnotations;
using static Azure.Core.HttpHeader;

namespace TicketBookingProject.Server;

public record RequestRefundRequest
{
    [Required]
    public int BookingId { get; init; }

    [Required, StringLength(255, MinimumLength = 5)]
    public string Reason { get; init; } = default!;
}

public record RefundResponse(
    int Id,
    int BookingId,
    int PaymentId,
    long Amount,
    string Reason,
    byte Status,
    string StatusLabel,
    DateTime? ProcessedAt,
    DateTime CreatedAt);

// ─────────────────────────────────────────────
// REFUND LIST (user)
// ─────────────────────────────────────────────

public record RefundListRequest : PagedRequest
{
    public byte? Status { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record RefundListItemResponse(
    int Id,
    string EventName,
    long Amount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);

// ─────────────────────────────────────────────
// ADMIN — PROCESS REFUND
// ─────────────────────────────────────────────

public record ProcessRefundRequest
{
    [Required, Range(1, 3)]
    public byte Status { get; init; }
    // 1 = approved, 2 = rejected, 3 = completed

    [StringLength(255)]
    public string? Note { get; init; }
}

public record AdminRefundListRequest : PagedRequest
{
    public string? Search { get; init; }
    public byte? Status { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record AdminRefundDetailResponse(
    int Id,
    string UserEmail,
    string UserFullName,
    string EventName,
    int BookingId,
    int PaymentId,
    string PaymentTransaction,
    long Amount,
    string Reason,
    byte Status,
    string StatusLabel,
    string? ProcessedByName,
    DateTime? ProcessedAt,
    DateTime CreatedAt);