using AutoMapper;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Mappings;
public class RolesMappings : Profile
{
    public RolesMappings()
    {
        CreateMap<AppRole, RoleDto>()
            .ForMember(dest => dest.roleName, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.updatedAt, opt => opt.MapFrom(src => src.ModifiedAt))
            .ForMember(dest => dest.createdAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.roleId, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();



    }
}
