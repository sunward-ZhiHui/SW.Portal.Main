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
    public interface ICreateEmailTopicEditQueryRepository : IQueryRepository<EmailTopics>
    {
        Task<IReadOnlyList<Documents>> GetAllAsync(Guid? sessionId);
        Task<long> Update(EmailTopics emailTopics);
    }
    
}
