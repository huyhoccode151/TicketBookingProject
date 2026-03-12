using System.ComponentModel.DataAnnotations;

namespace TicketBookingProject.Server;

public record CategoryResponse(
    int Id,
    string Name,
    string? Description);

// ─────────────────────────────────────────────
// EVENT — LIST
// ─────────────────────────────────────────────

public record EventListRequest : PagedRequest
{
    public string? Search { get; init; }
    public int? CategoryId { get; init; }
    public int? VenueId { get; init; }
    public byte? Status { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public bool OnSaleOnly { get; init; } = false;
}

public record EventListItemResponse(
    int Id,
    string Name,
    string CategoryName,
    string VenueName,
    string Province,
    string OrganizerName,
    byte Status,
    string StatusLabel,
    DateTime ActiveAt,
    DateTime EndAt,
    DateTime SaleStartAt,
    DateTime SaleEndAt,
    long MinPrice,
    long MaxPrice,
    string? ThumbnailUrl,
    bool IsSaleOpen);

// ─────────────────────────────────────────────
// EVENT — DETAIL
// ─────────────────────────────────────────────

public record EventDetailResponse(
    int Id,
    string Name,
    string Description,
    CategoryResponse Category,
    EventVenueResponse Venue,
    EventOrganizerResponse Organizer,
    byte Status,
    string StatusLabel,
    DateTime ActiveAt,
    DateTime EndAt,
    DateTime SaleStartAt,
    DateTime SaleEndAt,
    int MaxTicketsPerBooking,
    List<EventPosterResponse> Posters,
    List<TicketTypeResponse> TicketTypes,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record EventVenueResponse(
    int Id,
    string Name,
    string Province,
    string AddressDetail,
    decimal? Latitude,
    decimal? Longitude);

public record EventOrganizerResponse(
    int Id,
    string Username,
    string Firstname,
    string Lastname);

// ─────────────────────────────────────────────
// EVENT — CREATE / UPDATE
// ─────────────────────────────────────────────

public record CreateEventRequest
{
    [Required, StringLength(512, MinimumLength = 5)]
    public string Name { get; init; } = default!;

    [Required]
    public string Description { get; init; } = default!;

    [Required]
    public int VenueId { get; init; }

    [Required]
    public int CategoryId { get; init; }

    [Required]
    public DateTime ActiveAt { get; init; }

    [Required]
    public DateTime EndAt { get; init; }

    [Required]
    public DateTime SaleStartAt { get; init; }

    [Required]
    public DateTime SaleEndAt { get; init; }

    [Range(1, 100)]
    public int MaxTicketsPerBooking { get; init; } = 10;

    [Required, MinLength(1)]
    public List<CreateTicketTypeRequest> TicketTypes { get; init; } = [];
}

public record UpdateEventRequest
{
    [StringLength(512, MinimumLength = 5)]
    public string? Name { get; init; }

    public string? Description { get; init; }

    public int? CategoryId { get; init; }

    public DateTime? ActiveAt { get; init; }
    public DateTime? EndAt { get; init; }
    public DateTime? SaleStartAt { get; init; }
    public DateTime? SaleEndAt { get; init; }

    [Range(1, 100)]
    public int? MaxTicketsPerBooking { get; init; }
}

public record UpdateEventStatusRequest
{
    [Required, Range(0, 4)]
    public byte Status { get; init; }
}

// ─────────────────────────────────────────────
// EVENT POSTER
// ─────────────────────────────────────────────

public record EventPosterResponse(
    int Id,
    string ImageUrl,
    byte ImageType,   // 0=poster, 1=banner, 2=thumbnail
    bool IsPrimary);

public record UploadEventPosterRequest
{
    [Required, Range(0, 2)]
    public byte ImageType { get; init; }

    public bool IsPrimary { get; init; } = false;

    // ImageUrl được set sau khi upload file lên storage
    [Required]
    public string ImageUrl { get; init; } = default!;
}

// ─────────────────────────────────────────────
// TICKET TYPE
// ─────────────────────────────────────────────

public record TicketTypeResponse(
    int Id,
    string Name,
    long Price,
    int Quantity,
    int SoldQuantity,
    int AvailableQuantity,
    int MaxPerUser,
    byte Status,
    string StatusLabel);

public record CreateTicketTypeRequest
{
    [Required, StringLength(255, MinimumLength = 1)]
    public string Name { get; init; } = default!;

    [Required, Range(0, long.MaxValue)]
    public long Price { get; init; }

    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Range(0, 100)]
    public int MaxPerUser { get; init; } = 0;
}

public record UpdateTicketTypeRequest
{
    [StringLength(255, MinimumLength = 1)]
    public string? Name { get; init; }

    [Range(0, long.MaxValue)]
    public long? Price { get; init; }

    [Range(1, int.MaxValue)]
    public int? Quantity { get; init; }

    [Range(0, 100)]
    public int? MaxPerUser { get; init; }

    [Range(0, 3)]
    public byte? Status { get; init; }
}

// ─────────────────────────────────────────────
// EVENT SEAT MAP
// ─────────────────────────────────────────────

public record EventSeatMapResponse(
    int EventId,
    string EventName,
    List<EventSectionSeatResponse> Sections);

public record EventSectionSeatResponse(
    int SectionId,
    string SectionName,
    List<EventSeatResponse> Seats);

public record EventSeatResponse(
    int Id,
    int SeatId,
    string Row,
    string SeatNumber,
    byte SeatType,
    int TicketTypeId,
    string TicketTypeName,
    long Price,
    byte Status,          // 0=available, 1=reserved, 2=sold, 3=locked
    string StatusLabel);
