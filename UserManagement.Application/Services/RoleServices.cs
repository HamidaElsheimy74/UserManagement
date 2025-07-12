using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Common.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class RoleServices : IRoleServices
{

    private readonly IUserManagementRepository<AppRole> _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleServices> _logger;
    private readonly ILocalizer _localizer;
    private readonly RoleManager<AppRole> _roleManager;

    public RoleServices(IUserManagementRepository<AppRole> roleRepository,
                        IMapper mapper, ILocalizer localizer,
                        ILogger<RoleServices> logger, RoleManager<AppRole> roleManager)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
        _roleManager = roleManager;
    }
    public async Task<APIResponse> CreateRoleAsync(RoleDto role)
    {

        _logger.LogTrace("Creating role: {@Role}", role);
        if (role == null)
        {
            _logger.LogError("Role data is required.");
            return new APIResponse(400, "Role " + _localizer["Required"]);
        }

        var isRoleExists = await _roleRepository.AnyAsync(r => r.Name == role.RoleName && !r.IsDeleted);
        if (isRoleExists)
        {
            _logger.LogError($"This Role {role.RoleName} is already exist.");
            return new APIResponse(400, "Role " + _localizer["Exists"]);
        }
        else
        {
            var mappedRole = _mapper.Map<AppRole>(role);
            var isAddedRole = await _roleManager.CreateAsync(mappedRole);
            if (!isAddedRole.Succeeded)
            {
                _logger.LogError($"Role {role.RoleName} creation failed: {string.Join(", ", isAddedRole.Errors.Select(e => e.Description))}");
                return new APIResponse(400, $"{_localizer["FailedCreation"]} Role {role.RoleName}");
            }
            _logger.LogTrace($"Role {role.RoleName} is Created Successfully");

            return new APIResponse(200, "", true);
        }
    }

    public async Task<APIResponse> DeleteRoleAsync(long roleId)
    {
        _logger.LogTrace($"Deleting Role with Id: {roleId}");
        if (roleId <= 0)
        {
            _logger.LogError("RoleId is not valid.");
            return new APIResponse(400, "RoleId" + _localizer["NotValis"]);
        }
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null || role.IsDeleted)
        {
            _logger.LogError("Role not found.");
            return new APIResponse(400, "Role " + _localizer["NotFound"]);
        }
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        await _roleManager.UpdateAsync(role);
        return new APIResponse(200, "", true);
    }

    public async Task<APIResponse> GetAllRolesAsync()
    {
        _logger.LogTrace("Getting All Roles");
        var roles = await _roleRepository.Where(obj => !obj.IsDeleted);
        if (roles == null || !roles.Any())
        {
            _logger.LogError("No roles found.");
            return new APIResponse(200, "", new List<RoleDto>());
        }
        var rolesDto = _mapper.Map<List<RoleDto>>(roles);
        return new APIResponse(200, "", rolesDto);
    }

    public async Task<APIResponse> GetRoleAsync(long roleId)
    {

        _logger.LogTrace($"Getting Role with Id: {roleId}");
        if (roleId <= 0)
        {
            _logger.LogError("RoleId is not valid.");
            return new APIResponse(400, "roleId" + _localizer["NotValid"]);
        }

        var role = await _roleRepository.Where(r => r.Id == roleId && !r.IsDeleted);
        return role == null || !role.Any() ? new APIResponse(400, "Role" + _localizer["NotFound"])
                                : new APIResponse(200, "", _mapper.Map<RoleDto>(role));
    }

    public async Task<APIResponse> UpdateRoleAsync(RoleDto role)
    {
        _logger.LogTrace($"Updating role with name: {role.RoleName}");
        if (role == null)
        {
            _logger.LogError("Role data is required.");
            return new APIResponse(400, "Role " + _localizer["Required"]);
        }
        var oldRole = await _roleRepository.WhereAsync(r => r.Id == role.RoleId && !r.IsDeleted);
        if (oldRole == null)
        {
            _logger.LogError("Role is not exist.");
            return new APIResponse(400, "Role" + _localizer["Exists"]);
        }

        oldRole.Description = role.Description;
        oldRole.Name = role.RoleName;
        oldRole.ModifiedAt = DateTime.UtcNow;

        var isUpdated = await _roleManager.UpdateAsync(oldRole);
        if (!isUpdated.Succeeded)
        {
            _logger.LogError($"Role {role.RoleName} update failed: {string.Join(", ", isUpdated.Errors.Select(e => e.Description))}");
            return new APIResponse(500, "Role" + _localizer["FailedUpdate"]);
        }
        _logger.LogTrace($"Role {oldRole.Name} is Updated Successfully");
        return new APIResponse(200, "", true);
    }
}
