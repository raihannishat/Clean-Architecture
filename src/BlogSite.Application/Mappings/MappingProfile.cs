using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.BlogPosts.Count));

        CreateMap<CreateAuthorDto, Author>();
        CreateMap<UpdateAuthorDto, Author>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPosts, opt => opt.Ignore());

        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.BlogPosts.Count));

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPosts, opt => opt.Ignore());

        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.FullName))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

        CreateMap<CreateBlogPostDto, BlogPost>();
        CreateMap<UpdateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore());

        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.BlogPostTitle, opt => opt.MapFrom(src => src.BlogPost.Title));

        CreateMap<CreateCommentDto, Comment>();
        CreateMap<UpdateCommentDto, Comment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPostId, opt => opt.Ignore())
            .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPost, opt => opt.Ignore());
    }
}