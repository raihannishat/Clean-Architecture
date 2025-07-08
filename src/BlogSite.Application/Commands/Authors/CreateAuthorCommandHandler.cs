using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Commands.Authors;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public CreateAuthorCommandHandler(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingAuthor = await _authorRepository.GetByEmailAsync(request.Email);
        if (existingAuthor != null)
        {
            throw new InvalidOperationException($"Author with email {request.Email} already exists.");
        }

        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Bio = request.Bio,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdAuthor = await _authorRepository.AddAsync(author);
        return _mapper.Map<AuthorDto>(createdAuthor);
    }
}