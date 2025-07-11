using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class UserServices : IUserServices
{
    private readonly IUserManagementRepository<AppUser> _userRepository;
    private readonly IUserManagementUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserServices> _logger;
    private readonly IUserManagementRepository<AppRole> _roleRepository;

    private readonly UserManager<AppUser> _userManager;
    public UserServices(IUserManagementRepository<AppUser> userRepository,
                             IUserManagementUnitOfWork unitOfWork,
                             IMapper mapper, ILogger<UserServices> logger,
                             IUserManagementRepository<AppRole> roleRepository,
                             UserManager<AppUser> userManager)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _roleRepository = roleRepository;
        _userManager = userManager;
    }

    public async Task<bool> CreateUserAsync(UserDto user)
    {
        if (user == null)
        {
            _logger.LogError("User data is required.");
            return false;
        }

        var isuserExists = await _userRepository.AnyAsync(r => r.Email == user.Email);
        if (isuserExists)
        {
            _logger.LogError($"This User with Email {user.Email} is already exist.");
            return false;
        }
        else
        {
            var rolesExist = await _roleRepository.AnyAsync(obj => !obj.UserRoles.Any(ur => ur.RoleId != obj.Id));
            if (!rolesExist)
            {
                _logger.LogError($"Invalid roles were inserted to the user.");
                return false;
            }

            var mappedUser = _mapper.Map<AppUser>(user);
            var identityUser = await _userManager.CreateAsync(mappedUser, mappedUser.PasswordHash!);

            if (identityUser.Succeeded)
            {
                _logger.LogTrace($"User with Email {user.Email} is Created Successfully");
                return true;
            }
            else
            {
                _logger.LogError($"Failed to create user with Email {user.Email}. Errors: {string.Join(", ", identityUser.Errors.Select(e => e.Description))}");
                return false;
            }
        }
    }


    public Task<bool> DeleteUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserAsync(UserDto user)
    {
        throw new NotImplementedException();
    }


    private AppUser UserDtoToDB(UserDto userDto)
    {
        return new AppUser
        {
            UserName = userDto.UserName,
            Email = userDto.Email,
            CreatedAt = userDto.CreatedAt,
            UserRoles = userDto.UserRoles.Select(ur => new AppUserRoles
            {
                RoleId = ur.RoleId,
                UserId = ur.UserId
            }).ToList()
        };
    }
}
