using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISoSalesOrderLineQueryRepository : IQueryRepository<View_SoSalesOrderLine>
    {
        Task<IReadOnlyList<View_SoSalesOrderLine>> GetAllAsync();
        Task<View_SoSalesOrderLine> GetByIdAsync(Int64 id);
    }
   
}
