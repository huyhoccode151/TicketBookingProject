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
            .ForMember(d => d.Event,
                o => o.MapFrom(s => s.EventSeat.Event))
            .ForMember(d => d.Seat,
                o => o.MapFrom(s => s.EventSeat))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())))
            .ForMember(d => d.CheckedInByName,
                o => o.MapFrom(s => s.CheckedInByNavigation != null
                    ? $"{s.CheckedInByNavigation.Firstname} {s.CheckedInByNavigation.Lastname}"
                    : null));

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
    }
}
