using AutoMapper;
using BlogApp.API.Core.Entities;
using BlogApp.API.Application.Features.Auth.DTOs;

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