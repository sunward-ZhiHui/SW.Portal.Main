using Application.Queries;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetDistStockBalanceQueryHandler : IRequestHandler<DistStockBalanceQuery, List<NavStockBalanceModel>>
    {

        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public GetDistStockBalanceQueryHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<List<NavStockBalanceModel>> Handle(DistStockBalanceQuery request, CancellationToken cancellationToken)
        {
            return (List<NavStockBalanceModel>)await _distStockBalanceQueryRepository.GetAllDistStockBalanceAsync(request.StockBalanceSearch);
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
    public class UploadStockBalanceQueryHandler : IRequestHandler<UploadStockBalanceQuery, StockBalanceSearch>
    {

        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public UploadStockBalanceQueryHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<StockBalanceSearch> Handle(UploadStockBalanceQuery request, CancellationToken cancellationToken)
        {
            return (StockBalanceSearch)await _distStockBalanceQueryRepository.UploadStockBalance(request.StockBalanceSearch);
        }


    }
    public class GetNavItemStockBalanceByIdHandler : IRequestHandler<GetNavItemStockBalanceById, List<NavItemStockBalanceModel>>
    {

        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public GetNavItemStockBalanceByIdHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<List<NavItemStockBalanceModel>> Handle(GetNavItemStockBalanceById request, CancellationToken cancellationToken)
        {
            return (List<NavItemStockBalanceModel>)await _distStockBalanceQueryRepository.GetNavItemStockBalanceById(request.Id);
        }


    }

    public class UpdateNavItemStockBalanceHandler : IRequestHandler<UpdateNavItemStockBalance, NavItemStockBalanceModel>
    {

        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public UpdateNavItemStockBalanceHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<NavItemStockBalanceModel> Handle(UpdateNavItemStockBalance request, CancellationToken cancellationToken)
        {
            return (NavItemStockBalanceModel)await _distStockBalanceQueryRepository.UpdateNavItemStockBalance(request);
        }


    }
    public class GetNoACItemsListHandler : IRequestHandler<GetNoACItemsList, List<Acitems>>
    {
        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public GetNoACItemsListHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<List<Acitems>> Handle(GetNoACItemsList request, CancellationToken cancellationToken)
        {
            return (List<Acitems>)await _distStockBalanceQueryRepository.GetNoACItemsList();
        }


    }
    public class UpdateNoACItemsHandler : IRequestHandler<UpdateNoACItems, Acitems>
    {

        private readonly IDistStockBalanceQueryRepository _distStockBalanceQueryRepository;
        public UpdateNoACItemsHandler(IDistStockBalanceQueryRepository distStockBalanceQueryRepository)
        {
            _distStockBalanceQueryRepository = distStockBalanceQueryRepository;
        }
        public async Task<Acitems> Handle(UpdateNoACItems request, CancellationToken cancellationToken)
        {
            return (Acitems)await _distStockBalanceQueryRepository.UpdateNoACItems(request.Acitems);
        }


    }
}
