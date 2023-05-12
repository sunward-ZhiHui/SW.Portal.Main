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
    public class EmployeeOtherDutyInformationQueryRepository : QueryRepository<View_EmployeeOtherDutyInformation>, IEmployeeOtherDutyInformationQueryRepository
    {
        public EmployeeOtherDutyInformationQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<View_EmployeeOtherDutyInformation>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from View_EmployeeOtherDutyInformation where employeeId=" + id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_EmployeeOtherDutyInformation>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_EmployeeOtherDutyInformation> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM View_EmployeeOtherDutyInformation WHERE EmployeeOtherDutyInformationId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_EmployeeOtherDutyInformation>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
