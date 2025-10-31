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
using Application.Common;
using Core.Entities;

namespace Infrastructure.Repository.Query
{
    public class ApplicationUserRoleQueryRepository : QueryRepository<ApplicationUserRole>, IApplicationUserRoleQueryRepository
    {
        public ApplicationUserRoleQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<ApplicationUserRole> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT t1.*,t2.RoleName FROM ApplicationUserRole t1 LEFT JOIN ApplicationRole t2 ON t1.RoleID=t2.RoleID\r\n WHERE t1.UserId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUserRole>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
