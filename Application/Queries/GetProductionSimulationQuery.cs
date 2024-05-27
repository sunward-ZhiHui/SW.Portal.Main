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
        public string SearchString { get; set; }
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
