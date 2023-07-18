using Application.Queries;
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
    public class GetAllSalesOrderMasterPricingLineHandler : IRequestHandler<GetAllSalesOrderMasterPricingLineQuery, List<View_SalesOrderMasterPricingLine>>
    {

        private readonly ISalesOrderMasterPricingLineQueryRepository _salesOrderMasterPricingLineQueryRepository;
        public GetAllSalesOrderMasterPricingLineHandler(ISalesOrderMasterPricingLineQueryRepository salesOrderMasterPricingLineQueryRepository)
        {
            _salesOrderMasterPricingLineQueryRepository = salesOrderMasterPricingLineQueryRepository;
        }
        public async Task<List<View_SalesOrderMasterPricingLine>> Handle(GetAllSalesOrderMasterPricingLineQuery request, CancellationToken cancellationToken)
        {
            return (List<View_SalesOrderMasterPricingLine>)await _salesOrderMasterPricingLineQueryRepository.GetAllAsync(request.SalesOrderMasterPricingId);
        }
    }

}
