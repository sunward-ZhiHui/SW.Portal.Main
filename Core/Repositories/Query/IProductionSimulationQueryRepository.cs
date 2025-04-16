using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IProductionSimulationQueryRepository 
    {
        Task<IReadOnlyList<ProductionSimulation>> GetProductionSimulationListAsync(long? CompanyId,bool? IsReresh,long? UserId);
        Task<long> Update(ProductionSimulation company);
        Task<long> Delete(long id);
    }
    
    
}
