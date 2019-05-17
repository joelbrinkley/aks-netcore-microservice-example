using System;

namespace ContactsService.Infrastructure
{
    public class Notification
    {
        public Guid Id { get; set; }
        public DateTime OccurredOn { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime? ProcessedOn { get; set; }
    }
}