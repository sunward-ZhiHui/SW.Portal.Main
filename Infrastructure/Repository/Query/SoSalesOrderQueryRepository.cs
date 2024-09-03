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
    public class SoSalesOrderQueryRepository : QueryRepository<View_SoSalesOrder>, ISoSalesOrderQueryRepository
    {
        public SoSalesOrderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_SoSalesOrder>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_SoSalesOrder Order By ModifiedDate Desc ";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_SoSalesOrder>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SoSalesOrder> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM SoSalesOrder  WHERE SoSalesOrderId = @SoSalesOrderId";
                var parameters = new DynamicParameters();
                parameters.Add("SoSalesOrderId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SoSalesOrder>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_SoSalesOrder> GetAllBySessionAsync(Guid? SessionId)
        {
            try
            {
                var query = "SELECT * FROM view_SoSalesOrder  WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_SoSalesOrder>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}


