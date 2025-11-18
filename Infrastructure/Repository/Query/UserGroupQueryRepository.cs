using Core.Entities;
using Core.Repositories.Query;
using Dapper;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Infrastructure.Repository.Query.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class UserGroupQueryRepository : QueryRepository<UserGroup>, IUserGroupQueryRepository
    {
        private readonly IHRMasterAuditTrailQueryRepository _HRMasterAuditTrailQueryRepository;
        public UserGroupQueryRepository(IConfiguration configuration, IHRMasterAuditTrailQueryRepository hRMasterAuditTrailQueryRepository)
            : base(configuration)
        {
            _HRMasterAuditTrailQueryRepository = hRMasterAuditTrailQueryRepository;
        }
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUser()
        {
            try
            {
                var query = "select  t1.*,t2.FirstName as FirstName from UserGroupUser t1\r\nLEFT JOIN Employee t2 ON t1.UserID=t2.UserID";

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
                    parameters.Add("Name", value, DbType.String);

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
                            var result = await GetAllOne(value.UserGroupId);
                            var query = " UPDATE UserGroup SET Name = @Name,Description =@Description,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,IsTms=@IsTms,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID\n\r" +
                                "WHERE UserGroupId = @UserGroupId";
                            await connection.ExecuteAsync(query, parameters);
                            var guid = Guid.NewGuid();
                            var uid = Guid.NewGuid();
                            var userId = value.ModifiedByUserId;
                            if (result != null)
                            {
                                bool isUpdate = false;
                                if (result?.Name != value?.Name)
                                {
                                    isUpdate = true;
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result?.Name, value?.Name, value.UserGroupId, guid, userId, DateTime.Now, false, "Name", uid);
                                }
                                if (result?.Description != value?.Description)
                                {
                                    isUpdate = true;
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result?.Description, value?.Description, value.UserGroupId, guid, userId, DateTime.Now, false, "Description", uid);
                                }
                                var newList = value?.SelectUserIDs.ToList();
                                newList = newList != null ? newList : new List<long?>();
                                var oldList = result?.SelectUserIDs.ToList();
                                oldList = oldList != null ? oldList : new List<long?>();
                                var added = newList.Except(oldList).ToList();     // items in new but not old
                                var removed = oldList.Except(newList).ToList();   // items in old but not new
                                string oldValues = string.Empty; string newValues = string.Empty;
                                if (added.Any() || removed.Any())
                                {
                                    isUpdate = true;
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result.SelectUser.Any() ? string.Join(",", result?.SelectUser.ToList()) : null, value.SelectUser.Any() ? string.Join(",", value?.SelectUser.ToList()) : null, value.UserGroupId, guid, userId, DateTime.Now, false, "User", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result.SelectUserIDs.Any() ? string.Join(",", result?.SelectUserIDs.ToList()) : null, value.SelectUserIDs.Any() ? string.Join(",", value?.SelectUserIDs.ToList()) : null, value.UserGroupId, guid, userId, DateTime.Now, false, "UserId", uid);
                                }

                                if (result?.StatusCodeId != value?.StatusCodeId)
                                {
                                    isUpdate = true;
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result?.StatusCodeId?.ToString(), value?.StatusCodeId?.ToString(), value.UserGroupId, guid, userId, DateTime.Now, false, "StatusCodeID", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", value?.StatusCode, value?.StatusCode, value.UserGroupId, guid, userId, DateTime.Now, false, "StatusCode", uid);

                                }
                                if (isUpdate)
                                {
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", value?.Name, value?.Name, value.UserGroupId, guid, userId, DateTime.Now, false, "DisplayName", uid);

                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result?.ModifiedByUserId?.ToString(), value?.ModifiedByUserId?.ToString(), value.UserGroupId, guid, userId, DateTime.Now, false, "ModifiedByUserId", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, value?.ModifiedDate != null ? value.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, value.UserGroupId, guid, userId, DateTime.Now, false, "ModifiedDate", uid);
                                    uid = Guid.NewGuid();
                                    await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Update", result?.ModifiedBy, value?.ModifiedBy, value.UserGroupId, guid, userId, DateTime.Now, false, "ModifiedBy", uid);
                                }
                            }
                        }
                        else
                        {
                            var query = "INSERT INTO UserGroup(Name,Description,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsTms)  " +
                                "OUTPUT INSERTED.UserGroupId VALUES " +
                                "(@Name,@Description,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsTms)";

                            value.UserGroupId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            var guid = Guid.NewGuid();
                            var uid = Guid.NewGuid();
                            var userId = value.AddedByUserId;
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.Name, value.UserGroupId, guid, userId, DateTime.Now, false, "Name", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.Description, value.UserGroupId, guid, userId, DateTime.Now, false, "Description", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", value?.Name, value?.Name, value.UserGroupId, guid, userId, DateTime.Now, false, "DisplayName", uid);

                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null,value.SelectUser.Any() ? string.Join(",", value?.SelectUser.ToList()) : null, value.UserGroupId, guid, userId, DateTime.Now, false, "User", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null,value.SelectUserIDs.Any() ? string.Join(",", value?.SelectUserIDs.ToList()) : null, value.UserGroupId, guid, userId, DateTime.Now, false, "UserId", uid);



                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.StatusCodeId?.ToString(), value.UserGroupId, guid, userId, DateTime.Now, false, "StatusCodeID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.StatusCode, value.UserGroupId, guid, userId, DateTime.Now, false, "StatusCode", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.AddedByUserId?.ToString(), value.UserGroupId, guid, userId, DateTime.Now, false, "AddedByUserID", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.AddedDate != null ? value.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, value.UserGroupId, guid, userId, DateTime.Now, false, "AddedDate", uid);
                            uid = Guid.NewGuid();
                            await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Add", null, value?.AddedBy, value.UserGroupId, guid, userId, DateTime.Now, false, "AddedBy", uid);

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
        public async Task<UserGroup> GetAllOne(long? UserGroupId)
        {
            UserGroup usersGroupLists = new UserGroup();
            var userGroupUsers = await GetUserGroupUser();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserGroupId", UserGroupId);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from UserGroup t1\r\nLEFT JOIN ApplicationUser t2 ON t1.AddedByUserID=t2.UserID\r\nLEFT JOIN ApplicationUser t3 ON t1.ModifiedByUserID=t3.UserID\r\nLEFT JOIN CodeMaster t4 ON t1.StatusCodeID=t4.CodeID where t1.UserGroupId=@UserGroupId";
                using (var connection = CreateConnection())
                {
                    usersGroupLists = await connection.QueryFirstOrDefaultAsync<UserGroup>(query, parameters);
                }
                if (usersGroupLists != null)
                {
                    var userss = userGroupUsers.Where(w => w.UserGroupId == usersGroupLists.UserGroupId).Select(s => s.UserId).ToList();
                    var usersNames = userGroupUsers.Where(w => w.UserGroupId == usersGroupLists.UserGroupId).Select(s => s.FirstName).ToList();
                    usersGroupLists.SelectUserIDs = userss != null && userss.Count > 0 ? userss : new List<long?>();
                    usersGroupLists.SelectUser = usersNames != null && usersNames.Count > 0 ? usersNames : new List<string?>();
                }
                return usersGroupLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<UserGroup> DeleteUserGroup(UserGroup value, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var result = await GetAllOne(value.UserGroupId);
                        var parameters = new DynamicParameters();
                        parameters.Add("UserGroupId", value.UserGroupId);
                        var query = "DELETE FROM UserGroupUser WHERE UserGroupId= @UserGroupId;";
                        query += "DELETE FROM UserGroup WHERE UserGroupId= @UserGroupId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        var guid = Guid.NewGuid();
                        var uid = Guid.NewGuid();

                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.Name, null, value.UserGroupId, guid, userId, DateTime.Now, true, "Name", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.Description, null, value.UserGroupId, guid, userId, DateTime.Now, true, "Description", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.Name, result?.Name, value.UserGroupId, guid, userId, DateTime.Now, true, "DisplayName", uid);

                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result.SelectUser.Any() ? string.Join(",", result?.SelectUser.ToList()) : null, null, value.UserGroupId, guid, userId, DateTime.Now, true, "User", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result.SelectUserIDs.Any() ? string.Join(",", result?.SelectUserIDs.ToList()) : null, null, value.UserGroupId, guid, userId, DateTime.Now, true, "UserId", uid);



                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.StatusCodeId?.ToString(), null, value.UserGroupId, guid, userId, DateTime.Now, true, "StatusCodeID", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.StatusCode, null, value.UserGroupId, guid, userId, DateTime.Now, true, "StatusCode", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.ModifiedByUserId?.ToString(), null, value.UserGroupId, guid, userId, DateTime.Now, true, "ModifiedByUserId", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, null, value.UserGroupId, guid, userId, DateTime.Now, true, "ModifiedDate", uid);
                        uid = Guid.NewGuid();
                        await _HRMasterAuditTrailQueryRepository.InsertHRMasterAuditTrail("UserGroup", "Delete", result?.ModifiedBy, null, value.UserGroupId, guid, userId, DateTime.Now, true, "ModifiedBy", uid);

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
