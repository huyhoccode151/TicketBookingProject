using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public interface IUserRepository : IBaseRepository<User>
{
    Task<List<string>> GetPermissionsByUserIdAsync(int userId, CancellationToken ct = default);
    Task<User?> GetByUserNameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(IQueryable<User>, int TotalCount)> GetAllUsers(UserListRequest req, bool IsSuperAdmin = false);
    Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default);
    string? GetJwtConfig(string key);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task<User?> UpdateAsync(User user, CancellationToken ct = default);
    Task<bool> SoftDeleteUser(User user, CancellationToken ct = default);
    Task<bool> ForceDeleteUser(User user, CancellationToken ct = default);
    Task<int> MultiRestoreUser(List<int> ids);
    Task<int> MultiSoftDelete(List<int> ids);
    Task<int> MultiForceDeleteUser(List<int> ids, CancellationToken ct = default);
    Task<List<string>> GetRolePermissions(int userId, CancellationToken ct = default);
    Task<UserStatsDto> GetUserStats();
    Task<List<string>> GetUserName(string? req);
    Task<UserAuthDto> VerifyEmail(User user);
    Task<User?> GetUserByEmail(string email);
    Task UserEvent(int id);
}
