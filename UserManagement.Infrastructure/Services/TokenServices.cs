using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UserManagement.Common.DTOs;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Services;
public class TokenServices : ITokenServices
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly ILocalizer _localizer;
    public TokenServices(IConfiguration config, ILocalizer localizer)
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]!));
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

        //user.AddIdentity(new ClaimsIdentity(new Claim[]{
        //        new Claim(JwtRegisteredClaimNames.Email,tokenDto.Email),

        //           }));
        // var roles = tokenDto.Roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        return new APIResponse(200, "", tokenDto);
    }

    public async Task<APIResponse> GenerateTokenAsync(TokenDto tokenDto)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, tokenDto.Email),
            };

        claims.AddRange(tokenDto.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = creds,
            Issuer = _config["Token:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new APIResponse(200, "", tokenHandler.WriteToken(token));

    }

    public Task<APIResponse> RefreshTokenAsync(string token, string secretKey, string email)
    {
        throw new NotImplementedException();
    }
}
