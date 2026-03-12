using System.ComponentModel.DataAnnotations;
using static Azure.Core.HttpHeader;

namespace TicketBookingProject.Server;

public record TicketListRequest : PagedRequest
{
    public byte? Status { get; init; }
    public int? EventId { get; init; }
    public int? BookingId { get; init; }
}

public record TicketListItemResponse(
    int Id,
    string EventName,
    DateTime EventActiveAt,
    string VenueName,
    string SectionName,
    string Row,
    string SeatNumber,
    byte SeatType,
    string TicketTypeName,
    long Price,
    string QrCode,
    byte Status,
    string StatusLabel,
    DateTime CreatedAt);

// ─────────────────────────────────────────────
// TICKET DETAIL
// ─────────────────────────────────────────────

public record TicketDetailResponse(
    int Id,
    int BookingId,
    TicketEventDto Event,
    TicketSeatDto Seat,
    string QrCode,
    byte Status,
    string StatusLabel,
    DateTime? CheckedInAt,
    string? CheckedInByName,
    DateTime CreatedAt);

public record TicketEventDto(
    int Id,
    string Name,
    string VenueName,
    string Province,
    string AddressDetail,
    DateTime ActiveAt,
    DateTime EndAt,
    string? PosterUrl);

public record TicketSeatDto(
    string Section,
    string Row,
    string SeatNumber,
    byte SeatType,
    string SeatTypeLabel,
    string TicketTypeName,
    long Price);

// ─────────────────────────────────────────────
// CHECK-IN (staff scan QR)
// ─────────────────────────────────────────────

public record CheckInRequest
{
    [Required]
    public string QrCode { get; init; } = default!;
}

public record CheckInResponse(
    bool Success,
    string Message,
    int? TicketId,
    string? GuestName,
    string? EventName,
    string? SeatInfo,
    byte? TicketStatus,
    string? StatusLabel);

// ─────────────────────────────────────────────
// ADMIN — TICKET MANAGEMENT
// ─────────────────────────────────────────────

public record AdminTicketListRequest : PagedRequest
{
    public string? Search { get; init; }   // QR code hoặc email
    public int? EventId { get; init; }
    public byte? Status { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
