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
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Repository.Query
{
    public class UserGroupQueryRepository : QueryRepository<UserGroup>, IUserGroupQueryRepository
    {
        public UserGroupQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUser()
        {
            try
            {
                var query = "select  * from UserGroupUser";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserGroupUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<UserGroup>> GetAllAsync()
        {
            List<UserGroup> usersGroupLists = new List<UserGroup>();
            var userGroupUsers = await GetUserGroupUser();
            try
            {
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from UserGroup t1\r\nLEFT JOIN ApplicationUser t2 ON t1.AddedByUserID=t2.UserID\r\nLEFT JOIN ApplicationUser t3 ON t1.ModifiedByUserID=t3.UserID\r\nLEFT JOIN CodeMaster t4 ON t1.StatusCodeID=t4.CodeID";
                var usersGroupList = new List<UserGroup>();
                using (var connection = CreateConnection())
                {
                    usersGroupList = (await connection.QueryAsync<UserGroup>(query)).ToList();
                }
                if (usersGroupList != null && usersGroupList.Count > 0)
                {
                    usersGroupList.ForEach(s =>
                    {
                        var userss = userGroupUsers.Where(w => w.UserGroupId == s.UserGroupId).Select(s => s.UserId).ToList();
                        s.SelectUserIDs = userss != null && userss.Count > 0 ? userss : new List<long?>();
                        usersGroupLists.Add(s);
                    });
                }
                return usersGroupLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public UserGroup GetUserGroupNameCheckValidation(string? value, long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("Name", value);
                if (id > 0)
                {
                    parameters.Add("UserGroupId", id);
                    parameters.Add("Name", value,DbType.String);

                    query = "SELECT * FROM UserGroup Where StatusCodeId=1 AND  UserGroupId!=@UserGroupId AND Name = @Name";
                }
                else
                {
                    query = "SELECT * FROM UserGroup Where StatusCodeId=1 AND Name = @Name";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<UserGroup>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUserByUserGroupID(long? UserGroupId)
        {
            try
            {
                var query = "select  * from UserGroupUser where UserGroupId=" + UserGroupId + "";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserGroupUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<UserGroup> InsertOrUpdateUserGroup(UserGroup value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserGroupId", value.UserGroupId);
                        parameters.Add("Name", value.Name, DbType.String);
                        parameters.Add("Description", value.Description, DbType.String);
                        parameters.Add("AddedByUserID", value.AddedByUserId);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserId);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", value.StatusCodeId);
                        parameters.Add("IsTms", value.IsTms);
                        if (value.UserGroupId > 0)
                        {
                            var query = " UPDATE UserGroup SET Name = @Name,Description =@Description,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,IsTms=@IsTms,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID\n\r" +
                                "WHERE UserGroupId = @UserGroupId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO UserGroup(Name,Description,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsTms)  " +
                                "OUTPUT INSERTED.UserGroupId VALUES " +
                                "(@Name,@Description,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsTms)";

                            value.UserGroupId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        var userList = await GetUserGroupUserByUserGroupID(value.UserGroupId);
                        if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                        {
                            var varnotIn = userList.Where(i => !value.SelectUserIDs.Contains(i.UserId)).Select(a => a.UserGroupUserId).ToList();
                            var listData = value.SelectUserIDs.ToList();
                            if (listData.Count > 0)
                            {
                                var querys = string.Empty;
                                listData.ForEach(s =>
                                {
                                    if (userList == null)
                                    {
                                        querys += "INSERT INTO [UserGroupUser](UserId,UserGroupId) " +
                                                                "VALUES ( " + s + "," + value.UserGroupId + ");\r\n";
                                    }
                                    else
                                    {
                                        var usersExits = userList.FirstOrDefault(a => a.UserId == s);
                                        if (usersExits == null)
                                        {
                                            querys += "INSERT INTO [UserGroupUser](UserId,UserGroupId) " +
                                                                "VALUES ( " + s + "," + value.UserGroupId + ");\r\n";
                                        }
                                    }
                                });
                                if (!string.IsNullOrEmpty(querys))
                                {
                                    await connection.ExecuteAsync(querys, null);
                                }
                            }
                            if (varnotIn.Count > 0)
                            {
                                var UserGroupUserIds = varnotIn != null && varnotIn.Count > 0 ? varnotIn : new List<long>() { -1 };
                                var Deletequery = "DELETE from UserGroupUser where UserGroupUserId in(" + string.Join(',', UserGroupUserIds) + ");";
                                await connection.ExecuteAsync(Deletequery);
                            }
                        }
                        else
                        {
                            var Deletequery = "DELETE  FROM UserGroupUser WHERE UserGroupId = " + value.UserGroupId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        return value;
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
        public async Task<UserGroup> DeleteUserGroup(UserGroup value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserGroupId", value.UserGroupId);
                        var query = "DELETE FROM UserGroup WHERE " +
                            "UserGroupId= @UserGroupId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
