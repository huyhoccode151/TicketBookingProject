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
    [Required, StringLength(255, MinimumLength = 2)]
    public string Name { get; init; } = default!;

    [Required, StringLength(255)]
    public string Province { get; init; } = default!;

    [Required, StringLength(1024)]
    public string AddressDetail { get; init; } = default!;

    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }

    [Range(0, int.MaxValue)]
    public int Capacity { get; init; }
}

public record UpdateVenueRequest
{
    [StringLength(255, MinimumLength = 2)]
    public string? Name { get; init; }

    [StringLength(255)]
    public string? Province { get; init; }

    [StringLength(1024)]
    public string? AddressDetail { get; init; }

    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }

    [Range(0, int.MaxValue)]
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