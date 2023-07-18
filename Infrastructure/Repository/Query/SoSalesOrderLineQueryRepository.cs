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
    public class SoSalesOrderLineQueryRepository : QueryRepository<View_SoSalesOrderLine>, ISoSalesOrderLineQueryRepository
    {
        public SoSalesOrderLineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_SoSalesOrderLine>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from view_SoSalesOrderLine WHERE SoSalesOrderId = @SoSalesOrderId";
                var parameters = new DynamicParameters();
                parameters.Add("SoSalesOrderId", id, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_SoSalesOrderLine>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SoSalesOrderLine> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_SoSalesOrderLine  WHERE SoSalesOrderLineId = @SoSalesOrderLineId";
                var parameters = new DynamicParameters();
                parameters.Add("SoSalesOrderLineId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SoSalesOrderLine>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}

