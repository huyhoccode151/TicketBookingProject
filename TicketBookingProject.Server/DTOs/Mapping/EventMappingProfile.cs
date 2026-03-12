using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class EventMappingProfile : Profile
{
    private static readonly Dictionary<EventStatus, string> StatusLabels = new()
    {
        [EventStatus.Draft] = "Nháp",
        [EventStatus.Published] = "Đã đăng",
        [EventStatus.Ongoing] = "Đang diễn ra",
        [EventStatus.Completed] = "Đã kết thúc",
        [EventStatus.Cancelled] = "Đã huỷ",
    };

    private static readonly Dictionary<TicketTypeStatus, string> TicketStatusLabels = new()
    {
        [TicketTypeStatus.Hidden] = "Ẩn",
        [TicketTypeStatus.OnSale] = "Đang bán",
        [TicketTypeStatus.SoldOut] = "Hết vé",
        [TicketTypeStatus.Paused] = "Tạm dừng",
    };

    private static readonly Dictionary<EventSeatStatus, string> SeatStatusLabels = new()
    {
        [EventSeatStatus.Available] = "Còn trống",
        [EventSeatStatus.Reserved] = "Đang giữ",
        [EventSeatStatus.Sold] = "Đã bán",
        [EventSeatStatus.Locked] = "Bị khoá",
    };

    public EventMappingProfile()
    {
        // ── Category → CategoryResponse ───────────────────────
        CreateMap<Category, CategoryResponse>();

        // ── CreateEventRequest → Event ────────────────────────
        CreateMap<CreateEventRequest, Event>()
            .ForMember(d => d.Status, o => o.MapFrom(_ => EventStatus.Draft))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.DeletedAt, o => o.Ignore());

        // ── Event → EventListItemResponse ─────────────────────
        CreateMap<Event, EventListItemResponse>()
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.VenueName,
                o => o.MapFrom(s => s.Venue.Name))
            .ForMember(d => d.Province,
                o => o.MapFrom(s => s.Venue.Province))
            .ForMember(d => d.OrganizerName,
                o => o.MapFrom(s => $"{s.Organizer.Firstname} {s.Organizer.Lastname}"))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())))
            .ForMember(d => d.MinPrice,
                o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Min(t => t.Price) : 0L))
            .ForMember(d => d.MaxPrice,
                o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Max(t => t.Price) : 0L))
            .ForMember(d => d.ThumbnailUrl,
                o => o.MapFrom(s => s.EventPosters
                    .Where(p => p.ImageType == ImageType.Thumbnail || p.IsPrimary)
                    .Select(p => p.ImageUrl)
                    .FirstOrDefault()))
            .ForMember(d => d.IsSaleOpen,
                o => o.MapFrom(s =>
                    s.SaleStartAt <= DateTime.UtcNow &&
                    s.SaleEndAt >= DateTime.UtcNow &&
                    s.Status == EventStatus.Published));

        // ── Event → EventDetailResponse ───────────────────────
        CreateMap<Event, EventDetailResponse>()
            .ForMember(d => d.Category,
                o => o.MapFrom(s => s.Category))
            .ForMember(d => d.Venue,
                o => o.MapFrom(s => s.Venue))
            .ForMember(d => d.Organizer,
                o => o.MapFrom(s => s.Organizer))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())))
            .ForMember(d => d.Posters,
                o => o.MapFrom(s => s.EventPosters))
            .ForMember(d => d.TicketTypes,
                o => o.MapFrom(s => s.TicketTypes));

        // ── Venue → EventVenueResponse ────────────────────────
        CreateMap<Venue, EventVenueResponse>();

        // ── User → EventOrganizerResponse ─────────────────────
        CreateMap<User, EventOrganizerResponse>();

        // ── EventPoster → EventPosterResponse ─────────────────
        CreateMap<EventPoster, EventPosterResponse>()
            .ForMember(d => d.ImageType, o => o.MapFrom(s => (byte)s.ImageType));

        // ── CreateTicketTypeRequest → TicketType ──────────────
        CreateMap<CreateTicketTypeRequest, TicketType>()
            .ForMember(d => d.SoldQuantity, o => o.MapFrom(_ => 0))
            .ForMember(d => d.Status, o => o.MapFrom(_ => TicketTypeStatus.OnSale))
            .ForMember(d => d.Version, o => o.MapFrom(_ => 0))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

        // ── TicketType → TicketTypeResponse ───────────────────
        CreateMap<TicketType, TicketTypeResponse>()
            .ForMember(d => d.AvailableQuantity,
                o => o.MapFrom(s => s.Quantity - s.SoldQuantity))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => TicketStatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── EventSeat → EventSeatResponse ─────────────────────
        CreateMap<EventSeat, EventSeatResponse>()
            .ForMember(d => d.Row,
                o => o.MapFrom(s => s.Seat.Row))
            .ForMember(d => d.SeatNumber,
                o => o.MapFrom(s => s.Seat.SeatNumber))
            .ForMember(d => d.SeatType,
                o => o.MapFrom(s => (byte)s.Seat.SeatType))
            .ForMember(d => d.TicketTypeName,
                o => o.MapFrom(s => s.TicketType.Name))
            .ForMember(d => d.Price,
                o => o.MapFrom(s => s.Price ?? s.TicketType.Price))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => SeatStatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── VenueSection → EventSectionSeatResponse ───────────
        CreateMap<VenueSection, EventSectionSeatResponse>()
            .ForMember(d => d.SectionId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Seats, o => o.Ignore());  // populated from event_seats in service
    }
}
