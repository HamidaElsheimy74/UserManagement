using UserManagement.Application.DTOs;
using UserManagement.Common.Helpers;

namespace UserManagement.Application.Interfaces;
public interface IUserRoleServices
{
    public Task<APIResponse> GetUserRoleAsync(long userId, long roleId);
    public Task<APIResponse> AddUserRoleAsync(UserRoleDto userRoleDto);
    public Task<APIResponse> DeleteUserRoleAsync(long userId, long roleId);
    public Task<APIResponse> UpdateUserRoleAsync(UserRoleDto userRoleDto);
    public Task<APIResponse> GetUserRolesAsync();
}
