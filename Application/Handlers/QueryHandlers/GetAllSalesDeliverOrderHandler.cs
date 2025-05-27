using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllSalesDeliverOrderHandler : IRequestHandler<GetAllSalesDeliverOrderQuery, List<NavpostedShipment>>
    {
        private readonly ISalesDeliverOrderQueryRepository _queryRepository;
        public GetAllSalesDeliverOrderHandler(ISalesDeliverOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavpostedShipment>> Handle(GetAllSalesDeliverOrderQuery request, CancellationToken cancellationToken)
        {
            return (List<NavpostedShipment>)await _queryRepository.GetAllByAsync(request.NavpostedShipment);
        }
    }
    public class InsertOrUpdateTenderOrdersHandler : IRequestHandler<InsertOrUpdateSalesDeliverOrder, NavpostedShipment>
    {
        private readonly ISalesDeliverOrderQueryRepository _queryRepository;
        public InsertOrUpdateTenderOrdersHandler(ISalesDeliverOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavpostedShipment> Handle(InsertOrUpdateSalesDeliverOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateSalesDeliverOrder(request);

        }
    }
    public class GetSyncSalesDeliverOrderQueryHandlers : IRequestHandler<GetSyncSalesDeliverOrderQuery, NavpostedShipment>
    {
        private readonly ISalesDeliverOrderQueryRepository _queryRepository;
        public GetSyncSalesDeliverOrderQueryHandlers(ISalesDeliverOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavpostedShipment> Handle(GetSyncSalesDeliverOrderQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetSevrviceNavpostedShipment(request.NavpostedShipment);

        }
    }
}
