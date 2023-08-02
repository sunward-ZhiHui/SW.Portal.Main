using Core.Entities;
using Core.Entities.Views;
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
    }
}
