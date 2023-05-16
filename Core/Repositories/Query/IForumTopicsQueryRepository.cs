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
        Task<List<ForumTopics>> GetByIdAsync(Int64 id);
        Task<List<ForumTopics>> GetUserTopicList(Int64 UserId);
        Task<List<ForumTopics>> GetTreeTopicList(Int64 UserId);
        Task<List<TopicParticipant>> GetParticipantList(Int64 topicId);        
        Task<ForumTopics> GetCustomerByEmail(string email);
        long Insert(ForumTopics forumTopics);
        Task<long> InsertParticipant(TopicParticipant topicParticipant);
        Task<ForumTopics> GetTopicListAsync();
        Task<IReadOnlyList<ForumCategorys>> GetCategoryByTypeId(Int64 typeId);
    }
}
