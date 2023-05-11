
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IEmployeeEmailInfoQueryRepository : IQueryRepository<View_EmployeeEmailInfo>
    {
        Task<IReadOnlyList<View_EmployeeEmailInfo>> GetAllAsync(long? id);
        Task<View_EmployeeEmailInfo> GetByIdAsync(Int64 id);
    }
}
