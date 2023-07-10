using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class SoSalesOrderLineQueryRepository : QueryRepository<ViewSoSalesOrderLine>, ISoSalesOrderLineQueryRepository
    {
        public SoSalesOrderLineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewSoSalesOrderLine>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_SoSalesOrderLine";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewSoSalesOrderLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewSoSalesOrderLine> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_SoSalesOrderLine  WHERE SoSalesOrderLineId = @SoSalesOrderLineId";
                var parameters = new DynamicParameters();
                parameters.Add("SoSalesOrderLineId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewSoSalesOrderLine>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}

