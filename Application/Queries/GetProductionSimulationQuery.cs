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
    public class GetProductionSimulationQuery : PagedRequest, IRequest<List<ProductionSimulation>>
    {
        public string? SearchString { get; set; }
        public long? CompanyId { get; set; }
        public bool? IsReresh {  get; set; }
        public long? UserId { get; set; }
        public GetProductionSimulationQuery(long? companyId, bool? isReresh, long? userId)
        {
            CompanyId = companyId;
            IsReresh = isReresh;
            UserId = userId;
        }
    }
    public class EditProductionSimulation : ProductionSimulation, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteProductionSimulation : ProductionSimulation, IRequest<long>
    {
        public long Id { get; private set; }

        public DeleteProductionSimulation(long Id)
        {
            this.Id = Id;
        }
    }

}
