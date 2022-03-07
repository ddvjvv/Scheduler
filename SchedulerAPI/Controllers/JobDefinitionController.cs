using Microsoft.AspNetCore.Mvc;
using Scheduler.Application;
using Scheduler.Application.Interfaces;
using Scheduler.Data;
using Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobDefinitionController : ControllerBase
    {
        private IJobDefinitionRepository _jobDefinitionRepository;
        public JobDefinitionController(IJobDefinitionRepository jobDefinitionRepository)
        {
            _jobDefinitionRepository = jobDefinitionRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<JobDefinition>> GetAll()
        {
            return _jobDefinitionRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<JobDefinition> Get(int id)
        {
            if (_jobDefinitionRepository.GetById(id) is JobDefinition value)
            {
                return value;
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Post([FromBody] JobDefinition jobDefinition)
        {
            // Job name is the identifier in the Hangfire recurring job queue.
            // So, we need to make sure it's unique before posting.
            if (_jobDefinitionRepository.IsUniqueName(jobDefinition.Name))
            {
                //jobDefinition.Id = 0;
                jobDefinition = _jobDefinitionRepository.Add(jobDefinition);
                Uri uri = new Uri($"{Url.Link(null, null)}/{jobDefinition.Id}");
                return Created(uri, jobDefinition);
            }

            return Conflict("Job name must be unique.");
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] JobDefinition jobDefinition)
        {
            if (_jobDefinitionRepository.GetById(id) is JobDefinition value)
            {
                value.Description = jobDefinition.Description;
                value.AssemblyName = jobDefinition.AssemblyName;
                value.MethodName = jobDefinition.MethodName;
                value.CronExpression = jobDefinition.CronExpression;
                value.IsPriority = jobDefinition.IsPriority;
                value.IsDeleted = jobDefinition.IsDeleted;
                _jobDefinitionRepository.Update(value);
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (_jobDefinitionRepository.GetById(id) is JobDefinition value)
            {
                _jobDefinitionRepository.Delete(value);
                return Ok();
            }

            return NotFound();
        }
    }
}