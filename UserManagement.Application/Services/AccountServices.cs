using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Common.DTOs;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class AccountServices : IAccountServices
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AccountServices> _logger;
    private readonly ILocalizer _localizer;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenServices _tokenServices;
    public AccountServices(UserManager<AppUser> userManager, ILogger<AccountServices> logger,
                            SignInManager<AppUser> signInManagerSingin, ILocalizer localizer, ITokenServices tokenServices)
    {
        _userManager = userManager;
        _logger = logger;
        _localizer = localizer;
        _signInManager = signInManagerSingin;
        _tokenServices = tokenServices;
    }
    public async Task<APIResponse> LoginUser(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user == null)
        {
            return new APIResponse(401);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
        {
            return new APIResponse(401);
        }

        return new APIResponse(200, "", _tokenServices.GenerateTokenAsync(new TokenDto(user.Email!, new List<string>())));
    }

    public Task<APIResponse> RefeshToken(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<APIResponse> RegisterUser(RegisterDto registerDto)
    {
        if (CheckUserExistsAsync(registerDto.UserName).Result)
        {
            return new APIResponse(400, $"{_localizer["EmailInUse"]}");
        }
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
        };
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return new APIResponse(500, $"{_localizer["FailedCreation"]} of user {user.Email}");
        return new APIResponse(200);
    }

    private async Task<bool> CheckUserExistsAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName) != null;
    }
}
