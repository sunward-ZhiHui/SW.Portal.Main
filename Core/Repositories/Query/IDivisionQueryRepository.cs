
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDivisionQueryRepository : IQueryRepository<ViewDivision>
    {
        Task<IReadOnlyList<ViewDivision>> GetAllAsync();
        Task<ViewDivision> GetByIdAsync(Int64 id);
        Task<IReadOnlyList<ViewDivision>> GetDivisionByCompanyAsync(long? companyId);
    }
}
