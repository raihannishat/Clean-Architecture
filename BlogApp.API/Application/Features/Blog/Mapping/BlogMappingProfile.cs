using AutoMapper;
using BlogApp.API.Core.Entities;
using BlogApp.API.Application.Features.Blog.DTOs;

namespace BlogApp.API.Application.Features.Blog.Mapping
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<BlogPost, BlogPostDTO>();
            CreateMap<BlogPost, BlogPostListDTO>();
            CreateMap<Tag, TagDTO>();
            CreateMap<Category, CategoryDTO>();
        }
    }
} 