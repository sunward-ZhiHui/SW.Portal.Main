using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISubSectionQueryRepository : IQueryRepository<ViewSubSection>
    {
        Task<IReadOnlyList<ViewSubSection>> GetAllAsync();
        Task<ViewSubSection> GetByIdAsync(Int64 id);
    }
}
