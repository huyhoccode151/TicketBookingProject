using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class TicketMappingProfile : Profile
{
    private static readonly Dictionary<TicketStatus, string> StatusLabels = new()
    {
        [TicketStatus.Valid] = "Hợp lệ",
        [TicketStatus.Used] = "Đã sử dụng",
        [TicketStatus.Cancelled] = "Đã huỷ",
        [TicketStatus.Expired] = "Hết hạn",
    };

    private static readonly Dictionary<SeatType, string> SeatTypeLabels = new()
    {
        [SeatType.Normal] = "Thường",
        [SeatType.Vip] = "VIP",
        [SeatType.Standing] = "Đứng",
    };

    public TicketMappingProfile()
    {
        // ── Ticket → TicketListItemResponse ───────────────────
        CreateMap<Ticket, TicketListItemResponse>()
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.EventSeat.Event.Name))
            .ForMember(d => d.EventActiveAt,
                o => o.MapFrom(s => s.EventSeat.Event.ActiveAt))
            .ForMember(d => d.VenueName,
                o => o.MapFrom(s => s.EventSeat.Event.Venue.Name))
            .ForMember(d => d.SectionName,
                o => o.MapFrom(s => s.EventSeat.Seat.Section.Name))
            .ForMember(d => d.Row,
                o => o.MapFrom(s => s.EventSeat.Seat.Row))
            .ForMember(d => d.SeatNumber,
                o => o.MapFrom(s => s.EventSeat.Seat.SeatNumber))
            .ForMember(d => d.SeatType,
                o => o.MapFrom(s => (byte)s.EventSeat.Seat.SeatType))
            .ForMember(d => d.TicketTypeName,
                o => o.MapFrom(s => s.EventSeat.TicketType.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.EventSeat.Price ?? s.EventSeat.TicketType.Price))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Ticket → TicketDetailResponse ─────────────────────
        CreateMap<Ticket, TicketDetailResponse>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("BookingId", o => o.MapFrom(s => s.BookingId))
            .ForCtorParam("QrCode", o => o.MapFrom(s => s.QrCode))
            .ForCtorParam("StatusLabel", o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam("VenueName", o => o.MapFrom(s => s.Booking.Event.Venue.Name))
            .ForCtorParam("Province", o => o.MapFrom(s => s.Booking.Event.Venue.Province))
            .ForCtorParam("AddressDetails", o => o.MapFrom(s => s.Booking.Event.Venue.AddressDetail))
            .ForCtorParam("EventName", o => o.MapFrom(s => s.Booking.Event.Name))
            .ForCtorParam("ImageUrl", o => o.MapFrom(s => s.Booking.Event.EventPosters.Where(e => e.IsPrimary == true).Select(p => p.ImageUrl).FirstOrDefault()))
            .ForCtorParam("EventActiveAt", o => o.MapFrom(s => s.Booking.Event.ActiveAt))
            .ForCtorParam("TicketTypeName", o => o.MapFrom(s => s.TicketType.Name))
            .ForCtorParam("SeatLabel", o => o.MapFrom(s => s.EventSeat.Seat.SeatNumber))
            .ForCtorParam("IsCheckedIn", o => o.MapFrom(s => s.CheckedInAt != null))
            .ForCtorParam("CheckedInAt", o => o.MapFrom(s => s.CheckedInAt))
            .ForCtorParam("CheckedInByName", o => o.MapFrom(s => 
                s.CheckedInByNavigation != null
                ? s.CheckedInByNavigation.Username
                : null))
            .ForMember("CreatedAt", o => o.MapFrom(s => s.CreatedAt));

        // ── Event → TicketEventDto ────────────────────────────
        CreateMap<Event, TicketEventDto>()
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

        // ── EventSeat → TicketSeatDto ─────────────────────────
        CreateMap<EventSeat, TicketSeatDto>()
            .ForMember(d => d.Section,
                o => o.MapFrom(s => s.Seat.Section.Name))
            .ForMember(d => d.Row,
                o => o.MapFrom(s => s.Seat.Row))
            .ForMember(d => d.SeatNumber,
                o => o.MapFrom(s => s.Seat.SeatNumber))
            .ForMember(d => d.SeatType,
                o => o.MapFrom(s => (byte)s.Seat.SeatType))
            .ForMember(d => d.SeatTypeLabel,
                o => o.MapFrom(s => SeatTypeLabels.GetValueOrDefault(s.Seat.SeatType, s.Seat.SeatType.ToString())))
            .ForMember(d => d.TicketTypeName,
                o => o.MapFrom(s => s.TicketType.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.Price ?? s.TicketType.Price));

        // ── Booking → BookingTicketListItemResponse ─────────────────────────
        CreateMap<Booking, BookingTicketListItemResponse>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("EventName", o => o.MapFrom(s => s.Event.Name))
            .ForCtorParam("ImageUrl", o => o.MapFrom(s => s.Event.EventPosters.Where(p => p.IsPrimary).Select(p => p.ImageUrl).FirstOrDefault()))
            .ForCtorParam("VenueName", o => o.MapFrom(s => s.Event.Venue.Name))
            .ForCtorParam("EventActiveAt", o => o.MapFrom(s => s.Event.ActiveAt))
            .ForCtorParam("SeatCount", o => o.MapFrom(s => s.BookingDetails.Count()))
            .ForCtorParam("TotalAmount", o => o.MapFrom(s => s.TotalAmount))
            .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
            .ForCtorParam("StatusLabel", o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt))
            .ForCtorParam("Tickets", o => o.MapFrom(s => s.Tickets));

        // ── Ticket → TicketBookingListItemResponse ─────────────────────────
        CreateMap<Ticket, TicketBookingListItemResponse>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("SectionName", o => o.MapFrom(s => s.EventSeat.Seat.Section.Name != null
                                                            ? s.EventSeat.Seat.Section.Name
                                                            : null))
            .ForCtorParam("Row", o => o.MapFrom(s => s.EventSeat.Seat.Row != null
                                                            ? s.EventSeat.Seat.Row
                                                            : null))
            .ForCtorParam("SeatNumber", o => o.MapFrom(s => s.EventSeat.Seat.SeatNumber != null
                                                            ? s.EventSeat.Seat.SeatNumber
                                                            : null))
            .ForCtorParam("SeatType", o => o.MapFrom(s => s.EventSeat.Seat.SeatType))
            .ForCtorParam("TicketTypeName", o => o.MapFrom(s => s.TicketType.Name))
            .ForCtorParam("Price", o => o.MapFrom(s => s.TicketType.Price))
            .ForCtorParam("QrCode", o => o.MapFrom(s => s.QrCode))
            .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
            .ForCtorParam("StatusLabel", o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt));

    }

}
