using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _repository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync()
    {
        var comments = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsByPostAsync(Guid postId)
    {
        var comments = await _repository.GetCommentsByPostAsync(postId);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<IEnumerable<CommentDto>> GetApprovedCommentsByPostAsync(Guid postId)
    {
        var comments = await _repository.GetApprovedCommentsAsync(postId);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);
        return comment != null ? _mapper.Map<CommentDto>(comment) : null;
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createDto)
    {
        var comment = _mapper.Map<Comment>(createDto);
        comment.IsApproved = false; // Comments start as unapproved
        
        var createdComment = await _repository.AddAsync(comment);
        
        // Reload to get related data
        var commentWithRelations = await _repository.GetByIdAsync(createdComment.Id);
        return _mapper.Map<CommentDto>(commentWithRelations);
    }

    public async Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto updateDto)
    {
        var existingComment = await _repository.GetByIdAsync(id);
        if (existingComment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {id} not found.");
        }

        _mapper.Map(updateDto, existingComment);
        var updatedComment = await _repository.UpdateAsync(existingComment);
        return _mapper.Map<CommentDto>(updatedComment);
    }

    public async Task<CommentDto> ApproveCommentAsync(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {id} not found.");
        }

        comment.IsApproved = true;
        var updatedComment = await _repository.UpdateAsync(comment);
        return _mapper.Map<CommentDto>(updatedComment);
    }

    public async Task<CommentDto> RejectCommentAsync(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {id} not found.");
        }

        comment.IsApproved = false;
        var updatedComment = await _repository.UpdateAsync(comment);
        return _mapper.Map<CommentDto>(updatedComment);
    }

    public async Task DeleteCommentAsync(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {id} not found.");
        }

        await _repository.DeleteAsync(id);
    }
}