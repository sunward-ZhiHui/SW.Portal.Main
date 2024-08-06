using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllJobScheduleHandler : IRequestHandler<GetAllJobScheduleQuery, List<JobSchedule>>
    {
        private readonly IJobScheduleQueryRepository _queryRepository;
        public GetAllJobScheduleHandler(IJobScheduleQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<JobSchedule>> Handle(GetAllJobScheduleQuery request, CancellationToken cancellationToken)
        {
            return (List<JobSchedule>)await _queryRepository.GetAllByAsync();

        }
    }
    public class DeleteJobScheduleHandler : IRequestHandler<DeleteJobSchedule, long>
    {
        private readonly IJobScheduleQueryRepository _queryRepository;

        public DeleteJobScheduleHandler(IJobScheduleQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteJobSchedule request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteJobSchedule(request.JobSchedule);
        }
    }
    public class InsertOrUpdateJobScheduleHandler : IRequestHandler<InsertOrUpdateJobSchedule, JobSchedule>
    {
        private readonly IJobScheduleQueryRepository _queryRepository;

        public InsertOrUpdateJobScheduleHandler(IJobScheduleQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<JobSchedule> Handle(InsertOrUpdateJobSchedule request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateJobSchedule(request);
        }
    }
    public class GetJobScheduleHandler : IRequestHandler<GetJobScheduleQuery, List<JobSchedule>>
    {
        private readonly IJobScheduleQueryRepository _queryRepository;
        public GetJobScheduleHandler(IJobScheduleQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<JobSchedule>> Handle(GetJobScheduleQuery request, CancellationToken cancellationToken)
        {
            return (List<JobSchedule>)await _queryRepository.GetJobScheduleAsync();

        }
    }
    public class GetJobScheduleFunQueryHandler : IRequestHandler<GetJobScheduleFunQuery, List<JobScheduleFun>>
    {
        private readonly IJobScheduleQueryRepository _queryRepository;
        public GetJobScheduleFunQueryHandler(IJobScheduleQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<JobScheduleFun>> Handle(GetJobScheduleFunQuery request, CancellationToken cancellationToken)
        {
            return (List<JobScheduleFun>)await _queryRepository.GetJobScheduleFunAsync();

        }
    }
}
