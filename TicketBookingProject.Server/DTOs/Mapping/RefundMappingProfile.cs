using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class RefundMappingProfile : Profile
{
    private static readonly Dictionary<RefundStatus, string> StatusLabels = new()
    {
        [RefundStatus.Pending] = "Chờ xử lý",
        [RefundStatus.Approved] = "Đã duyệt",
        [RefundStatus.Rejected] = "Từ chối",
        [RefundStatus.Completed] = "Hoàn tất",
    };

    public RefundMappingProfile()
    {
        // ── Refund → RefundResponse ───────────────────────────
        CreateMap<Refund, RefundResponse>()
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Refund → RefundListItemResponse ───────────────────
        CreateMap<Refund, RefundListItemResponse>()
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.Booking.Event.Name))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Refund → AdminRefundDetailResponse ────────────────
        CreateMap<Refund, AdminRefundDetailResponse>()
            .ForMember(d => d.UserEmail,
                o => o.MapFrom(s => s.Booking.User.Email))
            .ForMember(d => d.UserFullName,
                o => o.MapFrom(s => $"{s.Booking.User.Firstname} {s.Booking.User.Lastname}"))
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.Booking.Event.Name))
            .ForMember(d => d.PaymentTransaction,
                o => o.MapFrom(s => s.Payment.PaymentTransaction))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())))
            .ForMember(d => d.ProcessedByName,
                o => o.MapFrom(s => s.ProcessedByNavigation != null
                    ? $"{s.ProcessedByNavigation.Firstname} {s.ProcessedByNavigation.Lastname}"
                    : null));
    }
}
