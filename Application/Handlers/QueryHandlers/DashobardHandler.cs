using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;
using System;
using System.Collections.Generic;
using static Application.Queries.GetEmailRatio;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class DashobardHandler : IRequestHandler<Dashboard, List<EmailScheduler>>
    {

        private readonly IQueryRepository<EmailScheduler> _queryRepository;
        public DashobardHandler(IQueryRepository<EmailScheduler> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailScheduler>> Handle(Dashboard request, CancellationToken cancellationToken)
        {
            return (List<EmailScheduler>)await _queryRepository.GetListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class EmailDashobardHandler : IRequestHandler<GetEmailDasboard, List<EmailTopics>>
    {

        private readonly IDashboardQueryRepository _queryRepository;
        public EmailDashobardHandler(IDashboardQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<EmailTopics>> Handle(GetEmailDasboard request, CancellationToken cancellationToken)
        {
            return (List<EmailTopics>)await _queryRepository.GetEailDashboard();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
   

        public class GetEmployeeCountHandler : IRequestHandler<GetEmployeeCount, List<GeneralDashboard>>
        {

            private readonly IDashboardQueryRepository _queryRepository;
            public GetEmployeeCountHandler(IDashboardQueryRepository queryRepository)
            {
                _queryRepository = queryRepository;
            }
            public async Task<List<GeneralDashboard>> Handle(GetEmployeeCount request, CancellationToken cancellationToken)
            {

                return (List<GeneralDashboard>)await _queryRepository.GetEmployeeCountAsync();
            }
        }
        public class GetGenderRatioHandler : IRequestHandler<GetGenderRatio, List<GenderRatio>>
        {

            private readonly IDashboardQueryRepository _queryRepository;
            public GetGenderRatioHandler(IDashboardQueryRepository queryRepository)
            {
                _queryRepository = queryRepository;
            }
            public async Task<List<GenderRatio>> Handle(GetGenderRatio request, CancellationToken cancellationToken)
            {

                return (List<GenderRatio>)await _queryRepository.GetGenderRatioAsync();
            }
        }

        public class EmailRatioHandler : IRequestHandler<GetEmailRatio, List<EmailRatio>>
        {

            private readonly IDashboardQueryRepository _queryRepository;
            public EmailRatioHandler(IDashboardQueryRepository queryRepository)
            {
                _queryRepository = queryRepository;
            }
            public async Task<List<EmailRatio>> Handle(GetEmailRatio request, CancellationToken cancellationToken)
            {

                return (List<EmailRatio>)await _queryRepository.GetEmailRatioAsync(request.UserId);
            }
        }


        public class GetEmailSchedulerListHandler1 : IRequestHandler<GetEmailSchedulerListTodo, List<EmailScheduler>>
        {
            private readonly IDashboardQueryRepository _dashboardQueryRepository;
            public GetEmailSchedulerListHandler1(IDashboardQueryRepository dashboardQueryRepository)
            {
                _dashboardQueryRepository = dashboardQueryRepository;
            }
            public async Task<List<EmailScheduler>> Handle(GetEmailSchedulerListTodo request, CancellationToken cancellationToken)
            {
                return (List<EmailScheduler>)await _dashboardQueryRepository.GetAllEmailSchedulerTodoAsync(request.UserId);

            }

        }
    
}
