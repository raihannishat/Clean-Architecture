using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence.Contexts;

namespace BlogApp.API.Infrastructure.Services
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly CommandDbContext _dbContext;
        private readonly IMongoDatabase _mongoDatabase;

        public OutboxProcessor(CommandDbContext dbContext, IMongoDatabase mongoDatabase)
        {
            _dbContext = dbContext;
            _mongoDatabase = mongoDatabase;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _dbContext.OutboxMessages
                    .Where(m => m.ProcessedOn == null)
                    .OrderBy(m => m.OccurredOn)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var collection = _mongoDatabase.GetCollection<BsonDocument>(message.Type);
                        var doc = BsonDocument.Parse(message.Payload);
                        await collection.InsertOneAsync(doc, cancellationToken: stoppingToken);
                        message.ProcessedOn = DateTime.UtcNow;
                        await _dbContext.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        // Log error if needed
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
} 