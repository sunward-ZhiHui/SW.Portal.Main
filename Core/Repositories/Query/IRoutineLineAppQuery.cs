using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IRoutineLineAppQuery : IQueryRepository<ProductionActivityRoutineAppLine>
    {
        Task<IReadOnlyList<ProductionActivityRoutineAppLine>> GetAllAsync(long? CompanyId);
        Task<IReadOnlyList<ProductionActivityRoutineAppLine>> GetAllAsyncPO(long? CompanyId);
    }
}
