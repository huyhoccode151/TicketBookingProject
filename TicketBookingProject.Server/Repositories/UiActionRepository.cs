using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using TicketBookingProject.Server.DTOs;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UiActionRepository : BaseRepository<UiAction>, IUiActionRepository
{
    public UiActionRepository(TicketBookingProjectContext db) : base(db)
    {
    }

    public async Task<List<UiAction>> GetAllUiActionsAsync()
    {
        return await _db.UiActions
            .Include(x => x.Children)
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<(IQueryable<UiAction>, int TotalCount)> GetListUIAction(ListUIActionRequest req)
    {
        var query = _dbset.AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Keyword))
            query = query.Where(x =>
                x.Label.Contains(req.Keyword) ||
                x.ActionKey.Contains(req.Keyword));

        if (!string.IsNullOrWhiteSpace(req.ActionType))
            query = query.Where(x => x.ActionType == req.ActionType);

        if (req.IsActive.HasValue)
            query = query.Where(x => x.IsActive == req.IsActive.Value);

        var totalCount = await query.CountAsync();

        query = query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize);

        return (query, totalCount);
    }

    public async Task<List<UiAction>> GetActiveNavItemsAsync()
    {
        return await _db.UiActions
            .Include(x => x.Children)
            .Where(x => x.IsActive && x.ActionType == "nav" && x.ParentId == null)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<UiAction?> GetByUiActionIdAsync(int id)
    {
        return await _db.UiActions
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UiAction> CreateAsync(UiAction entity)
    {
        _db.UiActions.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<UiAction?> UpdateUiActionAsync(UiAction entity)
    {
        var existing = await _db.UiActions.FindAsync(entity.Id);
        if (existing is null) return null;

        existing.ActionKey = entity.ActionKey;
        existing.Label = entity.Label;
        existing.Icon = entity.Icon;
        existing.RoutePath = entity.RoutePath;
        existing.PermissionRequired = entity.PermissionRequired;
        existing.ActionType = entity.ActionType;
        existing.ParentId = entity.ParentId;
        existing.DisplayOrder = entity.DisplayOrder;
        existing.IsActive = entity.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.UiActions.FindAsync(id);
        if (existing is null) return false;

        _db.UiActions.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }
}
