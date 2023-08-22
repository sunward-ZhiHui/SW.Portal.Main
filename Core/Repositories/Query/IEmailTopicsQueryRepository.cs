using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IEmailTopicsQueryRepository : IQueryRepository<EmailTopics>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<EmailTopics>> GetAllAsync();
        Task<List<EmailTopics>> GetByIdAsync(Int64 id);             
        Task<List<EmailTopics>> GetUserTopicList(Int64 UserId);
        Task<List<EmailTopics>> GetBySessionTopicList(string SessionId);        
        Task<List<EmailTopics>> GetByIdTopicToList(Int64 UserId);
        Task<List<EmailTopics>> GetByIdTopicCCList(Int64 UserId);
        Task<List<EmailTopics>> GetTopicToList(Int64 UserId);
        Task<long> SetPinTopicToList(Int64 Id);
        Task<long> UnSetPinTopicToList(Int64 Id);
        Task<long> UpdateMarkasReadList(Int64 Id);
        Task<long> UpdateMarkasunReadList(Int64 Id);
        Task<List<EmailTopics>> GetTopicAllList(Int64 UserId);
        Task<List<EmailTopics>> GetTopicMasterSearchList(Int64 UserId,string MasterSearch);        
        Task<List<EmailTopics>> GetTopicCCList(Int64 UserId);
        Task<List<EmailTopics>> GetTopicDraftList(Int64 UserId);
        long DeleteTopicDraftList(Int64 TopicId);
        Task<List<EmailTopics>> GetSubTopicToList(Int64 TopicId,long UserId);
        Task<List<EmailTopics>> GetSubTopicSearchAllList(Int64 TopicId, long UserId,string SearchTxt);        
        Task<List<EmailTopics>> GetSubTopicCCList(Int64 TopicId, long UserId);
        Task<List<EmailTopics>> GetSubTopicSentList(Int64 TopicId, long UserId);
        Task<List<EmailTopics>> GetSubTopicAllList(Int64 TopicId, long UserId);

        Task<List<EmailTopics>> GetTopicSentList(Int64 UserId);
        Task<List<EmailParticipant>> GetParticipantList(Int64 topicId,long UserId);
        Task<List<EmailParticipant>> GetConversationPList(Int64 ConversationId);

        Task<EmailTopics> GetCustomerByEmail(string email);
        long Insert(EmailTopics EmailTopics);
        long EmailTopicUpdate(EmailTopics EmailTopics);
        long Insert_sp_Participant(TopicParticipant topicParticipant);
        Task<long> InsertParticipant(TopicParticipant topicParticipant);
        Task<EmailTopics> GetTopicListAsync();      
        Task<long> UpdateDueDate(EmailTopics EmailTopics);
        Task<long> UpdateTopicClose(EmailTopics EmailTopics);
        Task<long> UpdateTopicArchive(EmailTopics emailTopics);
        Task<List<ActivityEmailTopics>> GetByActivityEmailSessionList(Guid sessionId);
        Task<List<Documents>> GetCreateEmailDocumentListAsync(Guid sessionId);
       
        Task<long> Delete(long id);
    }
}
