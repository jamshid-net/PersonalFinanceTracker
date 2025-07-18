using FiTrack.Application.Models.Auth;
using FiTrack.Domain.Entities.Auth;

namespace FiTrack.Application.Mappings;
public class AuthMappings : Profile
{
    public AuthMappings()
    {
        CreateMap<AuthUser, UserResponseModel>();
        CreateMap<AuthRole, RoleResponseModel>();
        CreateMap<AuthRole, RoleDetailsResponseModel>()
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions.Select(p => p.Name)));
    }
}
