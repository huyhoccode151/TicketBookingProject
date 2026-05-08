using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketBookingProject.Server;

public partial class UiAction : IEntities
{
    public int Id { get; set; }

    public string ActionKey { get; set; } = string.Empty;  // vd: "nav.payments"

    public string Label { get; set; } = string.Empty;      // vd: "Payments"

    public string? Icon { get; set; }                      // vd: "Payments" (Material icon)

    public string? RoutePath { get; set; }                 // vd: "/payments"

    public string? PermissionRequired { get; set; }        // vd: "payment:manage"

    public string ActionType { get; set; } = "nav";        // "nav" | "button" | "menu"

    public int? ParentId { get; set; }

    public UiAction? Parent { get; set; }
    public ICollection<UiAction> Children { get; set; } = new List<UiAction>();
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
