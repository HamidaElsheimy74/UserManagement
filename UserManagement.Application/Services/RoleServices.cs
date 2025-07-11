using AutoMapper;
using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class RoleServices : IRoleServices
{

    private readonly IUserManagementRepository<AppRole> _roleRepository;
    private readonly IUserManagementUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleServices> _logger;
    private readonly ILocalizer _localizer;

    public RoleServices(IUserManagementRepository<AppRole> roleRepository,
                        IUserManagementUnitOfWork unitOfWork,
                        IMapper mapper, ILocalizer localizer,
                        ILogger<RoleServices> logger)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
    }
    public async Task<bool> CreateRoleAsync(AppRole role)
    {

        _logger.LogTrace("Creating role: {@Role}", role);
        if (role == null)
        {
            _logger.LogError("Role data is required.");
            return false;
        }

        var isRoleExists = await _roleRepository.AnyAsync(r => r.Name == role.Name);
        if (isRoleExists)
        {
            _logger.LogError($"This Role {role.Name} is already exist.");
            return false;
        }
        else
        {
            await _roleRepository.AddAsync(role);
            await _unitOfWork.Save();
            _logger.LogTrace($"Role {role.Name} is Created Successfully");
            return true;
        }
    }

    public async Task<bool> DeleteRoleAsync(long roleId)
    {
        _logger.LogTrace($"Deleting Role with Id: {roleId}");
        if (roleId <= 0)
        {
            _logger.LogError("RoleId is not valid.");
            return false;
        }
        var isDeleted = await _roleRepository.DeleteAsync(roleId);
        return isDeleted;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        _logger.LogTrace("Getting All Roles");
        var roles = await _roleRepository.ListAllAsync();
        var rolesDto = _mapper.Map<List<RoleDto>>(roles);
        return rolesDto;
    }

    public async Task<RoleDto> GetRolesAsync(long roleId)
    {

        _logger.LogTrace($"Getting Role with Id: {roleId}");
        if (roleId <= 0)
        {
            _logger.LogError("RoleId is not valid.");
            return null;
        }
        var role = await _roleRepository.GetByIdAsync(roleId);
        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> UpdateRoleAsync(AppRole role)
    {
        _logger.LogTrace($"Updating role with name: {role.Name}");
        if (role == null)
        {
            _logger.LogError("Role data is required.");
            return false;
        }
        var oldRole = await _roleRepository.GetByIdAsync(role.Id);
        if (oldRole == null)
        {
            _logger.LogError("Role data is required.");
            return false;
        }

        oldRole.Description = role.Description;
        oldRole.Name = role.Name;
        oldRole.ModifiedAt = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(oldRole);
        await _unitOfWork.Save();
        _logger.LogTrace($"Role {oldRole.Name} is Updated Successfully");
        return true;
    }
}
