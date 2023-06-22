using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ITopicTodoListQueryRepository : IQueryRepository<TopicToDoList>
    {        
        Task<IReadOnlyList<TopicToDoList>> GetAllAsync(long Uid);
        Task<long> Insert(TopicToDoList todolist);
        Task<long> Update(TopicToDoList todolist);
        Task<long> Delete(long id);
    }
}
