using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UserManagement.Common.DTOs;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Models;

namespace UserManagement.Infrastructure.Services;
public class TokenServices : ITokenServices
{
    private readonly IConfiguration _config;
    private readonly ILocalizer _localizer;
    private readonly JWT _jwt;
    private readonly UserManager<AppUser> _userManager;
    private readonly DateTime AccessTokenExpiryTime;
    public TokenServices(IConfiguration config, ILocalizer localizer, UserManager<AppUser> userManager)
    {
        _config = config;
        _jwt = _config.GetSection("JWT")?.Get<JWT>();
        AccessTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationTime);
        _userManager = userManager;
        _localizer = localizer;
    }
    public async Task<APIResponse> DecodeToken(string token, ClaimsPrincipal user)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken decodedToken = handler.ReadToken(token) as JwtSecurityToken;

        if (decodedToken != null && decodedToken.ValidTo < DateTime.UtcNow)
        {
            return new APIResponse(400, $"{_localizer["TokenExpired"]}");
        }
        var tokenDto = JsonSerializer.Deserialize<TokenDto>(decodedToken!.Payload.SerializeToJson());

        return new APIResponse(200, "", tokenDto);
    }

    public async Task<APIResponse> GenerateTokenAsync(TokenDto tokenDto)
    {
        var accessToekn = CreateAccessToken(tokenDto);
        var refreshToken = CreateRefreshToken();

        var AuthResponse = new AuthResponse()
        {
            AccessToken = accessToekn,
            RefreshToken = refreshToken,
            AccessTokenExpiry = AccessTokenExpiryTime
        };
        return new APIResponse(200, "", AuthResponse);

    }

    public async Task<APIResponse> RefreshTokenAsync(string token, string secretKey, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        }, out SecurityToken validatedToken);

        var user = _userManager.FindByEmailAsync(email).Result;
        if (user == null)
        {
            return new APIResponse(400, $"{_localizer["UserNotFound"]}");
        }
        var roles = _userManager.GetRolesAsync(user).Result.ToList();

        var generatedTokenResult = await GenerateTokenAsync(new TokenDto
        {
            Email = user.Email!,
            Roles = roles
        });
        return generatedTokenResult;
    }

    private string CreateAccessToken(TokenDto tokenDto)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, tokenDto.Email),
            };
        if (tokenDto.Roles.Count > 0)
            claims.AddRange(tokenDto.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt!.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = AccessTokenExpiryTime,
            SigningCredentials = creds,
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,

        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string CreateRefreshToken()
    {
        var randmNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randmNumber);
        }
        return Convert.ToBase64String(randmNumber);

    }
}
