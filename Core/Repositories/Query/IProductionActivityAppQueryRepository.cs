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
        Task<ProductionActivityApp> GetAllOneLocationAsync(string? locationName);
        Task<long> Insert(ProductActivityAppModel PPAlist);
        Task<IReadOnlyList<NavprodOrderLineModel>> GetAllAsyncPO(long? CompanyId);
        Task<IReadOnlyList<NavprodOrderLineModel>> GetAllNavprodOrderLineAsync(long? CompanyId, string? Replanrefno);
        Task<IReadOnlyList<ProductActivityCaseLineModel>> GetProductActivityCaseLineTemplateItemsAsync(long? ManufacturingProcessId, long? CategoryActionId);
    }
   
}
