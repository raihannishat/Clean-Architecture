

namespace BlogApp.API.Infrastructure.Services
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMongoDatabase _mongoDatabase;

        public OutboxProcessor(IServiceProvider serviceProvider, IMongoDatabase mongoDatabase)
        {
            _serviceProvider = serviceProvider;
            _mongoDatabase = mongoDatabase;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
                    var messages = await dbContext.OutboxMessages
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
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            // Log error if needed
                        }
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
} 