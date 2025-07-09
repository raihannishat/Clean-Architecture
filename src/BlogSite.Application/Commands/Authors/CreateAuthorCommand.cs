using MediatR;
using BlogSite.Application.DTOs;
using BlogSite.Application.Attributes;

namespace BlogSite.Application.Commands.Authors;

[OperationDescription("Creates a new author in the system with the provided details", "Create Author")]
public record CreateAuthorCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Bio = null
) : IRequest<AuthorDto>;