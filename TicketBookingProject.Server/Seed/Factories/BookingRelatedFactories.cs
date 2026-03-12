using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class BookingFactory
{
    private static readonly string[] CancelReasons =
    [
        "Hết hạn thanh toán",
        "Khách hàng huỷ đặt vé",
        "Sự kiện bị huỷ",
        "Lỗi hệ thống thanh toán",
    ];

    private static readonly Faker _faker = new("vi");

    public static Booking Create(
        int userId,
        int eventId,
        long totalAmount,
        BookingStatus status = BookingStatus.Confirmed)
    {
        var createdAt = _faker.Date.Past(1).ToUniversalTime();

        return new Booking
        {
            UserId = userId,
            EventId = eventId,
            Status = status,
            TotalAmount = totalAmount,
            ExpiresAt = status == BookingStatus.Pending
                                  ? createdAt.AddMinutes(15)
                                  : null,
            CancelledReason = status is BookingStatus.Cancelled or BookingStatus.Expired
                                  ? _faker.PickRandom(CancelReasons)
                                  : null,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }
}

// ─────────────────────────────────────────────
// BookingDetailFactory
// ─────────────────────────────────────────────
public static class BookingDetailFactory
{
    public static List<BookingDetail> CreateFromEventSeats(
        int bookingId,
        List<(EventSeat Seat, long Price)> items)
        => items.Select(x => new BookingDetail
        {
            BookingId = bookingId,
            EventSeatId = x.Seat.Id,
            Price = x.Price,
        }).ToList();
}

// ─────────────────────────────────────────────
// SeatHoldFactory
// ─────────────────────────────────────────────
public static class SeatHoldFactory
{
    public static SeatHold Create(
        int eventSeatId,
        int userId,
        int bookingId,
        SeatHoldStatus status = SeatHoldStatus.Active)
        => new()
        {
            EventSeatId = eventSeatId,
            UserId = userId,
            BookingId = bookingId,
            Status = status,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            CreatedAt = DateTime.UtcNow,
        };
}
