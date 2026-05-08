using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserService : IUserService
{
    private readonly IConfiguration _cfg;
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly IPermissionRepository _permissionRepo;
    private readonly IUserPermissionRepository _userPermissionRepo;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    public UserService(
        IUserRepository userRepo, 
        IRoleRepository roleRepo, 
        IMapper mapper, 
        IPermissionRepository permissionRepo, 
        IUserPermissionRepository userPermissionRepo, 
        IEmailService emailService, 
        ICurrentUserService currentUser,
        IConfiguration cfg   )
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _mapper = mapper;
        _permissionRepo = permissionRepo;
        _userPermissionRepo = userPermissionRepo;
        _emailService = emailService;
        _currentUser = currentUser;
        _cfg = cfg;
    }
    public async Task<Result<bool>> DeleteUserAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return Result<bool>.Failure("User does not exist");
        var isDeleted = await _userRepo.SoftDeleteUser(user);

        return isDeleted ? Result<bool>.Success(isDeleted, "Delete User Successfully") : Result<bool>.Failure("Delete User Failed");
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

    public async Task<Result<PagedResponse<UserListItemResponse>>> GetListUserAsync(UserListRequest req)
    {
        var currentUserId = _currentUser.UserId;

        var currentUser = currentUserId.HasValue ? await _userRepo.GetByIdAsync(currentUserId.Value) : null;

        var isSuperAdmin = currentUser != null ? currentUser.Status == UserStatus.SuperAdmin : false;
           
        var (users, total) = await _userRepo.GetAllUsers(req, isSuperAdmin);
        
        var pagedUser = await users.ProjectTo<UserListItemResponse>(_mapper.ConfigurationProvider).ToListAsync();

        if (pagedUser.Count == 0) return Result<PagedResponse<UserListItemResponse>>.Failure("Get List User Failed", StatusCodes.Status204NoContent);

        var result =  new PagedResponse<UserListItemResponse>(
            pagedUser,
            req.Page,
            req.PageSize,
            total);

        return Result<PagedResponse<UserListItemResponse>>.Success(result, "Get List User Successfully!!!");
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

    public async Task<Result<UserDetailResponse>> StoreUserAsync(CreateUserRequest user)
    {
        var currentUserId = _currentUser.UserId;

        var currentUser = currentUserId.HasValue ? await _userRepo.GetByIdAsync(currentUserId.Value) : null;

        var isSuperAdmin = currentUser != null ? currentUser.Status == UserStatus.SuperAdmin : false;

        var isExisted = await _userRepo.GetByUserNameAsync(user.Username) != null || await _userRepo.GetUserByEmail(user.Email!) != null;

        var userCreated = new User();

        if (isExisted) return Result<UserDetailResponse>.Failure("Account already existed", StatusCodes.Status400BadRequest);

        else if (user.Roles.Contains("admin") && isSuperAdmin)
        {
            var entity = _mapper.Map<User>(user);
            entity.Password = PasswordHasher.Hash(user.Password);
            entity.Roles = await _roleRepo.GetListRoleByListString(user.Roles);
            userCreated = await _userRepo.CreateAsync(entity);
        }
        else if (user.Roles.Contains("admin")) return Result<UserDetailResponse>.Failure("Could not create this account", StatusCodes.Status203NonAuthoritative);

        else
        {
            var entity = _mapper.Map<User>(user);
            entity.Password = PasswordHasher.Hash(user.Password);
            entity.Roles = await _roleRepo.GetListRoleByListString(user.Roles);
            userCreated = await _userRepo.CreateAsync(entity);
        }

        if (userCreated == null) return Result<UserDetailResponse>.Failure("Could not create this account", StatusCodes.Status204NoContent);

        var result = _mapper.Map<UserDetailResponse>(userCreated);

        return Result<UserDetailResponse>.Success(result, StatusCodes.Status201Created, "Create new user successfully!!!");
    }

    public async Task<Result<UserDetailResponse>> UpdateUserAsync(int id, UpdateUserRequest user)
    {
        var currentUserId = _currentUser.UserId;

        var currentUser = currentUserId.HasValue ? await _userRepo.GetByIdAsync(currentUserId.Value) : null;

        var isSuperAdmin = currentUser != null ? currentUser.Status == UserStatus.SuperAdmin : false;

        var userUpdate = await _userRepo.GetByIdAsync(id);

        if (userUpdate == null) return Result<UserDetailResponse>.Failure("User Is Not Found", StatusCodes.Status204NoContent);

        _mapper.Map(user, userUpdate);

        if (user.Roles != null && user.Roles.Contains("admin") && isSuperAdmin)
        {
            userUpdate.Roles = await _roleRepo.GetListRoleByListString(user.Roles);
        } else if (user.Roles != null && user.Roles.Contains("admin"))
        {
            userUpdate.Roles = await _roleRepo.GetListRoleByListString(["customer"]);

        } else if (user.Roles != null)
        {
            userUpdate.Roles = await _roleRepo.GetListRoleByListString(user.Roles);
        }

        if (!string.IsNullOrEmpty(user.Password)) userUpdate.Password = PasswordHasher.Hash(user.Password);

        var updatedUser = await _userRepo.UpdateAsync(userUpdate);

        var result = _mapper.Map<UserDetailResponse>(updatedUser);

        return Result<UserDetailResponse>.Success(result, "Updated User Successfully");
    }

    //User
    public async Task<Result<UserDetailResponse>> RegisterUserAsync(RegisterRequest req)
    {
        var user = _mapper.Map<User>(req);
        user.Password = PasswordHasher.Hash(req.Password!);
        user.Roles = await _roleRepo.GetListRoleByListString(["customer"]);

        var userCreated = await _userRepo.CreateAsync(user);

        if (userCreated == null) return Result<UserDetailResponse>.Failure("Register User Failed", StatusCodes.Status403Forbidden);
        else
        {
            await _emailService.SendVerifyEmailNewUser(userCreated.Email!, userCreated.Username, _cfg["RouteNgrok:Route"] + "auth/verify?" + "userName=" + userCreated.Username);

            return Result<UserDetailResponse>.Success(_mapper.Map<UserDetailResponse>(userCreated), StatusCodes.Status201Created, "Account registered!!!");
        }
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

    public async Task<List<string>> GetUserName(string? req)
    {
        return await _userRepo.GetUserName(req);
    }

    public async Task<Result<UserAuthDto>> VerifyEmail(string userName)
    {
        var userVerify = await _userRepo.GetByUserNameAsync(userName);

        if (userVerify == null) return Result<UserAuthDto>.Failure("Can't found user to verify email!!!", StatusCodes.Status404NotFound);
        else
        {
            var result = await _userRepo.VerifyEmail(userVerify);

            return Result<UserAuthDto>.Success(result);
        }
    }

    public async Task<Result<UserDetailResponse>> ChangePassword(ChangePasswordRequest req)
    {
        var currentUserId = _currentUser.UserId;

        var user = await _userRepo.GetByIdAsync(currentUserId ?? 0);
        if (user == null) return Result<UserDetailResponse>.Failure("User not found", StatusCodes.Status404NotFound);

        if (!PasswordHasher.Verify(req.CurrentPassword, user.Password!)) return Result<UserDetailResponse>.Failure("Current password is incorrect", StatusCodes.Status400BadRequest);

        user.Password = PasswordHasher.Hash(req.NewPassword);
        await _userRepo.UpdateAsync(user);

        return Result<UserDetailResponse>.Success(_mapper.Map<UserDetailResponse>(user), StatusCodes.Status200OK, "Password changed successfully");
    }

    public async Task<Result<UserDetailResponse>> UpdateUserProfileAsync(UpdateUserProfile req)
    {
        var currentUserId = _currentUser.UserId;

        var user = await _userRepo.GetByIdAsync(currentUserId ?? 0);
        if (user == null) return Result<UserDetailResponse>.Failure("User not found", StatusCodes.Status404NotFound);

        if (!string.IsNullOrEmpty(req.Firstname)) user.Firstname = req.Firstname;
        if (!string.IsNullOrEmpty(req.Lastname)) user.Lastname = req.Lastname;
        if (req.Gender.HasValue) user.Gender = req.Gender.Value;

        await _userRepo.UpdateAsync(user);

        return Result<UserDetailResponse>.Success(_mapper.Map<UserDetailResponse>(user), StatusCodes.Status200OK, "Profile updated successfully");
    }
}