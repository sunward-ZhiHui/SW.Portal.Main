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
        Task<String> SendPushNotification(long Id);
        Task<long> LastUserIDUpdate(long ReplyId,long UserId);        
        Task<long> InsertAssignTo(EmailConversationAssignTo forumConversationAssignTo);
        Task<long> InsertAssignTo_sp(EmailConversationAssignTo emailConversationAssignTo);
        Task<long> InsertEmailNotifications(EmailNotifications forumNotifications);
        Task<long> Update(EmailConversations company);
        Task<long> Delete(EmailConversations company);
        Task<IReadOnlyList<ViewEmployee>> GetAllParticipantAsync(long topicId);
        Task<IReadOnlyList<ViewEmployee>> GetAddConversationPListAsync(long ConversationId);        
        Task<List<EmailConversations>> GetBySessionConversationList(string SessionId);
        long DeleteParticipant(TopicParticipant topicParticipant);
        Task<EmailConversations> GetByIdAsync(Int64 id);        
        Task<List<EmailConversations>> GetFullDiscussionListAsync(Int64 TopicId);
        Task<List<EmailConversations>> GetDiscussionListAsync(Int64 TopicId,Int64 UserId);
        Task<List<EmailConversations>> GetDemoEmailFileDataListAsync();
        Task<long> GetDemoUpdateEmailFileDataListAsync(long id, Byte[] fileData);
        Task<List<EmailConversations>> GetValidUserListAsync(Int64 TopicId, Int64 UserId);        
        Task<List<EmailConversations>> GetConversationListAsync(Int64 Id);
        Task<List<EmailConversations>> GetTopConversationListAsync(Int64 TopicId);
        Task<List<EmailConversations>> GetReplyDiscussionListAsync(Int64 TopicId,long UserId);
        Task<List<Documents>> GetTopicDocListAsync(long TopicId,long UserId,string option);
        Task<List<Documents>> GetSubTopicDocListAsync(long TopicId);
        Task<List<EmailAssignToList>> GetAllAssignToListAsync(long TopicId);
        Task<List<ViewEmployee>> GetAllPListAsync(long TopicId);        
        Task<List<ViewEmployee>> GetConvPListAsync(long ConversationId);
        Task<List<ViewEmployee>> GetAllConvTopicPListAsync(long ConversationId, long TopicId);
        Task<List<ViewEmployee>> GetAllConvTPListAsync(long TopicId);
        Task<List<ViewEmployee>> GetAllConvAssignToListAsync(long TopicId);
        Task<List<UserNotification>> GetUserTokenListAsync(long UserId);        
        Task<List<EmailTopicTo>> GetTopicToListAsync(long TopicId);
        Task<List<EmailConversationAssignTo>> GetConversationAssignToList(long ConversationId);
        Task<List<EmailConversations>> GetConversationTopicIdList(long TopicId);        
        Task<List<EmailConversationAssignTo>> GetConversationAssignCCList(long ConversationId);


	}
}

