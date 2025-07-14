using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
    public async Task<APIResponse> DecodeToken(string token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken decodedToken = handler.ReadToken(token) as JwtSecurityToken;

        if (decodedToken == null || decodedToken.ValidTo > DateTime.UtcNow)
        {
            return new APIResponse(401, $"{_localizer["InvalidToken"]}");
        }
        var email = decodedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        var roles = decodedToken.Claims.Where(c => c.Type == "role").Select(r => r?.Value).ToList();
        var tokenDto = new TokenDto(email, roles);

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

    public async Task<APIResponse> RefreshTokenAsync(string token, string secretKey, TokenDto tokenDto)
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

        var generatedTokenResult = await GenerateTokenAsync(new TokenDto
        {
            Email = tokenDto.Email!,
            Roles = tokenDto.Roles
        });
        return generatedTokenResult;
    }

    private string CreateAccessToken(TokenDto tokenDto)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, tokenDto.Email),
            };
        claims.AddRange(tokenDto.Roles.Select(role => new Claim("role", role)));

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
