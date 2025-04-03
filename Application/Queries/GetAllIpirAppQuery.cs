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
    public class GetAllIpirAppQuery : PagedRequest, IRequest<List<IpirApp>>
    {
        public string? SearchString { get; set; }
    }
    public class GetAllIpirMobileAppQuery : PagedRequest, IRequest<List<IpirApp>>
    {
        public string? SearchString { get; set; }
    }
    public class GetAllIpirAppOneQuery : PagedRequest, IRequest<IpirApp>
    {
        public string? SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetAllIpirAppOneQuery(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class UpdateIpirSupervisor : PagedRequest, IRequest<IpirApp>
    {
        public IpirApp IpirApp { get; private set; }
        public UpdateIpirSupervisor(IpirApp ipirApp)
        {
            this.IpirApp = ipirApp;
        }
    }
    public class UpdateIpirPIC : PagedRequest, IRequest<IpirApp>
    {
        public IpirApp IpirApp { get; private set; }
        public UpdateIpirPIC(IpirApp ipirApp)
        {
            this.IpirApp = ipirApp;
        }
    }
    public class InsertOrUpdateIpirApp : PagedRequest, IRequest<IpirApp>
    {
        public IpirApp IpirApp { get; private set; }
        public InsertOrUpdateIpirApp(IpirApp ipirApp)
        {
            this.IpirApp = ipirApp;
        }
    }
    public class DeleteIpirApp : PagedRequest, IRequest<IpirApp>
    {
        public IpirApp IpirApp { get; private set; }
        public DeleteIpirApp(IpirApp ipirApp)
        {
            this.IpirApp = ipirApp;
        }
    }

    public class InsertIpirAppCheckedDetails : PagedRequest, IRequest<IpirAppCheckedDetailsModel>
    {
        public IpirAppCheckedDetailsModel IpirAppCheckedDetailsModel { get; private set; }
        public InsertIpirAppCheckedDetails(IpirAppCheckedDetailsModel ipirAppCheckedDetailsModel)
        {
            this.IpirAppCheckedDetailsModel = ipirAppCheckedDetailsModel;
        }
    }
    public class DeleteIpirAppCheckedDetails : PagedRequest, IRequest<IpirAppCheckedDetailsModel>
    {
        public IpirAppCheckedDetailsModel IpirAppCheckedDetailsModel { get; private set; }
        public DeleteIpirAppCheckedDetails(IpirAppCheckedDetailsModel ipirAppCheckedDetailsModel)
        {
            this.IpirAppCheckedDetailsModel = ipirAppCheckedDetailsModel;
        }
    }
    public class GetIpirAppDetails : PagedRequest, IRequest<List<IpirAppCheckedDetailsModel>>
    {
        public long? IpirAppCheckedDetailsModel { get; private set; }
        public GetIpirAppDetails(long? ipirAppCheckedDetailsModel)
        {
            this.IpirAppCheckedDetailsModel = ipirAppCheckedDetailsModel;
        }
    }
    public class InsertOrUpdateIpirReportinginformation : PagedRequest, IRequest<IPIRReportingInformation>
    {
        public IPIRReportingInformation IpirApp { get; private set; }
        public InsertOrUpdateIpirReportinginformation(IPIRReportingInformation ipirApp)
        {
            this.IpirApp = ipirApp;
        }
    }
    public class DeleteIpirReportingInformation : PagedRequest, IRequest<IPIRReportingInformation>
    {
        public IPIRReportingInformation ReportingInformation { get; private set; }
        public DeleteIpirReportingInformation(IPIRReportingInformation reportingInformation)
        {
            this.ReportingInformation = reportingInformation;
        }
    }

    public class UpdateDynamicFormDataIssueDetails : PagedRequest, IRequest<IpirAppIssueDep>
    {
        public Guid? SessionId { get; set; }
        public long? ActivityInfoIssueId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public UpdateDynamicFormDataIssueDetails(Guid? sessionId, long? activityInfoIssueId, long? dynamicFormDataId)
        {
            this.SessionId = sessionId;
            this.ActivityInfoIssueId = activityInfoIssueId;
            this.DynamicFormDataId = dynamicFormDataId;
        }
    }
    public class GetIpirAppIssueDepByDynamicForm : PagedRequest, IRequest<IpirAppIssueDep>
    {
        public long? IpirAppIssueDepId { get; set; }
        public GetIpirAppIssueDepByDynamicForm(long? ipirAppIssueDepId)
        {
            this.IpirAppIssueDepId = ipirAppIssueDepId;
        }
    }
    public class GetEmailIpirAppBySessionId : PagedRequest, IRequest<IpirApp>
    {
        public string? SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetEmailIpirAppBySessionId(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class InsertCreateEmailIpirApp : IpirApp, IRequest<IpirApp>
    {
        public IpirApp IpirApp { get; set; }
        public InsertCreateEmailIpirApp(IpirApp registrationRequestAssignmentOfJob)
        {
            this.IpirApp = registrationRequestAssignmentOfJob;
        }
    }
}
