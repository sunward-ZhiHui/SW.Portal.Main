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
    public interface IFmGlobalLineQueryRepository : IQueryRepository<ViewFmglobalLine>
    {
        Task<IReadOnlyList<ViewFmglobalLine>> GetAllAsync(Int64 id);
        Task<IReadOnlyList<ViewFmglobalLine>>  GetAllbyIdsAsync(Int64 id);
        Task<ViewFmglobalLine> GetByIdAsync(Int64 id);
        Task<ViewFmglobalLine> GetByPalletNoAsync(string PalletNoYear, long? CompanyId);
        Task<IReadOnlyList<ViewFmglobalLine>> GetFmGlobalLineByPalletEntryNoAsync(long? CompanyId);
        Task<ViewFmglobalLine> GetBySessionIdAsync(Guid? SessionId);
        Task<FmglobalLine> GetFMGlobalLineFromIDAsync(long? FmglobalLineId);
        Task<IReadOnlyList<ViewFmglobalLine>> GetAllByLocationFromAsync(Int64 id);
        Task<long?> UpdatePreviosMoveQty(long? Id,long? LocationID,long? ModifiedByUserId);
        Task<FmglobalMove> GetFMGlobalMoveExits(long? LocationID, long? LocationToID, long? FmglobalId, int? IsHandQty, int? TransactionQty);
        Task<FmglobalMove> GetFMGlobalMoveCheckExits(long? LocationID, long? LocationToID, long? FmglobalId,long? FmglobalLineId, int? IsHandQty, int? TransactionQty);
    }
}
