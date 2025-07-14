namespace BlogApp.API.Application.Common
{
    public interface IOutboxService
    {
        Task AddAsync(string type, object payload, CancellationToken cancellationToken = default);
    }
} 