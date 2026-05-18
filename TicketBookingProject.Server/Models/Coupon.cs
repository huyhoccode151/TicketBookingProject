using TicketBookingProject.Server.Enums;

namespace TicketBookingProject.Server.Models
{
    public class Coupon : IEntities
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType? DiscountType { get; set; }
        public long? DiscountValue { get; set; }
        public int? MaxUsage { get; set; }
        public int UsedCount { get; set; } = 0;
        public long? MinOrderValue { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual User CreatedByUser { get; set; } = new User();
    }
}
