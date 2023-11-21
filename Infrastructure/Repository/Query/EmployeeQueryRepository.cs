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
        private string QueryString()
        {
            var query = @"SELECT concat(emp.FirstName,',',emp.LastName) as Name, emp.EmployeeID, emp.UserID, emp.SageID,emp.PlantID, emp.LevelID, emp.LanguageID, emp.CityID, emp.RegionID, emp.ReportID, emp.FirstName, emp.NickName, emp.LastName, emp.Gender, emp.JobTitle, emp.Email, u.LoginID, u.LoginPassword,u.UserCode,emp.TypeOfEmployeement, emp.Signature, emp.ImageUrl, emp.DateOfEmployeement,emp.LastWorkingDate, emp.Extension, emp.SpeedDial, emp.SkypeAddress, emp.Mobile,emp.IsActive, emp.SectionID,  emp.DivisionID,emp.DesignationId, emp.DepartmentId, emp.SubSectionId, emp.SubSectionTid, u.StatusCodeID,  emp.AddedByUserId, emp.ModifiedByUserId,emp.AddedDate, emp.ModifiedDate, emp.AcceptanceStatus, emp.ExpectedJoiningDate, emp.AcceptanceStatusDate, emp.HeadCount, a.UserName as AddedByUser, mo.UserName as ModifiedByUser, ag.Value as Status, u.SessionID, u.InvalidAttempts, 
                u.Locked,d.Name as DesignationName,p.PlantCode as CompanyName
                FROM Employee emp 
                LEFT JOIN ApplicationUser u ON u.userId = emp.userId
                LEFT JOIN ApplicationUser a ON a.UserId = emp.AddedByUserID 
                LEFT JOIN ApplicationUser mo ON mo.UserId = emp.ModifiedByUserID 
                LEFT JOIN ApplicationMasterDetail ag ON ag.ApplicationMasterDetailID = emp.AcceptanceStatus 
                LEFT JOIN plant p ON p.plantId = emp.plantId 
                LEFT JOIN Designation d ON d.DesignationID = emp.DesignationID
                WHERE ag.Value!='Resign' or ag.Value is null";
            return query;
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
                var query = QueryString();

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query)).ToList();
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
                var query = QueryString();

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ViewEmployee>> GetAllUserWithoutStatusAsync()
        {
            List<ViewEmployee> ViewEmployees = new List<ViewEmployee>();
            try
            {
                var query = "select  * from view_GetEmployee";
                var result = new List<ViewEmployee>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<ViewEmployee>(query)).ToList();
                }
                if (result == null || result.Count == 0)
                {

                }
                else
                {
                    result.ForEach(s =>
                    {
                        var empCount = ViewEmployees.Where(w => w.EmployeeID == s.EmployeeID).Count();
                        if (empCount == 0)
                        {
                            s.Locked = s.Locked == true ? true : false;
                            ViewEmployees.Add(s);
                        }
                    });
                }
                return ViewEmployees;

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
                var query = QueryString() + " AND emp.UserID = @UserID";
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

        public async Task<IReadOnlyList<ApplicationPermission>> GetAllApplicationPermissionAsync(Int64 RoleId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("RoleID", RoleId, DbType.Int64);

                var query = @"SELECT ARP.PermissionID,ARP.PermissionName,ARP.ParentID FROM ApplicationPermission ARP 
                            Left JOIN ApplicationRolePermission AP ON AP.PermissionID = ARP.PermissionID 
                                  WHERE AP.RoleID = @RoleID and ARP.IsDisplay =1 and ARP.PermissionID > =60000";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query, parameters)).ToList();
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }

}
