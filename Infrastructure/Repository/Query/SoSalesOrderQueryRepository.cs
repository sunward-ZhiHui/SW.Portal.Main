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
    public class SoSalesOrderQueryRepository : QueryRepository<ViewSoSalesOrder>, ISoSalesOrderQueryRepository
    {
        public SoSalesOrderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewSoSalesOrder>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_SoSalesOrder";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewSoSalesOrder>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewSoSalesOrder> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_SoSalesOrder  WHERE SoSalesOrderId = @SoSalesOrderId";
                var parameters = new DynamicParameters();
                parameters.Add("SoSalesOrderId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewSoSalesOrder>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}


