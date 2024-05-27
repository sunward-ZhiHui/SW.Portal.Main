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
    public class GetDistStockBalanceQueryHandler : IRequestHandler<DistStockBalanceQuery, List<DistStockBalance>>
    {
     
            private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
            public GetDistStockBalanceQueryHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
            {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
            }
            public async Task<List<DistStockBalance>> Handle(DistStockBalanceQuery request, CancellationToken cancellationToken)
            {
                return (List<DistStockBalance>)await _distStockBalanceQueryRepository.GetAllDistStockBalanceAsync(request.distStockBalance);
            }

        
    }
    public class GetNavStockBalanceQueryHandler : IRequestHandler<NavItemStockBalanceQuery, List<NavitemStockBalance>>
    {

        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public GetNavStockBalanceQueryHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<List<NavitemStockBalance>> Handle(NavItemStockBalanceQuery request, CancellationToken cancellationToken)
        {
            return (List<NavitemStockBalance>)await _distStockBalanceQueryRepository.GetAllNavItemStockBalanceAsync(request.navitemStockBalance);
        }


    }
   
}
