using System.ComponentModel.DataAnnotations;

namespace TicketBookingProject.Server;

public record VenueListRequest : PagedRequest
{
    public string? Search { get; init; }
    public string? Province { get; init; }
}

public record VenueListItemResponse(
    int Id,
    string Name,
    string Province,
    string AddressDetail,
    int Capacity,
    int SectionCount);

public record VenueDetailResponse(
    int Id,
    string Name,
    string Province,
    string AddressDetail,
    decimal? Latitude,
    decimal? Longitude,
    int Capacity,
    List<VenueSectionResponse> Sections,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateVenueRequest
{
    [Required(ErrorMessage = "Venue name is required.")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Venue name must be between 2 and 255 characters.")]
    public string Name { get; init; } = default!;

    [Required(ErrorMessage = "Province is required.")]
    [StringLength(255, ErrorMessage = "Province must not exceed 255 characters.")]
    public string Province { get; init; } = default!;

    [Required(ErrorMessage = "Address detail is required.")]
    [StringLength(1024, ErrorMessage = "Address detail must not exceed 1024 characters.")]
    public string AddressDetail { get; init; } = default!;

    [Range(typeof(decimal), "-90", "90", ErrorMessage = "Latitude must be between -90 and 90.")]
    public decimal? Latitude { get; init; }

    [Range(typeof(decimal), "-180", "180", ErrorMessage = "Longitude must be between -180 and 180.")]
    public decimal? Longitude { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "Capacity must be greater than or equal to 0.")]
    public int Capacity { get; init; }
}

public record UpdateVenueRequest
{
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 255 characters.")]
    public string? Name { get; init; }

    [StringLength(255, ErrorMessage = "Province must not exceed 255 characters.")]
    public string? Province { get; init; }

    [StringLength(1024, ErrorMessage = "Address detail must not exceed 1024 characters.")]
    public string? AddressDetail { get; init; }
    [Range(typeof(decimal), "-90", "90", ErrorMessage = "Latitude must be between -90 and 90.")]
    public decimal? Latitude { get; init; }
    [Range(typeof(decimal), "-180", "180", ErrorMessage = "Longitude must be between -180 and 180.")]
    public decimal? Longitude { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "Capacity must be a non-negative number.")]
    public int? Capacity { get; init; }
}

// ─────────────────────────────────────────────
// VENUE SECTION
// ─────────────────────────────────────────────

public record VenueSectionResponse(
    int Id,
    string Name,
    int Capacity,
    int SeatCount);

public record CreateVenueSectionRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = default!;

    [Range(0, int.MaxValue)]
    public int Capacity { get; init; }
}

public record UpdateVenueSectionRequest
{
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; init; }

    [Range(0, int.MaxValue)]
    public int? Capacity { get; init; }
}

// ─────────────────────────────────────────────
// SEAT
// ─────────────────────────────────────────────

public record SeatResponse(
    int Id,
    string Row,
    string SeatNumber,
    byte SeatType,       // 0=normal, 1=vip, 2=standing
    string SeatTypeLabel);

public record SeatMapResponse(
    int VenueId,
    string VenueName,
    List<SectionSeatMapResponse> Sections);

public record SectionSeatMapResponse(
    int SectionId,
    string SectionName,
    List<SeatResponse> Seats);

public record CreateSeatsRequest
{
    [Required]
    public int SectionId { get; init; }

    /// <summary>Grid mode: tạo hàng × cột</summary>
    public int? Rows { get; init; }
    public int? SeatsPerRow { get; init; }

    [Range(0, 2)]
    public byte SeatType { get; init; } = 0;
}