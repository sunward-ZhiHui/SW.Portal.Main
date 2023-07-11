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
    public interface ISoSalesOrderQueryRepository : IQueryRepository<View_SoSalesOrder>
    {
        Task<IReadOnlyList<View_SoSalesOrder>> GetAllAsync();
        Task<View_SoSalesOrder> GetByIdAsync(Int64 id);
    }
}
