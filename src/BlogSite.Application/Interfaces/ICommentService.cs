using BlogSite.Application.DTOs;

namespace BlogSite.Application.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllCommentsAsync();
    Task<IEnumerable<CommentDto>> GetCommentsByPostAsync(Guid postId);
    Task<IEnumerable<CommentDto>> GetApprovedCommentsByPostAsync(Guid postId);
    Task<CommentDto?> GetCommentByIdAsync(Guid id);
    Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto);
    Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto updateDto);
    Task<CommentDto> ApproveCommentAsync(Guid id);
    Task<CommentDto> RejectCommentAsync(Guid id);
    Task DeleteCommentAsync(Guid id);
}