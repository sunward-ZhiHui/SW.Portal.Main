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

namespace CMS.Application.Handlers.QueryHandlers
{
    public class DashobardHandler : IRequestHandler<Dashboard, List<EmailScheduler>>
    {
       
        private readonly IQueryRepository<EmailScheduler> _queryRepository;
        public DashobardHandler(IQueryRepository<EmailScheduler> queryRepository)
        {           
            _queryRepository= queryRepository;
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
    public class GetEmailSchedulerListHandler : IRequestHandler<GetEmailSchedulerList, List<EmailScheduler>>
    {   
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public GetEmailSchedulerListHandler(IDashboardQueryRepository dashboardQueryRepository)
        {            
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<List<EmailScheduler>> Handle(GetEmailSchedulerList request, CancellationToken cancellationToken)
        {
            return (List<EmailScheduler>)await _dashboardQueryRepository.GetAllEmailSchedulerAsync();
            //

            //DateTime date = DateTime.Now;
            //var dataSource = new List<EmailScheduler>() {
            //    new EmailScheduler {
            //        Caption = "Install New Router in Dev Room",
            //        StartDate = date + (new TimeSpan(0, 10, 0, 0)),
            //        EndDate = date + (new TimeSpan(0, 12, 30, 0)),
            //        Label = 6,
            //        Status = 4
            //    },
            //    new EmailScheduler {
            //        Caption = "Upgrade Personal Computers",
            //        StartDate = date + (new TimeSpan(0,  13, 0, 0)),
            //        EndDate = date + (new TimeSpan(0, 15, 30, 0)),
            //        Label = 1,
            //        Status = 4
            //    },
            //    new EmailScheduler {
            //        Caption = "Website Redesign Plan",
            //        StartDate = date + (new TimeSpan(1, 9, 30, 0)),
            //        EndDate = date + (new TimeSpan(1, 12, 0, 0)),
            //        Label = 1,
            //        Status = 1,
            //        Accepted = true
            //    },
            //    new EmailScheduler {
            //        Caption = "New Brochures",
            //        StartDate = date + (new TimeSpan(1, 13, 30, 0)),
            //        EndDate = date + (new TimeSpan(1, 15, 15, 0)),
            //        Label = 8,
            //        Status = 2,
            //        Accepted = true
            //    },
            //    new EmailScheduler {
            //        Caption = "Book Flights to San Fran for Sales Trip",
            //        StartDate = date + (new TimeSpan(1, 12, 0, 0)),
            //        EndDate = date + (new TimeSpan(1, 13, 0, 0)),
            //        AllDay = true,
            //        Label = 8,
            //        Status = 1
            //    },
            //    new EmailScheduler {
            //        Caption = "Approve Personal Computer Upgrade Plan",
            //        StartDate = date + (new TimeSpan(2, 10, 0, 0)),
            //        EndDate = date + (new TimeSpan(2, 13, 0, 0)),
            //        Label = 8,
            //        Status = 2
            //    },
            //    new EmailScheduler {
            //        Caption = "Final Budget Review",
            //        StartDate = date + (new TimeSpan(2, 14, 0, 0)),
            //        EndDate = date + (new TimeSpan(2, 16, 30, 0)),
            //        Label = 1,
            //        Status = 1
            //    },
            //    new EmailScheduler {
            //        Caption = "Install New Database",
            //        StartDate = date + (new TimeSpan(3, 9, 45, 0)),
            //        EndDate = date + (new TimeSpan(3, 11, 45, 0)),
            //        Label = 6,
            //        Status = 4,
            //        Accepted = true
            //    },
            //    new EmailScheduler {
            //        Caption = "Approve New Online Marketing Strategy",
            //        StartDate = date + (new TimeSpan(3,  12, 30, 0)),
            //        EndDate = date + (new TimeSpan(3, 15, 30, 0)),
            //        Label = 1,
            //        Status = 1,
            //        Accepted = true
            //    },
            //    new EmailScheduler {
            //        Caption = "Customer Workshop",
            //        StartDate = date + (new TimeSpan(4,  11, 0, 0)),
            //        EndDate = date + (new TimeSpan(4, 12, 0, 0)),
            //        AllDay = true,
            //        Label = 8,
            //        Status = 1
            //    },
            //    new EmailScheduler {
            //        Caption = "Prepare 2021 Marketing Plan",
            //        StartDate = date + (new TimeSpan(4,  10, 30, 0)),
            //        EndDate = date + (new TimeSpan(4, 13, 0, 0)),
            //        Label = 1,
            //        Status = 1,
            //        Accepted = true
            //    },
            //    new EmailScheduler {
            //        Caption = "Brochure Design Review",
            //        StartDate = date + (new TimeSpan(4, 14, 0, 0)),
            //        EndDate = date + (new TimeSpan(4, 16, 30, 0)),
            //        Label = 1,
            //        Status = 2,
            //        Accepted = true
            //    },
            //    new EmailScheduler {
            //        Caption = "Create Icons for Website",
            //        StartDate = date + (new TimeSpan(5, 10, 0, 0)),
            //        EndDate = date + (new TimeSpan(5, 12, 30, 0)),
            //        Label = 1,
            //        Status = 1
            //    },
            //    new EmailScheduler {
            //        Caption = "Launch New Website",
            //        StartDate = date + (new TimeSpan(5, 13, 20, 0)),
            //        EndDate = date + (new TimeSpan(5, 16, 0, 0)),
            //        Label = 8,
            //        Status = 1
            //    },
            //    new EmailScheduler {
            //        Caption = "Upgrade Server Hardware",
            //        StartDate = date + (new TimeSpan(6, 11, 0, 0)),
            //        EndDate = date + (new TimeSpan(6, 13, 30, 0)),
            //        Label = 8,
            //        Status = 1
            //    }
            //};
            //return dataSource;
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
    }
}
