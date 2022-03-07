using Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Application.Interfaces
{
    public interface IJobDefinitionRepository : IGenericRepository<JobDefinition>
    {
        JobDefinition GetByName(string name);
        bool IsUniqueName(string name);
    }
}