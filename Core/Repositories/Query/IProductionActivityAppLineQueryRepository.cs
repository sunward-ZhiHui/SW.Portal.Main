using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IProductionActivityQueryRepository : IQueryRepository<ProductionActivityAppLine>
    {
        Task<IReadOnlyList<ProductionActivityAppLine>> GetAllAsync(long? companyid, string? prododerno, long? locationid);
        Task<long> Insert(ProductionActivityAppLine PPALinelist);
        //Task<IReadOnlyList<ProductionActivityApp>> GetAllprodAsync(string? NAVProdOrder);
        
        Task<IReadOnlyList<ProductionActivityApp>>GetAlllocAsync(long? location);

    }
}
