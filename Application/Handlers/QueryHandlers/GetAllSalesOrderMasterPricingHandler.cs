using Application.Queries;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllSalesOrderMasterPricingMasterTypeHandler : IRequestHandler<GetAllSalesOrderMasterPricingMasterTypeQuery, List<View_SalesOrderMasterPricing>>
    {
        private readonly ISalesOrderMasterPricingQueryRepository _queryRepository;
        public GetAllSalesOrderMasterPricingMasterTypeHandler(ISalesOrderMasterPricingQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_SalesOrderMasterPricing>> Handle(GetAllSalesOrderMasterPricingMasterTypeQuery request, CancellationToken cancellationToken)
        {
            return (List<View_SalesOrderMasterPricing>)await _queryRepository.GetAllByMasterTypeAsync(request.MasterType);
        }
    }
    public class GetAllSalesOrderMasterPricingBySessionHandler : IRequestHandler<GetAllSalesOrderMasterPricingBySessionQuery, View_SalesOrderMasterPricing>
    {
        private readonly ISalesOrderMasterPricingQueryRepository _queryRepository;
        public GetAllSalesOrderMasterPricingBySessionHandler(ISalesOrderMasterPricingQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<View_SalesOrderMasterPricing> Handle(GetAllSalesOrderMasterPricingBySessionQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetBySessionIdAsync(request.SessionId);
        }
    }
}
