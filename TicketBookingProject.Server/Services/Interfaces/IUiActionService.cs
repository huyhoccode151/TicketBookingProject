using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public interface IUiActionService
{
    Task<List<UIActionDto>> GetAllAsync();
    Task<Result<PagedResponse<UIActionDto>>> GetAllUIActions(ListUIActionRequest req);
    Task<List<UIActionDto>> GetNavItemsAsync();
    Task<Result<UIActionDto>> GetByIdAsync(int id);
    Task<Result<UIActionDto>> CreateAsync(UIActionRequest request);
    Task<Result<UIActionDto>> UpdateAsync(int id, UIActionRequest request);
    Task<Result<bool>> DeleteAsync(int id);
}
