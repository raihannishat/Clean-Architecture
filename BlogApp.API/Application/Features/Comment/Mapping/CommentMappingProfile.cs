namespace BlogApp.API.Application.Features.Comment.Mapping
{
    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<BlogApp.API.Core.Entities.Comment, CommentDTO>();
        }
    }
} 