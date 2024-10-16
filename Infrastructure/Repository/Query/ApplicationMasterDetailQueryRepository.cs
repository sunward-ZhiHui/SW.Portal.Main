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
using Core.Entities.Views;
using Core.EntityModels;

namespace Infrastructure.Repository.Query
{
    public class ApplicationMasterDetailQueryRepository : QueryRepository<View_ApplicationMasterDetail>, IApplicationMasterDetailQueryRepository
    {
        public ApplicationMasterDetailQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ApplicationMasterDetail>> GetAllAsync()
        {
            try
            {
                var query = "select * from ApplicationMasterDetail WHERE (StatusCodeId=1 OR StatusCodeId IS Null);";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterDetail>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<View_ApplicationMasterDetail>> GetApplicationMasterByCode(long? Id)
        {
            try
            {
                var query = "select * from view_ApplicationMasterDetail WHERE StatusCodeId=1 AND  ApplicationMasterCodeID =" + "'" + Id + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_ApplicationMasterDetail>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<View_ApplicationMasterDetail>> GetApplicationMasterByAllCode(long? Id)
        {
            try
            {
                var query = "select * from view_ApplicationMasterDetail WHERE ApplicationMasterCodeID =" + "'" + Id + "'";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<View_ApplicationMasterDetail>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<View_ApplicationMasterDetail> GetByIdAsync(long? Id)
        {
            try
            {
                var query = "SELECT * FROM view_ApplicationMasterDetail WHERE ApplicationMasterDetailID = @ApplicationMasterDetailID";
                var parameters = new DynamicParameters();
                parameters.Add("ApplicationMasterDetailID", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<View_ApplicationMasterDetail>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMaster> GetByApplicationMasterCodeAsync()
        {
            try
            {
                var query = "SELECT * FROM ApplicationMaster order by ApplicationMasterCodeId desc";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationMaster>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMaster> InsertApplicationMaster(ApplicationMaster value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ApplicationMasterId", value.ApplicationMasterId);
                        parameters.Add("ApplicationMasterName", value.ApplicationMasterName, DbType.String);
                        parameters.Add("ApplicationMasterDescription", value.ApplicationMasterDescription, DbType.String);
                        if (value.ApplicationMasterId > 0)
                        {
                            var query = "UPDATE ApplicationMaster SET ApplicationMasterName=@ApplicationMasterName,ApplicationMasterDescription = @ApplicationMasterDescription " +
                                "WHERE ApplicationMasterId = @ApplicationMasterId";
                            await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            var checkLink = await GetByApplicationMasterCodeAsync();
                            long? ApplicationMasterCodeId = 100;
                            if (checkLink != null && checkLink.ApplicationMasterCodeId > 0)
                            {
                                ApplicationMasterCodeId = (long)checkLink.ApplicationMasterCodeId + 1;
                            }

                            parameters.Add("ApplicationMasterCodeId", ApplicationMasterCodeId);
                            var query = "INSERT INTO [ApplicationMaster](ApplicationMasterName,ApplicationMasterDescription,ApplicationMasterCodeId) OUTPUT INSERTED.ApplicationMasterId VALUES " +
                                "(@ApplicationMasterName,@ApplicationMasterDescription,@ApplicationMasterCodeId)";

                            value.ApplicationMasterId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUserList()
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
        public async Task<IReadOnlyList<LeveMasterUsersModel>> GetLeveMasterUsersList(IEnumerable<long?> SelectLevelMasterIDs)
        {
            try
            {
                var LevelIds = SelectLevelMasterIDs != null && SelectLevelMasterIDs.Count() > 0 ? SelectLevelMasterIDs : new List<long?>() { -1 };
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
        public async Task<IReadOnlyList<ApplicationMasterAccess>> GetApplicationMasterAccessSecurityEmptyAsync(long? ApplicationMasterId, string? AccessTypeFrom)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ApplicationMasterId", ApplicationMasterId);
                parameters.Add("AccessTypeFrom", AccessTypeFrom, DbType.String);
                var query = "select  * from ApplicationMasterAccess where  AccessTypeFrom=@AccessTypeFrom AND ApplicationMasterId=@ApplicationMasterId";
                if (AccessTypeFrom == "AppMasterParent")
                {
                    query = "select  * from ApplicationMasterAccess where AccessTypeFrom=@AccessTypeFrom AND  ApplicationMasterParentId=@ApplicationMasterId";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterAccess>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationMasterAccess>> GetApplicationMasterAccessCheckSecurityEmptyAsync(ApplicationMasterAccess value, string? AccessTypeFrom)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ApplicationMasterId", value.ApplicationMasterId);
                parameters.Add("ApplicationMasterParentId", value.ApplicationMasterParentId);
                parameters.Add("AccessTypeFrom", AccessTypeFrom, DbType.String);
                var query = "select  * from ApplicationMasterAccess where  AccessTypeFrom=@AccessTypeFrom AND ApplicationMasterId=@ApplicationMasterId";
                if (AccessTypeFrom == "AppMasterParent")
                {
                    query = "select  * from ApplicationMasterAccess where  AccessTypeFrom=@AccessTypeFrom AND ApplicationMasterParentId=@ApplicationMasterParentId";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterAccess>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationMasterAccess>> GetApplicationMasterAccessSecurityList(long? Id, string? AccessTypeFrom)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ApplicationMasterId", Id);
                parameters.Add("AccessTypeFrom", AccessTypeFrom, DbType.String);
                var query = "select t1.*,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.ApplicationMasterName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\n" +
                    "from ApplicationMasterAccess t1\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN ApplicationMaster t4 ON t4.ApplicationMasterId=t1.ApplicationMasterId\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE t1.AccessTypeFrom=@AccessTypeFrom AND " +
                    "t1.ApplicationMasterId=@ApplicationMasterId";
                if (AccessTypeFrom == "AppMasterParent")
                {
                    query = "select t1.*,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.ApplicationMasterName,\r\n" +
                   "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                   "t8.Name as DesignationName,\r\n" +
                   "CONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\n" +
                   "from ApplicationMasterAccess t1\r\n" +
                   "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                   "LEFT JOIN ApplicationMasterParent t4 ON t4.ApplicationMasterParentCodeId=t1.ApplicationMasterId\r\n" +
                   "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                   "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                   "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                   "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE t1.AccessTypeFrom=@AccessTypeFrom AND " +
                   "t1.ApplicationMasterParentId=@ApplicationMasterId";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationMasterAccess>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationMasterAccess> InsertApplicationMasterAccessSecurity(ApplicationMasterAccess value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var userExitsRoles = await GetApplicationMasterAccessCheckSecurityEmptyAsync(value, value.AccessTypeFrom);
                    var userGroupUsers = await GetUserGroupUserList();
                    var LevelUsers = await GetLeveMasterUsersList(value.SelectLevelMasterIDs);

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("AccessTypeFrom", value.AccessTypeFrom, DbType.String);
                        parameters.Add("UserType", value.UserType, DbType.String);
                        parameters.Add("ApplicationMasterId", value.ApplicationMasterId);
                        parameters.Add("ApplicationMasterParentId", value.ApplicationMasterParentId);
                        if (value.UserType == "User")
                        {
                            if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                            {
                                foreach (var item in value.SelectUserIDs)
                                {
                                    var counts = userExitsRoles.Where(w => w.UserId == item).FirstOrDefault();
                                    if (counts == null)
                                    {
                                        query += "INSERT INTO [ApplicationMasterAccess](AccessTypeFrom,ApplicationMasterId,UserId,UserType,ApplicationMasterParentId) OUTPUT INSERTED.ApplicationMasterAccessId " +
                                            "VALUES (@AccessTypeFrom,@ApplicationMasterId," + item + ",@UserType,@ApplicationMasterParentId);\r\n";

                                    }
                                    else
                                    {
                                        query += " UPDATE ApplicationMasterAccess SET AccessTypeFrom=@AccessTypeFrom,UserType=@UserType WHERE ApplicationMasterAccessId='" + counts.ApplicationMasterAccessId + "' AND ApplicationMasterId = @ApplicationMasterId;\r\n";
                                    }
                                }
                            }
                        }
                        if (value.UserType == "UserGroup")
                        {
                            if (value.SelectUserGroupIDs != null && value.SelectUserGroupIDs.Count() > 0)
                            {
                                var userGropuIds = userGroupUsers.Where(w => value.SelectUserGroupIDs.ToList().Contains(w.UserGroupId.Value)).ToList();
                                if (userGropuIds != null && userGropuIds.Count > 0)
                                {
                                    userGropuIds.ForEach(s =>
                                    {
                                        var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                        if (counts == null)
                                        {
                                            query += "INSERT INTO [ApplicationMasterAccess](AccessTypeFrom,ApplicationMasterId,UserId,UserGroupId,UserType,ApplicationMasterParentId) OUTPUT INSERTED.ApplicationMasterAccessId " +
                                                "VALUES (@AccessTypeFrom,@ApplicationMasterId," + s.UserId + "," + s.UserGroupId + ",@UserType,@ApplicationMasterParentId);\r\n";
                                        }
                                        else
                                        {
                                            query += " UPDATE ApplicationMasterAccess SET AccessTypeFrom=@AccessTypeFrom,UserType=@UserType WHERE ApplicationMasterAccessId='" + counts.ApplicationMasterAccessId + "' AND ApplicationMasterId = @ApplicationMasterId;\r\n";
                                        }
                                    });
                                }
                            }
                        }
                        if (value.UserType == "Level")
                        {
                            if (LevelUsers != null && LevelUsers.Count > 0)
                            {
                                LevelUsers.ToList().ForEach(s =>
                                {
                                    var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                    if (counts == null)
                                    {
                                        query += "INSERT INTO [ApplicationMasterAccess](AccessTypeFrom,ApplicationMasterId,UserId,LevelId,UserType,ApplicationMasterParentId) OUTPUT INSERTED.ApplicationMasterAccessId " +
                                            "VALUES (@AccessTypeFrom,@ApplicationMasterId," + s.UserId + "," + s.LevelId + ",@UserType,@ApplicationMasterParentId);\r\n";
                                    }
                                    else
                                    {
                                        query += " UPDATE ApplicationMasterAccess SET AccessTypeFrom=@AccessTypeFrom,UserType=@UserType WHERE ApplicationMasterAccessId='" + counts.ApplicationMasterAccessId + "' AND ApplicationMasterId = @ApplicationMasterId;\r\n";
                                    }
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
        public async Task<long> DeleteApplicationMasterAccess(long? Id, List<long?> Ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var query = string.Empty;
                        if (Ids != null && Ids.Count > 0)
                        {
                            string IdList = string.Join(",", Ids);
                            query += "Delete From ApplicationMasterAccess WHERE ApplicationMasterAccessId in (" + IdList + ");\r\n";
                        }
                        if (!string.IsNullOrEmpty(query))
                        {

                            await connection.QuerySingleOrDefaultAsync<long>(query);
                        }
                        return Id.GetValueOrDefault(0);
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
