using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDepartmentQueryRepository : IQueryRepository<ViewDepartment>
    {
        Task<IReadOnlyList<ViewDepartment>> GetAllAsync();
        Task<ViewDepartment> GetByIdAsync(Int64 id);
        Task<IReadOnlyList<ViewDepartment>> GetDepartmentByDivisionAsync(long? DivisionId);
        Task<IReadOnlyList<ViewDepartment>> GetDepartmentByCompanyAsync(long? companyId);
    }
}
