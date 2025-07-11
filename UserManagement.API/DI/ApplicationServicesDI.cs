using UserManagement.Application.Interfaces;
using UserManagement.Application.Mappings;
using UserManagement.Application.Services;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Data.Repositories;
using UserManagement.Infrastructure.Services;

namespace UserManagement.API.DI;

public static class ApplicationServicesDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(RolesMappings));
        services.AddAutoMapper(typeof(UserMappings));
        services.AddAutoMapper(typeof(UserRolesMappings));
        services.AddScoped<IAccountServices, AccountServices>();
        services.AddScoped<IRoleServices, RoleServices>();
        services.AddScoped<IUserServices, UserServices>();
        services.AddScoped<IUserRoleServices, UserRoleServices>();
        services.AddScoped(typeof(IUserManagementRepository<>), typeof(UserManagementRepository<>));
        services.AddScoped<IUserManagementUnitOfWork, UserManagementUnitOfWork>();
        services.AddSingleton<ILocalizer, Localizer>();
        return services;
    }
}
