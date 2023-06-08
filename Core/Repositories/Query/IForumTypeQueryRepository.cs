using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IForumTypeQueryRepository : IQueryRepository<ForumTypes>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ForumTypes>> GetAllAsync();
        Task<ForumTypes> GetByIdAsync(Int64 id);
        Task<ForumTypes> GetCustomerByEmail(string email);
        Task<IReadOnlyList<ForumTypes>> GetAllTypeUserAsync();
    }
}
