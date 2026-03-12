namespace TicketBookingProject.Server;

public interface IUserService
{
    Task<List<UserListItemResponse>> GetListUserAsync();
    Task<UserDetailResponse?> GetByIdAsync(int id);
    Task<UserDetailResponse?> StoreUserAsync(CreateUserRequest user);
    Task<UserDetailResponse?> UpdateUserAsync(int id, UpdateUserRequest user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ForceDeleteAsync(int id);
    Task<int> MultiRestoreAsync(List<int> ids);
    Task<int> MultiSoftDelete(List<int> ids);
    Task<int> MultiForceDelete(List<int> ids);
}
