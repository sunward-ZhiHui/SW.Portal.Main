using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ILayOutPlanTypeQueryRepository : IQueryRepository<ViewLayOutPlanType>
    {
        Task<IReadOnlyList<ViewLayOutPlanType>> GetAllAsync();
        Task<ViewLayOutPlanType> GetByIdAsync(Int64 id);

    }
}
