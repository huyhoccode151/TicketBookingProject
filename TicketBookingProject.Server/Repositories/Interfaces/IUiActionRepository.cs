using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public interface IUiActionRepository : IBaseRepository<UiAction>
{
    Task<List<UiAction>> GetAllUiActionsAsync();
    Task<(IQueryable<UiAction>, int TotalCount)> GetListUIAction(ListUIActionRequest req);
    Task<List<UiAction>> GetActiveNavItemsAsync();
    Task<UiAction?> GetByUiActionIdAsync(int id);
    Task<UiAction> CreateAsync(UiAction entity);
    Task<UiAction?> UpdateUiActionAsync(UiAction entity);
    Task<bool> DeleteAsync(int id);
}
