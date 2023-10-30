using Core.Entities;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IRoutineQueryRepository : IQueryRepository<ProductionActivityRoutineAppLine>
    {
      
        Task<IReadOnlyList<ProductionActivityRoutineAppLine>> GetAllAsync();

        //Task<long> Insert(ProductionActivityRoutineAppLine dynamicForm);
        //Task<long> Update(ProductionActivityRoutineAppLine dynamicForm);
       
    }
}
