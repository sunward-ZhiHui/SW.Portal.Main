using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllProductionSimulationHandler : IRequestHandler<GetProductionSimulationQuery, List<ProductionSimulation>>
    {

        private readonly IProductionSimulationQueryRepository _queryRepository;
        public GetAllProductionSimulationHandler(IProductionSimulationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ProductionSimulation>> Handle(GetProductionSimulationQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionSimulation>)await _queryRepository.GetProductionSimulationListAsync(request.CompanyId,request.IsReresh,request.UserId);
        }
    }
    public class EditProductionSimulationHandler : IRequestHandler<EditProductionSimulation, long>
    {
        private readonly IProductionSimulationQueryRepository _conversationQueryRepository;

        public EditProductionSimulationHandler(IProductionSimulationQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditProductionSimulation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteProductionSimulationHandler : IRequestHandler<DeleteProductionSimulation, long>
    {
        private readonly IProductionSimulationQueryRepository _conversationQueryRepository;

        public DeleteProductionSimulationHandler(IProductionSimulationQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(DeleteProductionSimulation request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.Delete(request.Id);
            return req;
        }
    }

}
