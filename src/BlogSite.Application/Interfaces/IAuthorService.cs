using BlogSite.Application.DTOs;

namespace BlogSite.Application.Interfaces;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
    Task<AuthorDto?> GetAuthorByIdAsync(Guid id);
    Task<AuthorDto?> GetAuthorByEmailAsync(string email);
    Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createDto);
    Task<AuthorDto> UpdateAuthorAsync(Guid id, UpdateAuthorDto updateDto);
    Task DeleteAuthorAsync(Guid id);
}