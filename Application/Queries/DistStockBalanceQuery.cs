using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public  class DistStockBalanceQuery : PagedRequest, IRequest<List<DistStockBalance>>
    {
        public DistStockBalance? distStockBalance { get; set; }

        public DistStockBalanceQuery(DistStockBalance distStockBalance)
        {
            this.distStockBalance = distStockBalance;
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
    
}
