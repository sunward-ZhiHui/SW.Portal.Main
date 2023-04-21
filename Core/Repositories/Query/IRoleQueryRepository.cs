using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IRoleQueryRepository : IQueryRepository<ApplicationRole>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ApplicationRole>> GetAllAsync();
        Task<ApplicationRole> GetByIdAsync(Int64 id);
        Task<ApplicationRole> GetCustomerByEmail(string email);
    }
}
