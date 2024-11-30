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
        
        private readonly ISoCustomerQueryRepository _queryRepository;
        public GetSoCustomerHandler(ISoCustomerQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SoCustomer>> Handle(GetSoCustomerQuery request, CancellationToken cancellationToken)
        {           
            return (List<SoCustomer>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetSoCustomerByTypeHandler : IRequestHandler<GetSoCustomerByTypeQuery, List<SoCustomer>>
    {

        private readonly ISoCustomerQueryRepository _queryRepository;
        public GetSoCustomerByTypeHandler(ISoCustomerQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SoCustomer>> Handle(GetSoCustomerByTypeQuery request, CancellationToken cancellationToken)
        {
            return (List<SoCustomer>)await _queryRepository.GetListByTypeAsync(request.Type);
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
            return (List<View_SoSalesOrderLine>)await _soSalesOrderLineQueryRepository.GetAllAsync(request.Id);            
        }
    }

   
}
