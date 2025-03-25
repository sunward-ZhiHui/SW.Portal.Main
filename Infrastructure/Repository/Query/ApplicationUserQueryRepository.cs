using Core.Entities;
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
using System.Security.Cryptography;
using Application.Queries;
using Azure.Core;
using System.Data.Common;
using Application.Common;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Infrastructure.Repository.Query
{
    public class ApplicationUserQueryRepository : QueryRepository<ApplicationUser>, IApplicationUserQueryRepository
    {
        private ILocalStorageService<ApplicationUser> _localStorageService;
        public ApplicationUser User { get; private set; }
        public ApplicationUserQueryRepository(IConfiguration configuration, Core.Repositories.Query.ILocalStorageService<ApplicationUser> localStorageService)
            : base(configuration)
        {
            _localStorageService = localStorageService;
        }
        public async Task<ApplicationUser> LoginStatus(string LoginID)
        {
            try
            {
                // Fetch the user by their login ID
                var existing = await GetByUsers(LoginID);

                // If the user exists
                if (existing != null)
                {
                    // Check if the user is locked
                    if (existing.Locked)
                    {
                        return existing;
                    }

                    // Check if the user has resigned
                    var checkResign = await GetByResignUser(LoginID);
                    if (checkResign != null)
                    {
                        return checkResign;
                    }
                }

                // Return the user object (null if no user found)
                return existing;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<ApplicationUser> LoginAuth(string LoginID, string Password)
        {
            try
            {
                var existing = await GetByUsers(LoginID);
                if (existing != null)
                {
                    if (existing.Locked)
                    {
                        return existing;
                    }

                    var checkResign = await GetByResignUser(LoginID);
                    if (checkResign != null)
                    {
                        return checkResign;
                    }
                    else
                    {
                        var query = "SELECT * FROM ApplicationUser WHERE LoginID = @LoginID and LoginPassword = @LoginPassword";
                        var parameters = new DynamicParameters();
                        parameters.Add("LoginID", LoginID);
                        var password = EncryptDecryptPassword.Encrypt(Password);
                        parameters.Add("LoginPassword", password);

                        using (var connection = CreateConnection())
                        {
                            var user = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters);
                            if (user == null)
                            {
                                if (existing.InvalidAttempts < 3)
                                {
                                    //this.AddMessage("Invalid Password, " + (3 - existing.InvalidAttempts - 1) + " attempt(s) left.");
                                    existing.InvalidAttempts = existing.InvalidAttempts + 1;
                                    existing.Locked = false;
                                    user = await AttemptUpdate(existing.UserID, existing.InvalidAttempts, existing.Locked);
                                }
                                else
                                {
                                    //this.AddMessage("Account locked.");                                                               
                                    existing.InvalidAttempts = existing.InvalidAttempts + 1;
                                    existing.Locked = true;
                                    user = await AttemptUpdate(existing.UserID, existing.InvalidAttempts, existing.Locked);
                                }

                                return user;
                            }
                            else
                            {
                                if (existing.Locked)
                                {
                                    return existing;
                                }
                                else
                                {
                                    existing.Locked = false;
                                    existing.InvalidAttempts = 0;
                                    user = await AttemptUpdate(existing.UserID, existing.InvalidAttempts, existing.Locked);
                                    return user;
                                }

                            }
                        }
                    }

                   
                }
                else
                {
                    //ApplicationUser ApplicationUser = new ApplicationUser();
                    return existing;
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> AttemptUpdate(long UserID, int InvalidAttempts, bool Locked)
        {
            try
            {
                var query = "update ApplicationUser set InvalidAttempts=@InvalidAttempts,Locked =@Locked where UserID=@UserID";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                parameters.Add("InvalidAttempts", InvalidAttempts);
                parameters.Add("Locked", Locked);

                using (var connection = CreateConnection())
                {
                    var user = await connection.ExecuteAsync(query, parameters);
                    return await GetByUserID(UserID.ToString());
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationUser> Auth(string LoginID, string Password)
        {

            try
            {
                var query = "SELECT * FROM ApplicationUser WHERE LoginID = @LoginID and LoginPassword = @LoginPassword";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", LoginID);
                var password = EncryptDecryptPassword.Encrypt(Password);
                //var spass = "5KfU/7xzDrzTexTebrZnaqUXqS3PLIh/8qKgeapts7g=";
                //var Decryptpassword = EncryptDecryptPassword.Decrypt(spass);
                parameters.Add("LoginPassword", password);

                using (var connection = CreateConnection())
                {
                    var user = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters);

                    return user;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationUser";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationUser> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ApplicationRole WHERE roleId = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationUser> GetByUsers(string name)
        {
            try
            {
                var query = "SELECT * FROM ApplicationUser WHERE LoginID = @LoginID";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> GetByApplicationUsersList(long userId)
        {
            try
            {
                var query = @"SELECT * FROM ApplicationUser WHERE UserID = @userId";
                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> GetByResignUser(string name)
        {
            try
            {
                var query = @"SELECT AP.*,t3.Value as Status FROM ApplicationUser AP
                            INNER JOIN Employee E on E.userid = AP.userid
                            LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=E.AcceptanceStatus 
                            where (t3.Value='Resign') AND AP.LoginID = @LoginID";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<UserNotification> GetUserNotificationValidatation(long UserId,string DeviceType,string TokenID)
        {
            try
            {
                var query = "SELECT * FROM UserNotifications WHERE UserId = @UserId AND DeviceType = @DeviceType AND TokenID = @TokenID";
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);
                parameters.Add("DeviceType", DeviceType);
                parameters.Add("TokenID", TokenID);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<UserNotification>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<UserNotification>> GetTokenList()
        {
            try
            {
                var query = "SELECT * FROM UserNotifications";
                var parameters = new DynamicParameters();
                //parameters.Add("LoginID", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserNotification>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<UserNotification>> GetAllTokenList(string DeviceType)
        {
            try
            {
                var query = "select TokenID From UserNotifications where DeviceType = @DeviceType";
                var parameters = new DynamicParameters();
                parameters.Add("DeviceType", DeviceType, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserNotification>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> GetByUserID(string name)
        {
            try
            {
                var query = "SELECT * FROM ApplicationUser WHERE UserID = @LoginID";
                var parameters = new DynamicParameters();
                parameters.Add("LoginID", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }        
        public async Task<string> UpdateDeviceId(string LoginID, string DeviceType, string TokenID)
        {
            try
            {
                var User = await GetByUsers(LoginID);
                if (User != null)
                {
                    var userId = User.UserID;


                    var validataion = await GetUserNotificationValidatation(userId, DeviceType, TokenID);
                    if(validataion == null)
                    {
                        var query = "insert into UserNotifications (UserId,DeviceType,TokenID) values (@UserID,@DeviceType,@TokenID)";
                        var parameters = new DynamicParameters();
                        parameters.Add("UserID", userId);
                        parameters.Add("DeviceType", DeviceType);
                        parameters.Add("TokenID", TokenID);
                        using (var connection = CreateConnection())
                        {
                            var user = await connection.ExecuteAsync(query, parameters);
                            return "Added successfully";
                        }
                    }
                    else
                    {
                        return "Already Added";
                    }

                   
                }
                else
                {                    
                    return "-1";
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<long> DeleteDeviceId(string TokenID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    
                    var parameters = new DynamicParameters();
                    parameters.Add("TokenID", TokenID);
                    var query = "DELETE FROM UserNotifications where TokenID = @TokenID AND DeviceType='Web'";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;                    
                    
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> UpdatePasswordUser(long UserID, string NewPassword, string OldPassword, string LoginID)
        {

            try
            {
                var usersList = await Auth(LoginID, OldPassword);
                if (usersList != null)
                {
                    var password = EncryptDecryptPassword.Encrypt(NewPassword);
                    if (usersList.LoginPassword == password)
                    {
                        ApplicationUser ApplicationUser = new ApplicationUser();
                        ApplicationUser.StatusCodeID = -1;
                        return ApplicationUser;
                    }
                    else
                    {
                        var query = "update ApplicationUser set LoginPassword=@NewPassword where UserID=@UserID";
                        var parameters = new DynamicParameters();
                        parameters.Add("UserID", UserID);
                        parameters.Add("NewPassword", password);

                        using (var connection = CreateConnection())
                        {
                            var user = await connection.ExecuteAsync(query, parameters);
                            return await GetByUserID(UserID.ToString());
                        }
                    }
                }
                else
                {
                    ApplicationUser ApplicationUser = new ApplicationUser();
                    return ApplicationUser;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> ForGotPasswordUser(string LoginID, string NewPassword)
        {

            try
            {
                var User = await GetByUsers(LoginID);
                if (User != null)
                {
                    var userId = User.UserID;
                    var query = "update ApplicationUser set LoginPassword=@NewPassword where UserID=@userId";
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userId);
                    var password = EncryptDecryptPassword.Encrypt(NewPassword);
                    parameters.Add("NewPassword", password);

                    using (var connection = CreateConnection())
                    {
                        var user = await connection.ExecuteAsync(query, parameters);
                        return User;
                    }
                }
                else
                {
                    ApplicationUser ApplicationUser = new ApplicationUser();
                    return ApplicationUser;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ApplicationUser> UnLockedPassword(string LoginID, string NewPassword,bool? locked)
        {
            try
            {
                var User = await GetByUsers(LoginID);
                if (User != null)
                {
                    var userId = User.UserID;
                   
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userId);
                    parameters.Add("locked", locked);
                    var password = EncryptDecryptPassword.Encrypt(NewPassword);
                    parameters.Add("NewPassword", password);
                    var query = "UPDATE ApplicationUser set InvalidAttempts = 0,Locked = @locked where UserID=@userId";
                    using (var connection = CreateConnection())
                    {
                        var user = await connection.ExecuteAsync(query, parameters);
                        return User;
                    }
                }
                else
                {
                    ApplicationUser ApplicationUser = new ApplicationUser();
                    return ApplicationUser;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> ActiveUser(string LoginID)
        {
            try
            {
                var User = await GetByUsers(LoginID);
                if (User != null)
                {
                    var userId = User.UserID;
                    var query = "UPDATE ApplicationUser set   StatusCodeID= 1 where UserID = @UserID";
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userId);
                   
                    using (var connection = CreateConnection())
                    {
                        var user = await connection.ExecuteAsync(query, parameters);
                        return User;
                    }
                }
                else
                {
                    ApplicationUser ApplicationUser = new ApplicationUser();
                    return ApplicationUser;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationUser> InActiveUser(string LoginID)
        {
            try
            {
                var User = await GetByUsers(LoginID);
                if (User != null)
                {
                    var userId = User.UserID;
                    var query = "UPDATE ApplicationUser set   StatusCodeID= 0  where UserID = @UserID";
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", userId);

                    using (var connection = CreateConnection())
                    {
                        var user = await connection.ExecuteAsync(query, parameters);
                        return User;
                    }
                }
                else
                {
                    ApplicationUser ApplicationUser = new ApplicationUser();
                    return ApplicationUser;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<ApplicationUser>("user");
        }
    }
}
