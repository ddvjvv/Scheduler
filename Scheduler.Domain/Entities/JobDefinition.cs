using System;

namespace Scheduler.Domain
{
    public class JobDefinition : SoftDeleteEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssemblyName { get; set; }
        public string MethodName { get; set; }
        public string CronExpression { get; set; }
        public bool IsPriority { get; set; }
    }
}