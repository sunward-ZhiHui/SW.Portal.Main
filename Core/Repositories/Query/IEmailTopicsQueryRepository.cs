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
        Task<List<EmailTopics>> GetTopicCCList(Int64 UserId);
        Task<List<EmailTopics>> GetTopicDraftList(Int64 UserId);
        Task<List<EmailTopics>> GetSubTopicToList(Int64 TopicId,long UserId);
        Task<List<EmailTopics>> GetSubTopicCCList(Int64 TopicId, long UserId);
        Task<List<EmailTopics>> GetSubTopicSentList(Int64 TopicId, long UserId);
        
        Task<List<EmailTopics>> GetTopicSentList(Int64 UserId);
        Task<List<EmailParticipant>> GetParticipantList(Int64 topicId);        
        Task<EmailTopics> GetCustomerByEmail(string email);
        long Insert(EmailTopics EmailTopics);
        long EmailTopicUpdate(EmailTopics EmailTopics);
        long Insert_sp_Participant(TopicParticipant topicParticipant);
        Task<long> InsertParticipant(TopicParticipant topicParticipant);
        Task<EmailTopics> GetTopicListAsync();      
        Task<long> UpdateDueDate(EmailTopics EmailTopics);
        Task<long> UpdateTopicClose(EmailTopics EmailTopics);
        Task<List<Documents>> GetCreateEmailDocumentListAsync(Guid sessionId);
    }
}
