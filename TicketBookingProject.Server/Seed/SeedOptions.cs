namespace TicketBookingProject.Server;
/// <summary>
/// Bind từ appsettings.json → "SeedOptions".
/// </summary>
public class SeedOptions
{
    // ── Users ────────────────────────────────
    public int AdminCount { get; set; } = 1;
    public int OrganizerCount { get; set; } = 5;
    public int StaffCount { get; set; } = 10;
    public int CustomerCount { get; set; } = 50;

    // ── Venues ───────────────────────────────
    public int VenueCount { get; set; } = 5;
    public int SectionsPerVenue { get; set; } = 4;
    public int RowsPerSection { get; set; } = 10;
    public int SeatsPerRow { get; set; } = 20;

    // ── Events ───────────────────────────────
    public int EventsPerOrganizer { get; set; } = 3;
    public int TicketTypesPerEvent { get; set; } = 3;
    public int PostersPerEvent { get; set; } = 3;

    // ── Bookings ─────────────────────────────
    public int ConfirmedBookingsPerEvent { get; set; } = 10;
    public int PendingBookingsPerEvent { get; set; } = 2;
    public int CancelledBookingsPerEvent { get; set; } = 2;
    public int MaxSeatsPerBooking { get; set; } = 4;

    // ── Refunds ──────────────────────────────
    /// <summary>0.0 – 1.0: fraction of cancelled bookings that generate a refund.</summary>
    public double RefundRatio { get; set; } = 0.5;
}
