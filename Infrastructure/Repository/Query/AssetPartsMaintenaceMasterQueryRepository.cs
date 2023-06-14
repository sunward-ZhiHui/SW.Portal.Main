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

namespace Infrastructure.Repository.Query
{
    public class AssetPartsMaintenaceMasterQueryRepository : QueryRepository<View_AssetPartsMaintenaceMaster>, IAssetPartsMaintenaceMasterQueryRepository
    {
        public AssetPartsMaintenaceMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_AssetPartsMaintenaceMaster>> GetAllAsync(long? AssetCatalogMasterId)
        {
            try
            {
                var query = "select  * from View_AssetPartsMaintenaceMaster WHERE AssetCatalogMasterId=@AssetCatalogMasterId";
                var parameters = new DynamicParameters();
                parameters.Add("AssetCatalogMasterId", AssetCatalogMasterId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_AssetPartsMaintenaceMaster>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_AssetPartsMaintenaceMaster> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_AssetPartsMaintenaceMaster WHERE AssetPartsMaintenaceMasterId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_AssetPartsMaintenaceMaster>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
