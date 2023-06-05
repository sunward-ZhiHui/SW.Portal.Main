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
    public interface IForumConversationsQueryRepository : IQueryRepository<ForumConversations>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<ForumConversations>> GetAllAsync();
        Task<long> Insert(ForumConversations company);
        Task<long> InsertAssignTo(ForumConversationAssignTo forumConversationAssignTo);
        Task<long> Update(ForumConversations company);
        Task<long> Delete(ForumConversations company);
        Task<IReadOnlyList<ViewEmployee>> GetAllParticipantAsync(long topicId);
        Task<long> DeleteParticipant(TopicParticipant topicParticipant);
        Task<ForumConversations> GetByIdAsync(Int64 id);
        Task<List<ForumConversations>> GetDiscussionListAsync(Int64 TopicId);
        Task<List<Documents>> GetTopicDocListAsync(long TopicId);
        Task<List<ForumAssignToList>> GetAllAssignToListAsync(long TopicId);
        Task<List<ForumTopicTo>> GetTopicToListAsync(long TopicId);
        Task<List<ForumConversationAssignTo>> GetConversationAssignToList(long ConversationId);


    }
}

