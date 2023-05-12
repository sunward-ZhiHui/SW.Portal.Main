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
    public class EmployeeICTHardInformationQueryRepository : QueryRepository<View_EmployeeICTHardInformation>, IEmployeeICTHardInformationQueryRepository
    {
        public EmployeeICTHardInformationQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_EmployeeICTHardInformation>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from View_EmployeeICTHardInformation where employeeId=" + id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_EmployeeICTHardInformation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_EmployeeICTHardInformation> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_EmployeeICTHardInformation WHERE EmployeeIctHardinformationId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_EmployeeICTHardInformation>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
