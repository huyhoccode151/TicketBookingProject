using System.ComponentModel.DataAnnotations;

namespace TicketBookingProject.Server;

public record HoldSeatsRequest
{
    [Required]
    public int EventId { get; init; }

    [Required, MinLength(1)]
    public List<int> EventSeatIds { get; init; } = [];
}

public record HoldSeatsResponse(
    int BookingId,
    List<HeldSeatDto> HeldSeats,
    DateTime ExpiresAt,
    long TotalAmount);

public record HeldSeatDto(
    int EventSeatId,
    string Row,
    string SeatNumber,
    string SectionName,
    string TicketTypeName,
    long Price);

// ─────────────────────────────────────────────
// CREATE BOOKING (confirm sau khi hold)
// ─────────────────────────────────────────────

public record CreateBookingRequest
{
    [Required]
    public int BookingId { get; init; }   // từ HoldSeatsResponse

    [Required]
    public string PaymentMethod { get; init; } = default!;
}

// ─────────────────────────────────────────────
// BOOKING RESPONSE
// ─────────────────────────────────────────────

public record BookingListRequest : PagedRequest
{
    public byte? Status { get; init; }
    public int? EventId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record BookingListItemResponse(
    int Id,
    string EventName,
    string VenueName,
    DateTime EventActiveAt,
    int SeatCount,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);

public record BookingDetailResponse(
    int Id,
    BookingEventDto Event,
    List<BookingDetailDto> Items,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime? ExpiresAt,
    string? CancelledReason,
    BookingPaymentSummaryDto? Payment,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record BookingEventDto(
    int Id,
    string Name,
    string VenueName,
    string Province,
    string AddressDetail,
    DateTime ActiveAt,
    DateTime EndAt,
    string? PosterUrl);

public record BookingDetailDto(
    int Id,
    int EventSeatId,
    string Row,
    string SeatNumber,
    string SectionName,
    byte SeatType,
    string TicketTypeName,
    long Price,
    string? QrCode,    // chỉ hiển thị khi booking confirmed
    byte? TicketStatus);

public record BookingPaymentSummaryDto(
    int Id,
    string PaymentMethod,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);

// ─────────────────────────────────────────────
// CANCEL BOOKING
// ─────────────────────────────────────────────

public record CancelBookingRequest
{
    [StringLength(255)]
    public string? Reason { get; init; }
}

// ─────────────────────────────────────────────
// SEAT HOLD STATUS (polling từ client)
// ─────────────────────────────────────────────

public record SeatHoldStatusResponse(
    int BookingId,
    byte Status,
    string StatusLabel,
    DateTime ExpiresAt,
    int SecondsRemaining);

// ─────────────────────────────────────────────
// ADMIN — BOOKING LIST
// ─────────────────────────────────────────────

public record AdminBookingListRequest : PagedRequest
{
    public string? Search { get; init; }   // user email hoặc booking id
    public byte? Status { get; init; }
    public int? EventId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record AdminBookingListItemResponse(
    int Id,
    string UserEmail,
    string UserFullName,
    string EventName,
    int SeatCount,
    long TotalAmount,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);