using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Query
{
    public class SettingsQueryRepository : QueryRepository<OpenAccessUser>, ISettingsQueryRepository
    {
        public SettingsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<OpenAccessUserLink> GetDMSAccessByUser(long? UserID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                var query = "select t1.* from OpenAccessUserLink t1\r\nJOIN OpenAccessUser t2 ON t1.OpenAccessUserID=t2.OpenAccessUserID AND t2.AccessType='DMSAccess'\r\nWHERE t1.UserID=@UserID";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<OpenAccessUserLink>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<OpenAccessUserLink> GetEmailAccessByUser(long? UserID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                var query = "select t1.* from OpenAccessUserLink t1\r\nJOIN OpenAccessUser t2 ON t1.OpenAccessUserID=t2.OpenAccessUserID AND t2.AccessType='EmailAccess'\r\nWHERE t1.UserID=@UserID";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<OpenAccessUserLink>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<OpenAccessUserLink> GetEmailOtherTagAccessByUser(long? UserID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                var query = "select t1.* from OpenAccessUserLink t1\r\nJOIN OpenAccessUser t2 ON t1.OpenAccessUserID=t2.OpenAccessUserID AND t2.AccessType='EmailOtherTagAccess'\r\nWHERE t1.UserID=@UserID";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<OpenAccessUserLink>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<OpenAccessUserLink>> GetAllAsync(long? OpenAccessUserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("OpenAccessUserId", OpenAccessUserId, DbType.String);
                var query = "select t1.*,t2.AccessType,t3.Name as UserGroup,t3.Description as UserGroupDescription,t5.Name as LevelName,t6.NickName,t6.FirstName,t6.LastName,t7.Name as DepartmentName,t8.Name as DesignationName," +
                    "CONCAT(case when t6.NickName is NULL then  t6.FirstName ELSE  t6.FirstName END,' | ',t6.LastName) as FullName\r\n    from OpenAccessUserLink t1\r\n    " +
                    "JOIN OpenAccessUser t2 ON t1.OpenAccessUserID=t2.OpenAccessUserID\r\n    " +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n    " +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n    " +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n    " +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n   " +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID WHERE t2.OpenAccessUserId=@OpenAccessUserId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<OpenAccessUserLink>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<OpenAccessUser> GetAllByAsync(string? AccessType)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AccessType", AccessType);
                var query = "SELECT * FROM OpenAccessUser Where AccessType=@AccessType";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<OpenAccessUser>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<OpenAccessUserLink>> GetDocumentAccessTypeEmptyAsync(string? accessType)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AccessType", accessType);
                var query = "SELECT t1.*,t2.AccessType FROM OpenAccessUserLink t1 JOIN OpenAccessUser t2 ON t1.OpenAccessUserID=t2.OpenAccessUserID Where t2.AccessType=@AccessType";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<OpenAccessUserLink>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUserType()
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
        public async Task<IReadOnlyList<LeveMasterUsersModel>> GetLeveMasterUsersType(IEnumerable<long>? SelectLevelMasterIDs)
        {
            try
            {
                var LevelIds = SelectLevelMasterIDs != null && SelectLevelMasterIDs.Count() > 0 ? SelectLevelMasterIDs : new List<long>() { -1 };
                var query = "select  t1.LevelID,t1.DesignationID,t3.UserID from Designation t1 \r\n" +
                    "JOIN LevelMaster t2 ON t1.LevelID=t2.LevelID\r\n" +
                    "JOIN Employee t3 ON t3.DesignationID=t1.DesignationID " +
                    "where t1.LevelID in(" + string.Join(',', LevelIds) + ")"; ;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<LeveMasterUsersModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long?> InsertOpenAccessUserLinkOne(OpenAccessUserLink value)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("AccessType", value.AccessType, DbType.String);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("SessionId", Guid.NewGuid(), DbType.Guid);
                        var querys = "INSERT INTO [OpenAccessUser](AccessType,AddedByUserID,ModifiedByUserID,StatusCodeID,AddedDate,ModifiedDate,SessionId) OUTPUT INSERTED.OpenAccessUserId " +
                            "VALUES (@AccessType,@AddedByUserID,@ModifiedByUserID,@StatusCodeID,@AddedDate,@ModifiedDate,@SessionId)";
                        var values = await connection.QuerySingleOrDefaultAsync<long>(querys, parameters);

                        return values;
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
        public async Task<OpenAccessUserLink> InsertOpenAccessUserLink(OpenAccessUserLink value)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var IsDmsAccess = value.IsDmsAccess == true ? 1 : 0;
                        var IsDmsCreateMainFolder = value.IsDmsCreateMainFolder == true ? 1 : 0;
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("IsDmsAccess", value.IsDmsAccess);
                        parameters.Add("IsDmsCreateMainFolder", value.IsDmsCreateMainFolder);
                        parameters.Add("IsAdd", value.IsAdd);
                        parameters.Add("IsDelete", value.IsDelete);
                        parameters.Add("IsEdit", value.IsEdit);
                        parameters.Add("IsPermission", value.IsPermission);
                        var lists = await GetAllByAsync(value.AccessType);
                        if (lists != null)
                        {
                            value.OpenAccessUserId = lists.OpenAccessUserId;
                        }
                        else
                        {
                            value.OpenAccessUserId = await InsertOpenAccessUserLinkOne(value);
                        }
                        if (value.OpenAccessUserId > 0)
                        {
                            var userExitsRoles = await GetDocumentAccessTypeEmptyAsync(value.AccessType);
                            var userGroupUsers = await GetUserGroupUserType();
                            var LevelUsers = await GetLeveMasterUsersType(value.SelectLevelMasterIDs);
                            if (value.Type == "User")
                            {
                                if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                                {
                                    foreach (var item in value.SelectUserIDs)
                                    {
                                        var counts = userExitsRoles.FirstOrDefault(w => w.UserId == item);
                                        if (counts == null)
                                        {
                                            query += "INSERT INTO [OpenAccessUserLink](OpenAccessUserId,UserId,IsDmsAccess,IsDmsCreateMainFolder,IsAdd,IsEdit,IsDelete,IsPermission) OUTPUT INSERTED.OpenAccessUserLinkId " +
                                               "VALUES (" + value.OpenAccessUserId + "," + item + ",@IsDmsAccess,@IsDmsCreateMainFolder,@IsAdd,@IsEdit,@IsDelete,@IsPermission);\n";
                                        }
                                        else
                                        {
                                            if (value.AccessType == "DMSAccess")
                                            {
                                                query += "update  OpenAccessUserLink set IsDmsAccess=@IsDmsAccess,IsDmsCreateMainFolder=@IsDmsCreateMainFolder Where  OpenAccessUserLinkId=" + counts.OpenAccessUserLinkId + ";";
                                            }
                                            if (value.AccessType == "GeneralAccess")
                                            {
                                                query += "update  OpenAccessUserLink set IsAdd=@IsAdd,IsEdit=@IsEdit,IsDelete=@IsDelete,IsPermission=@IsPermission Where  OpenAccessUserLinkId=" + counts.OpenAccessUserLinkId + ";";
                                            }
                                        }
                                    }
                                }
                            }
                            if (value.Type == "UserGroup")
                            {
                                if (value.SelectUserGroupIDs != null && value.SelectUserGroupIDs.Count() > 0)
                                {
                                    var userGropuIds = userGroupUsers.Where(w => value.SelectUserGroupIDs.ToList().Contains(w.UserGroupId.Value)).ToList();
                                    if (userGropuIds != null && userGropuIds.Count > 0)
                                    {
                                        userGropuIds.ForEach(s =>
                                        {
                                            var counts = userExitsRoles.FirstOrDefault(w => w.UserId == s.UserId);
                                            if (counts == null)
                                            {

                                                query += "INSERT INTO [OpenAccessUserLink](OpenAccessUserId,UserId,UserGroupId,IsDmsAccess,IsDmsCreateMainFolder,IsAdd,IsEdit,IsDelete,IsPermission) OUTPUT INSERTED.OpenAccessUserLinkId " +
                                                    "VALUES (" + value.OpenAccessUserId + "," + s.UserId + "," + s.UserGroupId + ",@IsDmsAccess,@IsDmsCreateMainFolder,@IsAdd,@IsEdit,@IsDelete,@IsPermission);\n";
                                            }
                                            else
                                            {
                                                if (value.AccessType == "DMSAccess")
                                                {
                                                    query += "update  OpenAccessUserLink set IsDmsAccess=@IsDmsAccess,IsDmsCreateMainFolder=@IsDmsCreateMainFolder Where  OpenAccessUserLinkId=" + counts.OpenAccessUserLinkId + ";";
                                                }
                                                if (value.AccessType == "GeneralAccess")
                                                {
                                                    query += "update  OpenAccessUserLink set IsAdd=@IsAdd,IsEdit=@IsEdit,IsDelete=@IsDelete,IsPermission=@IsPermission Where  OpenAccessUserLinkId=" + counts.OpenAccessUserLinkId + ";";
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            if (value.Type == "Level")
                            {
                                if (LevelUsers != null && LevelUsers.Count > 0)
                                {
                                    LevelUsers.ToList().ForEach(s =>
                                    {
                                        var counts = userExitsRoles.FirstOrDefault(w => w.UserId == s.UserId);
                                        if (counts == null)
                                        {

                                            query += "INSERT INTO [OpenAccessUserLink](OpenAccessUserId,UserId,LevelId,IsDmsAccess,IsDmsCreateMainFolder,IsAdd,IsEdit,IsDelete,IsPermission) OUTPUT INSERTED.OpenAccessUserLinkId " +
                                                "VALUES (" + value.OpenAccessUserId + "," + s.UserId + "," + s.LevelId + ",@IsDmsAccess,@IsDmsCreateMainFolder,@IsAdd,@IsEdit,@IsDelete,@IsPermission);\n";

                                        }
                                        else
                                        {
                                            if (value.AccessType == "DMSAccess")
                                            {
                                                query += "update  OpenAccessUserLink set IsDmsAccess=@IsDmsAccess,IsDmsCreateMainFolder=@IsDmsCreateMainFolder Where  OpenAccessUserLinkId=" + counts.OpenAccessUserLinkId + ";";
                                            }
                                            if (value.AccessType == "GeneralAccess")
                                            {
                                                query += "update  OpenAccessUserLink set IsAdd=@IsAdd,IsEdit=@IsEdit,IsDelete=@IsDelete,IsPermission=@IsPermission Where  OpenAccessUserLinkId=" + counts.OpenAccessUserLinkId + ";";
                                            }
                                        }
                                    });
                                }
                            }
                            if (!string.IsNullOrEmpty(query))
                            {
                                connection.QueryFirstOrDefault<long>(query, parameters);
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
        public async Task<OpenAccessUserLink> DeleteOpenAccessUserLink(OpenAccessUserLink value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("OpenAccessUserLinkId", value.OpenAccessUserLinkId);
                        var query = "DELETE FROM OpenAccessUserLink WHERE " +
                            "OpenAccessUserLinkId= @OpenAccessUserLinkId";
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
