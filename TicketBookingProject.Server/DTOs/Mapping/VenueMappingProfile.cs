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


        CreateMap<UpdateVenueRequest, Venue>()
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.Events, o => o.Ignore())
            .ForMember(d => d.Seats, o => o.Ignore())
            .ForMember(d => d.VenueSections, o => o.Ignore());
        //CreateMap<CreateVenueRequest, Venue>()
        //    .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
        //    .ForCtorParam("Province", o => o.MapFrom(s => s.Province))
        //    .ForCtorParam("AddressDetail", o => o.MapFrom(s => s.AddressDetail))
        //    .ForCtorParam("Latitude", o => o.MapFrom(s => s.Latitude))
        //    .ForCtorParam("Longitude", o => o.MapFrom(s => s.Longitude))
        //    .ForCtorParam("Capacity", o => o.MapFrom(s => s.Capacity))
        //    .ForCtorParam("CreatedAt", o => o.MapFrom(_ => DateTime.UtcNow))
        //    .ForCtorParam("UpdatedAt", o => o.MapFrom(_ => DateTime.UtcNow));

        // ── Venue → VenueListItemResponse ─────────────────────
        CreateMap<Venue, VenueListItemResponse>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
            .ForCtorParam("Province", o => o.MapFrom(s => s.Province))
            .ForCtorParam("AddressDetail", o => o.MapFrom(s => s.AddressDetail))
            .ForCtorParam("Capacity", o => o.MapFrom(s => s.Capacity))
            .ForCtorParam("SectionCount",
                o => o.MapFrom(s => s.VenueSections.Count));

        // ── Venue → VenueDetailResponse ───────────────────────
        //CreateMap<Venue, VenueDetailResponse>()
        //    .ForMember(d => d.Sections,
        //        o => o.MapFrom(s => s.VenueSections));
        CreateMap<Venue, VenueDetailResponse>()
            .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
            .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
            .ForCtorParam("Province", o => o.MapFrom(s => s.Province))
            .ForCtorParam("AddressDetail", o => o.MapFrom(s => s.AddressDetail))
            .ForCtorParam("Latitude", o => o.MapFrom(s => s.Latitude))
            .ForCtorParam("Longitude", o => o.MapFrom(s => s.Longitude))
            .ForCtorParam("Capacity", o => o.MapFrom(s => s.Capacity))
            .ForCtorParam("Sections", o => o.MapFrom(s => s.VenueSections))
            .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt ?? DateTime.UtcNow))
            .ForCtorParam("UpdatedAt", o => o.MapFrom(s => s.UpdatedAt ?? DateTime.UtcNow));

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
