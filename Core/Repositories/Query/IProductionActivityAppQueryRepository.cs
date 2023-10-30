using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IProductionActivityAppQueryRepository : IQueryRepository<ProductionActivityApp>
    {
        Task<IReadOnlyList<ProductionActivityApp>> GetAllAsync(long? CompanyId);
        Task<IReadOnlyList<ProductionActivityApp>> GetAllListAsync();
        
        Task<long> Insert(ProductionActivityModel PPAlist);
        Task<IReadOnlyList<NavprodOrderLine>> GetAllAsyncPO(long? CompanyId);
    }
   
}
