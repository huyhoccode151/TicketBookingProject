using System.ComponentModel.DataAnnotations;
using TicketBookingProject.Server.Models;

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
    public string? Search { get; init; }   // user email hoặc booking id hoặc event name
    public BookingStatus? Status { get; init; }
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

public record BookingTicketDetails
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    public long TotalAmount { get; set; } = 0;
    public DateTime? ExpiresAt { get; set; }
    public List<BookingDetails> Details { get; set; } = new List<BookingDetails>();
    public List<SeatHolds> SeatHolds { get; set; } = new List<SeatHolds>();
}

public record SeatHolds
{
    public int Id { get; set; }
    public int? EventSeatId { get; set; }
    public int TicketTypeId { get; set; }
    public int Quantity { get; set; }
}

    public record BookingDetails
{
    public int? EventSeatId { get; set; }

    public long Price { get; set; }

    public int Quantity { get; set; }

    public int TicketTypeId { get; set; }
    public string? TicketTypeName { get; set; } = string.Empty;
}

public class BookingResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }

    public List<BookingDetailResponse> Details { get; set; } = new List<BookingDetailResponse>();
}

public class BookingDetailResponse
{
    public int? EventSeatId { get; set; }
    public int TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class QuickPayResquest
{
    public string orderInfo { get; set; }
    public string partnerCode { get; set; }
    public string redirectUrl { get; set; }
    public string ipnUrl { get; set; }
    public long amount { get; set; }
    public string orderId { get; set; }
    public string requestId { get; set; }
    public string requestType { get; set; }
    public string extraData { get; set; }
    public string partnerName { get; set; }
    public string storeId { get; set; }
    public string paymentCode { get; set; }
    public string orderGroupId { get; set; }
    public bool autoCapture { get; set; }
    public string lang { get; set; }
    public string signature { get; set; }
}

public class MomoResponse { public string? payUrl { get; set; } }

public class VnPayResponse { public string? payUrl { get; set; } }