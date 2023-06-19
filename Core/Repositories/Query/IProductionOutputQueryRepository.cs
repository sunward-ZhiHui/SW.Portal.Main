using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IProductionOutputQueryRepository : IQueryRepository<ViewProductionOutput>
    {
        Task<IReadOnlyList<ViewProductionOutput>> GetAllAsync();
    }
    
}
