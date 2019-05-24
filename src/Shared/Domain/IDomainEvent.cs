using System;

namespace Domain
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}