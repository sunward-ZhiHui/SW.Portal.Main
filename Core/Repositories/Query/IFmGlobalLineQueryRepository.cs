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
        Task<ViewFmglobalLine> GetByIdAsync(Int64 id);
        Task<ViewFmglobalLine> GetByPalletNoAsync(string PalletNoYear,long? CompanyId);

        Task<ViewFmglobalLine> GetBySessionIdAsync(Guid? SessionId);
    }
}
