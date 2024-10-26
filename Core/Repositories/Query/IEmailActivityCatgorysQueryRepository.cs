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
    public interface IEmailActivityCatgorysQueryRepository 
    {
        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllAsync();        
        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllTopicCategoryAsync(long TopicId);
        Task<List<long>> GetActionTagMultipleAsync(long TopicId);
        Task<bool> GetTagLockInfoAsync(long TopicId);
        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllemailCategoryAsync(long TopicId);
        Task<long> Insert(EmailActivityCatgorys emailActivityCatgorys);
        Task<long> UpdateAsync(EmailActivityCatgorys emailActivityCatgorys);
        Task<long> DeleteAsync(long id,long TopicId);
        Task<string> UpdateOtherAsync(string othertag,string Name,long ModifiedByUserID);
        Task<string> UpdateOtherTagAsync(long id, string Name, long ModifiedByUserID);
        Task<string> UpdateuserAsync(string userTag, string Name);
        Task<long> DeleteUserTagAsync(long ID);
        Task<List<EmailActivityCatgorys>> GetAllUserTagAsync(long UserID);
        Task<List<EmailActivityCatgorys>> GetByUserTagAsync(long TopicID,long UserID);
        Task<List<EmailActivityCatgorys>> GetAllOthersAsync(string Others);

    }
}
