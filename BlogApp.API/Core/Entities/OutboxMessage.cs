using System;

namespace BlogApp.API.Core.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
    }
} 