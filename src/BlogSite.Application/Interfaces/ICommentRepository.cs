using BlogSite.Domain.Entities;

namespace BlogSite.Application.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByPostAsync(Guid postId);
    Task<IEnumerable<Comment>> GetApprovedCommentsAsync(Guid postId);
}