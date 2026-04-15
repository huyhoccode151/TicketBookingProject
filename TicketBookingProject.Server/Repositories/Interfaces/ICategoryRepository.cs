using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<List<string>> GetCategoryNames(string? req);
    Task<Category?> GetByName(string req);

    Task<List<Category>> ListCategory();
}
