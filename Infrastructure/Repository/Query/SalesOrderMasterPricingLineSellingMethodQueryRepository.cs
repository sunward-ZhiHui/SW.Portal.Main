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
    public class SalesOrderMasterPricingLineSellingMethodQueryRepository : QueryRepository<SalesOrderMasterPricingLineSellingMethod>, ISalesOrderMasterPricingLineSellingMethodQueryRepository
    {
        public SalesOrderMasterPricingLineSellingMethodQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<SalesOrderMasterPricingLineSellingMethod> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM SalesOrderMasterPricingLineSellingMethod WHERE SalesOrderMasterPricingLineSellingMethodId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<SalesOrderMasterPricingLineSellingMethod>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<SalesOrderMasterPricingLineSellingMethod>> GetAllSalesOrderMasterPricingLineAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM SalesOrderMasterPricingLineSellingMethod WHERE SalesOrderMasterPricingLineId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<SalesOrderMasterPricingLineSellingMethod>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
