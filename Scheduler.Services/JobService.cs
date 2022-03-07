using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using Microsoft.Extensions.Configuration;
using Scheduler.Application;
using Scheduler.Application.Interfaces;
using Scheduler.Data;
using Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scheduler.Services
{
    public class JobService : IJobService
    {
        private readonly string _server = Environment.MachineName;
        private SchedulerContext _context;
        private readonly IConfiguration configuration;
        private List<RecurringJobDto> _recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
        private RecurringJobManager jobManager = new();
        IJobDefinitionRepository _jobDefinitionRepository;

        public JobService(IJobDefinitionRepository jobDefinitionRepository, IConfiguration configuration)
        {
            this.configuration = configuration;
            _jobDefinitionRepository = jobDefinitionRepository;
        }

        public void SyncDefinedJobsFromDb()
        {
            //.ForEach(jobDefinition =>
            foreach (var jobDefinition in _jobDefinitionRepository.GetAll())
            {
                // Check if defined job is already in the recurring job queue.
                if (_recurringJobs.Where(job => job.Id == jobDefinition.Name).FirstOrDefault() is RecurringJobDto recurringJob)
                {
                    // If any settings for the job definition have changed let's update the recurring job queue.
                    string assemblyName = $"{recurringJob.Job.Type.FullName}, {recurringJob.Job.Type.Namespace}";
                    if (assemblyName != jobDefinition.AssemblyName || recurringJob.Job.Method.Name != jobDefinition.MethodName
                        || recurringJob.Cron != jobDefinition.CronExpression || recurringJob.Queue != (jobDefinition.IsPriority ? "priority" : "default"))
                    {
                        Type classType = Type.GetType(jobDefinition.AssemblyName);
                        MethodInfo method = classType.GetMethod(jobDefinition.MethodName);
                        Job job = new(classType, method);
                        jobManager.AddOrUpdate(jobDefinition.Name, job, jobDefinition.CronExpression, TimeZoneInfo.Local, jobDefinition.IsPriority ? "priority" : "default");
                    }
                }
                else
                {
                    // Defined job is not in the recurring job queue so let's add it.
                    Type classType = Type.GetType(jobDefinition.AssemblyName);
                    MethodInfo method = classType.GetMethod(jobDefinition.MethodName);
                    Job job = new(classType, method);

                    jobManager.AddOrUpdate(jobDefinition.Name, job, jobDefinition.CronExpression, TimeZoneInfo.Local, jobDefinition.IsPriority ? "priority" : "default");
                }

            }
            foreach (var job in _recurringJobs)
            {
                var jobDefinition = _jobDefinitionRepository.GetByName(job.Id); // Hangfire uses job name as identifier
                if (jobDefinition is null)
                {
                    jobManager.RemoveIfExists(job.Id);
                }
            }
        }

        public void ClearRecurringJobQueue()
        {
            _recurringJobs.ForEach(job => jobManager.RemoveIfExists(job.Id));
        }
    }
}
