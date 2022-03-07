using System;

namespace Scheduler.Domain
{
    public abstract class BaseEntity
    {
        public virtual int Id { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime Created { get; set; }
    }
}