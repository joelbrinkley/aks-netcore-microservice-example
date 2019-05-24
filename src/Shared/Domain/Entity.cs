using System;
using System.Collections.Generic;

namespace Domain
{
    public abstract class Entity
    {
        private bool isTrackingEvents = true;
        protected List<IDomainEvent> domainEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents?.AsReadOnly();

        protected void TrackEvents()
        {
            this.isTrackingEvents = true;
        }

        protected void StopTrackingEvents()
        {
            this.isTrackingEvents = false;
        }

        protected void AddEvent(IDomainEvent domainEvent)
        {
            if (this.isTrackingEvents)
            {
                this.domainEvents.Add(domainEvent);
            }
        }

        public void ClearEvents()
        {
            this.domainEvents.Clear();
        }
    }


}
