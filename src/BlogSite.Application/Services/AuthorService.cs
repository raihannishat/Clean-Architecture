using AutoMapper;
using BlogSite.Application.DTOs;
using BlogSite.Application.Interfaces;
using BlogSite.Domain.Entities;

namespace BlogSite.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _repository;
    private readonly IMapper _mapper;

    public AuthorService(IAuthorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AuthorDto>>(authors);
    }

    public async Task<AuthorDto?> GetAuthorByIdAsync(Guid id)
    {
        var author = await _repository.GetByIdAsync(id);
        return author != null ? _mapper.Map<AuthorDto>(author) : null;
    }

    public async Task<AuthorDto?> GetAuthorByEmailAsync(string email)
    {
        var author = await _repository.GetByEmailAsync(email);
        return author != null ? _mapper.Map<AuthorDto>(author) : null;
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createDto)
    {
        // Check if email already exists
        var existingAuthor = await _repository.GetByEmailAsync(createDto.Email);
        if (existingAuthor != null)
        {
            throw new ArgumentException($"Author with email {createDto.Email} already exists.");
        }

        var author = _mapper.Map<Author>(createDto);
        var createdAuthor = await _repository.AddAsync(author);
        return _mapper.Map<AuthorDto>(createdAuthor);
    }

    public async Task<AuthorDto> UpdateAuthorAsync(Guid id, UpdateAuthorDto updateDto)
    {
        var existingAuthor = await _repository.GetByIdAsync(id);
        if (existingAuthor == null)
        {
            throw new KeyNotFoundException($"Author with ID {id} not found.");
        }

        _mapper.Map(updateDto, existingAuthor);
        var updatedAuthor = await _repository.UpdateAsync(existingAuthor);
        return _mapper.Map<AuthorDto>(updatedAuthor);
    }

    public async Task DeleteAuthorAsync(Guid id)
    {
        var author = await _repository.GetByIdAsync(id);
        if (author == null)
        {
            throw new KeyNotFoundException($"Author with ID {id} not found.");
        }

        if (author.BlogPosts.Any())
        {
            throw new InvalidOperationException("Cannot delete author that has blog posts. Delete or reassign posts first.");
        }

        await _repository.DeleteAsync(id);
    }
}