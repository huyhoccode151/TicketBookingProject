using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server.DTOs.Mapping
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile() {
            CreateMap<Coupon, CouponListItemResponse>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("Code", o => o.MapFrom(s => s.Code))
                .ForCtorParam("DiscountType", o => o.MapFrom(s => s.DiscountType))
                .ForCtorParam("DiscountValue", o => o.MapFrom(s => s.DiscountValue))
                .ForCtorParam("MaxUsage", o => o.MapFrom(s => s.MaxUsage))
                .ForCtorParam("UsedCount", o => o.MapFrom(s => s.UsedCount))
                .ForCtorParam("MinOrderValue", o => o.MapFrom(s => s.MinOrderValue))
                .ForCtorParam("ExpiredAt", o => o.MapFrom(s => s.ExpiredAt))
                .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt))
                .ForCtorParam("Bookings", o => o.MapFrom(s => s.Bookings))
                .ForCtorParam("User", o => o.MapFrom(s => s.CreatedByUser));
        }
    }
}
