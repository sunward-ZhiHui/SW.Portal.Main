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
   public interface IIctmasterQueryRepository : IQueryRepository<ViewIctmaster>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<Ictmaster>> GetIctmasterByMasterType(int mastertype);
        Task<IReadOnlyList<ViewIctmaster>> GetAllAsync();
        Task<ViewIctmaster> GetByIdAsync(Int64 id);
    }
    
}
