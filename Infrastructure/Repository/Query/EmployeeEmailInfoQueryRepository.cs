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
    public class EmployeeEmailInfoQueryRepository : QueryRepository<View_EmployeeEmailInfo>, IEmployeeEmailInfoQueryRepository
    {
        public EmployeeEmailInfoQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_EmployeeEmailInfo>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from View_EmployeeEmailInfo where employeeId=" + id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_EmployeeEmailInfo>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_EmployeeEmailInfo> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_EmployeeEmailInfo WHERE EmployeeEmailInfoID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_EmployeeEmailInfo>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
