namespace TicketBookingProject.Server;

public interface IUserService
{
    Task<Result<PagedResponse<UserListItemResponse>>> GetListUserAsync(UserListRequest req);
    Task<UserDetailResponse?> GetByIdAsync(int id);
    Task<Result<UserDetailResponse>> StoreUserAsync(CreateUserRequest user);
    Task<Result<UserDetailResponse>> UpdateUserAsync(int id, UpdateUserRequest user);
    Task<bool> ForceChangePassword(int id, ForceChangePasswordRequest req);
    Task<Result<bool>> DeleteUserAsync(int id);
    Task<bool> ForceDeleteAsync(int id);
    Task<int> MultiRestoreAsync(List<int> ids);
    Task<int> MultiSoftDelete(List<int> ids);
    Task<int> MultiForceDelete(List<int> ids);
    Task<Result<UserDetailResponse>> RegisterUserAsync(RegisterRequest req);
    Task<UserDetailResponse> AssignPermissionToUserAsync(int id, AssignPermissionRequest req);
    Task<List<string>> GetPermissionsByUserIdAsync(int userId);
    Task<List<string>> GetRolePermissionByUserIdAsync(int userId);
    Task<UserStatsDto> GetUserStats();
    Task<List<string>> GetUserName(string? req);
    Task<Result<UserAuthDto>> VerifyEmail(string userName);
    Task<Result<UserDetailResponse>> ChangePassword(ChangePasswordRequest req);
    Task<Result<UserDetailResponse>> UpdateUserProfileAsync(UpdateUserProfile req);
    Task GetEventOfUser();
}
