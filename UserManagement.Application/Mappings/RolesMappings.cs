using AutoMapper;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Mappings;
public class RolesMappings : Profile
{
    public RolesMappings()
    {
        CreateMap<AppRole, RoleDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.ModifiedAt))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();



    }
}
