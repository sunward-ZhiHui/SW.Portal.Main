
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
    public interface IEmployeeEmailInfoAuthorityQueryRepository : IQueryRepository<EmployeeEmailInfoAuthority>
    {
        Task<IReadOnlyList<EmployeeEmailInfoAuthority>> GetAllAsync(long? id);
        Task<EmployeeEmailInfoAuthority> GetByIdAsync(Int64 id);
    }
}
