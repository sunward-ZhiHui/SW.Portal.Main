using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IJobScheduleQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<JobSchedule>> GetAllByAsync();
        Task<JobSchedule> InsertOrUpdateJobSchedule(JobSchedule jobSchedule);
        Task<long> DeleteJobSchedule(JobSchedule jobSchedule);
        Task<IReadOnlyList<JobSchedule>> GetJobScheduleAsync();
        Task<IReadOnlyList<JobScheduleFun>> GetJobScheduleFunAsync();
        Task<string> GetJobScheduleNavFuctionAsync(string JobType);

    }
}
