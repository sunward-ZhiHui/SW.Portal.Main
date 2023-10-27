
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
   
     public class GetAllProductionActivityAppLineQuery : PagedRequest, IRequest<List<ProductionActivityAppLine>>
    {
       
    }
    public class CreateProductionActivityAppLineCommand : ProductionActivityAppLine, IRequest<long>
    {
    }
}
