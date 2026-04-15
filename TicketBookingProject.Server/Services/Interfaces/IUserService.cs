namespace TicketBookingProject.Server;

public interface IUserService
{
    Task<PagedResponse<UserListItemResponse>> GetListUserAsync(UserListRequest req);
    Task<UserDetailResponse?> GetByIdAsync(int id);
    Task<UserDetailResponse?> StoreUserAsync(CreateUserRequest user);
    Task<UserDetailResponse?> UpdateUserAsync(int id, UpdateUserRequest user);
    Task<bool> ForceChangePassword(int id, ForceChangePasswordRequest req);  
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ForceDeleteAsync(int id);
    Task<int> MultiRestoreAsync(List<int> ids);
    Task<int> MultiSoftDelete(List<int> ids);
    Task<int> MultiForceDelete(List<int> ids);
    Task<UserDetailResponse?> RegisterUserAsync(RegisterUserRequest req);
    Task<UserDetailResponse> AssignPermissionToUserAsync(int id, AssignPermissionRequest req);
    Task<List<string>> GetPermissionsByUserIdAsync(int userId);
    Task<List<string>> GetRolePermissionByUserIdAsync(int userId);
    Task<UserStatsDto> GetUserStats();
}
