using Core.Entities.Views;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IAssetCatalogMasterQueryRepository : IQueryRepository<View_AssetCatalogMaster>
    {
        Task<IReadOnlyList<View_AssetCatalogMaster>> GetAllAsync(long? CategoryId,long? SubSectionId);
        Task<View_AssetCatalogMaster> GetByIdAsync(Int64 id);
    }
}
