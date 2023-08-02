using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllSOMPricingLineSellingMethodHandler : IRequestHandler<GetAllSOMPricingLineSellingMethodQuery, List<SalesOrderMasterPricingLineSellingMethod>>
    {

        private readonly ISalesOrderMasterPricingLineSellingMethodQueryRepository _salesOrderMasterPricingLineSellingMethodQueryRepository;
        public GetAllSOMPricingLineSellingMethodHandler(ISalesOrderMasterPricingLineSellingMethodQueryRepository salesOrderMasterPricingLineSellingMethodQueryRepository)
        {
            _salesOrderMasterPricingLineSellingMethodQueryRepository = salesOrderMasterPricingLineSellingMethodQueryRepository;
        }
        public async Task<List<SalesOrderMasterPricingLineSellingMethod>> Handle(GetAllSOMPricingLineSellingMethodQuery request, CancellationToken cancellationToken)
        {
            return (List<SalesOrderMasterPricingLineSellingMethod>)await _salesOrderMasterPricingLineSellingMethodQueryRepository.GetAllSalesOrderMasterPricingLineAsync(request.SalesOrderMasterPricingLineId,request.SellingMethodName);
        }
    }

}
