using Core.Entities;
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

namespace Infrastructure.Repository.Query
{
    public class RoleQueryRepository : QueryRepository<ApplicationRole>, IRoleQueryRepository
    {
        public RoleQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ApplicationRole>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationRole>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationRole> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole WHERE roleId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationRole>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationRole> GetCustomerByEmail(string name)
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole WHERE RoleName = @RoleName";
                var parameters = new DynamicParameters();
                parameters.Add("RoleName", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationRole>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
