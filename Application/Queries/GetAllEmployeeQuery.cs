using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllEmployeeQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllEmployeeListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public string SearchString { get; set; }
    }

    public class GetEmployeeResetPasswordQuery : ViewEmployee, IRequest<ViewEmployee>
    {
        public long LoginID { get; set; }
    }
    public class GetEmployeeEmailInfoForwardQuery : PagedRequest, IRequest<List<EmployeeEmailInfoForward>>
    {
        public long? Id { get; set; }
        public GetEmployeeEmailInfoForwardQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetEmployeeEmailInfoAuthorityQuery : PagedRequest, IRequest<List<EmployeeEmailInfoAuthority>>
    {
        public long? Id { get; set; }
        public GetEmployeeEmailInfoAuthorityQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetEmployeeICTInformationQuery : PagedRequest, IRequest<List<View_EmployeeICTInformation>>
    {
        public long? Id { get; set; }
        public GetEmployeeICTInformationQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetEmployeeICTHardInformationQuery : PagedRequest, IRequest<List<View_EmployeeICTHardInformation>>
    {
        public long? Id { get; set; }
        public GetEmployeeICTHardInformationQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllEmployeeByStatusQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public string SearchString { get; set; }
    }

    public class GetAllEmployeeBySessionQuery : PagedRequest, IRequest<ViewEmployee>
    {
        public Guid? SessionId { get; set; }
        public GetAllEmployeeBySessionQuery(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetEmployeeReportToQuery : PagedRequest, IRequest<List<EmployeeReportTo>>
    {
        public long? Id { get; set; }
        public GetEmployeeReportToQuery(long? Id)
        {
            this.Id = Id;
        }
    }

}
