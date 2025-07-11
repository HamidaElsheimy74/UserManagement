using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces;
public interface IRoleServices
{
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<RoleDto> GetRolesAsync(long roleId);
    Task<bool> CreateRoleAsync(AppRole role);
    Task<bool> UpdateRoleAsync(AppRole role);
    Task<bool> DeleteRoleAsync(long roleId);

}
