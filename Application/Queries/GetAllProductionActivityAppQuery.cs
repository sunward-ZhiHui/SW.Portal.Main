using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllProductionActivityAppQuery : PagedRequest, IRequest<List<ProductionActivityApp>>
    {
        public long? companyID { get; set; }
        public GetAllProductionActivityAppQuery(long?  companyid)
        {
           this.companyID=companyid;
        }
    }
    public class GetAllProductionActivityAppQueryList : PagedRequest, IRequest<List<ProductionActivityApp>>
    {

    }
    public class CreateProductionActivityAppCommand : ProductionActivityApp, IRequest<long>
    {
    }
    public class GetAllProductionActivityPONumberAppQuery : PagedRequest, IRequest<List<ProductionActivityApp>>
    {
        public long? companyID { get; set; }
        public GetAllProductionActivityPONumberAppQuery(long? companyid)
        {
            this.companyID = companyid;
        }
    }

}
