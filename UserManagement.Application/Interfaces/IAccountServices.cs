using UserManagement.Application.DTOs;
using UserManagement.Common.Helpers;

namespace UserManagement.Application.Interfaces;
public interface IAccountServices
{
    Task<APIResponse> RegisterUser(RegisterDto userDto);
    Task<APIResponse> LoginUser(LoginDto userDto);
    Task<APIResponse> RefeshToken(string email);
}
