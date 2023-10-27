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
    public class GetAllRoutineLineList : PagedRequest, IRequest<List<ProductionActivityRoutineAppLine>>
    {
        

    }
    public class GetRoutineLineAppQuery : PagedRequest, IRequest<List<ProductionActivityRoutineAppLine>>
    {
        public long? companyID { get; set; }
        public GetRoutineLineAppQuery(long? companyid)
        {
            this.companyID = companyid;
        }

    }
}
