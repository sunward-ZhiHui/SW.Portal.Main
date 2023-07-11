using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetSoCustomerHandler : IRequestHandler<GetSoCustomerQuery, List<SoCustomer>>
    {
        
        private readonly IQueryRepository<SoCustomer> _queryRepository;
        public GetSoCustomerHandler(IQueryRepository<SoCustomer> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SoCustomer>> Handle(GetSoCustomerQuery request, CancellationToken cancellationToken)
        {           
            return (List<SoCustomer>)await _queryRepository.GetListAsync();
        }
    }

    public class GetSalesOrderLineHandler : IRequestHandler<GetSalesOrderLine, List<View_SoSalesOrderLine>>
    {

        private readonly ISoSalesOrderLineQueryRepository _soSalesOrderLineQueryRepository;
        public GetSalesOrderLineHandler(ISoSalesOrderLineQueryRepository soSalesOrderLineQueryRepository)
        {
            _soSalesOrderLineQueryRepository = soSalesOrderLineQueryRepository;
        }
        public async Task<List<View_SoSalesOrderLine>> Handle(GetSalesOrderLine request, CancellationToken cancellationToken)
        {
            return (List<View_SoSalesOrderLine>)await _soSalesOrderLineQueryRepository.GetAllAsync();            
        }
    }
}
