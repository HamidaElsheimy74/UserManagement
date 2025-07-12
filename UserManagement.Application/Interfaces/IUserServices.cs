using UserManagement.Application.DTOs;
using UserManagement.Common.Helpers;

namespace UserManagement.Application.Interfaces;
public interface IUserServices
{
    Task<APIResponse> GetUserByIdAsync(long userId);
    Task<APIResponse> GetAllUsersAsync();
    Task<APIResponse> CreateUserAsync(UserDto user);
    Task<APIResponse> UpdateUserAsync(UserDto user);
    Task<APIResponse> DeleteUserAsync(long userId);
}
