using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISectionQueryRepository : IQueryRepository<ViewSection>
    {
        Task<IReadOnlyList<ViewSection>> GetAllAsync();
        Task<ViewSection> GetByIdAsync(Int64 id);
    }
}
