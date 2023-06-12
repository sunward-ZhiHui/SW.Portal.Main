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
    public class AssetCatalogMasterQueryRepository : QueryRepository<View_AssetCatalogMaster>, IAssetCatalogMasterQueryRepository
    {
        public AssetCatalogMasterQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_AssetCatalogMaster>> GetAllAsync(long? CategoryId, long? SubSectionId)
        {
            try
            {
                var query = "select  * from view_AssetCatalogMaster WHERE AssetCategoryId=@AssetCategoryId AND AssetSubSectionId = @AssetSubSectionId";
                var parameters = new DynamicParameters();
                parameters.Add("AssetCategoryId", CategoryId, DbType.Int64);
                parameters.Add("AssetSubSectionId", SubSectionId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_AssetCatalogMaster>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_AssetCatalogMaster> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_AssetCatalogMaster WHERE AssetCatalogMasterId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_AssetCatalogMaster>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
