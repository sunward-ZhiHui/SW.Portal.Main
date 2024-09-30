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
using Core.EntityModels;
using Core.Entities;

namespace Infrastructure.Repository.Query
{
    public class TransferPermissionsQueryRepository : QueryRepository<DocumentUserRoleModel>, ITransferPermissionsQueryRepository
    {
        public TransferPermissionsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUsers(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "select t1.*,t2.Name as UserGroupName,\r\n " +
                    "CONCAT(case when t3.NickName is NULL\r\n then  t3.FirstName\r\n ELSE\r\n  t3.NickName END,' | ',t3.LastName) as FirstName\r\n  " +
                    "from UserGroupUser t1\r\n" +
                    "JOIN UserGroup t2 ON t1.UserGroupID=t2.UserGroupID\r\n" +
                    "JOIN Employee t3 ON t3.UserID=t1.UserID WHERE  t1.UserID in(" + string.Join(',', userIds) + ")";

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
        public async Task<List<long?>> DeleteTransferPermissionsUserGroupUser(List<long?> ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        var parameters = new DynamicParameters();

                        var query = "delete from UserGroupUser WHERE UserGroupUserId in(" + string.Join(',', ids) + ")";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return ids;
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

        public async Task<List<long?>> UpdateTransferPermissionsUserGroupUser(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();

                            var query = "Update UserGroupUser set UserID=" + userId + " WHERE  UserGroupUserId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        return ids;
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
        public async Task<IReadOnlyList<DocumentUserRoleModel>> GetTransferPermissionDocumentUserRoleList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserID", Id);
                var query = "select t1.*,t2.DocumentRoleName,t2.DocumentRoleDescription,\r\nt3.Name as UserGroup,\r\n" +
                    "t3.Description as UserGroupDescription,\r\nt4.Name as FileProfileType,\r\n" +
                    "t5.Name as LevelName,\r\n" +
                    "t6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "t9.Name as FileProfileTypeName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\n" +
                    "from DocumentUserRole t1\r\n" +
                    "JOIN DocumentRole t2 ON t1.RoleID=t2.DocumentRoleID\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN FileProfileType t4 ON t4.FileProfileTypeID=t1.FileProfileTypeID\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n " +
                     "LEFT JOIN FileProfileType t9 ON t9.FileProfileTypeId=t1.FileProfileTypeId\r\n\r\n " +
                    "WHERE t1.UserID=@UserID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentUserRoleModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<List<long?>> DeleteTransferPermissionsDocumentUserRoleUser(List<long?> ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        var parameters = new DynamicParameters();

                        var query = "delete from DocumentUserRole WHERE DocumentUserRoleID in(" + string.Join(',', ids) + ")";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return ids;
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

        public async Task<List<long?>> UpdateTransferPermissionsDocumentUserRoleUser(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            var query = "Update DocumentUserRole set UserID=" + userId + " WHERE  DocumentUserRoleID in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        return ids;
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


        public async Task<IReadOnlyList<EmailConversations>> GetTransferPermissionsEmailConversationParticipant(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "select t1.*,t3.TopicName,t4.Name as EmailConversationName,t2.UserName from EmailConversationParticipant t1\r\n" +
                    "JOIN ApplicationUser t2 ON t1.UserId=t2.UserID\r\n" +
                    "JOIN EmailTopics t3 ON t3.ID=t1.TopicId\r\nJOIN EmailConversations t4 ON t4.ID=t1.ConversationId\r\n\r\n " +
                    "WHERE  t1.UserID in(" + string.Join(',', userIds) + ")";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailConversations>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public EmailConversations CheckTransferPermissionsEmailConversationParticipant(EmailConversations emailConversations, long? ToUserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", emailConversations.TopicID);
                parameters.Add("ConversationId", emailConversations.ConversationId);
                parameters.Add("UserId", ToUserId);
                var query = "select ID,ConversationId,UserId,TopicId from EmailConversationParticipant where TopicId=@TopicId and ConversationId=@ConversationId And UserId=@UserId\r\n";

                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<EmailConversations>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<EmailConversations> UpdateTransferPermissionsEmailConversationParticipant(List<EmailConversations> EmailConversations, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        //List<long?> ids = new List<long?>();
                        var query = string.Empty;
                        if (EmailConversations != null && EmailConversations.Count > 0)
                        {
                            EmailConversations.ForEach(s =>
                            {
                                var result = CheckTransferPermissionsEmailConversationParticipant(s, userId);
                                if (result == null)
                                {
                                    query += "Update EmailConversationParticipant set UserID=" + userId + " WHERE  ID in(" + s.ID + ");\n\r";

                                    // ids.Add(s.ID);
                                }
                                else
                                {
                                    var result1 = CheckTransferPermissionsEmailConversationParticipant(s, s.UserId);
                                    if (result1 != null)
                                    {
                                        query += "DELETE FROM  EmailConversationParticipant WHERE ID = " + result1.ID + ";\n\r";
                                    }
                                }
                            });
                        }
                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query, null);
                        }
                        return EmailConversations.FirstOrDefault();
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

        public async Task InsertEmailTransferHistory(long fromUserId, long toUserId, long emailConversationId, long topicId, long addedByUserId)
        {
            var query = @"INSERT INTO EmailTransferHistory(FromUserID, ToUserID, EmailConversationID, TopicID, TransferDate, AddedByUserID, AddedDate)
                        VALUES(@FromUserID, @ToUserID, @EmailConversationID, @TopicID, GETDATE(), @AddedByUserID, GETDATE());";

            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(query, new
                {
                    FromUserID = fromUserId,
                    ToUserID = toUserId,
                    EmailConversationID = emailConversationId,
                    TopicID = topicId,
                    AddedByUserID = addedByUserId
                });
            }
        }
        public async Task<IReadOnlyList<DynamicFormWorkFlow>> GetTransferDynamicFormWorkFlow(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "select tt2.*\r\nfrom (select tt1.*,\r\nDynamicFormSectionName = STUFF(( SELECT ',' + CAST(a1.SectionName AS VARCHAR(MAX)) FROM DynamicFormWorkFlowSection md  JOIN DynamicFormSection a1 ON a1.DynamicFormSectionID=md.DynamicFormSectionID Where (a1.IsDeleted is null OR a1.IsDeleted=0) AND md.DynamicFormWorkFlowID=tt1.DynamicFormWorkFlowID  Order by md.DynamicFormWorkFlowSectionID asc FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')\r\nfrom (select t1.*,t2.UserName FirstName,t3.Name as DynamicFormName from DynamicFormWorkFlow t1 JOIN\r\nApplicationUser t2 ON t1.UserID=t2.UserID\r\nJOIN DynamicForm t3 ON t3.ID=t1.DynamicFormID " +
                    "where t1.UserID in(" + string.Join(',', userIds) + ") AND (t3.IsDeleted is null OR t3.IsDeleted=0))tt1)tt2\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlow>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long?>> UpdateTransferDynamicFormWorkFlow(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("UserGroupId", null);
                            parameters.Add("LevelId", null);
                            parameters.Add("Type", "User", DbType.String);
                            var query = "Update DynamicFormWorkFlow set Type=@Type,UserGroupId=@UserGroupId,LevelId=@LevelId,UserID=" + userId + " WHERE   DynamicFormWorkFlowId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        return ids;
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
        public async Task<IReadOnlyList<DynamicFormWorkFlowApproval>> GetTransferDynamicFormWorkFlowApproval(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "select tt1.*,\r\nDynamicFormSectionName = STUFF(( SELECT ',' + CAST(a1.SectionName AS VARCHAR(MAX)) FROM DynamicFormWorkFlowSection md  JOIN DynamicFormSection a1 ON a1.DynamicFormSectionID=md.DynamicFormSectionID Where (a1.IsDeleted is null OR a1.IsDeleted=0) AND md.DynamicFormWorkFlowID=tt1.DynamicFormWorkFlowID  Order by md.DynamicFormWorkFlowSectionID asc FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '')\r\nfrom (select t1.*,t2.SequenceNo,t3.UserName,t4.Name as DynamicFormName\r\nfrom DynamicFormWorkFlowApproval t1\r\nJOIN DynamicFormWorkFlow t2 ON t2.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID\r\nJOIN ApplicationUser t3 ON t1.UserID=t3.UserID\r\nJOIN  DynamicForm t4 ON t4.ID=t2.DynamicFormID\n\r" +
                     "where t1.UserID in(" + string.Join(',', userIds) + ") AND (t4.IsDeleted is null OR t4.IsDeleted=0))tt1";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowApproval>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<long?>> UpdateTransferDynamicFormWorkFlowApproval(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            var query = "Update DynamicFormWorkFlowApproval set UserID=" + userId + " WHERE   DynamicFormWorkFlowApprovalId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return ids;
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
        public async Task<IReadOnlyList<DynamicFormSectionSecurity>> GetTransferDynamicFormSectionSecurity(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "\r\nselect t1.*,t2.UserName as FirstName,t3.SectionName,t4.Name as FormName from DynamicFormSectionSecurity t1 \r\nJOIN ApplicationUser t2 ON t2.UserID=t1.UserID\r\nJOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID\r\nJOIN DynamicForm t4 ON t4.ID=t3.DynamicFormID " +
                    "WHERE t1.UserID in(" + string.Join(',', userIds) + ") AND (t3.IsDeleted is null OR t3.IsDeleted=0) AND (t4.IsDeleted is null OR t4.IsDeleted=0)";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionSecurity>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long?>> DeleteTransferDynamicFormSectionSecurity(List<long?> ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        var parameters = new DynamicParameters();

                        var query = "delete from DynamicFormSectionSecurity WHERE DynamicFormSectionSecurityId in(" + string.Join(',', ids) + ")";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return ids;
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
        public async Task<List<long?>> UpdateTransferDynamicFormSectionSecurity(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("UserGroupId", null);
                            parameters.Add("LevelId", null);
                            var query = "Update DynamicFormSectionSecurity set UserGroupId=@UserGroupId,LevelId=@LevelId,UserID=" + userId + " WHERE   DynamicFormSectionSecurityId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return ids;
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

        public async Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetTransferDynamicFormSectionAttributeSecurity(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "\r\nselect t1.*,t2.UserName as FirstName,t3.SectionName,t4.Name as FormName,t5.DisplayName from DynamicFormSectionAttributeSecurity t1 \r\nJOIN ApplicationUser t2 ON t2.UserID=t1.UserID\r\nJOIN DynamicFormSectionAttribute t5 ON t5.DynamicFormSectionAttributeID=t1.DynamicFormSectionAttributeID\r\nJOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t5.DynamicFormSectionID\r\nJOIN DynamicForm t4 ON t4.ID=t3.DynamicFormID " +
                    "WHERE t1.UserID in(" + string.Join(',', userIds) + ") AND (t3.IsDeleted is null OR t3.IsDeleted=0) AND (t4.IsDeleted is null OR t4.IsDeleted=0) AND (t5.IsDeleted is null OR t5.IsDeleted=0)";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttributeSecurity>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long?>> UpdateTransferDynamicFormSectionAttributeSecurity(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("UserGroupId", null);
                            parameters.Add("LevelId", null);
                            var query = "Update DynamicFormSectionAttributeSecurity set UserGroupId=@UserGroupId,LevelId=@LevelId,UserID=" + userId + " WHERE   DynamicFormSectionAttributeSecurityId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return ids;
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
        public async Task<List<long?>> DeleteTransferDynamicFormSectionAttributeSecurity(List<long?> ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        var parameters = new DynamicParameters();

                        var query = "delete from DynamicFormSectionAttributeSecurity WHERE DynamicFormSectionAttributeSecurityId in(" + string.Join(',', ids) + ")";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return ids;
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
        public async Task<IReadOnlyList<DynamicFormSectionAttributeSection>> GetTransferDynamicFormSectionAttributeSection(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "select t1.*,t3.DisplayName,t3.AttributeID,t5.DataSourceTable,t6.UserName,t7.SectionName as DynamicFormSectionSelectionBySectionName,t8.SectionName,t9.Name as FormName  from DynamicFormSectionAttributeSection t1\r\nJOIN DynamicFormSectionAttributeSectionParent t2 ON t1.DynamicFormSectionAttributeSectionParentID=t2.DynamicFormSectionAttributeSectionParentID\r\n" +
                    "JOIN DynamicFormSectionAttribute t3 ON t3.DynamicFormSectionAttributeID=t2.DynamicFormSectionAttributeID\r\nJOIN AttributeHeader t4 ON t4.AttributeID=t3.AttributeID\r\nJOIN AttributeHeaderDataSource t5 ON t5.HeaderDataSourceID=t4.DataSourceID \r\nJOIN ApplicationUser t6 ON t6.UserID=t1.DynamicFormSectionSelectionID\r\n" +
                    "JOIN DynamicFormSection t7 ON t7.DynamicFormSectionID=t1.DynamicFormSectionID\r\nJOIN DynamicFormSection t8 ON t8.DynamicFormSectionID=t3.DynamicFormSectionID\r\nJOIN DynamicForm t9 ON t9.ID=t8.DynamicFormID\r\n" +
                    "WHERE t5.DataSourceTable='Employee' AND (t4.IsDeleted is null OR t4.IsDeleted=1) \r\nAND (t3.IsDeleted is null OR t3.IsDeleted=1) AND (t7.IsDeleted is null OR t7.IsDeleted=1) \r\n" +
                    "AND (t8.IsDeleted is null OR t8.IsDeleted=1) AND (t9.IsDeleted is null OR t9.IsDeleted=1) AND t1.DynamicFormSectionSelectionID in(" + string.Join(',', userIds) + ");";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttributeSection>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long?>> UpdateDynamicFormSectionAttributeSection(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DynamicFormSectionSelectionById", "Employee_" + userId, DbType.String);
                            var query = "Update DynamicFormSectionAttributeSection set DynamicFormSectionSelectionById=@DynamicFormSectionSelectionById,DynamicFormSectionSelectionId=" + userId + " WHERE   DynamicFormSectionAttributeSectionId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return ids;
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
        public async Task<List<DynamicFormSectionAttributeSection>> DeleteDynamicFormSectionAttributeSection(List<DynamicFormSectionAttributeSection> ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        if (ids != null && ids.Count() > 0)
                        {
                            foreach (var item in ids)
                            {
                                if (item != null)
                                {
                                    var parameters = new DynamicParameters();
                                    parameters.Add("DynamicFormSectionAttributeSectionParentId", item.DynamicFormSectionAttributeSectionParentId);
                                    var query = "delete from DynamicFormSectionAttributeSection WHERE DynamicFormSectionAttributeSectionId in(" + string.Join(',', item.DynamicFormSectionAttributeSectionId) + ");";
                                    await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                    var query1 = "select DynamicFormSectionAttributeSectionId from DynamicFormSectionAttributeSection Where DynamicFormSectionAttributeSectionParentId=@DynamicFormSectionAttributeSectionParentId;";
                                    var result = (await connection.QueryAsync<DynamicFormSectionAttributeSection>(query1, parameters)).ToList();
                                    if (result != null && result.Count > 0)
                                    {

                                    }
                                    else
                                    {
                                        var querys = string.Empty;
                                        querys += "Delete from DynamicFormSectionAttributeSectionParent where DynamicFormSectionAttributeSectionParentId=" + item.DynamicFormSectionAttributeSectionParentId + ";\r\n";
                                        await connection.QuerySingleOrDefaultAsync<long>(querys, parameters);
                                    }
                                }
                            }

                        }
                        return new List<DynamicFormSectionAttributeSection>();
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


        public async Task<IReadOnlyList<DynamicFormApproval>> GetTransferDynamicFormApproval(long? userIds)
        {
            try
            {
                userIds = userIds != null ? userIds : -1;
                var query = "select t1.*,t2.UserName as ApprovalUser,t3.Name as FormName from DynamicFormApproval t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.ApprovalUserID\r\n" +
                    "JOIN DynamicForm t3 ON t3.ID=t1.DynamicFormID Where t1.ApprovalUserID in(" + string.Join(',', userIds) + ") AND  t3.IsDeleted is null OR t3.IsDeleted=0";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproval>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormApproval>> UpdateDynamicFormApprovalSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("SortOrderBy", SortOrderBy);
                query = "SELECT DynamicFormApprovalID,\r\nDynamicFormID,\r\nApprovalUserID,\r\nSortOrderBy,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nIsApproved,\r\nDescription,\r\nApprovedCountUsed FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId AND SortOrderBy>@SortOrderBy";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormApproval>> DeleteTransferDynamicFormApproval(List<DynamicFormApproval> ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        if (ids != null && ids.Count() > 0)
                        {
                            foreach (var item in ids)
                            {
                                var result = await UpdateDynamicFormApprovalSort(item.DynamicFormId, item.SortOrderBy);
                                var sortby = item.SortOrderBy;
                                var parameters = new DynamicParameters();
                                var query = "delete from DynamicFormApproval WHERE DynamicFormApprovalId in(" + string.Join(',', item.DynamicFormApprovalId) + ");";
                                if (result != null && result.Count() > 0)
                                {
                                    result.ForEach(s =>
                                    {
                                        query += "Update  DynamicFormApproval SET SortOrderBy=" + sortby + "  WHERE DynamicFormApprovalId =" + s.DynamicFormApprovalId + ";";
                                        sortby++;
                                    });
                                }
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            }
                        }
                        return ids;
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
        public async Task<List<long?>> UpdateTransferDynamicFormApproval(List<long?> ids, long? userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                        if (userId > 0)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("UserGroupId", null);
                            parameters.Add("LevelId", null);
                            var query = "Update DynamicFormApproval set ApprovalUserID=" + userId + " WHERE  DynamicFormApprovalId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return ids;
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
