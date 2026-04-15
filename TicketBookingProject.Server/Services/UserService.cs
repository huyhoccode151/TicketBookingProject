using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly IPermissionRepository _permissionRepo;
    private readonly IUserPermissionRepository _userPermissionRepo;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepo, IRoleRepository roleRepo, IMapper mapper, IPermissionRepository permissionRepo, IUserPermissionRepository userPermissionRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _mapper = mapper;
        _permissionRepo = permissionRepo;
        _userPermissionRepo = userPermissionRepo;
    }
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return false;
        var isDeleted = await _userRepo.SoftDeleteUser(user);

        return isDeleted;
    }

    public async Task<bool> ForceChangePassword(int id, ForceChangePasswordRequest req)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return false;

        user.Password = PasswordHasher.Hash(req.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user);
        return true;
    }

    public Task<bool> ForceDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDetailResponse?> GetByIdAsync(int id)
    {
        var userFound = await _userRepo.GetByIdAsync(id);
        return userFound is null ? 
            await Task.FromResult<UserDetailResponse?>(null) : 
            _mapper.Map<UserDetailResponse>(userFound);
    }

    public async Task<PagedResponse<UserListItemResponse>> GetListUserAsync(UserListRequest req)
    {
        var users = await _userRepo.GetAllUsers(req);

        var items = _mapper.Map<List<UserListItemResponse>>(users.Items);

        return new PagedResponse<UserListItemResponse>(
            items,
            req.Page,
            req.PageSize,
            users.TotalCount);
    }

    public Task<int> MultiForceDelete(List<int> ids)
    {
        throw new NotImplementedException();
    }

    public Task<int> MultiRestoreAsync(List<int> ids)
    {
        throw new NotImplementedException();
    }

    public Task<int> MultiSoftDelete(List<int> ids)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDetailResponse?> StoreUserAsync(CreateUserRequest user)
    {
        var entity = _mapper.Map<User>(user);
        entity.Password = PasswordHasher.Hash(user.Password);
        entity.Roles = await _roleRepo.GetListRoleByListString(user.Roles);
        var userCreated = await _userRepo.CreateAsync(entity);

        if (userCreated == null) return _mapper.Map<UserDetailResponse?>(null);

        return _mapper.Map<UserDetailResponse>(userCreated);
    }

    public async Task<UserDetailResponse?> UpdateUserAsync(int id, UpdateUserRequest user)
    {
        var userUpdate = await _userRepo.GetByIdAsync(id);

        if(userUpdate == null) return null;

        _mapper.Map(user, userUpdate);

        if (!string.IsNullOrEmpty(user.Password)) userUpdate.Password = PasswordHasher.Hash(user.Password);

        if (user.Roles != null)
        {
            userUpdate.Roles = await _roleRepo.GetListRoleByListString(user.Roles);
        }

        var updatedUser = await _userRepo.UpdateAsync(userUpdate);

        return _mapper.Map<UserDetailResponse>(updatedUser);
    }

    //User
    public async Task<UserDetailResponse?> RegisterUserAsync(RegisterUserRequest req)
    {
        var user = _mapper.Map<User>(req);
        user.Password = PasswordHasher.Hash(req.Password!);
        user.Roles = await _roleRepo.GetListRoleByListString(["customer"]);

        var userCreated = await _userRepo.CreateAsync(user);

        if (userCreated == null) return _mapper.Map<UserDetailResponse?>(null);

        return _mapper.Map<UserDetailResponse>(userCreated);
    }

    public async Task<UserDetailResponse> AssignPermissionToUserAsync(int id, AssignPermissionRequest req)
    {
        var rolePermissions = await _roleRepo.GetPermissionsByUserIdAsync(id);
        var userPermissions = await _userPermissionRepo.GetPermissionsByUserIdAsync(id);

        foreach (var name in req.Permissions)
        {
            var permission = await _permissionRepo.GetPermissionByName(name);
            if (permission == null) continue;
            var existing = await _userPermissionRepo.FirstOrDefaultAsync(p => p.PermissionId == permission.Id);

            if (existing != null && existing.Effect == -1)
            {
                await _userPermissionRepo.ForceDeleteAsync(existing);
            }
        }

        var notInRequest = rolePermissions.Where(rq => !req.Permissions.Contains(rq.Name));

        foreach (var permission in notInRequest)
        {
            var existing = await _userPermissionRepo.FirstOrDefaultAsync(p => p.PermissionId == permission.Id);

            if (existing == null)
            {
                await _userPermissionRepo.AddAsync(new UserPermission
                {
                    UserId = id,
                    PermissionId = permission.Id,
                    Effect = -1,
                    CreatedAt = DateTime.UtcNow,
                });
            }
        }

        await _userPermissionRepo.SaveChanges();

        return _mapper.Map<UserDetailResponse>(await _userRepo.GetByIdAsync(id));
    }

    public async Task<List<string>> GetPermissionsByUserIdAsync(int userId)
    {
        return await _userRepo.GetPermissionsByUserIdAsync(userId);
    }

    public async Task<List<string>> GetRolePermissionByUserIdAsync(int userId)
    {
        return await _userRepo.GetRolePermissions(userId);
    }

    public async Task<UserStatsDto> GetUserStats()
    {
        return await _userRepo.GetUserStats();
    }
}
