using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeQuery, List<ViewEmployee>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetAllEmployeeHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetAllUserWithoutStatusQueryHandler : IRequestHandler<GetAllUserWithoutStatusQuery, List<ViewEmployee>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetAllUserWithoutStatusQueryHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllUserWithoutStatusQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _queryRepository.GetAllUserWithoutStatusAsync();
        }
    }
    public class GetAllEmployeeListHandler : IRequestHandler<GetAllEmployeeListQuery, List<ViewEmployee>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetAllEmployeeListHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllEmployeeListQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _queryRepository.GetAllUserAsync();
        }
    }
    public class ResetPasswordEmployeeHandler : IRequestHandler<GetEmployeeResetPasswordQuery, ViewEmployee>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public ResetPasswordEmployeeHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<ViewEmployee> Handle(GetEmployeeResetPasswordQuery request, CancellationToken cancellationToken)
        {
            return  await _queryRepository.ResetEmployeePasswordAsync(request);
        }
    }
    public class EmployeeOtherDutyInformationHandeler : IRequestHandler<GetEmployeeOtherDutyInformationQuery, List<View_EmployeeOtherDutyInformation>>
    {
        private readonly IEmployeeOtherDutyInformationQueryRepository _queryRepository;
        public EmployeeOtherDutyInformationHandeler(IEmployeeOtherDutyInformationQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<View_EmployeeOtherDutyInformation>> Handle(GetEmployeeOtherDutyInformationQuery request, CancellationToken cancellationToken)
        {
            return (List<View_EmployeeOtherDutyInformation>)await _queryRepository.GetAllAsync(request.Id.Value);
        }
    }
    public class EmployeeEmailInfoHandeler : IRequestHandler<GetEmployeeEmailInfoQuery, List<View_EmployeeEmailInfo>>
    {
        private readonly IEmployeeEmailInfoQueryRepository _queryRepository;
        public EmployeeEmailInfoHandeler(IEmployeeEmailInfoQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<View_EmployeeEmailInfo>> Handle(GetEmployeeEmailInfoQuery request, CancellationToken cancellationToken)
        {
            return (List<View_EmployeeEmailInfo>)await _queryRepository.GetAllAsync(request.Id.Value);
        }
    }
    public class EmployeeEmailInfoForwardHandeler : IRequestHandler<GetEmployeeEmailInfoForwardQuery, List<EmployeeEmailInfoForward>>
    {
        private readonly IEmployeeEmailInfoForwardQueryRepository _queryRepository;
        public EmployeeEmailInfoForwardHandeler(IEmployeeEmailInfoForwardQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<EmployeeEmailInfoForward>> Handle(GetEmployeeEmailInfoForwardQuery request, CancellationToken cancellationToken)
        {
            return (List<EmployeeEmailInfoForward>)await _queryRepository.GetAllAsync(request.Id);
        }
    }
    public class EmployeeEmailInfoAuthorityHandeler : IRequestHandler<GetEmployeeEmailInfoAuthorityQuery, List<EmployeeEmailInfoAuthority>>
    {
        private readonly IEmployeeEmailInfoAuthorityQueryRepository _queryRepository;
        public EmployeeEmailInfoAuthorityHandeler(IEmployeeEmailInfoAuthorityQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<EmployeeEmailInfoAuthority>> Handle(GetEmployeeEmailInfoAuthorityQuery request, CancellationToken cancellationToken)
        {
            return (List<EmployeeEmailInfoAuthority>)await _queryRepository.GetAllAsync(request.Id);
        }
    }
    public class EmployeeICTInformationHandeler : IRequestHandler<GetEmployeeICTInformationQuery, List<View_EmployeeICTInformation>>
    {
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        public EmployeeICTInformationHandeler(IEmployeeICTInformationQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<View_EmployeeICTInformation>> Handle(GetEmployeeICTInformationQuery request, CancellationToken cancellationToken)
        {
            return (List<View_EmployeeICTInformation>)await _queryRepository.GetAllAsync(request.Id);
        }
    }
    public class EmployeeICTHardInformationHandeler : IRequestHandler<GetEmployeeICTHardInformationQuery, List<View_EmployeeICTHardInformation>>
    {
        private readonly IEmployeeICTHardInformationQueryRepository _queryRepository;
        public EmployeeICTHardInformationHandeler(IEmployeeICTHardInformationQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<View_EmployeeICTHardInformation>> Handle(GetEmployeeICTHardInformationQuery request, CancellationToken cancellationToken)
        {
            return (List<View_EmployeeICTHardInformation>)await _queryRepository.GetAllAsync(request.Id);
        }
    }
    public class GetAllEmployeeByStatusHandler : IRequestHandler<GetAllEmployeeByStatusQuery, List<ViewEmployee>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetAllEmployeeByStatusHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<ViewEmployee>> Handle(GetAllEmployeeByStatusQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewEmployee>)await _queryRepository.GetAllByStatusAsync();
        }
    }
    public class GetAllEmployeeBySessionHandler : IRequestHandler<GetAllEmployeeBySessionQuery, ViewEmployee>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetAllEmployeeBySessionHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<ViewEmployee> Handle(GetAllEmployeeBySessionQuery request, CancellationToken cancellationToken)
        {

            return (ViewEmployee)await _queryRepository.GetAllBySessionAsync(request.SessionId);
        }
    }
    public class EmployeeReportToHandeler : IRequestHandler<GetEmployeeReportToQuery, List<EmployeeReportTo>>
    {
        private readonly IEmployeeReportTQueryoRepository _queryRepository;
        public EmployeeReportToHandeler(IEmployeeReportTQueryoRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<EmployeeReportTo>> Handle(GetEmployeeReportToQuery request, CancellationToken cancellationToken)
        {
            return (List<EmployeeReportTo>)await _queryRepository.GetAllByIdAsync(request.Id);
        }
    }
    public class GetApplicationPermissionHandler : IRequestHandler<GetApplicationPermissionQuery, List<ApplicationPermission>>
    {
        private readonly IEmployeeQueryRepository _queryRepository;
        public GetApplicationPermissionHandler(IEmployeeQueryRepository plantQueryRepository)
        {
            _queryRepository = plantQueryRepository;
        }
        public async Task<List<ApplicationPermission>> Handle(GetApplicationPermissionQuery request, CancellationToken cancellationToken)
        {
            return (List<ApplicationPermission>)await _queryRepository.GetAllApplicationPermissionAsync(request.RoleID);
        }
    }
}