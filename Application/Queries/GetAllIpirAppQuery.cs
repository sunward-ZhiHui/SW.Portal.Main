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
    public class GetAllIpirAppOneQuery : PagedRequest, IRequest<IpirApp>
    {
        public string? SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetAllIpirAppOneQuery(Guid? sessionId)
        {
            this.SessionId = sessionId;
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

}
