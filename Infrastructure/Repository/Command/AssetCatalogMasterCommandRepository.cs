using Core.Entities;
using Core.Repositories.Command;
using Infrastructure.Data;
using Infrastructure.Repository.Command.Base;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository.Command
{
    public class AssetCatalogMasterCommandRepository : CommandRepository<AssetCatalogMaster>, IAssetCatalogMasterCommandRepository
    {
        public AssetCatalogMasterCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class AssetPartsMaintenaceMasterCommandRepository : CommandRepository<AssetPartsMaintenaceMaster>, IAssetPartsMaintenaceMasterCommandRepository
    {
        public AssetPartsMaintenaceMasterCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class AssetEquipmentMaintenaceMasterCommandRepository : CommandRepository<AssetEquipmentMaintenaceMaster>, IAssetEquipmentMaintenaceMasterCommandRepository
    {
        public AssetEquipmentMaintenaceMasterCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
    public class AssetEquipmentMaintenaceMasterAssetDocumentCommandRepository : CommandRepository<AssetEquipmentMaintenaceMasterAssetDocument>, IAssetEquipmentMaintenaceMasterAssetDocumentCommandRepository
    {
        public AssetEquipmentMaintenaceMasterAssetDocumentCommandRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
