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
    public class EmployeeQueryRepository : QueryRepository<ViewEmployee>, IEmployeeQueryRepository
    {
        public EmployeeQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<ViewEmployee>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_GetEmployee";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query)).Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewEmployee>> GetAllUserAsync()
        {
            try
            {
                var query = "select  * from View_Employee";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query)).Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewEmployee> GetAllUserByIDAsync(long UserID)
        {
            try
            {
                var query = "select  * from View_Employee where UserID=@UserID";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                using (var connection = CreateConnection())
                {
                    return  (await connection.QueryFirstOrDefaultAsync<ViewEmployee>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ViewEmployee> ResetEmployeePasswordAsync(ViewEmployee viewEmployee)
        {
            try
            {
                var query = "update ApplicationUser set LoginPassword=@NewPassword where UserID=@UserID";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", viewEmployee.UserID);
                var password = EncryptDecryptPassword.Encrypt("1234");
                parameters.Add("NewPassword", password);

                using (var connection = CreateConnection())
                {
                    var user = await connection.ExecuteAsync(query, parameters);
                    return await GetAllUserByIDAsync(viewEmployee.UserID.Value);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        

    }
}
