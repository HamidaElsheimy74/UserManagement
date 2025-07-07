using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;
public interface IUserRoleServices
{
    public Task<IEnumerable<UserRoleDto>> GetUserRolesAsync(long userId);
    public Task<bool> AddUserRoleAsync(UserRoleDto userRoleDto);
    public Task<bool> DeleteUserRoleAsync(long userId, long roleId);
    public Task<bool> UpdateUserRoleAsync(UserRoleDto userRoleDto);
}
