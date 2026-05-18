using Microsoft.Identity.Client;
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
    public List<string>? Category { get; init; }
    public string? Venue { get; init; }
    public EventStatus? Status { get; init; }
    public DatePreset? DatePreset { get; init; }
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
    int TicketQuantity,
    int TicketSold,
    EventStatus Status,
    string StatusLabel,
    DateTime ActiveAt,
    DateTime EndAt,
    DateTime SaleStartAt,
    DateTime SaleEndAt,
    long MinPrice,
    long MaxPrice,
    string? ThumbnailUrl,
    bool IsSaleOpen,
    bool IsSubscribe);

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
    EventStatus Status,
    string StatusLabel,
    DateTime? ActiveAt,
    DateTime? EndAt,
    DateTime? SaleStartAt,
    DateTime? SaleEndAt,
    int MaxTicketsPerBooking,
    List<EventPosterResponse> Posters,
    List<TicketTypeResponse> TicketTypes,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);

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

public record CreateEventRequest : IValidatableObject
{
    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(512, MinimumLength = 5, ErrorMessage = "Event name must be between 5 and 512 characters.")]
    public string Name { get; init; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; init; } = default!;

    [Required(ErrorMessage = "Venue is required.")]
    public int VenueId { get; init; }

    [Required(ErrorMessage = "Category is required.")]
    public int CategoryId { get; init; }

    [Required(ErrorMessage = "Active date is required.")]
    public DateTime ActiveAt { get; init; }

    [Required(ErrorMessage = "End date is required.")]
    public DateTime EndAt { get; init; }

    [Required(ErrorMessage = "Sale start date is required.")]
    public DateTime SaleStartAt { get; init; }

    [Required(ErrorMessage = "Sale end date is required.")]
    public DateTime SaleEndAt { get; init; }

    [Range(1, 100, ErrorMessage = "Max tickets per booking must be between 1 and 100.")]
    public int MaxTicketsPerBooking { get; init; } = 10;

    [Required(ErrorMessage = "At least one ticket type is required.")]
    [MinLength(1, ErrorMessage = "At least one ticket type must be provided.")]
    public List<CreateTicketTypeRequest> TicketTypes { get; init; } = [];

    [Required(ErrorMessage = "At least one poster is required.")]
    [MinLength(1, ErrorMessage = "You must upload at least one poster.")]
    public List<IFormFile> Posters { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt <= ActiveAt)
        {
            yield return new ValidationResult(
                "End date must be after active date.",
                new[] { nameof(EndAt) }
            );
        }

        if (SaleEndAt <= SaleStartAt)
        {
            yield return new ValidationResult(
                "Sale end date must be after sale start date.",
                new[] { nameof(SaleEndAt) }
            );
        }

        if (SaleStartAt > ActiveAt)
        {
            yield return new ValidationResult(
                "Sale start date must be before event start date.",
                new[] { nameof(SaleStartAt) }
            );
        }
    }
}

public record UpdateEventRequest : IValidatableObject
{
    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(512, MinimumLength = 5, ErrorMessage = "Event name must be between 5 and 512 characters.")]
    public string Name { get; init; } = default!;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; init; } = default!;

    [Required(ErrorMessage = "Venue is required.")]
    public int VenueId { get; init; }

    [Required(ErrorMessage = "Category is required.")]
    public int CategoryId { get; init; }

    [Required(ErrorMessage = "Active date is required.")]
    public DateTime ActiveAt { get; init; }

    [Required(ErrorMessage = "End date is required.")]
    public DateTime EndAt { get; init; }

    [Required(ErrorMessage = "Sale start date is required.")]
    public DateTime SaleStartAt { get; init; }

    [Required(ErrorMessage = "Sale end date is required.")]
    public DateTime SaleEndAt { get; init; }

    [Range(1, 100, ErrorMessage = "Max tickets per booking must be between 1 and 100.")]
    public int MaxTicketsPerBooking { get; init; } = 10;

    [Required(ErrorMessage = "At least one ticket type is required.")]
    [MinLength(1, ErrorMessage = "At least one ticket type must be provided.")]
    public List<UpdateTicketTypeRequest> TicketTypes { get; init; } = [];

    public List<IFormFile>? Posters { get; init; } = [];
    [Required(ErrorMessage = "Poster metadata is required.")]
    public string PosterMeta { get; init; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt <= ActiveAt)
        {
            yield return new ValidationResult(
                "End date must be after active date.",
                new[] { nameof(EndAt) }
            );
        }

        if (SaleEndAt <= SaleStartAt)
        {
            yield return new ValidationResult(
                "Sale end date must be after sale start date.",
                new[] { nameof(SaleEndAt) }
            );
        }

        if (SaleStartAt > ActiveAt)
        {
            yield return new ValidationResult(
                "Sale start date must be before event start date.",
                new[] { nameof(SaleStartAt) }
            );
        }

        //if (PosterMeta.Select(p => p.Is))
        //{

        //}
    }
}

public class PosterMetaDto
{
    public int? PosterId { get; set; }
    public bool IsPrimary { get; set; }
}

public record UpdateEventStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    [EnumDataType(typeof(EventStatus), ErrorMessage = "Invalid event status.")]
    public EventStatus Status { get; init; }
    [StringLength(512, ErrorMessage = "Cancel reason must be ")]
    public string? CancelReason { get; set; }
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
    public int? Id { get; init; }
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

public record UploadPosterRequest(
    int Index,
    string Url
    );

public record TicketTypeHoldRequest
{
    public int Id { get; set; }
    public int Quantity { get; set; }
}

public record HoldTicketsRequest
{
    [Required, MinLength(1)]
    public List<TicketTypeHoldRequest> Items { get; init; } = new List<TicketTypeHoldRequest>();
    
    public int EventSeatIds { get; init; }
}
public record TicketWithEventType(
    string EventType,
    int Stock,
    int Sold
    );

public record EventTrendingResponse(
    string ImageUrl,
    string EventName,
    int Sold,
    int Stock
    );