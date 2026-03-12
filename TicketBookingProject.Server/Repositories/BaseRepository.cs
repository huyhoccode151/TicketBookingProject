using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntities
{
    protected readonly TicketBookingProjectContext _db;
    protected readonly DbSet<T> _dbset;
    public BaseRepository(TicketBookingProjectContext db)
    {
        _db = db;
        _dbset = _db.Set<T>();
    }
    public virtual async Task AddAsync(T entity) => await _dbset.AddAsync(entity);
    public virtual Task ForceDeleteAsync(T entity)
    {
        _dbset.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<int> MultiForceDelete(List<int> ids)
    {
        return await _dbset.Where(e => ids.Contains(e.Id)).ExecuteDeleteAsync();
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbset.ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbset.FindAsync(id);

    public virtual Task UpdateAsync(T entity)
    {
        _dbset.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbset.Where(predicate).ToListAsync().ContinueWith(t => (IEnumerable<T>)t.Result);

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        => await _dbset.FirstOrDefaultAsync(predicate);

    public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate)
        => await _dbset.AnyAsync(predicate);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate == null ? await _dbset.CountAsync() : await _dbset.CountAsync(predicate);

    public virtual IQueryable<T> Query() => _dbset.AsNoTracking();
}
