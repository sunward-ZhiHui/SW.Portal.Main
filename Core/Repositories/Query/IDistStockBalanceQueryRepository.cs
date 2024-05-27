using Core.Entities.Views;
using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDistStockBalanceQueryRepository 
    {
        Task<IReadOnlyList<DistStockBalance>> GetAllDistStockBalanceAsync(DistStockBalance value);
        Task<IReadOnlyList<NavitemStockBalance>> GetAllNavItemStockBalanceAsync(NavitemStockBalance value);
      
    }
}
