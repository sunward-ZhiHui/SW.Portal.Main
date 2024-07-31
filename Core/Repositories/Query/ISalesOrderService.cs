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
    public interface ISalesOrderService : IQueryRepository<PostSalesOrder>
    {
        Task PostSalesOrderAsync(PostSalesOrder postSalesOrder);
        Task<List<Core.Entities.ItemBatchInfo>> SyncBatchAsync(string company, string itemNo);
        Task<List<Core.Entities.ItemBatchInfo>> NavItemBatchAsync(string company);
        Task<string> RawMatItemAsync(string companyname, long companyid, string type);
        Task<string> PackagingItemAsync(string companyname, long companyid, string type);
        Task<string> ProcessItemAsync(string companyname, long companyid, string type);
        Task<List<Navitems>> GetNavItemsAdd(ViewPlants company);
        Task<string> GetNAVStockBalance(StockBalanceSearch StockBalanceSearch);
    }
}
