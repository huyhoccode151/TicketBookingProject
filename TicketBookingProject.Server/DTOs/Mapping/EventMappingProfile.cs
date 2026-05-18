using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class EventMappingProfile : Profile
{
    private static readonly Dictionary<EventStatus, string> StatusLabels = new()
    {
        [EventStatus.Draft] = "Draft",
        [EventStatus.Published] = "Published",
        [EventStatus.Ongoing] = "On Going",
        [EventStatus.Completed] = "Completed",
        [EventStatus.Cancelled] = "Cancelled",
    };

    private static readonly Dictionary<TicketTypeStatus, string> TicketStatusLabels = new()
    {
        [TicketTypeStatus.Hidden] = "Hidden",
        [TicketTypeStatus.OnSale] = "On Sale",
        [TicketTypeStatus.SoldOut] = "Sold out",
        [TicketTypeStatus.Paused] = "Pause",
    };

    private static readonly Dictionary<EventSeatStatus, string> SeatStatusLabels = new()
    {
        [EventSeatStatus.Available] = "Available",
        [EventSeatStatus.Reserved] = "Reserved",
        [EventSeatStatus.Sold] = "Sold",
        [EventSeatStatus.Locked] = "Locked",
    };

    public EventMappingProfile()
    {
        // ── Category → CategoryResponse ───────────────────────
        CreateMap<Category, CategoryResponse>();

        // ── CreateEventRequest → Event ────────────────────────
        CreateMap<CreateEventRequest, Event>()
            .ForMember(d => d.Status, o => o.MapFrom(_ => EventStatus.Draft))
            .ForMember(d => d.TicketTypes, o => o.Ignore())
            .ForMember(d => d.EventPosters, o => o.Ignore());

        CreateMap<UpdateEventRequest, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrganizerId, opt => opt.Ignore())
            .ForMember(dest => dest.EventPosters, opt => opt.Ignore())
            .ForMember(dest => dest.TicketTypes, opt => opt.Ignore());

        // ── Event → EventListItemResponse ─────────────────────
        CreateMap<Event, EventListItemResponse>()
            .ForCtorParam("CategoryName",
                o => o.MapFrom(s => s.Category.Name))
            .ForCtorParam("VenueName",
                o => o.MapFrom(s => s.Venue.Name))
            .ForCtorParam("Province",
                o => o.MapFrom(s => s.Venue.Province))
            .ForCtorParam("OrganizerName",
                o => o.MapFrom(s => $"{s.Organizer.Firstname} {s.Organizer.Lastname}"))
            .ForCtorParam("TicketQuantity", o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Sum(t => t.Quantity) : 0))
            .ForCtorParam("TicketSold", o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Sum(t => t.SoldQuantity) : 0))
            .ForCtorParam("Status",
                o => o.MapFrom(s => (byte)s.Status))
            .ForCtorParam("StatusLabel",
                o => o.MapFrom(s => s.Status.ToString()))
            .ForCtorParam("MinPrice",
                o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Min(t => t.Price) : 0L))
            .ForCtorParam("MaxPrice",
                o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Max(t => t.Price) : 0L))
            .ForCtorParam("ThumbnailUrl",
                o => o.MapFrom(s => s.EventPosters
                    .Where(p => p.ImageType == ImageType.Thumbnail || p.IsPrimary)
                    .Select(p => p.ImageUrl)
                    .FirstOrDefault()))
            .ForCtorParam("IsSaleOpen",
                o => o.MapFrom(s =>
                    s.SaleStartAt <= DateTime.UtcNow &&
                    s.SaleEndAt >= DateTime.UtcNow &&
                    s.Status == EventStatus.Published))
            .ForCtorParam("IsSubscribe", o => o.MapFrom(s => s.EventSubscriptions.Any()));

        CreateMap<Event, RelatedEventResponse>()
            .ForCtorParam("id",
                o => o.MapFrom(s => s.Id))
            .ForCtorParam("name",
                o => o.MapFrom(s => s.Name))
            .ForCtorParam("categoryName",
                o => o.MapFrom(s => s.Category.Name))
            .ForCtorParam("minPrice",
                o => o.MapFrom(s => s.TicketTypes.Any() ? s.TicketTypes.Min(t => t.Price) : 0L))
            .ForCtorParam("thumbnailUrl",
                o => o.MapFrom(s => s.EventPosters
                    .Where(p => p.ImageType == ImageType.Thumbnail || p.IsPrimary)
                    .Select(p => p.ImageUrl)
                    .FirstOrDefault()))
            .ForCtorParam("activeAt",
                o => o.MapFrom(s => s.ActiveAt));

        // ── Event → EventDetailResponse ───────────────────────
        CreateMap<Event, EventDetailResponse>()
            .ForCtorParam("Category",
                o => o.MapFrom(s => s.Category))
            .ForCtorParam("Venue",
                o => o.MapFrom(s => s.Venue))
            .ForCtorParam("Organizer",
                o => o.MapFrom(s => s.Organizer))
            .ForCtorParam("Status",
                o => o.MapFrom(s => (byte)s.Status))
            .ForCtorParam("StatusLabel",
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())))
            .ForCtorParam("Posters",
                o => o.MapFrom(s => s.EventPosters))
            .ForCtorParam("TicketTypes",
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

        CreateMap<UpdateTicketTypeRequest, TicketType>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => _.Id))
            .ForMember(d => d.Status, o => o.MapFrom(_ => TicketTypeStatus.OnSale))
            .ForMember(d => d.Version, o => o.MapFrom(_ => 0))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

        // ── TicketType → TicketTypeResponse ───────────────────
        CreateMap<TicketType, TicketTypeResponse>()
            .ForCtorParam("AvailableQuantity",
                o => o.MapFrom(s => s.Quantity - s.SoldQuantity))
            .ForCtorParam("Status",
                o => o.MapFrom(s => (byte)s.Status))
            .ForCtorParam("StatusLabel",
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

        CreateMap<Category, TicketWithEventType>()
            .ForCtorParam("EventType", o => o.MapFrom(s => s.Name))
            .ForCtorParam("Stock", o => o.MapFrom(s => s.Events.SelectMany(e => e.TicketTypes).Sum(tt => tt.Quantity)))
            .ForCtorParam("Sold", o => o.MapFrom(s => s.Events.SelectMany(e => e.TicketTypes).Sum(tt => tt.SoldQuantity)));
    }
}
