using AutoMapper;
using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class UserRoleServices : IUserRoleServices
{
    private readonly IUserManagementRepository<AppUserRoles> _userRoleRepository;
    private readonly IUserManagementRepository<AppUser> _userRepository;
    private readonly IUserManagementRepository<AppRole> _roleRepository;
    private readonly IUserManagementUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserRoleServices> _logger;
    private readonly ILocalizer _localizer;

    public UserRoleServices(IUserManagementRepository<AppUserRoles> userRoleRepository,
                            IUserManagementRepository<AppUser> userRepository,
                            IUserManagementRepository<AppRole> roleRepository,
                            IUserManagementUnitOfWork unitOfWork,
                            IMapper mapper, ILocalizer localizer,
                            ILogger<UserRoleServices> logger)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<APIResponse> AddUserRoleAsync(UserRoleDto userRoleDto)
    {

        _logger.LogInformation("Adding user roles: {@UserRoleDto}", userRoleDto);
        if (userRoleDto == null)
        {
            _logger.LogError("User Role" + _localizer["RequiredDto"]);
            return new APIResponse(400, _localizer["RequiresDto"]);
        }

        var isUserExists = await _userRepository.AnyAsync(r => r.Id == userRoleDto.UserId && !r.IsDeleted);
        if (!isUserExists)
        {
            _logger.LogError(userRoleDto.UserId + _localizer["UserNotFound"]);
            return new APIResponse(400, _localizer["UserNotFound"]);
        }

        var isRoleExists = await _roleRepository.AnyAsync(r => r.Id == userRoleDto.RoleId);

        if (!isRoleExists)
        {
            _logger.LogError(userRoleDto.RoleId + _localizer["RoleNotFound"]);
            return new APIResponse(400, _localizer["RoleNotFound"]);
        }

        var isUserRoleExists = await _userRoleRepository.AnyAsync(r => r.UserId == userRoleDto.UserId && r.RoleId == userRoleDto.RoleId && !r.IsDeleted);
        if (isUserRoleExists)
        {
            _logger.LogError(_localizer["UserRoleAlreadyExists"]);
            return new APIResponse(400, _localizer["UserRoleAlreadyExists"]);
        }

        var mappedUserRole = _mapper.Map<AppUserRoles>(userRoleDto);
        await _userRoleRepository.AddAsync(mappedUserRole);
        await _unitOfWork.Save();
        return new APIResponse(200, "", true);
    }

    public async Task<APIResponse> DeleteUserRoleAsync(long userId, long roleId)
    {
        _logger.LogInformation($"Deleting user role for UserId: {userId}, RoleId: {roleId}");
        if (userId <= 0 || roleId <= 0)
        {
            _logger.LogError("UserId or RoleId is not valid.");
            return new APIResponse(400, _localizer["InvalidUserOrRoleId"]);
        }

        var userRole = await _userRoleRepository.WhereAsync(r => r.UserId == userId && r.RoleId == roleId);
        if (userRole == null)
        {
            _logger.LogError(_localizer["UserRoleNotFound"]);
            return new APIResponse(404, _localizer["UserRoleNotFound"]);
        }

        userRole.IsDeleted = true;
        userRole.DeletedAt = DateTime.UtcNow;

        await _userRoleRepository.UpdateAsync(userRole);
        await _unitOfWork.Save();
        return new APIResponse(200, "", true);


    }

    public async Task<APIResponse> GetUserRoleAsync(long userId, long roleId)
    {
        _logger.LogTrace("Getting user Roles");
        var userRole = await _userRoleRepository.WhereAsync(obj => obj.UserId == userId &&
        obj.RoleId == roleId && !obj.IsDeleted);
        if (userRole == null)
        {
            _logger.LogError(_localizer["UserRoleNotFound"]);
            return new APIResponse(404, _localizer["UserRoleNotFound"]);
        }

        var userroleDto = _mapper.Map<UserRoleDto>(userRole);
        return new APIResponse(200, "", userroleDto);
    }

    public async Task<APIResponse> GetUserRolesAsync()
    {
        _logger.LogTrace("Getting All user Roles");
        var userRoles = await _userRoleRepository.Where(obj => !obj.IsDeleted);
        if (userRoles == null || !userRoles.Any())
        {
            _logger.LogError(_localizer["UserRolesNotFound"]);
            return new APIResponse(200, "", new List<UserRoleDto>());
        }
        var userrolesDto = _mapper.Map<List<UserRoleDto>>(userRoles);
        return new APIResponse(200, "", userrolesDto);
    }

    public async Task<APIResponse> UpdateUserRoleAsync(UserRoleDto userRoleDto)
    {
        _logger.LogTrace($"Updating user role with userId: {userRoleDto.UserId} and roleId: {userRoleDto.RoleId}");
        if (userRoleDto == null)
        {
            _logger.LogError("UserRole data is required.");
            return new APIResponse(400, "User roles " + _localizer["Required"]);
        }
        var userRole = await _userRoleRepository.WhereAsync(r => r.UserId == userRoleDto.UserId &&
                                                    r.RoleId == userRoleDto.RoleId && !r.IsDeleted);
        if (userRole == null)
        {
            _logger.LogError("Role data is not exist.");
            return new APIResponse(400, "user roles " + _localizer["Exists"]);
        }

        userRole.RoleId = userRoleDto.RoleId;
        userRole.ModifiedAt = DateTime.UtcNow;

        await _userRoleRepository.UpdateAsync(userRole);
        await _unitOfWork.Save();
        _logger.LogTrace($"user role with RoleId {userRole.RoleId} and userId {userRole.UserId} is Updated Successfully");
        return new APIResponse(200, "", true);
    }
}
