using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IPostSalesOrderQueryRepository : IQueryRepository<PostSalesOrder>
    {
        Task<IReadOnlyList<PostSalesOrder>> GetAllAsync();
        Task<PostSalesOrder> GetByIdAsync(Int64 id);
        Task<string> InsertRawMatItemList(IEnumerable<RawMatItemList> rawMatItemList);
        Task<SotckBalanceItemsList> GetSotckBalanceItemsListAsync(StockBalanceSearch searchModel);
        Task<string> UpdateStockBalanceData(string query);
        Task<NavitemStockBalance> InsertOrUpdateNavitemStockBalance(List<NavitemStockBalance> navitemStockBalance);
        Task<DistStockBalanceKiv> InsertOrUpdateDistStockBalanceKiv(List<DistStockBalanceKiv> distStockBalanceKivs);
    }

}
