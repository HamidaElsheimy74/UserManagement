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
        try
        {
            if (role == null)
            {
                _logger.LogWarning("Role data is required.");
                return false;
            }
            _logger.LogInformation("Creating role: {@Role}", role);

            var isRoleExists = await _roleRepository.AnyAsync(r => r.Name == role.Name);
            if (isRoleExists)
            {
                _logger.LogInformation($"This Role {role.Name} is already exist.");
                return false;
            }
            else
            {
                await _roleRepository.AddAsync(role);
                await _unitOfWork.Save();
                _logger.LogInformation($"Role {role.Name} is Created Successfully");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role: {@Role}", role);
            return false;
        }
    }

    public Task<bool> DeleteRoleAsync(long roleId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<RoleDto> GetRolesAsync(long roleId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateRoleAsync(AppRole role)
    {
        throw new NotImplementedException();
    }
}
