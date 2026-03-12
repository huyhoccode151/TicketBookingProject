using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class VenueMappingProfile : Profile
{
    public VenueMappingProfile()
    {
        // ── CreateVenueRequest → Venue ────────────────────────
        CreateMap<CreateVenueRequest, Venue>()
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

        // ── Venue → VenueListItemResponse ─────────────────────
        CreateMap<Venue, VenueListItemResponse>()
            .ForMember(d => d.SectionCount,
                o => o.MapFrom(s => s.VenueSections.Count));

        // ── Venue → VenueDetailResponse ───────────────────────
        CreateMap<Venue, VenueDetailResponse>()
            .ForMember(d => d.Sections,
                o => o.MapFrom(s => s.VenueSections));

        // ── CreateVenueSectionRequest → VenueSection ──────────
        CreateMap<CreateVenueSectionRequest, VenueSection>()
            .ForMember(d => d.CreatedAt, o => o.MapFrom(_ => DateTime.UtcNow));

        // ── VenueSection → VenueSectionResponse ───────────────
        CreateMap<VenueSection, VenueSectionResponse>()
            .ForMember(d => d.SeatCount,
                o => o.MapFrom(s => s.Seats.Count));

        // ── Seat → SeatResponse ───────────────────────────────
        CreateMap<Seat, SeatResponse>()
            .ForMember(d => d.SeatType,
                o => o.MapFrom(s => (byte)s.SeatType))
            .ForMember(d => d.SeatTypeLabel,
                o => o.MapFrom(s => s.SeatType.ToString()));

        // ── VenueSection → SectionSeatMapResponse ─────────────
        CreateMap<VenueSection, SectionSeatMapResponse>()
            .ForMember(d => d.SectionId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.SectionName, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Seats, o => o.MapFrom(s => s.Seats));
    }
}
