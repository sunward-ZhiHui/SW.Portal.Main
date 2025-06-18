using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class DistStockBalanceQuery : PagedRequest, IRequest<List<NavStockBalanceModel>>
    {
        public StockBalanceSearch StockBalanceSearch { get; set; }

        public DistStockBalanceQuery(StockBalanceSearch distStockBalance)
        {
            this.StockBalanceSearch = distStockBalance;
        }
    }
    public class NavItemStockBalanceQuery : PagedRequest, IRequest<List<NavitemStockBalance>>
    {
        public NavitemStockBalance? navitemStockBalance { get; set; }

        public NavItemStockBalanceQuery(NavitemStockBalance navitemStockBalance)
        {
            this.navitemStockBalance = navitemStockBalance;
        }
    }
    public class UploadStockBalanceQuery : PagedRequest, IRequest<StockBalanceSearch>
    {
        public StockBalanceSearch StockBalanceSearch { get; set; }

        public UploadStockBalanceQuery(StockBalanceSearch stockBalanceSearch)
        {
            this.StockBalanceSearch = stockBalanceSearch;
        }
    }
    public class GetNavItemStockBalanceById : PagedRequest, IRequest<List<NavItemStockBalanceModel>>
    {
        public long? Id { get; set; }

        public GetNavItemStockBalanceById(long? id)
        {
            this.Id = id;
        }
    }
    public class UpdateNavItemStockBalance : NavItemStockBalanceModel, IRequest<NavItemStockBalanceModel>
    {
    }
    public class DeleteNavItemStockBalance : PagedRequest, IRequest<NavItemStockBalanceModel>
    {
        public NavItemStockBalanceModel NavItemStockBalanceModel { get; set; }

        public DeleteNavItemStockBalance(NavItemStockBalanceModel navItemStockBalanceModel)
        {
            this.NavItemStockBalanceModel = navItemStockBalanceModel;
        }
    }
    public class GetNoACItemsList : PagedRequest, IRequest<List<Acitems>>
    {

    }
    public class UpdateNoACItems : PagedRequest, IRequest<Acitems>
    {
        public Acitems Acitems { get; set; }

        public UpdateNoACItems(Acitems acitems)
        {
            this.Acitems = acitems;
        }
    }
    public class GetNavDistStockBalanceById : PagedRequest, IRequest<List<DistStockBalanceModel>>
    {
        public long? Id { get; set; }

        public GetNavDistStockBalanceById(long? id)
        {
            this.Id = id;
        }
    }
    public class UpdateDistStockBalance : DistStockBalanceModel, IRequest<DistStockBalanceModel>
    {
    }
    public class DeleteDistStockBalance : PagedRequest, IRequest<DistStockBalanceModel>
    {
        public DistStockBalanceModel DistStockBalanceModel { get; set; }

        public DeleteDistStockBalance(DistStockBalanceModel navItemStockBalanceModel)
        {
            this.DistStockBalanceModel = navItemStockBalanceModel;
        }
    }
}
