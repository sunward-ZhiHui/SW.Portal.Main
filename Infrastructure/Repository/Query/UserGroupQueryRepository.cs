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
        public async Task<List<Employee>> GetUserGroupUserEmployee()
        {
            try
            {
                var query = "select  * from Employee";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Employee>(query)).ToList();
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
                            var res = await GetUserGroupUserEmployee();
                            var result = await GetAllOne(value.UserGroupId);
                            var query = " UPDATE UserGroup SET Name = @Name,Description =@Description,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,IsTms=@IsTms,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID\n\r" +
                                "WHERE UserGroupId = @UserGroupId";
                            await connection.ExecuteAsync(query, parameters);
                            var userId = value.ModifiedByUserId;
                            if (result != null)
                            {
                                bool isUpdate = false; List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();
                                if (result?.Name != value?.Name)
                                {
                                    isUpdate = true;
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = value?.Name, ColumnName = "Name" });
                                }
                                if (result?.Description != value?.Description)
                                {
                                    isUpdate = true;
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, CurrentValue = value?.Description, ColumnName = "Description" });
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
                                    oldValues = string.Join(",", res.Where(w => oldList.Contains(w.UserId)).Select(w => w.FirstName));
                                    newValues = string.Join(",", res.Where(w => newList.Contains(w.UserId)).Select(w => w.FirstName));
                                    isUpdate = true;
                                    auditList.Add(new HRMasterAuditTrail { PreValue = oldList?.Any() == true ? string.Join(",", oldList) : null, CurrentValue = newList?.Any() == true ? string.Join(",", newList) : null, ColumnName = "UserId" });
                                    auditList.Add(new HRMasterAuditTrail { PreValue = oldValues, CurrentValue = newValues, ColumnName = "User" });
                                }

                                if (result?.StatusCodeId != value?.StatusCodeId)
                                {
                                    isUpdate = true;
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), CurrentValue = value?.StatusCodeId?.ToString(), ColumnName = "StatusCodeID" });
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, CurrentValue = value?.StatusCode, ColumnName = "StatusCode" });
                                }
                                if (isUpdate)
                                {
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = value?.Name, ColumnName = "DisplayName" });
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), CurrentValue = value?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserID" });
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, CurrentValue = value?.ModifiedDate != null ? value.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                                    auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy, CurrentValue = value?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                                }
                                if (auditList.Count() > 0)
                                {
                                    HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                    {
                                        HRMasterAuditTrailItems = auditList,
                                        Type = "UserGroup",
                                        FormType = "Update",
                                        HRMasterSetId = result?.UserGroupId,
                                        AuditUserId = userId,
                                    };
                                    await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                                }

                            }
                        }
                        else
                        {
                            var query = "INSERT INTO UserGroup(Name,Description,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsTms)  " +
                                "OUTPUT INSERTED.UserGroupId VALUES " +
                                "(@Name,@Description,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsTms)";

                            value.UserGroupId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();

                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Name, ColumnName = "Name" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = value?.Name, CurrentValue = value?.Name, ColumnName = "DisplayName" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.Description, ColumnName = "Description" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value.SelectUser.Any() ? string.Join(",", value?.SelectUser.ToList()) : null, ColumnName = "User" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value.SelectUserIDs.Any() ? string.Join(",", value?.SelectUserIDs.ToList()) : null, ColumnName = "UserId" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StatusCodeId?.ToString(), ColumnName = "StatusCodeID" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.StatusCode, ColumnName = "StatusCode" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AddedByUserId?.ToString(), ColumnName = "AddedByUserId" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AddedDate != null ? value.AddedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "AddedDate" });
                            auditList.Add(new HRMasterAuditTrail { CurrentValue = value?.AddedBy?.ToString(), ColumnName = "AddedBy" });
                            if (auditList.Count() > 0)
                            {
                                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                {
                                    HRMasterAuditTrailItems = auditList,
                                    Type = "UserGroup",
                                    FormType = "Add",
                                    HRMasterSetId = value?.UserGroupId,
                                    AuditUserId = value?.AddedByUserId,
                                };
                                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                            }
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
                        if (result != null)
                        {
                            List<HRMasterAuditTrail?> auditList = new List<HRMasterAuditTrail?>();

                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, ColumnName = "Name" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.Name, CurrentValue = result?.Name, ColumnName = "DisplayName" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.Description, ColumnName = "Description" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result.SelectUser.Any() ? string.Join(",", result?.SelectUser.ToList()) : null, ColumnName = "User" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result.SelectUserIDs.Any() ? string.Join(",", result?.SelectUserIDs.ToList()) : null, ColumnName = "UserId" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCodeId?.ToString(), ColumnName = "StatusCodeID" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.StatusCode, ColumnName = "StatusCode" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedByUserId?.ToString(), ColumnName = "ModifiedByUserID" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedDate != null ? result.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt") : null, ColumnName = "ModifiedDate" });
                            auditList.Add(new HRMasterAuditTrail { PreValue = result?.ModifiedBy?.ToString(), ColumnName = "ModifiedBy" });
                            if (auditList.Count() > 0)
                            {
                                HRMasterAuditTrail hRMasterAuditTrail = new HRMasterAuditTrail()
                                {
                                    HRMasterAuditTrailItems = auditList,
                                    Type = "UserGroup",
                                    FormType = "Delete",
                                    HRMasterSetId = result?.UserGroupId,
                                    IsDeleted = true,
                                    AuditUserId = userId,
                                };
                                await _HRMasterAuditTrailQueryRepository.BulkInsertAudit(hRMasterAuditTrail);
                            }
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
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
