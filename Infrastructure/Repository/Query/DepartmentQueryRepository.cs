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

namespace Infrastructure.Repository.Query
{
    public class DepartmentQueryRepository : QueryRepository<ViewDepartment>, IDepartmentQueryRepository
    {
        public DepartmentQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewDepartment>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_Department";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewDepartment>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewDepartment> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM view_Department WHERE DepartmentId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewDepartment>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewDepartment>> GetDepartmentByDivisionAsync(long? DivisionId)
        {
            try
            {
                var query = "select  * from view_Department  WHERE DivisionId = @DivisionId";
                var parameters = new DynamicParameters();
                parameters.Add("DivisionId", DivisionId, DbType.Int64);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewDepartment>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

    }
}
