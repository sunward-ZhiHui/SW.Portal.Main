using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IForumTopicsQueryRepository : IQueryRepository<ForumTopics>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ForumTopics>> GetAllAsync();
        Task<ForumTopics> GetByIdAsync(Int64 id);
        Task<ForumTopics> GetCustomerByEmail(string email);
        long Insert(ForumTopics forumTopics);
        Task<ForumTopics> GetTopicListAsync();
        Task<IReadOnlyList<ForumCategorys>> GetCategoryByTypeId(long typeId);
    }
}
