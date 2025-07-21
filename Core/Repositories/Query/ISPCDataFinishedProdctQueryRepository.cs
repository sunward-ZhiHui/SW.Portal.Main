using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface ISPCDataFinishedProdctQueryRepository
    {
        Task<IReadOnlyList<SPCDataFinishedProdct>> GetAllAsync();
        Task<SPCDataFinishedProdct> InsertAsync(SPCDataFinishedProdct SPCDataFinishedProdct);
        Task<SPCDataFinishedProdct> UpdateAsync(SPCDataFinishedProdct dataItem, SPCDataFinishedProdct changedItem);
        Task<bool> DeleteAsync(long SbRegistrationID);
    }
}
