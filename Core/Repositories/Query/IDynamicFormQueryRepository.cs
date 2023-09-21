using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IDynamicFormQueryRepository : IQueryRepository<DynamicForm>
    {
        Task<IReadOnlyList<DynamicForm>> GetAllAsync();
       Task<long> Insert(DynamicForm dynamicForm);
        //Task<long> Update(TopicToDoList todolist);
        //Task<long> Delete(long id);
    }
   
}
