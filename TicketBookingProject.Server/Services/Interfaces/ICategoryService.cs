using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface ICategoryService
{
    Task<List<string>> ListCategoryNames(string? req);
    Task<Category?> GetCategoryByName(string name);
    Task<List<CategoryResponse>> ListCategory();
}
