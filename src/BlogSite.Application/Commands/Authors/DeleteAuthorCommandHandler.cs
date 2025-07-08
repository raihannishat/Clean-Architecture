using MediatR;
using BlogSite.Application.Interfaces;

namespace BlogSite.Application.Commands.Authors;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, bool>
{
    private readonly IAuthorRepository _authorRepository;

    public DeleteAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<bool> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(request.Id);
        if (author == null)
        {
            return false;
        }

        await _authorRepository.DeleteAsync(request.Id);
        return true;
    }
}