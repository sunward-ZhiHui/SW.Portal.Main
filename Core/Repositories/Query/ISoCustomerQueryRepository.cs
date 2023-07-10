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
    public interface ISoCustomerQueryRepository : IQueryRepository<SoCustomer>
    {
        Task<IReadOnlyList<SoCustomer>> GetAllAsync();
        Task<SoCustomer> GetByIdAsync(Int64 id);
        
    }
    
}
