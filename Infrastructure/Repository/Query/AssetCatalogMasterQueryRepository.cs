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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                string query = "SELECT * FROM view_AssetCatalogMaster WHERE AssetCategoryId = @AssetCategoryId AND (@AssetSubSectionId IS NULL OR AssetSubSectionId = @AssetSubSectionId)";

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
        public async Task<string?> GenerateAssetCatalogNo()
        {
            var AssetCatalogMaster = new AssetCatalogMaster();
            try
            {
                var query = "SELECT * FROM AssetCatalogMaster where AssetCatalogNo <>'' order by AssetCatalogNo desc";
                using (var connection = CreateConnection())
                {
                    AssetCatalogMaster = await connection.QueryFirstOrDefaultAsync<AssetCatalogMaster>(query);
                }
                if (AssetCatalogMaster == null)
                {
                    var num = 1;
                    return num.ToString("D6"); ;
                }
                else
                {
                    var num = Convert.ToInt64(AssetCatalogMaster.AssetCatalogNo) + 1;
                    return num.ToString("D6");
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
