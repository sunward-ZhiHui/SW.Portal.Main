using Application.Queries;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllIpirAppQueryHandler : IRequestHandler<GetAllIpirAppQuery, List<IpirApp>>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public GetAllIpirAppQueryHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<List<IpirApp>> Handle(GetAllIpirAppQuery request, CancellationToken cancellationToken)
        {
            return (List<IpirApp>)await iIpirAppQueryRepostitory.GetAllByAsync();
        }

    }
    public class GetAllIpirMobileAppQueryHandler : IRequestHandler<GetAllIpirMobileAppQuery, List<IpirApp>>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public GetAllIpirMobileAppQueryHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<List<IpirApp>> Handle(GetAllIpirMobileAppQuery request, CancellationToken cancellationToken)
        {
            return (List<IpirApp>)await iIpirAppQueryRepostitory.GetAllListByAsync();
        }

    }
    public class GetAllIpirAppOneQueryHandler : IRequestHandler<GetAllIpirAppOneQuery, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public GetAllIpirAppOneQueryHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirApp> Handle(GetAllIpirAppOneQuery request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.GetAllByOneAsync(request.SessionId);
        }

    }
    public class IpirAppHandler : IRequestHandler<InsertOrUpdateIpirApp, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public IpirAppHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirApp> Handle(InsertOrUpdateIpirApp request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.InsertOrUpdateIpirApp(request.IpirApp);
        }

    }
    public class IpirSupervisorHandler : IRequestHandler<UpdateIpirSupervisor, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public IpirSupervisorHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirApp> Handle(UpdateIpirSupervisor request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.UpdateIpirSupervisor(request.IpirApp);
        }

    }
    public class IpirPICHandler : IRequestHandler<UpdateIpirPIC, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public IpirPICHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirApp> Handle(UpdateIpirPIC request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.UpdateIpirPIC(request.IpirApp);
        }

    }
    public class DeleteIpirAppHandler : IRequestHandler<DeleteIpirApp, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public DeleteIpirAppHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirApp> Handle(DeleteIpirApp request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.DeleteIpirApp(request.IpirApp);
        }

    }

    public class GetAllIpirAppCheckDetailsQueryHandler : IRequestHandler<GetIpirAppDetails, List<IpirAppCheckedDetailsModel>>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public GetAllIpirAppCheckDetailsQueryHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<List<IpirAppCheckedDetailsModel>> Handle(GetIpirAppDetails request, CancellationToken cancellationToken)
        {
            return (List<IpirAppCheckedDetailsModel>)await iIpirAppQueryRepostitory.GetIpirAppDetails(request.IpirAppCheckedDetailsModel);
        }
    }
    public class DeleteIpirAppCheckedDetailsHandler : IRequestHandler<DeleteIpirAppCheckedDetails, IpirAppCheckedDetailsModel>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public DeleteIpirAppCheckedDetailsHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirAppCheckedDetailsModel> Handle(DeleteIpirAppCheckedDetails request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.DeleteIpirAppCheckedDetails(request.IpirAppCheckedDetailsModel);
        }
    }
    public class IpirAppCheckedDetailsHandler : IRequestHandler<InsertIpirAppCheckedDetails, IpirAppCheckedDetailsModel>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public IpirAppCheckedDetailsHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirAppCheckedDetailsModel> Handle(InsertIpirAppCheckedDetails request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.InsertIpirAppCheckedDetails(request.IpirAppCheckedDetailsModel);
        }
    }
    public class IpirReportinginformationHandler : IRequestHandler<InsertOrUpdateIpirReportinginformation, IPIRReportingInformation>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public IpirReportinginformationHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IPIRReportingInformation> Handle(InsertOrUpdateIpirReportinginformation request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.InsertOrUpdateIpirReportingInformation(request.IpirApp);
        }

    }
    public class DeleteIpirReportingInformationHandler : IRequestHandler<DeleteIpirReportingInformation, IPIRReportingInformation>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public DeleteIpirReportingInformationHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IPIRReportingInformation> Handle(DeleteIpirReportingInformation request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.DeleteIpirReportingInformation(request.ReportingInformation);
        }

    }
    public class UpdateDynamicFormDataIssueDeltailsHandler : IRequestHandler<UpdateDynamicFormDataIssueDetails, IpirAppIssueDep>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public UpdateDynamicFormDataIssueDeltailsHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirAppIssueDep> Handle(UpdateDynamicFormDataIssueDetails request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.UpdateDynamicFormDataIssueDetails(request.SessionId, request.ActivityInfoIssueId, request.DynamicFormDataId);
        }
    }
    public class GetIpirAppIssueDepByDynamicFormHandler : IRequestHandler<GetIpirAppIssueDepByDynamicForm, IpirAppIssueDep>
    {
        private readonly IIpirAppQueryRepostitory iIpirAppQueryRepostitory;
        public GetIpirAppIssueDepByDynamicFormHandler(IIpirAppQueryRepostitory _iIpirAppQueryRepostitory)
        {
            iIpirAppQueryRepostitory = _iIpirAppQueryRepostitory;
        }
        public async Task<IpirAppIssueDep> Handle(GetIpirAppIssueDepByDynamicForm request, CancellationToken cancellationToken)
        {
            return await iIpirAppQueryRepostitory.GetIpirAppIssueDepByDynamicForm(request.IpirAppIssueDepId);
        }

    }
    public class GetEmailIpirAppBySessionIdHandler : IRequestHandler<GetEmailIpirAppBySessionId, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory _dynamicFormQueryRepository;
        public GetEmailIpirAppBySessionIdHandler(IIpirAppQueryRepostitory dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<IpirApp> Handle(GetEmailIpirAppBySessionId request, CancellationToken cancellationToken)
        {
            return await _dynamicFormQueryRepository.GetEmailIpirAppBySessionId(request.SessionId);
        }


    }
    public class InsertCreateEmailIpirAppHandler : IRequestHandler<InsertCreateEmailIpirApp, IpirApp>
    {
        private readonly IIpirAppQueryRepostitory _queryRepository;
        public InsertCreateEmailIpirAppHandler(IIpirAppQueryRepostitory dynamicFormQueryRepository)
        {
            _queryRepository = dynamicFormQueryRepository;
        }
        public async Task<IpirApp> Handle(InsertCreateEmailIpirApp request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertCreateEmailIpirApp(request.IpirApp);
        }
    }
}

