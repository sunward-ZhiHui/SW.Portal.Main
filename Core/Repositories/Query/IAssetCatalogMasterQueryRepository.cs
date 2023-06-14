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
    public interface IAssetCatalogMasterQueryRepository : IQueryRepository<View_AssetCatalogMaster>
    {
        Task<IReadOnlyList<View_AssetCatalogMaster>> GetAllAsync(long? CategoryId,long? SubSectionId);
        Task<View_AssetCatalogMaster> GetByIdAsync(Int64 id);
    }
    public interface IAssetPartsMaintenaceMasterQueryRepository : IQueryRepository<View_AssetPartsMaintenaceMaster>
    {
        Task<IReadOnlyList<View_AssetPartsMaintenaceMaster>> GetAllAsync(long? AssetCatalogMasterId);
        Task<View_AssetPartsMaintenaceMaster> GetByIdAsync(Int64 id);
    }
    public interface IAssetEquipmentMaintenaceMasterQueryRepository : IQueryRepository<View_AssetEquipmentMaintenaceMaster>
    {
        Task<IReadOnlyList<View_AssetEquipmentMaintenaceMaster>> GetAllAsync(long? AssetPartsMaintenaceMasterId);
        Task<View_AssetEquipmentMaintenaceMaster> GetByIdAsync(Int64 id);
        Task<View_AssetEquipmentMaintenaceMaster> DeleteAssetEquipmentMaintenaceMasterAssetDocument(long? id);
    }
    public interface IAssetEquipmentMaintenaceMasterAssetDocumentQueryRepository : IQueryRepository<AssetEquipmentMaintenaceMasterAssetDocument>
    {
        Task<IReadOnlyList<AssetEquipmentMaintenaceMasterAssetDocument>> GetAllByIdAsync(long? id);
    }
    
}
