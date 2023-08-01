using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IFmGlobalLineItemQueryRepository : IQueryRepository<ViewFmglobalLineItem>
    {
        Task<IReadOnlyList<ViewFmglobalLineItem>> GetAllAsync(Int64 id);
        Task<IReadOnlyList<ViewFmglobalLineItem>> GetAllByIdAsync(Int64 id);
        Task<ViewFmglobalLineItem> GetByIdAsync(Int64 id);
        Task<IReadOnlyList<ViewFmglobalLineItem>> GetPalletMovementListingdAsync();

        Task<ViewFmglobalLineItem> GetBySessionIdAsync(Guid? SessionId);
    }
}
