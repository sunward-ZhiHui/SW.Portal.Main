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
    public class SalesOrderMasterPricingQueryRepository : QueryRepository<View_SalesOrderMasterPricing>, ISalesOrderMasterPricingQueryRepository
    {
        public SalesOrderMasterPricingQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_SalesOrderMasterPricing>> GetAllByMasterTypeAsync(string MasterType)
        {
            try
            {
                var query = "select  * from View_SalesOrderMasterPricing where MasterType=@MasterType";
                var parameters = new DynamicParameters();
                parameters.Add("MasterType", MasterType);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_SalesOrderMasterPricing>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SalesOrderMasterPricing> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM View_SalesOrderMasterPricing WHERE SalesOrderMasterPricingId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SalesOrderMasterPricing>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SalesOrderMasterPricing> GetBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM View_SalesOrderMasterPricing WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SalesOrderMasterPricing>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
