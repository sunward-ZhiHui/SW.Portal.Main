using Application.Queries;
using Core.Entities;
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
    public class GetAllSoCustomerHandler : IRequestHandler<GetAllSoCustomer, List<SoCustomer>>
    {
        private readonly ISoCustomerQueryRepository _queryRepository;
        public GetAllSoCustomerHandler(ISoCustomerQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SoCustomer>> Handle(GetAllSoCustomer request, CancellationToken cancellationToken)
        {
            return (List<SoCustomer>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetAllSoCustomerByIDHandler : IRequestHandler<GetAllSoCustomerByID,SoCustomer>
    {
        private readonly ISoCustomerQueryRepository _queryRepository;
        public GetAllSoCustomerByIDHandler(ISoCustomerQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<SoCustomer> Handle(GetAllSoCustomerByID request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetByIdAsync(request.Id.Value);
        }
    }
}
