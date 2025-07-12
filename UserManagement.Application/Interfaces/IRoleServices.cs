using UserManagement.Application.DTOs;
using UserManagement.Common.Helpers;

namespace UserManagement.Application.Interfaces;
public interface IRoleServices
{
    Task<APIResponse> GetAllRolesAsync();
    Task<APIResponse> GetRoleAsync(long roleId);
    Task<APIResponse> CreateRoleAsync(RoleDto role);
    Task<APIResponse> UpdateRoleAsync(RoleDto role);
    Task<APIResponse> DeleteRoleAsync(long roleId);

}
