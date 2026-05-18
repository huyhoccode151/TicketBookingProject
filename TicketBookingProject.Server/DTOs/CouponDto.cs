using TicketBookingProject.Server.Enums;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server.DTOs
{

    public record CouponListItemRequest : PagedRequest
    {
        public string? Search {  get; init;  }
        public DiscountType? DiscountType { get; init; }
        public DateTime? DateCreateFrom { get; init; }
        public DateTime? DateCreateTo { get; init; }
        public DateTime? DateExpiredFrom { get; init; }
        public DateTime? DateExpiredTo { get; init; }
    }

    public record CouponListItemResponse(
        int Id,
        string Code,
        DiscountType? DiscountType,
        long? DiscountValue,
        int? MaxUsage,
        int UsedCount,
        long? MinOrderValue,
        DateTime? ExpiredAt,
        DateTime? CreatedAt,
        List<Booking> Bookings,
        User User
    );
}
