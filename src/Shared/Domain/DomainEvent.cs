using System;

namespace Domain
{
    public class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn {get;}

        public DomainEvent()
        {
            this.OccurredOn = DateTime.UtcNow;
        }
    }
}