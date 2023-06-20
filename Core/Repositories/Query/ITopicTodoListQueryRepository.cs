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
        //Custom operation which is not generic
        Task<IReadOnlyList<TopicToDoList>> GetAllAsync();
        Task<long> Insert(TopicToDoList todolist);
        Task<long> Update(TopicToDoList todolist);
        Task<long> Delete(TopicToDoList todolist);
    }
}
