using Application.Queries.Base;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllDepartmentQuery : PagedRequest, IRequest<List<ViewDepartment>>
    {
        public string SearchString { get; set; }
    }
    public class GetDepartmentByDivision : PagedRequest, IRequest<List<ViewDepartment>>
    {
        public long? DivisionId { get; set; }
        public GetDepartmentByDivision(long? divisionId)
        {
            this.DivisionId = divisionId;
        }
    }
    public class GetDepartmentByCompany : PagedRequest, IRequest<List<ViewDepartment>>
    {
        public long? CompanyId { get; set; }
        public GetDepartmentByCompany(long? companyId)
        {
            this.CompanyId = companyId;
        }
    }
}
