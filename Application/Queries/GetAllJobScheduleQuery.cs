using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllJobScheduleQuery : PagedRequest, IRequest<List<JobSchedule>>
    {
        public string? SearchString { get; set; }
    }
    public class GetJobScheduleQuery : PagedRequest, IRequest<List<JobSchedule>>
    {
        public string? SearchString { get; set; }
    }
    public class GetJobScheduleFunQuery : PagedRequest, IRequest<List<JobScheduleFun>>
    {
        public string? SearchString { get; set; }
    }
    public class InsertOrUpdateJobSchedule : JobSchedule, IRequest<JobSchedule>
    {

    }
    public class DeleteJobSchedule : JobSchedule, IRequest<long>
    {
        public JobSchedule JobSchedule { get; set; }
        public DeleteJobSchedule(JobSchedule jobSchedule)
        {
            this.JobSchedule = jobSchedule;
        }
    }
}
