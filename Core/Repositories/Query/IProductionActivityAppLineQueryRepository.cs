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
    public interface IProductionActivityQueryRepository : IQueryRepository<ProductionActivityAppLine>
    {
        Task<IReadOnlyList<ProductActivityAppModel>> GetAllAsync(long? CompanyID, string? prodorderNo, long? userId, long? locationID);
        Task<long> Insert(ProductionActivityAppLine PPALinelist);        
        Task<IReadOnlyList<ProductionActivityApp>>GetAlllocAsync(long? location);

    }
}
