using System.Linq.Expressions;

namespace TicketBookingProject.Server;

public interface IBaseRepository<T> where T : class, IEntities
{
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task ForceDeleteAsync(T entity);
    Task<int> MultiForceDelete(List<int> ids);
}
