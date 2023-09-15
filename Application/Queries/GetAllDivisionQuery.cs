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
    public class GetAllDivisionQuery : PagedRequest, IRequest<List<ViewDivision>>
    {
        public string SearchString { get; set; }
    }
    public class GetDivisionByCompany : PagedRequest, IRequest<List<ViewDivision>>
    {
        public long? CompanyId { get; set; }
        public GetDivisionByCompany(long? companyId)
        {
            this.CompanyId= companyId;
        }
    }
    

}
