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
    public class GetSchedulerListHandler : IRequestHandler<GetSchedulerList, List<Appointment>>
    {

        private readonly IDashboardQueryRepository _queryRepository;
        public GetSchedulerListHandler(IDashboardQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<Appointment>> Handle(GetSchedulerList request, CancellationToken cancellationToken)
        {

            return (List<Appointment>)await _queryRepository.GetSchedulerListAsync();
        }
    }
    public class GetUserListHandler : IRequestHandler<GetUserListQuery, List<Appointment>>
    {

        private readonly IDashboardQueryRepository _queryRepository;
        public GetUserListHandler(IDashboardQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<Appointment>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {

            return (List<Appointment>)await _queryRepository.GetUserListAsync(request.AppointmentID);
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
    public class GetAppointmentListHandler : IRequestHandler<GetAppointmentList, List<Appointment>>
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public GetAppointmentListHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<List<Appointment>> Handle(GetAppointmentList request, CancellationToken cancellationToken)
        {
            return (List<Appointment>)await _dashboardQueryRepository.GetAppointments(request.UserId);

        }

    }
    public class AddAppointmentHandler : IRequestHandler<AddAppointment, long>
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public AddAppointmentHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<long> Handle(AddAppointment request, CancellationToken cancellationToken)
        {
            var newlist = await _dashboardQueryRepository.AddAppointmentAsync(request);
            request.ID = newlist;
            if(request.userIds != null)
            {
                foreach (var item in request.userIds)
                {
                    request.UserID = item;
                    var newappointment = await _dashboardQueryRepository.AddAppointmentinsertAsync(request);
                }
            }
           

            return newlist;

        }
    }
    public class EditAppointmentHandler : IRequestHandler<EditAppointment, long>
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public EditAppointmentHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<long> Handle(EditAppointment request, CancellationToken cancellationToken)
        {
            var newlist = await _dashboardQueryRepository.UpdateAppointmentAsync(request);
           
            if (request.userIds != null)
            {
                var deletemultiple = await _dashboardQueryRepository.DeleteUsermultipleAsync(request.ID);
                foreach (var item in request.userIds)
                {
                    request.UserID = item;
                    var newappointment = await _dashboardQueryRepository.AddAppointmentinsertAsync(request);
                }
            }
            return newlist;
        }
    }
    public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointment, long>
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public DeleteAppointmentHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<long> Handle(DeleteAppointment request, CancellationToken cancellationToken)
        {
            var list = await _dashboardQueryRepository.GetUserListAsync(request.Id);
            if(list != null)
            {
                var deletemultiple = await _dashboardQueryRepository.DeleteUsermultipleAsync(request.Id);
            }
            var newlist = await _dashboardQueryRepository.DeleteAppointmentAsync(request.Id);
            return newlist;
        }
    }
    public class DynamicApprovalHandler : IRequestHandler<DynamicApprovalListOne, List<DynamicForm>>
    {

        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public DynamicApprovalHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<List<DynamicForm>> Handle(DynamicApprovalListOne request, CancellationToken cancellationToken)
        {
            return (List<DynamicForm>)await _dashboardQueryRepository.GetApprovalListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class DynamicApprovaltwoHandler : IRequestHandler<DynamicApprovalListtwo, List<DynamicFormData>>
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public DynamicApprovaltwoHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<List<DynamicFormData>> Handle(DynamicApprovalListtwo request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormData>)await _dashboardQueryRepository.GetDynamicDataAsync(request.DynamicId);
            
        }
    }
    
    public class DynamicApprovalThiredHandler : IRequestHandler<DynamicApprovalListthired, List<DynamicFormApproved>>
    {
        private readonly IDashboardQueryRepository _dashboardQueryRepository;
        public DynamicApprovalThiredHandler(IDashboardQueryRepository dashboardQueryRepository)
        {
            _dashboardQueryRepository = dashboardQueryRepository;
        }
        public async Task<List<DynamicFormApproved>> Handle(DynamicApprovalListthired request, CancellationToken cancellationToken)
        {
            return (List<DynamicFormApproved>)await _dashboardQueryRepository.GetDynamicApprovedStatusAsync(request.FormDataId);

        }
    }
}
