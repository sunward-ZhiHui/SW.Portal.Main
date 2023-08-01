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
    public interface IFmGlobalQueryRepository : IQueryRepository<ViewFmglobal>
    {
        Task<IReadOnlyList<ViewFmglobal>> GetAllAsync();
        Task<ViewFmglobal> GetByIdAsync(Int64 id);
        Task<ViewFmglobal> GetBySessionIdAsync(Guid? SessionId);
        Task<IReadOnlyList<View_FMGlobalAddess>> GetFmGlobalAddressdAsync(long? id, string billingType);
        Task<long> InsertFmAddress(View_FMGlobalAddess address);
        Task<long> InsertFmGlobalAddress(FMGlobalAddess fmGlobalAddress);
        Task<long> EditAddress(View_FMGlobalAddess address);
        Task<long> DeleteAddressAsync(long? AddressID, long? FMGlobalID);
    }
}
