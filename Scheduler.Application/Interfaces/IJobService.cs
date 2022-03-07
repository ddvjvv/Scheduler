using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Application.Interfaces
{
    public interface IJobService
    {
        void SyncDefinedJobsFromDb();
        void ClearRecurringJobQueue();
    }
}
