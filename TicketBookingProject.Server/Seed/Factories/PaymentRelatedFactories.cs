using Bogus;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public static class TicketFactory
{
    private static readonly Faker _faker = new("vi");

    public static Ticket Create(
        int bookingId,
        int eventSeatId,
        TicketStatus status = TicketStatus.Valid,
        int? checkedInBy = null)
    {
        var createdAt = _faker.Date.Past(1).ToUniversalTime();
        var isUsed = status == TicketStatus.Used;

        return new Ticket
        {
            BookingId = bookingId,
            EventSeatId = eventSeatId,
            QrCode = Guid.NewGuid().ToString("N").ToUpper(),
            Status = status,
            CheckedInAt = isUsed ? createdAt.AddHours(_faker.Random.Int(1, 48)) : null,
            CheckedInBy = isUsed ? checkedInBy : null,
            CreatedAt = createdAt,
        };
    }

    public static List<Ticket> CreateFromDetails(
        int bookingId,
        List<BookingDetail> details,
        TicketStatus status = TicketStatus.Valid,
        int? checkedInBy = null)
        => details.Select(d => Create(bookingId, d.EventSeatId ?? 0, status, checkedInBy)).ToList();
}

// ─────────────────────────────────────────────
// PaymentFactory
// ─────────────────────────────────────────────
public static class PaymentFactory
{
    private static readonly Faker _faker = new("vi");

    public static Payment Create(
        int bookingId,
        int userId,
        long amount,
        PaymentStatus status = PaymentStatus.Success)
    {
        var createdAt = _faker.Date.Past(1).ToUniversalTime();

        return new Payment
        {
            BookingId = bookingId,
            UserId = userId,
            PaymentMethod = _faker.PickRandom<PaymentMethod>(),
            PaymentTransaction = Guid.NewGuid().ToString("N"),
            TotalAmount = amount,
            Status = status,
            IdempotencyKey = Guid.NewGuid().ToString("N"),
            MetaData = null,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }

    public static Payment CreateSuccess(int bookingId, int userId, long amount) =>
        Create(bookingId, userId, amount, PaymentStatus.Success);

    public static Payment CreatePending(int bookingId, int userId, long amount) =>
        Create(bookingId, userId, amount, PaymentStatus.Pending);

    public static Payment CreateFailed(int bookingId, int userId, long amount) =>
        Create(bookingId, userId, amount, PaymentStatus.Failed);
}

// ─────────────────────────────────────────────
// RefundFactory
// ─────────────────────────────────────────────
public static class RefundFactory
{
    private static readonly string[] Reasons =
    [
        "Sự kiện bị huỷ bởi ban tổ chức",
        "Khách hàng yêu cầu hoàn tiền",
        "Lỗi thanh toán trùng lặp",
        "Không thể tham dự sự kiện",
        "Vé bị lỗi kỹ thuật",
    ];

    private static readonly Faker _faker = new("vi");

    public static Refund Create(
        int paymentId,
        int bookingId,
        long amount,
        RefundStatus status = RefundStatus.Pending,
        int? processedBy = null)
    {
        var createdAt = _faker.Date.Past(1).ToUniversalTime();

        return new Refund
        {
            PaymentId = paymentId,
            BookingId = bookingId,
            Amount = amount,
            Reason = _faker.PickRandom(Reasons),
            Status = status,
            ProcessedBy = status is RefundStatus.Approved or RefundStatus.Completed or RefundStatus.Rejected
                              ? processedBy
                              : null,
            ProcessedAt = status == RefundStatus.Completed
                              ? createdAt.AddDays(_faker.Random.Int(1, 3))
                              : null,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
        };
    }
}

// ─────────────────────────────────────────────
// EventSeatLogFactory
// ─────────────────────────────────────────────
public static class EventSeatLogFactory
{
    public static EventSeatLog Create(
        int eventSeatId,
        EventSeatStatus oldStatus,
        EventSeatStatus newStatus,
        string action,
        int? bookingId = null,
        int? userId = null)
        => new()
        {
            EventSeatId = eventSeatId,
            BookingId = bookingId,
            UserId = userId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Action = action,
            CreatedAt = DateTime.UtcNow,
        };

    public static EventSeatLog Hold(int eventSeatId, int bookingId, int userId) =>
        Create(eventSeatId, EventSeatStatus.Available, EventSeatStatus.Reserved, "hold", bookingId, userId);

    public static EventSeatLog Confirm(int eventSeatId, int bookingId, int userId) =>
        Create(eventSeatId, EventSeatStatus.Reserved, EventSeatStatus.Sold, "confirm", bookingId, userId);

    public static EventSeatLog Release(int eventSeatId, int bookingId, int userId) =>
        Create(eventSeatId, EventSeatStatus.Reserved, EventSeatStatus.Available, "release", bookingId, userId);

    public static EventSeatLog Cancel(int eventSeatId, int bookingId, int userId) =>
        Create(eventSeatId, EventSeatStatus.Sold, EventSeatStatus.Available, "cancel", bookingId, userId);

    public static EventSeatLog AdminLock(int eventSeatId, int adminId) =>
        Create(eventSeatId, EventSeatStatus.Available, EventSeatStatus.Locked, "admin_lock", userId: adminId);
}
