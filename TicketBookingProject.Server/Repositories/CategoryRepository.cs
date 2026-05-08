using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(TicketBookingProjectContext db) : base(db)
    {
    }
    public async Task<List<string>> GetCategoryNames(string? req)
    {
        if (string.IsNullOrWhiteSpace(req))
            return await _dbset
            .Select(v => v.Name!)
            .ToListAsync();

        var categoryNames = await _dbset.Where(n => n.Name == req).Select(c => c.Name).ToListAsync();

        return categoryNames;
    }

    public async Task<Category?> GetByName(string req) 
    {
        var category = await _dbset.FirstOrDefaultAsync(n => n.Name == req);
        if (category == null) return null;
        return category;
    }

    public async Task<List<Category>> ListCategory()
    {
        var category = await _dbset.ToListAsync();
        return category;
    }

    public async Task<List<TicketWithEventType>>  ListCategoryWithTicketType()
    {
        return await _dbset
        .Select(c => new
        {
            EventType = c.Name,
            Stock = c.Events
                .SelectMany(e => e.TicketTypes)
                .Sum(tt => tt.Quantity),
            Sold = c.Events
                .SelectMany(e => e.TicketTypes)
                .Sum(tt => tt.SoldQuantity)
        })
        .OrderByDescending(x => x.Stock)
        .ThenByDescending(x => x.Sold)
        .Select(x => new TicketWithEventType(
            x.EventType,
            x.Stock,
            x.Sold
        ))
        .ToListAsync();
    }
}
