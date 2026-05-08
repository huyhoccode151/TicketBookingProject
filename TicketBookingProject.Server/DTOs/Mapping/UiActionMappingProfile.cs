using AutoMapper;
using Microsoft.VisualBasic.FileIO;

namespace TicketBookingProject.Server.DTOs.Mapping
{
    public class UiActionMappingProfile : Profile
    {
        public UiActionMappingProfile()
        {
            // ── UIActionRequest → UIAction ────────────────────────
            CreateMap<UIActionRequest, UiAction>();

            // ── UIAction → UIActionDto ────────────────────────────
            CreateMap<UiAction, UIActionDto>()
                .ForMember(d => d.Children,
                    o => o.MapFrom(s => s.Children));
        }
    }
}
