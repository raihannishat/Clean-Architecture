using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BlogApp.API.Application.Common;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;

namespace BlogApp.API.Infrastructure.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly CommandDbContext _dbContext;

        public OutboxService(CommandDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(string type, object payload, CancellationToken cancellationToken = default)
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = JsonSerializer.Serialize(payload),
                OccurredOn = DateTime.UtcNow
            };
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
        }
    }
} 