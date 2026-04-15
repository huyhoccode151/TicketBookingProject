using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class BookingMappingProfile : Profile
{
    private static readonly Dictionary<BookingStatus, string> StatusLabels = new()
    {
        [BookingStatus.Pending] = "Chờ thanh toán",
        [BookingStatus.Confirmed] = "Đã xác nhận",
        [BookingStatus.Cancelled] = "Đã huỷ",
        [BookingStatus.Expired] = "Hết hạn",
        [BookingStatus.Refunded] = "Đã hoàn tiền",
    };

    private static readonly Dictionary<PaymentStatus, string> PaymentStatusLabels = new()
    {
        [PaymentStatus.Pending] = "Chờ thanh toán",
        [PaymentStatus.Success] = "Thành công",
        [PaymentStatus.Failed] = "Thất bại",
        [PaymentStatus.Refunded] = "Đã hoàn tiền",
    };

    private static readonly Dictionary<PaymentMethod, string> PaymentMethodLabels = new()
    {
        [PaymentMethod.CreditCard] = "Thẻ tín dụng",
        [PaymentMethod.BankTransfer] = "Chuyển khoản",
        [PaymentMethod.Momo] = "MoMo",
        [PaymentMethod.ZaloPay] = "ZaloPay",
        [PaymentMethod.VnPay] = "VNPay",
    };

    public BookingMappingProfile()
    {
        // ── Booking → BookingListItemResponse ─────────────────
        CreateMap<Booking, BookingListItemResponse>()
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.Event.Name))
            .ForMember(d => d.VenueName,
                o => o.MapFrom(s => s.Event.Venue.Name))
            .ForMember(d => d.EventActiveAt,
                o => o.MapFrom(s => s.Event.ActiveAt))
            .ForMember(d => d.SeatCount,
                o => o.MapFrom(s => s.BookingDetails.Count))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Booking → BookingDetailResponse ───────────────────
        //CreateMap<Booking, BookingDetailResponse>()
        //    .ForMember(d => d.Event,
        //        o => o.MapFrom(s => s.Event))
        //    .ForMember(d => d.Items,
        //        o => o.MapFrom(s => s.BookingDetails))
        //    .ForMember(d => d.Status,
        //        o => o.MapFrom(s => (byte)s.Status))
        //    .ForMember(d => d.StatusLabel,
        //        o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())))
        //    .ForMember(d => d.Payment,
        //        o => o.MapFrom(s => s.Payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault()));

        // ── Event → BookingEventDto ───────────────────────────
        CreateMap<Event, BookingEventDto>()
            .ForMember(d => d.VenueName,
                o => o.MapFrom(s => s.Venue.Name))
            .ForMember(d => d.Province,
                o => o.MapFrom(s => s.Venue.Province))
            .ForMember(d => d.AddressDetail,
                o => o.MapFrom(s => s.Venue.AddressDetail))
            .ForMember(d => d.PosterUrl,
                o => o.MapFrom(s => s.EventPosters
                    .Where(p => p.IsPrimary)
                    .Select(p => p.ImageUrl)
                    .FirstOrDefault()));

        // ── BookingDetail → BookingDetailDto ──────────────────
        CreateMap<BookingDetail, BookingDetailDto>()
            .ForMember(d => d.Row,
                o => o.MapFrom(s => s.EventSeat.Seat.Row))
            .ForMember(d => d.SeatNumber,
                o => o.MapFrom(s => s.EventSeat.Seat.SeatNumber))
            .ForMember(d => d.SectionName,
                o => o.MapFrom(s => s.EventSeat.Seat.Section.Name))
            .ForMember(d => d.SeatType,
                o => o.MapFrom(s => (byte)s.EventSeat.Seat.SeatType))
            .ForMember(d => d.TicketTypeName,
                o => o.MapFrom(s => s.EventSeat.TicketType.Name))
            .ForMember(d => d.QrCode,
                o => o.MapFrom(s =>
                    s.Booking.Tickets
                        .Where(t => t.EventSeatId == s.EventSeatId)
                        .Select(t => t.QrCode)
                        .FirstOrDefault()
                ))

            .ForMember(d => d.TicketStatus,
                o => o.MapFrom(s =>
                    s.Booking.Tickets
                        .Where(t => t.EventSeatId == s.EventSeatId)
                        .Select(t => (byte?)t.Status)
                        .FirstOrDefault()
                ));

        // ── Payment → BookingPaymentSummaryDto ────────────────
        CreateMap<Payment, BookingPaymentSummaryDto>()
            .ForMember(d => d.PaymentMethod,
                o => o.MapFrom(s => PaymentMethodLabels.GetValueOrDefault(s.PaymentMethod, s.PaymentMethod.ToString())))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => PaymentStatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Booking → AdminBookingListItemResponse ────────────
        CreateMap<Booking, AdminBookingListItemResponse>()
            .ForMember(d => d.UserEmail,
                o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.UserFullName,
                o => o.MapFrom(s => $"{s.User.Firstname} {s.User.Lastname}"))
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.Event.Name))
            .ForMember(d => d.SeatCount,
                o => o.MapFrom(s => s.BookingDetails.Count))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── SeatHold → SeatHoldStatusResponse ─────────────────
        CreateMap<SeatHold, SeatHoldStatusResponse>()
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.SecondsRemaining,
                o => o.MapFrom(s => s.ExpiresAt == null
                ? 0
                : (int)Math.Max(0, (s.ExpiresAt.Value - DateTime.UtcNow).TotalSeconds)));

        // ── EventSeat → HeldSeatDto ───────────────────────────
        CreateMap<EventSeat, HeldSeatDto>()
            .ForMember(d => d.EventSeatId,
                o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Row,
                o => o.MapFrom(s => s.Seat.Row))
            .ForMember(d => d.SeatNumber,
                o => o.MapFrom(s => s.Seat.SeatNumber))
            .ForMember(d => d.SectionName,
                o => o.MapFrom(s => s.Seat.Section.Name))
            .ForMember(d => d.TicketTypeName,
                o => o.MapFrom(s => s.TicketType.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.Price ?? s.TicketType.Price));

        //CreateMap<SeatHold, BookingDetails>()
        //    .ForMember(d => d.EventSeatId, o => o.MapFrom(s => s.EventSeatId))
        //    .ForMember(d => d.TicketTypeId, o => o.MapFrom(s => s.TicketTypeId))
        //    .ForMember(d => d.Price, o => o.MapFrom(s => s.TicketType.Price * s.Quantity));

        CreateMap<Booking, BookingTicketDetails>()
            .ForMember(d => d.UserId, o => o.MapFrom(b => b.UserId))
            .ForMember(d => d.EventId, o => o.MapFrom(b => b.EventId))
            .ForMember(d => d.TotalAmount, o => o.MapFrom(b => b.TotalAmount))
            .ForMember(d => d.ExpiresAt, o => o.MapFrom(b => b.ExpiresAt))
            .ForMember(d => d.Details, o => o.MapFrom(b => b.BookingDetails))
            .ForMember(d => d.SeatHolds, o => o.MapFrom(b => b.SeatHolds));

        CreateMap<SeatHold, SeatHolds>()
            .ForMember(d => d.EventSeatId, o => o.MapFrom(s => s.EventSeatId))
            .ForMember(d => d.TicketTypeId, o => o.MapFrom(s => s.TicketTypeId))
            .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));

        CreateMap<BookingDetail, BookingDetails>()
            .ForMember(d => d.EventSeatId, o => o.MapFrom(s => s.EventSeatId))
            .ForMember(d => d.TicketTypeId, o => o.MapFrom(s => s.TicketTypeId))
            .ForMember(d => d.TicketTypeName, o => o.MapFrom(s => s.TicketType.Name))
            .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price));

        CreateMap<Booking, AdminBookingListItemResponse>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("UserEmail", o => o.MapFrom(s => s.User.Email))
            .ForCtorParam("UserFullName", o => o.MapFrom(s => s.User.Firstname + s.User.Lastname))
            .ForCtorParam("EventName", o => o.MapFrom(s => s.Event.Name))
            .ForCtorParam("SeatCount", o => o.MapFrom(s => s.BookingDetails.Count()))
            .ForCtorParam("TotalAmount", o => o.MapFrom(s => s.TotalAmount))
            .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
            .ForCtorParam("StatusLabel", o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt));
    }
}