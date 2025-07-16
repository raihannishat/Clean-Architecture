

namespace BlogApp.API.Application.Features.Auth.Mapping
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
} 