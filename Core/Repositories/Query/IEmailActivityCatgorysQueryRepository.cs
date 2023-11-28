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
    public interface IEmailActivityCatgorysQueryRepository : IQueryRepository<EmailActivityCatgorys>
    {
        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllAsync();        
        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllTopicCategoryAsync(long TopicId);
        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllemailCategoryAsync(long TopicId);
        Task<long> Insert(EmailActivityCatgorys emailActivityCatgorys);
        Task<long> UpdateAsync(EmailActivityCatgorys emailActivityCatgorys);
        Task<long> DeleteAsync(long id);

        Task<IReadOnlyList<EmailActivityCatgorys>> GetAllUserTagAsync(long UserID);
        Task<List<EmailActivityCatgorys>> GetByUserTagAsync(long TopicID,long UserID);
        
    }
}
