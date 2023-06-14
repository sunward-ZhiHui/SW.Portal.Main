using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
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
    public class AssetEquipmentMaintenaceMasterQueryRepository : QueryRepository<View_AssetEquipmentMaintenaceMaster>, IAssetEquipmentMaintenaceMasterQueryRepository
    {
        public AssetEquipmentMaintenaceMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_AssetEquipmentMaintenaceMaster>> GetAllAsync(long? AssetPartsMaintenaceMasterId)
        {
            try
            {
                var query = "select  * from View_AssetEquipmentMaintenaceMaster WHERE AssetPartsMaintenaceMasterId=@AssetPartsMaintenaceMasterId";
                var parameters = new DynamicParameters();
                parameters.Add("AssetPartsMaintenaceMasterId", AssetPartsMaintenaceMasterId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_AssetEquipmentMaintenaceMaster>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_AssetEquipmentMaintenaceMaster> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_AssetEquipmentMaintenaceMaster WHERE AssetEquipmentMaintenaceMasterId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_AssetEquipmentMaintenaceMaster>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_AssetEquipmentMaintenaceMaster> DeleteAssetEquipmentMaintenaceMasterAssetDocument(long? Id)
        {
            View_AssetEquipmentMaintenaceMaster viewEmployee = new View_AssetEquipmentMaintenaceMaster();
            try
            {
                var query = "Delete from AssetEquipmentMaintenaceMasterAssetDocument where AssetEquipmentMaintenaceMasterID=@Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);
                using (var connection = CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters);
                    viewEmployee.AssetEquipmentMaintenaceMasterId = Id.Value;
                    return viewEmployee;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
