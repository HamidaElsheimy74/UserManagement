using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class UserServices : IUserServices
{
    private readonly IUserManagementRepository<AppUser> _userRepository;
    private readonly IUserManagementUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserServices> _logger;
    private readonly ILocalizer _localizer;
    private readonly IUserManagementRepository<AppRole> _roleRepository;

    private readonly UserManager<AppUser> _userManager;
    public UserServices(IUserManagementRepository<AppUser> userRepository,
                             IUserManagementUnitOfWork unitOfWork,
                             IMapper mapper, ILogger<UserServices> logger,
                                ILocalizer localizer,
                             IUserManagementRepository<AppRole> roleRepository,
                             UserManager<AppUser> userManager)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
        _roleRepository = roleRepository;
        _userManager = userManager;
    }

    public async Task<APIResponse> CreateUserAsync(UserDto user)
    {
        if (user == null)
        {
            _logger.LogError("User data is required.");
            return new APIResponse(400, "User" + _localizer["Required"]);
        }

        var isuserExists = await _userRepository.AnyAsync(r => r.Email == user.Email);
        if (isuserExists)
        {
            _logger.LogError($"This User with Email {user.Email} is already exist.");
            return new APIResponse(400, "User" + _localizer["Exists"]);
        }
        else
        {
            var rolesExist = await _roleRepository.AnyAsync(obj => !obj.UserRoles.Any(ur => ur.RoleId != obj.Id || ur.IsDeleted));
            if (!rolesExist)
            {
                _logger.LogError($"Invalid roles were inserted to the user.");
                return new APIResponse(400, "Roles Added too the user are" + _localizer["NotValid"]);
            }

            var mappedUser = _mapper.Map<AppUser>(user);
            var identityUser = await _userManager.CreateAsync(mappedUser, mappedUser.PasswordHash!);

            if (identityUser.Succeeded)
            {
                _logger.LogTrace($"User with Email {user.Email} is Created Successfully");
                return new APIResponse(200, "", true);
            }
            else
            {
                _logger.LogError($"Failed to create user with Email {user.Email}. Errors: {string.Join(", ", identityUser.Errors.Select(e => e.Description))}");
                return new APIResponse(500, $"{_localizer["FailedCreation"]} User with email {user.Email}");
            }
        }
    }


    public async Task<APIResponse> DeleteUserAsync(long userId)
    {
        _logger.LogTrace($"Deleting user with Id: {userId}");
        if (userId <= 0)
        {
            _logger.LogError("UserId is not valid.");
            return new APIResponse(400, $"userId {userId} {_localizer["NotValid"]}");
        }
        var user = await _userRepository.WhereAsync(u => u.Id == userId && !u.IsDeleted);
        if (user != null)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.Save();
            _logger.LogTrace($"User with Id {userId} is deleted successfully.");
            return new APIResponse(200, "", true);
        }
        else
        {
            _logger.LogError($"User with Id {userId} does not exist.");
            return new APIResponse(400, $"user with id {userId} {_localizer["Exists"]}");

        }
    }

    public async Task<APIResponse> GetAllUsersAsync()
    {
        _logger.LogTrace("Getting All users");
        var users = await _userRepository.Where(u => !u.IsDeleted);
        if (users == null || !users.Any())
        {
            _logger.LogInformation("No users found.");
            return new APIResponse(200, "", new List<UserDto>());
        }
        var usersDto = _mapper.Map<List<UserDto>>(users);
        return new APIResponse(200, "", usersDto);
    }

    public async Task<APIResponse> GetUserByIdAsync(long userId)
    {
        _logger.LogTrace($"Getting User with Id: {userId}");
        if (userId <= 0)
        {
            _logger.LogError("UserId is not valid.");
            return null;
        }
        var user = await _userRepository.WhereAsync(u => u.Id == userId && !u.IsDeleted);
        return user == null ? new APIResponse(400, $"User with id {userId} {_localizer["Exists"]}")
                : new APIResponse(200, "", _mapper.Map<UserDto>(user));
    }

    public async Task<APIResponse> UpdateUserAsync(UserDto user)
    {
        _logger.LogTrace($"Updating user with email: {user.Email}");
        if (user == null)
        {
            _logger.LogError("User data is required.");
            return new APIResponse(400, $"User {_localizer["Required"]}");
        }
        var oldUser = await _userRepository.WhereAsync(u => u.Id == user.UserId && !u.IsDeleted);
        if (oldUser == null)
        {
            _logger.LogError("User is not exist.");
            return new APIResponse(400, $"User with Id: {user.UserId} {_localizer["Exists"]}");
        }

        oldUser.Email = user.Email;
        oldUser.ModifiedAt = DateTime.UtcNow;

        var isUpdated = await _userManager.UpdateAsync(oldUser);
        if (!isUpdated.Succeeded)
        {
            _logger.LogError($"User {user.Email} update failed: {string.Join(", ", isUpdated.Errors.Select(e => e.Description))}");
            return new APIResponse(500, $"{_localizer["FailedUpdate"]} user with id {user.UserId}");
        }
        _logger.LogTrace($"user with email {oldUser.Email} is Updated Successfully");
        return new APIResponse(200, "", true);
    }


}
