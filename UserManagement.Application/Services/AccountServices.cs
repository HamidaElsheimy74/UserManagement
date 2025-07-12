using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Common.DTOs;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Models;

namespace UserManagement.Application.Services;
public class AccountServices : IAccountServices
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AccountServices> _logger;
    private readonly ILocalizer _localizer;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenServices _tokenServices;
    private readonly IConfiguration _config;
    private JWT _jwt;
    private readonly IUserManagementRepository<AppUserRoles> _userRoleRepo;
    private readonly IUserManagementRepository<AppRole> _roleRepo;
    private readonly IUserManagementUnitOfWork _unitOfWork;
    public AccountServices(UserManager<AppUser> userManager, ILogger<AccountServices> logger,
                            SignInManager<AppUser> signInManagerSingin, ILocalizer localizer,
                            ITokenServices tokenServices, IConfiguration configuration,
                            IUserManagementRepository<AppUserRoles> userRoleRepo,
                            IUserManagementRepository<AppRole> roleRepo,
                            IUserManagementUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _logger = logger;
        _localizer = localizer;
        _signInManager = signInManagerSingin;
        _tokenServices = tokenServices;
        _config = configuration;
        _jwt = _config.GetSection("JWT")?.Get<JWT>();
        _userRoleRepo = userRoleRepo;
        _roleRepo = roleRepo;
        _unitOfWork = unitOfWork;
    }
    public async Task<APIResponse> LoginUser(LoginDto loginDto)
    {
        _logger.LogInformation($"Logging in user: {loginDto.UserName}");
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user == null)
        {
            _logger.LogError($"Incorrect user Name: {loginDto.UserName}");
            return new APIResponse(401, $"{_localizer["InvalidUser"]}");
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
        {
            _logger.LogError($"Incorrect user Password: {loginDto.UserName}");

            return new APIResponse(401, "InvalidPassword");
        }
        var roles = GetUserRoles(user.Id).Result;

        var tokensResponse = await _tokenServices.GenerateTokenAsync(new TokenDto(user.Email!, roles));
        var authResponsetokens = tokensResponse.Data as AuthResponse;
        user.RefreshToken = authResponsetokens!.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationTime);

        await _userManager.UpdateAsync(user);

        return tokensResponse;
    }

    public async Task<APIResponse> RefeshToken(RefreshTokenModel model, string email)
    {
        if (string.IsNullOrEmpty(model.RefreshToken) || string.IsNullOrEmpty(model.RefreshToken) || string.IsNullOrEmpty(email))
            return new APIResponse(400, $"Refresh token model {_localizer["Required"]}");


        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new APIResponse(400, $"{_localizer["UserNotFound"]}");


        if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return new APIResponse(400, $"{_localizer["InvalidRefreshToken"]}");

        var roles = GetUserRoles(user.Id).Result;
        var tokenResponse = await _tokenServices.GenerateTokenAsync(new TokenDto(user.Email!, roles));
        var authResponse = tokenResponse.Data as AuthResponse;
        user.RefreshToken = authResponse.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationTime);

        var isUpdated = await _userManager.UpdateAsync(user);
        if (!isUpdated.Succeeded)
            return new APIResponse(500, $"{_localizer["FailedUpdate"]} creating Refresh token");

        return tokenResponse;
    }

    public async Task<APIResponse> RegisterUser(RegisterDto registerDto)
    {
        if (CheckUserExistsAsync(registerDto.UserName).Result)
        {
            return new APIResponse(400, $"{_localizer["InvalidUser"]}");
        }
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
        };


        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return new APIResponse(500, $"{_localizer["FailedCreation"]} of user {user.Email}");
        var userRoleName = _config["UserRoleName"];
        var role = await _roleRepo.WhereAsync(obj => obj.Name == userRoleName && !obj.IsDeleted);
        var userRole = new AppUserRoles()
        {
            UserId = user.Id,
            RoleId = role.Id
        };
        await _userRoleRepo.AddAsync(userRole);
        await _unitOfWork.Save();

        return new APIResponse(200);
    }

    private async Task<bool> CheckUserExistsAsync(string userName) => await _userManager.FindByNameAsync(userName) != null;

    private async Task<List<string>> GetUserRoles(long userId)
    {
        var userRoles = await _userRoleRepo.Where(obj => obj.UserId == userId && !obj.IsDeleted, r => r.Role);
        List<string> roles = new List<string>();
        if (userRoles != null)
            roles = userRoles.Select(r => r.Role.Name).ToList();
        return roles;
    }
}
