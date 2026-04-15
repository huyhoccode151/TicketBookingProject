namespace TicketBookingProject.Server.Enums
{
    public static class AuditAction
    {
        public const string Login = "login";
        public const string BookingCreated = "booking_created";
        public const string BookingCancelled = "booking_cancelled";
        public const string PaymentSuccess = "payment_success";
    }
}
