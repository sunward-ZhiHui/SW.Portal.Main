using Core.Entities.Views;
using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModels;

namespace Core.Repositories.Query
{
    public interface IDistStockBalanceQueryRepository 
    {
        Task<IReadOnlyList<NavStockBalanceModel>> GetAllDistStockBalanceAsync(StockBalanceSearch value);
        Task<IReadOnlyList<NavitemStockBalance>> GetAllNavItemStockBalanceAsync(NavitemStockBalance value);
        Task<StockBalanceSearch> UploadStockBalance(StockBalanceSearch TenderOrderModel);
        Task<IReadOnlyList<NavItemStockBalanceModel>> GetNavItemStockBalanceById(long? id);
        Task<NavItemStockBalanceModel> UpdateNavItemStockBalance(NavItemStockBalanceModel navItemStockBalanceModel);
        Task<NavItemStockBalanceModel> DeleteNavItemStockBalance(NavItemStockBalanceModel navItemStockBalanceModel);
        Task<IReadOnlyList<Acitems>> GetNoACItemsList();
        Task<Acitems> UpdateNoACItems(Acitems acitems);
    }
}
