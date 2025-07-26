using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISPCDataTrendingQueryRepository
    {
        Task<IReadOnlyList<SPCDataTrending>> GetAllAsync();
        Task<SPCDataTrending> InsertAsync(SPCDataTrending SPCDataTrending);
        Task<SPCDataTrending> UpdateAsync(SPCDataTrending dataItem, SPCDataTrending changedItem);
        Task<bool> DeleteAsync(long SbRegistrationID);
    }
}
