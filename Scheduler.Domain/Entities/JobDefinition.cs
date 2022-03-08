using System;
using System.ComponentModel.DataAnnotations;

namespace Scheduler.Domain
{
    public class JobDefinition : SoftDeleteEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string AssemblyName { get; set; }
        [Required]
        public string MethodName { get; set; }
        [Required]
        public string CronExpression { get; set; }
        public bool IsPriority { get; set; }
    }
}