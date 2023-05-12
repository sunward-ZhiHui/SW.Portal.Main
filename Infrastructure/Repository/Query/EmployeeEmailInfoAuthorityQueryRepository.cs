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
    public class EmployeeEmailInfoAuthorityQueryRepository : QueryRepository<EmployeeEmailInfoAuthority>, IEmployeeEmailInfoAuthorityQueryRepository
    {
        public EmployeeEmailInfoAuthorityQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<EmployeeEmailInfoAuthority>> GetAllAsync(long? id)
        {
            try
            {
                var query = "select  * from EmployeeEmailInfoAuthority where EmployeeEmailInfoID=" + id;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmployeeEmailInfoAuthority>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<EmployeeEmailInfoAuthority> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM EmployeeEmailInfoAuthority WHERE EmployeeEmailInfoAuthorityID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<EmployeeEmailInfoAuthority>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
