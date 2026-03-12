using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.NetworkInformation;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }
    public Task<bool> DeleteUserAsync(int id)
    {
        throw new NotImplementedException();
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

    public async Task<List<UserListItemResponse>> GetListUserAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return users is null ?
            new List<UserListItemResponse>() :
            users.Select(u => _mapper.Map<UserListItemResponse>(u)).ToList();
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
        var userCreate = await _userRepo.CreateAsync(entity);

        if (userCreate == null) return _mapper.Map<UserDetailResponse?>(null);

        return _mapper.Map<UserDetailResponse>(userCreate);
    }

    public async Task<UserDetailResponse?> UpdateUserAsync(int id, UpdateUserRequest user)
    {
        var userUpdate = await _userRepo.GetByIdAsync(id);

        if(userUpdate == null) return null;

        _mapper.Map(user, userUpdate);

        var updatedUser = await _userRepo.UpdateAsync(userUpdate);

        return _mapper.Map<UserDetailResponse>(updatedUser);
    }
}
