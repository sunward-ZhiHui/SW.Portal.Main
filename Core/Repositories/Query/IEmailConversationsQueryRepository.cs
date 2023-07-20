using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query.Base;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IEmailConversationsQueryRepository : IQueryRepository<EmailConversations>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<EmailConversations>> GetAllAsync();
        Task<long> Insert(EmailConversations company);
        Task<long> InsertAssignTo(EmailConversationAssignTo forumConversationAssignTo);
        Task<long> InsertAssignTo_sp(EmailConversationAssignTo emailConversationAssignTo);
        Task<long> InsertEmailNotifications(EmailNotifications forumNotifications);
        Task<long> Update(EmailConversations company);
        Task<long> Delete(EmailConversations company);
        Task<IReadOnlyList<ViewEmployee>> GetAllParticipantAsync(long topicId);
        long DeleteParticipant(TopicParticipant topicParticipant);
        Task<EmailConversations> GetByIdAsync(Int64 id);        
        Task<List<EmailConversations>> GetFullDiscussionListAsync(Int64 TopicId);
        Task<List<EmailConversations>> GetDiscussionListAsync(Int64 TopicId,Int64 UserId);
        Task<List<EmailConversations>> GetConversationListAsync(Int64 Id);        
        Task<List<EmailConversations>> GetReplyDiscussionListAsync(Int64 TopicId,long UserId);
        Task<List<Documents>> GetTopicDocListAsync(long TopicId);
        Task<List<EmailAssignToList>> GetAllAssignToListAsync(long TopicId);
        Task<List<ViewEmployee>> GetAllConvAssignToListAsync(long TopicId);
        Task<List<EmailTopicTo>> GetTopicToListAsync(long TopicId);
        Task<List<EmailConversationAssignTo>> GetConversationAssignToList(long ConversationId);
		Task<List<EmailConversationAssignTo>> GetConversationAssignCCList(long ConversationId);


	}
}

