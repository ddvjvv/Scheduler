using Microsoft.EntityFrameworkCore;
using Scheduler.Application.Interfaces;
using Scheduler.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Scheduler.Data.Repositories
{
    public class JobDefinitionsRepository : GenericRepository<JobDefinition>, IJobDefinitionRepository
    {
        private readonly DbSet<JobDefinition> _jobDefinitions;

        public JobDefinitionsRepository(SchedulerContext context) : base(context)
        {
            _jobDefinitions = context.Set<JobDefinition>();
        }

        public JobDefinition GetByName(string name)
        {
            return _jobDefinitions.FirstOrDefault(job => job.Name == name);
        }

        public override IReadOnlyList<JobDefinition> GetPagedList(int pageNumber, int pageSize)
        {
            return _jobDefinitions.Where(job => job.IsDeleted == false)
                .Skip<JobDefinition>((pageNumber - 1) * pageSize).Take<JobDefinition>(pageSize).ToList();
        }

        public override IReadOnlyList<JobDefinition> GetAll()
        {
            return _jobDefinitions.Where(job => job.IsDeleted == false).ToList();
        }

        public bool IsUniqueName(string name)
        {
            return _jobDefinitions.All(job => job.Name != name);
        }
    }
}