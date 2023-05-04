using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ILevelMasterQueryRepository : IQueryRepository<ViewLevel>
    {
        Task<IReadOnlyList<ViewLevel>> GetAllAsync();
        Task<ViewLevel> GetByIdAsync(Int64 id);
    }
}
