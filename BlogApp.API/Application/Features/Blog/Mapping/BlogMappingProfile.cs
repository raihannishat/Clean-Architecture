

namespace BlogApp.API.Application.Features.Blog.Mapping
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<BlogPost, BlogPostDTO>();
            CreateMap<BlogPost, BlogPostListDTO>();
            CreateMap<Core.Entities.Tag, TagDTO>();
            CreateMap<Category, CategoryDTO>();
        }
    }
} 