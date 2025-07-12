using System.Security.Claims;
using UserManagement.Common.DTOs;
using UserManagement.Common.Helpers;

namespace UserManagement.Domain.Interfaces;
public interface ITokenServices
{
    Task<APIResponse> GenerateTokenAsync(TokenDto tokenDto);
    Task<APIResponse> RefreshTokenAsync(string token, string secretKey, string email);
    Task<APIResponse> DecodeToken(string token, ClaimsPrincipal user);
}
