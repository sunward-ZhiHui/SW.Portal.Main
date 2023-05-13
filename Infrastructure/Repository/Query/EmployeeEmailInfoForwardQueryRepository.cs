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
    public class EmployeeEmailInfoForwardQueryRepository : QueryRepository<EmployeeEmailInfoForward>, IEmployeeEmailInfoForwardQueryRepository
    {
        public EmployeeEmailInfoForwardQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<EmployeeEmailInfoForward>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from EmployeeEmailInfoForward where EmployeeEmailInfoID=" + id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmployeeEmailInfoForward>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<EmployeeEmailInfoForward> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM EmployeeEmailInfoForward WHERE EmployeeEmailInfoForwardID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<EmployeeEmailInfoForward>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
