using MediatR;
using BlogSite.Application.DTOs;

namespace BlogSite.Application.Commands.Authors;

public record UpdateAuthorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Bio = null
) : IRequest<AuthorDto>;