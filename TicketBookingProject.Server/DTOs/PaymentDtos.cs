using System.ComponentModel.DataAnnotations;
using static Azure.Core.HttpHeader;

namespace TicketBookingProject.Server;

public record InitiatePaymentRequest
{
    [Required]
    public int BookingId { get; init; }

    [Required, Range(1, 5)]
    public byte PaymentMethod { get; init; }
    // 1=credit_card, 2=bank_transfer, 3=momo, 4=zalopay, 5=vnpay

    /// <summary>URL redirect sau khi thanh toán xong (cho cổng thanh toán bên ngoài)</summary>
    public string? ReturnUrl { get; init; }
    public string? CancelUrl { get; init; }
}

public record InitiatePaymentResponse(
    int PaymentId,
    string PaymentTransaction,
    long TotalAmount,
    byte PaymentMethod,
    string PaymentMethodLabel,
    /// <summary>URL redirect sang cổng thanh toán (Momo, ZaloPay, VNPay)</summary>
    string? PaymentUrl,
    DateTime ExpiresAt);

// ─────────────────────────────────────────────
// PAYMENT CALLBACK (từ cổng thanh toán gọi về)
// ─────────────────────────────────────────────

public record PaymentCallbackRequest
{
    [Required]
    public string PaymentTransaction { get; init; } = default!;

    [Required]
    public string Status { get; init; } = default!;   // "success" | "failed"

    public string? Signature { get; init; }
    public string? RawData { get; init; }
}

// ─────────────────────────────────────────────
// PAYMENT STATUS
// ─────────────────────────────────────────────

public record PaymentStatusResponse(
    int Id,
    int BookingId,
    string PaymentTransaction,
    byte PaymentMethod,
    string PaymentMethodLabel,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt,
    DateTime UpdatedAt);

// ─────────────────────────────────────────────
// PAYMENT HISTORY (user)
// ─────────────────────────────────────────────

public record PaymentListRequest : PagedRequest
{
    public byte? Status { get; init; }
    public byte? Method { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record PaymentListItemResponse(
    int Id,
    int BookingId,
    string EventName,
    string PaymentTransaction,
    byte PaymentMethod,
    string PaymentMethodLabel,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);

// ─────────────────────────────────────────────
// ADMIN — PAYMENT LIST
// ─────────────────────────────────────────────

public record AdminPaymentListRequest : PagedRequest
{
    public string? Search { get; init; }   // transaction id hoặc user email
    public byte? Status { get; init; }
    public byte? Method { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record AdminPaymentListItemResponse(
    int Id,
    string UserEmail,
    string EventName,
    string PaymentTransaction,
    string PaymentMethodLabel,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);