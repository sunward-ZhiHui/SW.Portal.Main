using Core.Entities.Views;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.EntityModel;
using Core.Entities.Views;
using Core.Entities;

namespace Infrastructure.Repository.Query
{
    public class AssetEquipmentMaintenaceMasterAssetDocumentQueryRepository : QueryRepository<AssetEquipmentMaintenaceMasterAssetDocument>, IAssetEquipmentMaintenaceMasterAssetDocumentQueryRepository
    {
        public AssetEquipmentMaintenaceMasterAssetDocumentQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<AssetEquipmentMaintenaceMasterAssetDocument>> GetAllByIdAsync(long? Id)
        {
            try
            {
                var query = "select  * from AssetEquipmentMaintenaceMasterAssetDocument where  AssetDocumentId is not null and AssetEquipmentMaintenaceMasterId=" + Id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AssetEquipmentMaintenaceMasterAssetDocument>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}




