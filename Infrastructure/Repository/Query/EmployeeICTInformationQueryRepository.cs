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
    public class EmployeeICTInformationQueryRepository : QueryRepository<View_EmployeeICTInformation>, IEmployeeICTInformationQueryRepository
    {
        public EmployeeICTInformationQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_EmployeeICTInformation>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from View_EmployeeICTInformation where employeeId=" + id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_EmployeeICTInformation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_EmployeeICTInformation> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_EmployeeICTInformation WHERE EmployeeIctinformationId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_EmployeeICTInformation>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
