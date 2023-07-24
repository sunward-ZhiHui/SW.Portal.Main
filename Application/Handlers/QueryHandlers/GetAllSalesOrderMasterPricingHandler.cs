using Application.Queries;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MediatR;

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
    public class GetAllSalesOrderMasterPricingLineByItemByMethodHandler : IRequestHandler<GetAllSalesOrderMasterPricingLineByItemByMethodQuery, List<View_SalesOrderMasterPricingLineByItem>>
    {

        private readonly ISalesOrderMasterPricingQueryRepository _salesOrderMasterPricingLineQueryRepository;
        public GetAllSalesOrderMasterPricingLineByItemByMethodHandler(ISalesOrderMasterPricingQueryRepository salesOrderMasterPricingLineQueryRepository)
        {
            _salesOrderMasterPricingLineQueryRepository = salesOrderMasterPricingLineQueryRepository;
        }
        public async Task<List<View_SalesOrderMasterPricingLineByItem>> Handle(GetAllSalesOrderMasterPricingLineByItemByMethodQuery request, CancellationToken cancellationToken)
        {
            return (List<View_SalesOrderMasterPricingLineByItem>)await _salesOrderMasterPricingLineQueryRepository.GetSalesOrderLineByItemAsync(request.CompanyId, request.FromDate, request.SellingMethodId, request.ItemId);
        }
    }
    public class GetAllSalesOrderMasterPricingLineByItemByQtyHandler : IRequestHandler<GetAllSalesOrderMasterPricingLineByItemByQtyQuery, SalesOrderMasterPricingFromSalesModel>
    {

        private readonly ISalesOrderMasterPricingQueryRepository _salesOrderMasterPricingLineQueryRepository;
        public GetAllSalesOrderMasterPricingLineByItemByQtyHandler(ISalesOrderMasterPricingQueryRepository salesOrderMasterPricingLineQueryRepository)
        {
            _salesOrderMasterPricingLineQueryRepository = salesOrderMasterPricingLineQueryRepository;
        }
        public async Task<SalesOrderMasterPricingFromSalesModel> Handle(GetAllSalesOrderMasterPricingLineByItemByQtyQuery request, CancellationToken cancellationToken)
        {
            return await _salesOrderMasterPricingLineQueryRepository.GetPricingTypeForSellingMethod(request.CompanyId, request.FromDate, request.SellingMethodId, request.ItemId, request.Qty);
        }
    }
}
