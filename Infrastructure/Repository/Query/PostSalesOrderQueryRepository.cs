using Core.Entities;
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
    public class PostSalesOrderQueryRepository : QueryRepository<PostSalesOrder>, IPostSalesOrderQueryRepository
    {
        public PostSalesOrderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<PostSalesOrder>> GetAllAsync()
        {
            try
            {
                var query = "select  * from PostSalesOrder";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<PostSalesOrder>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<PostSalesOrder> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM PostSalesOrder  WHERE PostSalesOrderID = @PostSalesOrderId";
                var parameters = new DynamicParameters();
                parameters.Add("PostSalesOrderId", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<PostSalesOrder>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}

