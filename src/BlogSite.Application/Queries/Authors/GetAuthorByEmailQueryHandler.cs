using MediatR;
using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;

namespace BlogSite.Application.Queries.Authors;

public class GetAuthorByEmailQueryHandler : IRequestHandler<GetAuthorByEmailQuery, AuthorDto?>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public GetAuthorByEmailQueryHandler(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public async Task<AuthorDto?> Handle(GetAuthorByEmailQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByEmailAsync(request.Email);
        return author != null ? _mapper.Map<AuthorDto>(author) : null;
    }
}