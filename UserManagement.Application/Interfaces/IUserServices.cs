using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;
public interface IUserServices
{
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<bool> CreateUserAsync(UserDto user);
    Task<bool> UpdateUserAsync(UserDto user);
    Task<bool> DeleteUserAsync(Guid userId);
}
