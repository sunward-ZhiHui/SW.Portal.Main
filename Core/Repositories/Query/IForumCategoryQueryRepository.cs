using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public  interface IForumCategoryQueryRepository : IQueryRepository<ForumCategorys>
    {
        //Custom operation which is not generic
       // Task<IReadOnlyList<ForumCategorys>> GetAllAsync();
        Task<ForumCategorys> GetByIdAsync(Int64 id);
       //Task<ForumCategorys> GetCustomerByEmail(string email);
    }
   
}
