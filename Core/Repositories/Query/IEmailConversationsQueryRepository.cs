using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IEmailConversationsQueryRepository
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<EmailConversations>> GetAllAsync();
        Task<long> Insert(EmailConversations company);
        Task<long> InsertCopyEmail(EmailTopics copyemail);
        Task<long> DeleteCopyEmail(long ConversationID);
        Task<String> SendPushNotification(long Id);
        Task<long> LastUserIDUpdate(long ReplyId,long UserId);
        Task<long> UpdateDueDateReqested(long ReplyId, long UserId,int value);
        Task<long> LastUpdateDateEmailTopic(long TopicId);
        Task<long> InsertEmailDynamicFormDateUploadSession(long DynamicFormID, Guid FormDataSessionID, Guid FormSectionSessionID, Guid EmailSessionID);
        Task<long> DocInsertDynamicFormDateUpload(Guid id, Guid DynamicFormSectionID, Guid sessionId,long userid);        
        Task<long> InsertAssignTo(EmailConversationAssignTo forumConversationAssignTo);
        Task<long> InsertAssignTo_sp(EmailConversationAssignTo emailConversationAssignTo);
        Task<long> InsertAssignToUserGroup_sp(EmailConversationAssignToUserGroup AssignToUserGroup);
        Task<List<long>> GetGroupByUserIdList(string GroupIds,long TopicId);
        Task<long> InsertEmailNotifications(EmailNotifications forumNotifications);
        Task<long> Update(EmailConversations company);
        Task<long> Delete(EmailConversations company);
        Task<IReadOnlyList<ViewEmployee>> GetAllParticipantAsync(long topicId);
        Task<IReadOnlyList<ViewEmployee>> GetAddConversationPListAsync(long ConversationId);        
        Task<List<EmailConversations>> GetBySessionConversationList(string SessionId);
        long DeleteParticipant(TopicParticipant topicParticipant);
        Task<EmailConversations> GetByIdAsync(Int64 id);        
        Task<List<EmailConversations>> GetFullDiscussionListAsync(Int64 TopicId);
        Task<List<EmailConversations>> GetDiscussionListAsync(Int64 TopicId,Int64 UserId,string Option);
        Task<List<EmailConversations>> GetEmailPrintAllDiscussionListAsync(Int64 TopicId, Int64 UserId, string Option);
        Task<long> GetTotalNotificationCountAsync(long UserId);
        Task<List<EmailConversations>> GetUnReadNotificationAsync(long UserId);
        Task<List<EmailConversations>> GetByReplyDiscussionList(long replyId);
        Task<List<EmailConversations>> GetReplyListPagedAsync(long replyId, long UserId,int currentPage,int pageSize);
        Task<List<EmailConversations>> GetOnDiscussionListAsync(long replyId,long UserId);
        Task<List<EmailConversations>> GetDemoEmailFileDataListAsync();
        Task<long> GetDemoUpdateEmailFileDataListAsync(long id, Byte[] fileData);
        Task<List<EmailConversations>> GetValidUserListAsync(Int64 TopicId, Int64 UserId);        
        Task<List<EmailConversations>> GetConversationListAsync(Int64 Id);
        Task<List<EmailConversations>> GetTopConversationListAsync(Int64 TopicId);
        Task<List<EmailConversations>> GetReplyDiscussionListAsync(Int64 TopicId,long UserId);
        Task<List<EmailConversations>> GetPrintReplyDiscussionListAsync(Int64 TopicId, long UserId);
        Task<List<Documents>> GetTopicDocListAsync(long TopicId,long UserId,string option);
        Task<List<Documents>> GetSubTopicDocListAsync(long TopicId);
        Task<List<EmailAssignToList>> GetAllAssignToListAsync(long TopicId);
        Task<List<ViewEmployee>> GetAllPListAsync(long TopicId);        
        Task<List<ViewEmployee>> GetConvPListAsync(long ConversationId);
        Task<List<long>> GetConvPListUserGroupAsync(long ConversationId);        
        Task<List<ViewEmployee>> GetAllConvTopicPListAsync(long ConversationId, long TopicId);
        Task<List<ViewEmployee>> GetAllConvTPListAsync(long TopicId);
        Task<List<ViewEmployee>> GetAllConvAssignToListAsync(long TopicId);
        Task<List<UserNotification>> GetUserTokenListAsync(long UserId);        
        Task<List<EmailTopicTo>> GetTopicToListAsync(long TopicId);
        Task<byte[]> GetFileDataAsync(long conversationId);
        Task<List<EmailConversationAssignTo>> GetConversationAssignToList(long ConversationId);
        Task<List<EmailConversations>> GetConversationTopicIdList(long TopicId);        
        Task<List<EmailConversationAssignTo>> GetConversationAssignCCList(long ConversationId);
        Task<List<EmailConversationAssignToUserGroup>> GetAssignCCUserGroupList(long ConversationId);
        Task<List<EmailTopics>> GetEmailParticipantListAsync(long conversationID ,long Userid);
        Task<List<EmailTopics>> UpdateEmailCloseAsync(long conversationID, long Userid,long Isclose);

    }
}

