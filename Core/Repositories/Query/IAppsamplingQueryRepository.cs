using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IAppsamplingQueryRepository:IQueryRepository<AppSampling>
    {
        Task<IReadOnlyList<AppSampling>> GetAllAsync();
        Task<IReadOnlyList<AppSamplingLine>> GetAllLineAsync();
        //  Task<IReadOnlyList<AppSampling>> GetsamplingByStatus(int id);
    }
}
