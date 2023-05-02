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
    public class DesignationQueryRepository : QueryRepository<ViewDesignation>, IDesignationQueryRepository
    {
        public DesignationQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewDesignation>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_Designation";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewDesignation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewDesignation> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_Designation WHERE designationId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewDesignation>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}




