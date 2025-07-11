using AutoMapper;
using Microsoft.Extensions.Logging;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services;
public class UserRoleServices : IUserRoleServices
{
    private readonly IUserManagementRepository<UserRoleDto> _userRoleRepository;
    private readonly IUserManagementUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserRoleServices> _logger;
    private readonly ILocalizer _localizer;

    public UserRoleServices(IUserManagementRepository<UserRoleDto> userRoleRepository,
                            IUserManagementUnitOfWork unitOfWork,
                            IMapper mapper, ILocalizer localizer,
                            ILogger<UserRoleServices> logger)
    {
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<bool> AddUserRoleAsync(UserRoleDto userRoleDto)
    {

        _logger.LogInformation("Adding user role: {@UserRoleDto}", userRoleDto);
        if (userRoleDto == null)
        {
            _logger.LogWarning("User Role" + _localizer["RequiredDto"]);
            return false;
        }

        return true;
    }

    public Task<bool> DeleteUserRoleAsync(long userId, long roleId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserRoleDto>> GetUserRolesAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserRoleAsync(UserRoleDto userRoleDto)
    {
        throw new NotImplementedException();
    }
}
