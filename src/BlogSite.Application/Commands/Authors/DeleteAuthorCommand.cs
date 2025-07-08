using MediatR;

namespace BlogSite.Application.Commands.Authors;

public record DeleteAuthorCommand(Guid Id) : IRequest<bool>;