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
    public interface IProductionEntryQueryRepository : IQueryRepository<ViewProductionEntry>
    {
        Task<IReadOnlyList<ViewProductionEntry>> GetAllAsync();
        //Task<IReadOnlyList<AppSamplingLine>> GetAllLineAsync();
        //  Task<IReadOnlyList<AppSampling>> GetsamplingByStatus(int id);
    }
   
}
