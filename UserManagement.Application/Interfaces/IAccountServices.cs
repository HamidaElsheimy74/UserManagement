using UserManagement.Application.DTOs;
using UserManagement.Common.Helpers;
using UserManagement.Infrastructure.Models;

namespace UserManagement.Application.Interfaces;
public interface IAccountServices
{
    Task<APIResponse> RegisterUser(RegisterDto userDto);
    Task<APIResponse> LoginUser(LoginDto userDto);
    Task<APIResponse> RefeshToken(RefreshTokenModel model, string email);
}
