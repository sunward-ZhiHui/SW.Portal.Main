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
        public async Task<IReadOnlyList<ViewEmployee>> GetAllByStatusAsync()
        {
            try
            {
                var query = "select  * from view_GetEmployeeByName where Status!='Resign' or Status is null";

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
        public async Task<IReadOnlyList<ViewEmployee>> GetAllAsync()
        {
            try
            {
                var query = "select  * from view_GetEmployee where Status!='Resign' or Status is null";

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
                var query = "select  * from View_Employee where StatusName!='Resign' or StatusName is null";

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
                    return (await connection.QueryFirstOrDefaultAsync<ViewEmployee>(query, parameters));
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
        public async Task<ViewEmployee> GetAllBySessionAsync(Guid? SessionId)
        {
            try
            {
                var query = "select  * from view_GetEmployee where SessionId=@SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);
                using (var connection = CreateConnection())
                {
                    var employeeList = (await connection.QueryFirstOrDefaultAsync<ViewEmployee>(query, parameters));
                    if (employeeList != null && employeeList.LoginPassword != null)
                    {
                        employeeList.LoginPassword = EncryptDecryptPassword.Decrypt(employeeList.LoginPassword);
                    }
                    return employeeList;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ViewEmployee> GetAllByIdAsync(long? EmployeeId)
        {
            try
            {
                var query = "select  * from view_GetEmployee where EmployeeId=@EmployeeId";
                var parameters = new DynamicParameters();
                parameters.Add("EmployeeId", EmployeeId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ViewEmployee>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ViewEmployee> DeleteEmployeeReportAsync(long? EmployeeId)
        {
            ViewEmployee viewEmployee = new ViewEmployee();
            try
            {
                var query = "Delete from EmployeeReportTo where EmployeeId=@EmployeeId";
                var parameters = new DynamicParameters();
                parameters.Add("EmployeeId", EmployeeId);
                using (var connection = CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters);
                    viewEmployee.EmployeeID = EmployeeId.Value;
                    return viewEmployee;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
