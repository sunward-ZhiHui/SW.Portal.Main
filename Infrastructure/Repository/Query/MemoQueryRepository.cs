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
using IdentityModel.Client;
using NAV;
using Application.Queries;
using Infrastructure.Data;
using static iText.IO.Image.Jpeg2000ImageData;
using Google.Protobuf.Collections;
using static iTextSharp.text.pdf.AcroFields;
using Microsoft.Data.Edm.Values;

namespace Infrastructure.Repository.Query
{
    public class MemoQueryRepository : DbConnector, IMemoQueryRepository
    {
        public MemoQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<Memo>> GetAllByAsync()
        {
            try
            {
                List<Memo> Memolist = new List<Memo>(); List<MemoUser> MemoUser = new List<MemoUser>();
                var query = "select t1.MemoID,\r\nt1.Subject,\r\nt1.IsAttachment,\r\nt1.SessionID,\r\nt1.StartDate,\r\nt1.StatusCodeID,\r\nt1.AddedDate,\r\nt1.AddedByUserID,\r\nt1.ModifiedDate,\r\nt1.ModifiedByUserID,t2.CodeValue as StatusCode,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser  from Memo t1 LEFT JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID LEFT JOIN ApplicationUser t3 ON t1.AddedByUserID=t3.UserID LEFT JOIN ApplicationUser t4 ON t1.ModifiedByUserID=t4.UserID\r\n;";
                query += "select * from MemoUser;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query);
                    Memolist = results.Read<Memo>().ToList();
                    MemoUser = results.Read<MemoUser>().ToList();
                }
                if (Memolist != null && Memolist.Count > 0)
                {
                    Memolist.ForEach(s =>
                    {
                        var MemoUsers = MemoUser.Where(w => w.MemoId == s.MemoId && w.UserType != "CC User").ToList();
                        var MemoccUsers = MemoUser.Where(w => w.MemoId == s.MemoId && w.UserType == "CC User").ToList();
                        s.UserType = MemoUsers != null && MemoUsers.Count > 0 ? MemoUsers.FirstOrDefault()?.UserType : "User";
                        if (string.IsNullOrEmpty(s.UserType))
                        {
                            s.UserType = "User";
                        }
                        if (MemoUsers != null && MemoUsers.Count() > 0)
                        {
                            s.SelectUserIDs = MemoUsers.Where(w => w.UserId > 0).Select(s => s.UserId).Distinct().ToList();
                            s.SelectUserGroupIDs = MemoUsers.Where(w => w.UserGroupId > 0).Select(s => s.UserGroupId).Distinct().ToList();
                            s.SelectLevelMasterIDs = MemoUsers.Where(w => w.LevelId > 0).Select(s => s.LevelId).Distinct().ToList();
                            s.MemoUserList = MemoUsers;
                            // s.UserNameLists = string.Join(',', MemoUsers.Select(z => z.FirstName).ToList());
                            // s.AcknowledgeUserNameLists = string.Join(',', MemoUsers.Where(w => w.IsAcknowledgement == true).Select(z => z.FirstName).ToList());
                            s.AcknowledgeUserIDs = MemoUsers.Where(w => w.UserId > 0 && w.IsAcknowledgement == true).Select(s => s.UserId).Distinct().ToList();
                        }
                        if (MemoccUsers != null && MemoccUsers.Count() > 0)
                        {
                            s.SelectCCUserIDs = MemoccUsers.Where(w => w.UserId > 0).Select(s => s.UserId).Distinct().ToList();
                        }
                    });
                }
                return Memolist;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Memo> GetMemoSessionList(Guid? SessionID)
        {
            try
            {
                Memo Memolist = new Memo(); List<MemoUser> MemoUser = new List<MemoUser>();
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", SessionID);
                try
                {
                    var query = "select t1.MemoID,t1.MemoContent,\r\nt1.Subject,\r\nt1.IsAttachment,\r\nt1.SessionID,\r\nt1.StartDate,\r\nt1.StatusCodeID,\r\nt1.AddedDate,\r\nt1.AddedByUserID,\r\nt1.ModifiedDate,\r\nt1.ModifiedByUserID,t2.CodeValue as StatusCode,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser  from Memo t1 LEFT JOIN CodeMaster t2 ON t1.StatusCodeID=t2.CodeID LEFT JOIN ApplicationUser t3 ON t1.AddedByUserID=t3.UserID LEFT JOIN ApplicationUser t4 ON t1.ModifiedByUserID=t4.UserID where t1.SessionID=@SessionID\r\n;";

                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query, parameters);
                        Memolist = results.Read<Memo>().FirstOrDefault();
                        if (Memolist != null)
                        {
                            parameters.Add("MemoId", Memolist.MemoId);
                            var query1 = "select * from MemoUser where MemoId=@MemoId;";
                            MemoUser = (await connection.QueryAsync<MemoUser>(query1,parameters)).ToList();
                        }
                    }
                    if (Memolist != null)
                    {
                        var MemoUsers = MemoUser.Where(w => w.MemoId == Memolist.MemoId && w.UserType != "CC User").ToList();
                        var MemoccUsers = MemoUser.Where(w => w.MemoId == Memolist.MemoId && w.UserType == "CC User").ToList();
                        Memolist.UserType = MemoUsers != null && MemoUsers.Count > 0 ? MemoUsers.FirstOrDefault()?.UserType : "User";
                        if (string.IsNullOrEmpty(Memolist.UserType))
                        {
                            Memolist.UserType = "User";
                        }
                        if (MemoUsers != null && MemoUsers.Count() > 0)
                        {
                            Memolist.SelectUserIDs = MemoUsers.Where(w => w.UserId > 0).Select(s => s.UserId).Distinct().ToList();
                            Memolist.SelectUserGroupIDs = MemoUsers.Where(w => w.UserGroupId > 0).Select(s => s.UserGroupId).Distinct().ToList();
                            Memolist.SelectLevelMasterIDs = MemoUsers.Where(w => w.LevelId > 0).Select(s => s.LevelId).Distinct().ToList();
                            Memolist.MemoUserList = MemoUsers;
                            // Memolist.UserNameLists = string.Join(',', MemoUsers.Select(z => z.FirstName).ToList());
                            // Memolist.AcknowledgeUserNameLists = string.Join(',', MemoUsers.Where(w => w.IsAcknowledgement == true).Select(z => z.FirstName).ToList());
                            Memolist.AcknowledgeUserIDs = MemoUsers.Where(w => w.UserId > 0 && w.IsAcknowledgement == true).Select(s => s.UserId).Distinct().ToList();
                        }
                        if (MemoccUsers != null && MemoccUsers.Count() > 0)
                        {
                            Memolist.SelectCCUserIDs = MemoccUsers.Where(w => w.UserId > 0).Select(s => s.UserId).Distinct().ToList();
                        }
                    }
                    return Memolist;
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<Memo>> GetAllByUserAsync(long? userId)
        {
            try
            {
                List<Memo> Memolist = new List<Memo>();
                var parameters = new DynamicParameters();
                parameters.Add("UserID", userId);
                var query = "select t1.*,t2.IsAcknowledgement,t2.MemoUserId,t2.AcknowledgementDate from Memo t1 JOIN MemoUser t2 ON t1.MemoID=t2.MemoID where t1.StatusCodeId=2730 AND t2.UserID=@UserID order by t1.AddedDate desc; \r\n";

                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    Memolist = results.Read<Memo>().ToList();
                }
                return Memolist;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Memo> DeleteMemo(Memo memo)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("MemoId", memo.MemoId);

                        var query = "Delete from MemoUser where MemoId=@MemoId\r\n;";
                        query += "DELETE  FROM Memo WHERE MemoId = @MemoId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return memo;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
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
        public async Task<IReadOnlyList<MemoUser>> GetMemoUserByMemoIdync(long? MemoId)
        {
            try
            {
                List<MemoUser> memoUsers = new List<MemoUser>();
                var query = "select t1.MemoUserId,\r\nt1.IsAcknowledgement,t1.AcknowledgementDate,\r\nt1.MemoId,\r\nt1.UserType,\r\nt1.UserID,\r\nt1.UserGroupID,\r\nt1.LevelID,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\nt8.Name as DesignationName,\r\nCONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\nfrom MemoUser t1\r\n" +
                    "LEFT JOIN Memo t2 ON t1.MemoId=t2.MemoID\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\nJOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\nwhere  t1.MemoId =" + MemoId + "";
                using (var connection = CreateConnection())
                {
                    memoUsers = (await connection.QueryAsync<MemoUser>(query, null)).ToList();
                }
                return memoUsers != null ? memoUsers : new List<MemoUser>();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Memo> InsertOrUpdateMemo(Memo memo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("MemoId", memo.MemoId);
                        parameters.Add("Subject", memo.Subject, DbType.String);
                        parameters.Add("MemoContent", memo.MemoContent, DbType.String);
                        parameters.Add("IsAttachment", memo.IsAttachment == true ? true : null);
                        parameters.Add("SessionId", memo.SessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", memo.AddedByUserId);
                        parameters.Add("ModifiedByUserID", memo.ModifiedByUserId);
                        parameters.Add("AddedDate", memo.AddedDate, DbType.DateTime);
                        parameters.Add("StartDate", memo.StartDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", memo.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", memo.StatusCodeId);
                        if (memo.MemoId > 0)
                        {
                            var query = " UPDATE Memo SET StartDate=@StartDate,Subject=@Subject,MemoContent = @MemoContent,IsAttachment =@IsAttachment," +
                                "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID " +
                                "WHERE MemoId = @MemoId";
                            await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            var query = "INSERT INTO Memo(StartDate,Subject,MemoContent,IsAttachment,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID)  " +
                                "OUTPUT INSERTED.MemoId VALUES " +
                                "(@StartDate,@Subject,@MemoContent,@IsAttachment,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";
                            memo.MemoId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        var query1 = string.Empty;
                        List<long> MemoUserIds = new List<long>();
                        var userExitsByRoles = new List<MemoUser>();
                        if (memo.AcknowledgeUserIDs.Count() > 0)
                        {
                        }
                        else
                        {
                            //var userExitsByRoles = await GetMemoUsersync(memo.MemoId);
                            if (userExitsByRoles != null && userExitsByRoles.Count() > 0 && memo.SelectUserIDs != null && memo.SelectUserIDs.Count() > 0)
                            {
                                userExitsByRoles.ToList().ForEach(s =>
                                {
                                    var exits = memo.SelectUserIDs.Where(w => w == s.UserId).Count();
                                    if (exits > 0)
                                    {

                                    }
                                    else
                                    {
                                        MemoUserIds.Add(s.MemoUserId);
                                    }
                                });
                            }
                            query1 += "DELETE  FROM MemoUser WHERE MemoId=" + memo.MemoId + ";";
                            if (MemoUserIds != null && MemoUserIds.Count() > 0)
                            {
                                // query1 += "DELETE  FROM MemoUser WHERE MemoUserId in(" + string.Join(',', MemoUserIds) + ");";
                            }
                            var UserType = memo.UserType;
                            if (memo.UserType == "User")
                            {
                                if (memo.SelectUserIDs != null && memo.SelectUserIDs.Count() > 0)
                                {
                                    foreach (var item in memo.SelectUserIDs)
                                    {
                                        var counts = userExitsByRoles.Where(w => w.UserId == item).Count();
                                        if (counts == 0)
                                        {
                                            query1 += "INSERT INTO [MemoUser](MemoId,UserId,UserType) OUTPUT INSERTED.MemoUserId " +
                                               "VALUES (" + memo.MemoId + "," + item + ",'" + UserType + "'" + ");";
                                        }
                                    }
                                }
                            }
                            if (memo.UserType == "User Group")
                            {

                                if (memo.SelectUserGroupIDs != null && memo.SelectUserGroupIDs.Count() > 0)
                                {
                                    var userGroupUsers = await GetUserGroupUserList();
                                    var userGropuIds = userGroupUsers.Where(w => memo.SelectUserGroupIDs.ToList().Contains(w.UserGroupId.Value)).ToList();
                                    if (userGropuIds != null && userGropuIds.Count > 0)
                                    {
                                        userGropuIds.ForEach(s =>
                                        {
                                            var counts = userExitsByRoles.Where(w => w.UserId == s.UserId).Count();
                                            if (counts == 0)
                                            {
                                                query1 += "INSERT INTO [MemoUser](MemoId,UserId,UserGroupId,UserType) OUTPUT INSERTED.MemoUserId " +
                                                "VALUES (" + memo.MemoId + "," + s.UserId + "," + s.UserGroupId + ",'" + UserType + "'" + ");";
                                            }
                                        });
                                    }
                                }
                            }
                            if (memo.UserType == "Level")
                            {
                                var LevelUsers = await GetLeveMasterUsersList(memo.SelectLevelMasterIDs);
                                if (LevelUsers != null && LevelUsers.Count > 0)
                                {
                                    LevelUsers.ToList().ForEach(s =>
                                    {
                                        var counts = userExitsByRoles.Where(w => w.UserId == s.UserId).Count();
                                        if (counts == 0)
                                        {
                                            query1 += "INSERT INTO [MemoUser](MemoId,UserId,LevelId,UserType) OUTPUT INSERTED.MemoUserId " +
                                               "VALUES (" + memo.MemoId + "," + s.UserId + "," + s.LevelId + ",'" + UserType + "'" + ");";
                                        }
                                    });
                                }
                            }
                        }
                        if (memo.SelectCCUserIDs != null && memo.SelectCCUserIDs.Count() > 0)
                        {
                            var UserTypes = "CC User";
                            foreach (var item in memo.SelectCCUserIDs)
                            {
                                var counts = userExitsByRoles.Where(w => w.UserId == item).Count();
                                if (counts == 0)
                                {
                                    query1 += "INSERT INTO [MemoUser](MemoId,UserId,UserType) OUTPUT INSERTED.MemoUserId " +
                                       "VALUES (" + memo.MemoId + "," + item + ",'" + UserTypes + "'" + ");";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(query1))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query1, null);
                        }
                        return memo;
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

        public async Task<MemoUser> UpdateMemoUserAcknowledgement(long? MemoUserId, bool? IsAcknowledgement)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        MemoUser memoUser = new MemoUser();
                        memoUser.MemoUserId = (long)MemoUserId;
                        var parameters = new DynamicParameters();
                        parameters.Add("MemoUserId", MemoUserId);
                        parameters.Add("AcknowledgementDate", DateTime.Now);
                        parameters.Add("IsAcknowledgement", IsAcknowledgement == true ? true : null);
                        var query = "Update MemoUser set  IsAcknowledgement=@IsAcknowledgement,AcknowledgementDate=@AcknowledgementDate where MemoUserId=@MemoUserId;\r\n;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return memoUser;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<Memo> InsertCloneMemo(Memo memo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var PreviousMemoId = memo.MemoId;
                        var parameters = new DynamicParameters();
                        parameters.Add("MemoId", memo.MemoId);
                        parameters.Add("PreviousMemoId", PreviousMemoId);
                        parameters.Add("PreviousStatusCodeID", 2731);
                        parameters.Add("Subject", memo.Subject, DbType.String);
                        parameters.Add("MemoContent", memo.MemoContent, DbType.String);
                        parameters.Add("IsAttachment", memo.IsAttachment == true ? true : null);
                        parameters.Add("SessionId", memo.SessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", memo.AddedByUserId);
                        parameters.Add("ModifiedByUserID", memo.ModifiedByUserId);
                        parameters.Add("AddedDate", memo.AddedDate, DbType.DateTime);
                        parameters.Add("StartDate", memo.StartDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", memo.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", memo.StatusCodeId);
                        var query = "INSERT INTO Memo(StartDate,Subject,MemoContent,IsAttachment,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID)  " +
                            "OUTPUT INSERTED.MemoId VALUES " +
                            "(@StartDate,@Subject,@MemoContent,@IsAttachment,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID);";
                        memo.MemoId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        var query1 = string.Empty;
                        if (memo.MemoUserList != null && memo.MemoUserList.Count() > 0)
                        {
                            
                            memo.MemoUserList.ForEach(s =>
                            {
                                string? userId = s.UserId == null ? "null" : s.UserId.ToString();
                                string? userGroupId = s.UserGroupId == null ? "null" : s.UserGroupId.ToString();
                                string? levelId = s.LevelId == null ? "null" : s.LevelId.ToString();
                                query1 += "INSERT INTO [MemoUser](MemoId,UserId,UserType,UserGroupId,LevelId) OUTPUT INSERTED.MemoUserId " +
                                      "VALUES (" + memo.MemoId + "," + userId + ",'" + s.UserType + "'," + userGroupId + "," + levelId + ");";
                            });
                        }
                        query1 += "Update Memo set  StatusCodeID=@PreviousStatusCodeID where MemoId=@PreviousMemoId;\r\n;";
                        if (!string.IsNullOrEmpty(query1))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query1, parameters);
                        }
                        return memo;
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
