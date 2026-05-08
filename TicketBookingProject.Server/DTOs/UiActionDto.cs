using System.ComponentModel.DataAnnotations;

namespace TicketBookingProject.Server.DTOs
{
    public class UIActionDto
    {
        public int Id { get; set; }
        public string ActionKey { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RoutePath { get; set; }
        public string? PermissionRequired { get; set; }
        public string ActionType { get; set; } = "nav";
        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public List<UIActionDto> Children { get; set; } = new();
    }

    public class UIActionRequest
    {
        [Required, MaxLength(100)]
        public string ActionKey { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Label { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Icon { get; set; }

        [MaxLength(500)]
        public string? RoutePath { get; set; }

        [MaxLength(200)]
        public string? PermissionRequired { get; set; }

        [MaxLength(50)]
        public string ActionType { get; set; } = "nav";

        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public record ListUIActionRequest : PagedRequest
    {
        public string? Keyword { get; init; }          
        public string? ActionType { get; init; }   
        public bool? IsActive { get; init; }
    }
}
