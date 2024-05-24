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
  
}
