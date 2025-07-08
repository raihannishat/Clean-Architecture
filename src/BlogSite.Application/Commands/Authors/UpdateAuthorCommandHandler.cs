using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.Application.Commands.Authors;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, AuthorDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public UpdateAuthorCommandHandler(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public async Task<AuthorDto> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(request.Id);
        if (author == null)
        {
            throw new KeyNotFoundException($"Author with ID {request.Id} not found.");
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Bio = request.Bio;
        author.UpdatedAt = DateTime.UtcNow;

        var updatedAuthor = await _authorRepository.UpdateAsync(author);
        return _mapper.Map<AuthorDto>(updatedAuthor);
    }
}