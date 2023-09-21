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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                            var parameters = new DynamicParameters();

                            var query = "delete from UserGroupUser WHERE UserGroupUserId in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return ids;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                            if (userId > 0)
                            {
                                var parameters = new DynamicParameters();

                                var query = "Update UserGroupUser set UserID=" + userId + " WHERE  UserGroupUserId in(" + string.Join(',', ids) + ")";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            }
                            transaction.Commit();
                            return ids;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                            var parameters = new DynamicParameters();

                            var query = "delete from DocumentUserRole WHERE DocumentUserRoleID in(" + string.Join(',', ids) + ")";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return ids;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            ids = ids != null && ids.Count > 0 ? ids : new List<long?>() { -1 };
                            if (userId > 0)
                            {
                                var parameters = new DynamicParameters();
                                var query = "Update DocumentUserRole set UserID=" + userId + " WHERE  DocumentUserRoleID in(" + string.Join(',', ids) + ")";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            }
                            transaction.Commit();
                            return ids;
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            List<long?> ids = new List<long?>();
                            if (EmailConversations != null && EmailConversations.Count > 0)
                            {
                                EmailConversations.ForEach(s =>
                                {
                                    var result = CheckTransferPermissionsEmailConversationParticipant(s, userId);
                                    if (result == null)
                                    {
                                        ids.Add(s.ID);
                                    }
                                });
                            }
                            if (userId > 0 && ids.Count > 0)
                            {
                                var parameters = new DynamicParameters();
                                var query = "Update EmailConversationParticipant set UserID=" + userId + " WHERE  ID in(" + string.Join(',', ids) + ")";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            }
                            transaction.Commit();
                            return EmailConversations.FirstOrDefault();
                        }
                        catch (Exception exp)
                        {
                            transaction.Rollback();
                            throw new Exception(exp.Message, exp);
                        }
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
