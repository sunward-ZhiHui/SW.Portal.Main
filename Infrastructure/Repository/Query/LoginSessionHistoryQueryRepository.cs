using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DevExpress.XtraPrinting.Native;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using IdentityModel.Client;
using Infrastructure.Data;
using Infrastructure.Repository.Query.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NAV;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.IO.Image.Jpeg2000ImageData;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class LoginSessionHistoryQueryRepository : DbConnector, ILoginSessionHistoryQueryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginSessionHistoryQueryRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(configuration)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IReadOnlyList<LoginSessionHistory>> GetAllByAsync()
        {
            try
            {
                var parameters = new DynamicParameters();

                var query = "select t1.*,t2.UserName,\r\nCONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  '' ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,(case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as EmployeeName,\r\nt4.Name as DepartmentName,\r\nt5.Name as DesignationName\r\nfrom LoginSessionHistory t1\r\nJOIN ApplicationUser t2 ON t1.UserID=t2.UserID\r\nJOIN Employee t3 ON t2.UserID=t3.UserID\r\nLEFT JOIN Department t4 ON t4.DepartmentID=t3.DepartmentID\r\nLEFT JOIN Designation t5 ON t5.DesignationID=t3.DesignationID  order by t1.LoginSessionHistoryId desc";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<LoginSessionHistory>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<LoginSessionHistory> GetLoginSessionHistoryOne(Guid? SessionId, long? UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                parameters.Add("UserId", UserId);
                var query = "select t1.*\r\nfrom LoginSessionHistory t1 Where t1.SessionId=@SessionId AND t1.UserId=@UserId\r\n";

                using (var connection = CreateConnection())
                {
                    return await connection.QuerySingleOrDefaultAsync<LoginSessionHistory>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<LoginSessionHistory> InsertLoginSessionHistory(LoginSessionHistory loginSessionHistory)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var userAgent = _httpContextAccessor?.HttpContext?.Request.Headers["User-Agent"].ToString();
                        var IpAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", loginSessionHistory.SessionId, DbType.Guid);
                        parameters.Add("UserId", loginSessionHistory.UserId);
                        parameters.Add("LoginType", loginSessionHistory.LoginType, DbType.String);
                        parameters.Add("LoginTime", DateTime.Now, DbType.DateTime);
                        parameters.Add("IsActive", true);
                        parameters.Add("UserAgent", userAgent, DbType.String);
                        parameters.Add("IpAddress", IpAddress, DbType.String);
                        var query = "INSERT INTO LoginSessionHistory(UserId,LoginType,LoginTime,IsActive,SessionId,UserAgent,IpAddress)" +
                            "OUTPUT INSERTED.LoginSessionHistoryId VALUES " +
                            "(@UserId,@LoginType,@LoginTime,@IsActive,@SessionId,@UserAgent,@IpAddress)";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return loginSessionHistory;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<LoginSessionHistory> UpdateLastActivity(LoginSessionHistory loginSessionHistory)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", loginSessionHistory.SessionId, DbType.Guid);
                        parameters.Add("LastActivityTime", DateTime.Now, DbType.DateTime);
                        parameters.Add("UserId", loginSessionHistory.UserId);
                        parameters.Add("LogoutType", loginSessionHistory.LogoutType, DbType.String);
                        var query = "update LoginSessionHistory set LastActivityTime=@LastActivityTime,LogoutType=@LogoutType where IsActive=1 AND SessionId=@SessionId AND UserId=@UserId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return loginSessionHistory;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<LoginSessionHistory> UpdateLogOutActivity(LoginSessionHistory loginSessionHistory)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", loginSessionHistory.SessionId, DbType.Guid);
                        parameters.Add("LogoutType", loginSessionHistory.LogoutType, DbType.String);
                        parameters.Add("LogoutTime", DateTime.Now, DbType.DateTime);
                        parameters.Add("IsActive", loginSessionHistory.IsActive == true ? true : false);
                        parameters.Add("UserId", loginSessionHistory.UserId);
                        var query = "update LoginSessionHistory set IsActive=@IsActive,LogoutType=@LogoutType,LogoutTime=@LogoutTime where IsActive=1 AND  SessionId=@SessionId AND UserId=@UserId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return loginSessionHistory;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
    }

}
