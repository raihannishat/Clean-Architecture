using AutoMapper;
using BlogApp.API.Core.Entities;
using BlogApp.API.Application.Features.Blog.DTOs;
using BlogApp.API.Application.Features.Comment.DTOs;
using BlogApp.API.Application.Features.Auth.DTOs;

namespace BlogApp.API.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<BlogPost, BlogPostDTO>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.BlogPostTags.Select(bpt => bpt.Tag.Name)));

        CreateMap<BlogPost, BlogPostListDTO>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.BlogPostTags.Select(bpt => bpt.Tag.Name)))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

        CreateMap<Comment, CommentDTO>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.AuthorProfileImageUrl, opt => opt.MapFrom(src => src.Author.ProfileImageUrl));

        CreateMap<ApplicationUser, UserDTO>();
    }
} 