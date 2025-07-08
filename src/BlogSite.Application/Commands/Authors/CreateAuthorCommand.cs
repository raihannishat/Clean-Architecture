using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Commands.Authors;

public record CreateAuthorCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Bio = null
) : IRequest<AuthorDto>;