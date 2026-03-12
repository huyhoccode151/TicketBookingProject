using AutoMapper;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class PaymentMappingProfile : Profile
{
    private static readonly Dictionary<PaymentStatus, string> StatusLabels = new()
    {
        [PaymentStatus.Pending] = "Chờ thanh toán",
        [PaymentStatus.Success] = "Thành công",
        [PaymentStatus.Failed] = "Thất bại",
        [PaymentStatus.Refunded] = "Đã hoàn tiền",
    };

    private static readonly Dictionary<PaymentMethod, string> MethodLabels = new()
    {
        [PaymentMethod.CreditCard] = "Thẻ tín dụng",
        [PaymentMethod.BankTransfer] = "Chuyển khoản",
        [PaymentMethod.Momo] = "MoMo",
        [PaymentMethod.ZaloPay] = "ZaloPay",
        [PaymentMethod.VnPay] = "VNPay",
    };

    public PaymentMappingProfile()
    {
        // ── Payment → PaymentStatusResponse ───────────────────
        CreateMap<Payment, PaymentStatusResponse>()
            .ForMember(d => d.PaymentMethod,
                o => o.MapFrom(s => (byte)s.PaymentMethod))
            .ForMember(d => d.PaymentMethodLabel,
                o => o.MapFrom(s => MethodLabels.GetValueOrDefault(s.PaymentMethod, s.PaymentMethod.ToString())))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Payment → PaymentListItemResponse ─────────────────
        CreateMap<Payment, PaymentListItemResponse>()
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.Booking.Event.Name))
            .ForMember(d => d.PaymentMethod,
                o => o.MapFrom(s => (byte)s.PaymentMethod))
            .ForMember(d => d.PaymentMethodLabel,
                o => o.MapFrom(s => MethodLabels.GetValueOrDefault(s.PaymentMethod, s.PaymentMethod.ToString())))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));

        // ── Payment → AdminPaymentListItemResponse ────────────
        CreateMap<Payment, AdminPaymentListItemResponse>()
            .ForMember(d => d.UserEmail,
                o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.EventName,
                o => o.MapFrom(s => s.Booking.Event.Name))
            .ForMember(d => d.PaymentMethodLabel,
                o => o.MapFrom(s => MethodLabels.GetValueOrDefault(s.PaymentMethod, s.PaymentMethod.ToString())))
            .ForMember(d => d.Status,
                o => o.MapFrom(s => (byte)s.Status))
            .ForMember(d => d.StatusLabel,
                o => o.MapFrom(s => StatusLabels.GetValueOrDefault(s.Status, s.Status.ToString())));
    }
}
