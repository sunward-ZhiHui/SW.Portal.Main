
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
    public interface IEmployeeEmailInfoForwardQueryRepository : IQueryRepository<EmployeeEmailInfoForward>
    {
        Task<IReadOnlyList<EmployeeEmailInfoForward>> GetAllAsync(long? id);
        Task<EmployeeEmailInfoForward> GetByIdAsync(Int64 id);
    }
}
