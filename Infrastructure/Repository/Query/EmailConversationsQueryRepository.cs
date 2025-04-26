using Core.Entities;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Http;
using Application.Response;
using Core.Entities.Views;
using DevExpress.Data.Filtering.Helpers;
using Azure.Core;
using System.Threading;
using DevExpress.Xpo.DB.Helpers;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
using Core.EntityModels;
using Infrastructure.Data;
using Google.Cloud.Firestore;
using System.Data.SqlClient;

namespace Infrastructure.Repository.Query
{
    public class EmailConversationsQueryRepository : DbConnector, IEmailConversationsQueryRepository
    {
        private readonly IFileprofileQueryRepository _fileprofiletypeQueryRepository;


        public EmailConversationsQueryRepository(IConfiguration configuration, IFileprofileQueryRepository fileprofiletypeQueryRepository)
            : base(configuration)
        {
            _fileprofiletypeQueryRepository = fileprofiletypeQueryRepository ?? throw new ArgumentNullException(nameof(fileprofiletypeQueryRepository));
        }

        public async Task<long> Delete(EmailConversations forumConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                            parameters.Add("Message", forumConversations.Message, DbType.String);
                            parameters.Add("ID", forumConversations.ID, DbType.Int64);
                            parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                            parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);

                            var query = "DELETE  FROM EmailConversations WHERE ID = @ID";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);                           

                            return rowsAffected;
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
        public async Task<List<EmailConversations>> GetBySessionConversationList(string SessionId)
        {
            try
            {
                var query = @"SELECT * FROM EmailConversations  WHERE SessionId = @SessionId";

                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);

                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ViewEmployee>> GetAddConversationPListAsync(long ConversationId)
        {
            try
            {
                var query = @"select  * from View_Employee where (StatusName!='Resign' or StatusName is null) and UserID NOT IN (SELECT AU.UserID FROM EmailConversationParticipant TP INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID WHERE TP.ConversationId = @ConversationId)";
                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ViewEmployee>> GetAllParticipantAsync(long topicId)
        {
            try
            {
                var query = @"select  * from View_Employee where (StatusName!='Resign' or StatusName is null) and UserID NOT IN (SELECT AU.UserID FROM EmailConversationParticipant TP INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID WHERE TP.TopicId = @TopicId)";
                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).Distinct().ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
         public async Task<List<UserNotification>> GetUserTokenListAsync(long userId)
        {
            try
            {
                var query = @"select * from UserNotifications where UserId = @userId And DeviceType !='IPIR'";

                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);
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
        public async Task<List<ViewEmployee>> GetAllConvAssignToListAsync(long convId)
        {
            try
            {
                var query = @"select E.* from EmailConversationAssignTo ECAT
                            INNER JOIN Employee E ON E.UserID = ECAT.UserId
                            where ECAT.ConversationId = @convId
                                            UNION
                                            select E.* from EmailConversationAssignCC ECAC 
                            INNER JOIN Employee E ON E.UserID = ECAC.UserId
                            where ECAC.ConversationId = @convId";

                var parameters = new DynamicParameters();
                parameters.Add("convId", convId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<ViewEmployee>> GetConvPListAsync(long ConversationId)
        {
            try
            {
                var query = @"SELECT FT.UserId as UserID,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName FROM EmailConversationParticipant FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
								INNER JOIN Plant p on p.PlantID = E.PlantID
								INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                                WHERE FT.ConversationId = @ConversationId";

                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long>> GetConvPListUserGroupAsync(long ConversationId)
        {
            try
            {
                var query = @"SELECT FT.GroupId FROM EmailConversationParticipantUserGroup FT
                                WHERE FT.ConversationId = @ConversationId";

                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<long>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<ViewEmployee>> GetAllConvTopicPListAsync(long ConversationId, long topicId)
        {
            try
            {
                var query = @"SELECT distinct FT.UserId, concat(E.FirstName,',',E.LastName) as Name,E.FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName FROM EmailConversationParticipant FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
								LEFT JOIN Plant p on p.PlantID = E.PlantID
								LEFT JOIN Designation D ON D.DesignationID = E.DesignationID
                                WHERE FT.ConversationId = @ConversationId";

                //                        UNION

                //SELECT FCT.UserId,E.FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName from EmailConversationAssignTo FCT
                //                        INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                //                        INNER JOIN Employee E ON E.UserID = FCT.UserId
                //INNER JOIN Plant p on p.PlantID = E.PlantID
                //INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                //                        WHERE FCT.ConversationId = @ConversationId

                //                        UNION

                //SELECT FCT.UserId,E.FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName from EmailConversationAssignCC FCT
                //                        INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                //                        INNER JOIN Employee E ON E.UserID = FCT.UserId
                //INNER JOIN Plant p on p.PlantID = E.PlantID
                //INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                //                        WHERE FCT.ConversationId = @ConversationId

                //                        UNION
                //                        SELECT FT.ParticipantId as UserId,E.FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName FROM EmailConversations FT
                //                        INNER JOIN ApplicationUser AU ON AU.UserID = FT.ParticipantId
                //                        INNER JOIN Employee E ON E.UserID = FT.ParticipantId
                //                        INNER JOIN Plant p on p.PlantID = E.PlantID
                //                        INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                //                        WHERE FT.ID = @ConversationId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                parameters.Add("ConversationId", ConversationId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<ViewEmployee>> GetAllConvTPListAsync(long topicId)
        {
            try
            {
                var query = @"SELECT concat(E.FirstName,',',E.LastName) as Name, FT.UserId,E.FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName FROM EmailConversationParticipant FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
								INNER JOIN Plant p on p.PlantID = E.PlantID
								INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                                WHERE FT.ConversationId = (SELECT TOP 1 ID FROM EmailConversations WHERE TopicID = @TopicID AND ReplyId = 0)";

                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<ViewEmployee>> GetAllPListAsync(long topicId)
        {
            try
            {
                var query = @"SELECT FT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName FROM EmailConversationParticipant FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
								INNER JOIN Plant p on p.PlantID = E.PlantID
								INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                                WHERE FT.TopicId = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailAssignToList>> GetAllAssignToListAsync(long topicId)
        {
            try
            {
                var query = @"SELECT FT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName,E.NickName,D.Code AS DesignationName,P.PlantCode as CompanyName FROM EmailConversationParticipant FT
                               
                                INNER JOIN Employee E ON E.UserID = FT.UserId
								INNER JOIN Plant p on p.PlantID = E.PlantID
								INNER JOIN Designation D ON D.DesignationID = E.DesignationID
                                WHERE FT.TopicId = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailAssignToList>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailTopicTo>> GetTopicToListAsync(long topicId)
        {
            try
            {
                var query = @"SELECT FT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName FROM EmailConversationAssignTo FT                               
                                INNER JOIN Employee E ON E.UserID = FT.UserId
                                WHERE FT.TopicId = @TopicId";
                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailTopicTo>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public long DeleteParticipant(TopicParticipant topicParticipant)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ID", topicParticipant.ID, DbType.Int64);
                        parameters.Add("Option", "DELETE");                       
                        
                        var task = connection.ExecuteAsync("sp_Ins_EmailTopicParticipant", parameters, commandType: CommandType.StoredProcedure);
                        task.Wait(); // Synchronously wait for the task to complete
                        var rowsAffected = task.Result; // Retrieve the result


                        return rowsAffected;
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

        public async Task<IReadOnlyList<EmailConversations>> GetAllAsync()
        {
            try
            {
                var query1 = "SELECT * FROM EmailConversations";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailConversations>(query1)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<List<EmailConversations>> GetFullDiscussionListAsync(long TopicId)
        {
            try
            {

                var query = @"select * from EmailConversations FC                                 
                                where FC.TopicID = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                using (var connection = CreateConnection())
                {                    
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>> GetValidUserListAsync(long TopicId, long UserId)
        {
            try
            {
                var query = @"SELECT DISTINCT AddedByUserID from EmailConversations where TopicID = @TopicId and AddedByUserID =@UserId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {  
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> GetDemoUpdateEmailFileDataListAsync(long id, byte[] fileData)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id, DbType.Int64);
                            parameters.Add("fileData", fileData);

                            var query = " UPDATE EmailConversations SET FileData = @fileData  WHERE ID = @id";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters);

                            

                            return rowsAffected;
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
        public async Task<List<EmailConversations>> GetDemoEmailFileDataListAsync()
        {
            try
            {
                var query = @"SELECT * from EmailConversations where IsMobile is null";
                var parameters = new DynamicParameters();

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversations>> GetEmailPrintAllDiscussionListAsync(long TopicId, long UserId, string Option)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);
                parameters.Add("UserId", UserId, DbType.Int64);
                parameters.Add("Option", Option);
                var res = new List<EmailConversations>();

                using (var connection = CreateConnection())
                {
                    res = (await connection.QueryAsync<EmailConversations>("sp_Select_GetEmailDiscussionList", parameters, commandType: CommandType.StoredProcedure)).ToList();

                }
                if (res != null && res.Count > 0)
                {
                    var replyIds = res.Select(s => s.ID).Distinct().ToList();
                    var sessionids = res.Select(c => c.SessionId).Distinct().ToList();

                    await Task.WhenAll(res.Select(async topic =>
                    {
                        topic.ReplyConversation = await GetReplyList(new List<int> { topic.ID }, UserId);
                        topic.documents = await GetDocumentList(new List<Guid?> { topic.SessionId });


                        if (string.IsNullOrEmpty(topic.UserType) || topic.UserType == "Users")
                        {
                            topic.AssignToList = await GetAssignToList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCList(new List<int> { topic.ID });
                        }
                        else
                        {
                            //topic.AssignToList = await GetAssignToUserGroupList(new List<int> { topic.ID });
                            //topic.AssignCCList = await GetAssignCCUserGroupList(new List<int> { topic.ID });
                        }


                        await Task.WhenAll(topic.ReplyConversation.Select(async conversation =>
                        {
                            conversation.documents = await GetDocumentList(new List<Guid?> { conversation.SessionId });
                            if (string.IsNullOrEmpty(conversation.UserType) || conversation.UserType == "Users")
                            {
                                conversation.AssignToList = await GetAssignToList(new List<int> { conversation.ID });
                                conversation.AssignCCList = await GetAssignCCList(new List<int> { conversation.ID });
                            }
                            else
                            {
                                conversation.AssignToList = await GetAssignToUserGroupList(new List<int> { conversation.ID });
                                conversation.AssignCCList = await GetAssignCCUserGroupList(new List<int> { conversation.ID });
                            }


                        }));
                    }));
                }
                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> GetTotalNotificationCountAsyncold(long UserId)
        {
            try
            {
                var query = @"
            ;WITH UniqueTopics AS (
                SELECT 
                    COALESCE(FS.NotificationCount, 0) AS NotificationCount,
                    ROW_NUMBER() OVER (PARTITION BY TS.ID ORDER BY TS.ID DESC) AS RowNum
                FROM EmailTopics TS
                INNER JOIN EmailConversationParticipant ECPS 
                    ON ECPS.TopicId = TS.ID 
                    AND ECPS.UserId = @UserId 
                    AND (ECPS.IsArchive = 0 OR ECPS.IsArchive IS NULL)
                INNER JOIN EmailConversations ECS ON ECS.TopicId = TS.ID                            
                INNER JOIN EmailConversationAssignTo TP ON ECS.ID = TP.ConversationId
                INNER JOIN Employee E ON TS.TopicFrom = E.UserId
                CROSS APPLY (
                    SELECT SUM(FN.NotificationCount) AS NotificationCount 
                    FROM EmailConversations EC
                    INNER JOIN EmailConversationParticipant ECP 
                        ON ECP.ConversationId = EC.ID 
                        AND ECP.UserId = @UserId
                    CROSS APPLY (
                        SELECT DISTINCT ReplyId = 
                            CASE WHEN ECC.ReplyId > 0 THEN ECC.ReplyId ELSE ECC.ID END
                        FROM EmailConversations ECC 
                        LEFT JOIN EmailConversationAssignTo AST 
                            ON AST.ConversationId = ECC.ID 
                            AND (AST.UserId = @UserId OR AST.AddedByUserID = @UserId)
                        LEFT JOIN EmailConversationAssignCC ECA 
                            ON ECA.ConversationId = ECC.ID 
                            AND (ECA.UserId = @UserId OR ECA.AddedByUserID = @UserId)
                        WHERE ECC.TopicID = TS.ID 
                    ) K
                    CROSS APPLY (
                        SELECT COUNT(ECCS.ID) AS NotificationCount
                        FROM EmailConversations ECCS
                        INNER JOIN EmailNotifications EN 
                            ON ECCS.ID = EN.ConversationId 
                            AND EN.IsRead = 0
                        WHERE EN.TopicId = EC.TopicID 
                            AND EN.UserId = ECP.UserId 
                            AND (EC.ID = ECCS.ReplyId OR EC.ID = ECCS.ID)
                    ) FN 
                    WHERE EC.ID = K.ReplyId 
                ) FS 
                WHERE
                    TP.UserId = @UserId 
                    AND TS.OnDraft = 0 
            )
            SELECT COALESCE(SUM(NotificationCount), 0) AS TotalNotificationCount
            FROM UniqueTopics
            WHERE RowNum = 1;";

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteScalarAsync<long>(query, new { UserId });
                }
            }
            catch (Exception)
            {
                // Log the exception if needed, but return 0 on failure
                return 0;
            }
        }

        public async Task<long> GetTotalNotificationCountAsync(long UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId, DbType.Int64);

                using (var connection = CreateConnection()) 
                {
                    return await connection.ExecuteScalarAsync<long>(
                        "sp_Select_Email_NotificationCount",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
            catch (Exception)
            {
                // Log the exception if needed, but return 0 on failure
                return 0;
            }
        }

        public async Task<List<EmailConversations>> GetDiscussionListAsync(long TopicId, long UserId, string Option)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);
                parameters.Add("UserId", UserId, DbType.Int64);
                parameters.Add("Option", Option);
                var res = new List<EmailConversations>();

                using (var connection = CreateConnection())
                {
                    res = (await connection.QueryAsync<EmailConversations>("sp_Select_GetEmailDiscussionList", parameters, commandType: CommandType.StoredProcedure)).ToList();                    

                }
                if (res != null && res.Count > 0)
                {
                    var replyIds = res.Select(s => s.ID).Distinct().ToList();
                    var sessionids = res.Select(c => c.SessionId).Distinct().ToList();

                    await Task.WhenAll(res.Select(async topic =>
                    {
                        var replyCount = await GetReplyCount(new List<int> { topic.ID }, UserId);
                        topic.ReplyConversationCount = replyCount;
                        //topic.ReplyConversation = await GetReplyList(new List<int> { topic.ID }, UserId);
                        //topic.documents = await GetDocumentList(new List<Guid?> { topic.SessionId });
                        topic.documents = await GetCheckPermissionDocumentList(new List<Guid?> { topic.SessionId }, UserId);

                        if (string.IsNullOrEmpty(topic.UserType) || topic.UserType == "Users")
                        {
                            topic.AssignToList = await GetAssignToList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCList(new List<int> { topic.ID });
                        }
                        else
                        {
                            //topic.AssignToList = await GetAssignToUserGroupList(new List<int> { topic.ID });
                            //topic.AssignCCList = await GetAssignCCUserGroupList(new List<int> { topic.ID });
                        }
                       

                        //await Task.WhenAll(topic.ReplyConversation.Select(async conversation =>
                        //{
                        //    conversation.documents = await GetDocumentList(new List<Guid?> { conversation.SessionId });
                        //    if (string.IsNullOrEmpty(conversation.UserType) || conversation.UserType == "Users")
                        //    {
                        //        conversation.AssignToList = await GetAssignToList(new List<int> { conversation.ID });
                        //        conversation.AssignCCList = await GetAssignCCList(new List<int> { conversation.ID });
                        //    }
                        //    else
                        //    {
                        //        conversation.AssignToList = await GetAssignToUserGroupList(new List<int> { conversation.ID });
                        //        conversation.AssignCCList = await GetAssignCCUserGroupList(new List<int> { conversation.ID });
                        //    }

                            
                        //}));
                    }));                  
                }
                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversations>> GetDiscussionListAsync_vijay(long TopicId, long UserId,string Option)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("TopicId", TopicId, DbType.Int64);
                        parameters.Add("UserId", UserId, DbType.Int64);
                        parameters.Add("Option", Option);
                                             
                        var res = await connection.QueryAsync<EmailConversations>("sp_Select_GetEmailDiscussionList", parameters, commandType: CommandType.StoredProcedure);
                        foreach (var topic in res)
                        {

                            var subQuery = @"SELECT
                                        FC.Name,
                                        FC.ID,
                                        FC.AddedDate,
                                        FC.Message,
                                        AU.UserName,
                                        AU.UserID,
                                        FC.ReplyId,
                                        FC.SessionId,FC.FileData,
                                        EN.IsRead,
										EN.ID as EmailNotificationId,
                                        ISNULL(FC.IsMobile, 0) AS IsMobile
                                    FROM
                                        EmailConversations FC                                       
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                        LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
                                    WHERE
                                        FC.TopicId = @TopicId  AND FC.ReplyId = @ReplyId
                                    ORDER BY FC.AddedDate DESC";
                            
                            var parameterss = new DynamicParameters();
                            parameterss.Add("TopicId", TopicId, DbType.Int64);
                            parameterss.Add("UserId", UserId, DbType.Int64);
                            parameterss.Add("ReplyId", topic.ID, DbType.Int64);
                            var subQueryResults = await connection.QueryAsync<EmailConversations>(subQuery, parameterss);

                            var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";

                            var parametersDocs = new DynamicParameters();
                            parametersDocs.Add("SessionID", topic.SessionId);

                            var subQueryDocsResults = await connection.QueryAsync<EmailDocumentModel>(subQueryDocs, parametersDocs);

                            var subQueryassignTo = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignTo FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                            var parametersassignTo = new DynamicParameters();
                            parametersassignTo.Add("Id", topic.ID, DbType.Int64);
                            var subQueryAssignToResults = await connection.QueryAsync<EmailAssignToList>(subQueryassignTo, parametersassignTo);


                            var subQueryassignCC = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignCC FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                            var parametersassignCC = new DynamicParameters();
                            parametersassignCC.Add("Id", topic.ID, DbType.Int64);
                            var subQueryAssignCCResults = await connection.QueryAsync<EmailAssignToList>(subQueryassignCC, parametersassignCC);



                            topic.ReplyConversation = subQueryResults.ToList();
                            topic.documents = subQueryDocsResults.ToList();
                            topic.AssignToList = subQueryAssignToResults.ToList();
                            topic.AssignCCList = subQueryAssignCCResults.ToList();

                            foreach (var conversation in topic.ReplyConversation)
                            {
                                var subQueryReplyDocs = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                        INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                        WHERE D.SessionID = @SessionID";
                                var parametersReplyDocs = new DynamicParameters();
                                parametersReplyDocs.Add("SessionID", conversation.SessionId);
                                parametersReplyDocs.Add("ReplyId", conversation.ReplyId, DbType.Int64);

                                var subQueryReplyDocsResults = await connection.QueryAsync<EmailDocumentModel>(subQueryReplyDocs, parametersReplyDocs);

                                var replysubQueryassignTo = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                                var replyparametersassignTo = new DynamicParameters();
                                replyparametersassignTo.Add("ConversationId", conversation.ID, DbType.Int64);
                                var replysubQueryAssignToResults = await connection.QueryAsync<EmailAssignToList>(replysubQueryassignTo, replyparametersassignTo);

                                var replysubQueryassignCC = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignCC FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                                var replyparametersassignCC = new DynamicParameters();
                                replyparametersassignCC.Add("ConversationId", conversation.ID, DbType.Int64);
                                var replysubQueryAssignCCResults = await connection.QueryAsync<EmailAssignToList>(replysubQueryassignCC, replyparametersassignCC);



                                //var replyBysubQuery = @"SELECT
                                //        FC.Name,
                                //        FC.ID,
                                //        FC.AddedDate,
                                //        FC.Message,
                                //        AU.UserName,
                                //        AU.UserID,
                                //        FC.ReplyId,
                                //        FC.SessionId,FC.FileData
                                //    FROM
                                //        EmailConversations FC
                                //        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                //    WHERE
                                //        FC.TopicId = @TopicId AND FC.ReplyId = @ReplyId";

                                //var reparameterss = new DynamicParameters();
                                //reparameterss.Add("TopicId", TopicId, DbType.Int64);
                                //reparameterss.Add("ReplyId", conversation.ID, DbType.Int64);
                                //var replyBysubQueryResults = connection.Query<EmailConversations>(replyBysubQuery, reparameterss).ToList();


                                conversation.documents = subQueryReplyDocsResults.ToList();
                                conversation.AssignToList = replysubQueryAssignToResults.ToList();
                                conversation.AssignCCList = replysubQueryAssignCCResults.ToList();
                                //conversation.ReplyConversation = replyBysubQueryResults;
                            }

                        }

                        return res.ToList();

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
        public async Task<List<EmailConversations>> GetDiscussionListAsync_old(long TopicId,long UserId)
        {
            try
            {     
                var query = @"SELECT * FROM (
                            SELECT FC.TopicID, FC.ReplyId, FC.Name, FC.ID, FC.SessionId, FC.AddedDate, FC.Message, AU.UserName, AU.UserID, FC.FileData,
                                AET.Comment AS ActCommentName,AET.BackURL, EMPP.FirstName AS ActUserName, AET.AddedDate AS ActAddedDate,FC.DueDate,FC.IsAllowParticipants,
                                ONB.FirstName AS OnBehalfName,FC.Follow,FC.Urgent,FC.NotifyUser,AET.ActivityType
                            FROM EmailConversations FC
                            LEFT JOIN Employee ONB ON ONB.UserID = FC.OnBehalf
                            LEFT JOIN ActivityEmailTopics AET ON AET.EmailTopicSessionId = FC.SessionId
                            INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = FC.ID AND ECP.UserId = @UserId
                            CROSS APPLY (
                                SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId > 0 THEN ECC.ReplyId ELSE ECC.ID END
                                FROM EmailConversations ECC
                                WHERE (
                                        ECC.ParticipantId = @UserId
                                        OR EXISTS (SELECT * FROM EmailConversationAssignTo AST WHERE AST.ConversationId = ECC.ID AND AST.UserId = @UserId)
                                        OR EXISTS (SELECT * FROM EmailConversationAssignCC AST WHERE AST.ConversationId = ECC.ID AND AST.UserId = @UserId)
                                        OR EXISTS (SELECT * FROM EmailConversationParticipant ECP WHERE ECP.ConversationId = ECC.ID AND ECP.UserId = @UserId)
                                    ) AND ECC.TopicID = @TopicId
                            ) K
                            INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                            INNER JOIN Employee EMP ON EMP.UserID = AU.UserID
                            LEFT JOIN Employee EMPP ON EMPP.UserID = AET.AddedByUserID
                            WHERE K.ReplyId = FC.ID AND FC.ReplyId = 0   
                        ) AS CombinedResult
                        ORDER BY CombinedResult.AddedDate DESC";


                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);
                parameters.Add("UserId", UserId, DbType.Int64);

                using (var connection = CreateConnection())
                {                    
                    var res = connection.Query<EmailConversations>(query,parameters).ToList();

                    foreach (var topic in res)
                    {

                        var subQuery = @"SELECT
                                        FC.Name,
                                        FC.ID,
                                        FC.AddedDate,
                                        FC.Message,
                                        AU.UserName,
                                        AU.UserID,
                                        FC.ReplyId,
                                        FC.SessionId,FC.FileData,
                                        EN.IsRead,
										EN.ID as EmailNotificationId,
                                        ISNULL(FC.IsMobile, 0) AS IsMobile
                                    FROM
                                        EmailConversations FC                                       
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                        LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
                                    WHERE
                                        FC.TopicId = @TopicId  AND FC.ReplyId = @ReplyId
                                    ORDER BY FC.AddedDate DESC";
										//AND (FC.ParticipantId = @UserId
										//OR EXISTS(SELECT * FROM EmailConversationAssignTo AST WHERE AST.ConversationId = FC.ID AND AST.UserId = @UserId)
										//OR EXISTS(SELECT * FROM EmailConversationAssignCC AST WHERE AST.ConversationId = FC.ID AND AST.UserId = @UserId))";

                            var parameterss = new DynamicParameters();
                            parameterss.Add("TopicId", TopicId, DbType.Int64);
                            parameterss.Add("UserId", UserId, DbType.Int64);
                            parameterss.Add("ReplyId", topic.ID, DbType.Int64);
                        var subQueryResults = connection.Query<EmailConversations>(subQuery, parameterss).ToList();                     


                        var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";
                                   
                        var parametersDocs = new DynamicParameters();
                        parametersDocs.Add("SessionID", topic.SessionId);                                       

                        var subQueryDocsResults = connection.Query<EmailDocumentModel>(subQueryDocs, parametersDocs).ToList();

                        var subQueryassignTo = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignTo FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                        var parametersassignTo = new DynamicParameters();
                        parametersassignTo.Add("Id", topic.ID, DbType.Int64);
                        var subQueryAssignToResults = connection.Query<EmailAssignToList>(subQueryassignTo, parametersassignTo).ToList();


                        var subQueryassignCC = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignCC FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                        var parametersassignCC = new DynamicParameters();
                        parametersassignCC.Add("Id", topic.ID, DbType.Int64);
                        var subQueryAssignCCResults = connection.Query<EmailAssignToList>(subQueryassignCC, parametersassignCC).ToList();



                        topic.ReplyConversation = subQueryResults;
                        topic.documents = subQueryDocsResults;
                        topic.AssignToList = subQueryAssignToResults;
                        topic.AssignCCList = subQueryAssignCCResults;

                        foreach (var conversation in topic.ReplyConversation)
                        {
                            var subQueryReplyDocs = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                        INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                        WHERE D.SessionID = @SessionID";
                            var parametersReplyDocs = new DynamicParameters();
                            parametersReplyDocs.Add("SessionID", conversation.SessionId);
                            parametersReplyDocs.Add("ReplyId", conversation.ReplyId, DbType.Int64);                          

                            var subQueryReplyDocsResults = connection.Query<EmailDocumentModel>(subQueryReplyDocs, parametersReplyDocs).ToList();

                            var replysubQueryassignTo = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignTo = new DynamicParameters();
                            replyparametersassignTo.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignToResults = connection.Query<EmailAssignToList>(replysubQueryassignTo, replyparametersassignTo).ToList();

                            var replysubQueryassignCC = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignCC FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignCC = new DynamicParameters();
                            replyparametersassignCC.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignCCResults = connection.Query<EmailAssignToList>(replysubQueryassignCC, replyparametersassignCC).ToList();



                            var replyBysubQuery = @"SELECT
                                        FC.Name,
                                        FC.ID,
                                        FC.AddedDate,
                                        FC.Message,
                                        AU.UserName,
                                        AU.UserID,
                                        FC.ReplyId,
                                        FC.SessionId,FC.FileData
                                    FROM
                                        EmailConversations FC
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                    WHERE
                                        FC.TopicId = @TopicId AND FC.ReplyId = @ReplyId";

                            var reparameterss = new DynamicParameters();
                            reparameterss.Add("TopicId", TopicId, DbType.Int64);
                            reparameterss.Add("ReplyId", conversation.ID, DbType.Int64);
                            var replyBysubQueryResults = connection.Query<EmailConversations>(replyBysubQuery, reparameterss).ToList();


                            conversation.documents = subQueryReplyDocsResults;
                            conversation.AssignToList = replysubQueryAssignToResults;
                            conversation.AssignCCList = replysubQueryAssignCCResults;

                            conversation.ReplyConversation = replyBysubQueryResults;


                            foreach (var conversations in conversation.ReplyConversation)
                            {


                                var subQueryReplyDocs1 = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                        INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                        WHERE D.SessionID = @SessionID";
                                var parametersReplyDocs1 = new DynamicParameters();
                                parametersReplyDocs1.Add("SessionID", conversations.SessionId);
                                parametersReplyDocs1.Add("ReplyId", conversations.ReplyId, DbType.Int64);

                                var subQueryReplyDocsResultss = connection.Query<EmailDocumentModel>(subQueryReplyDocs1, parametersReplyDocs1).ToList();

                                var replysubQueryassignTo1 = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                                var replyparametersassignTo1 = new DynamicParameters();
                                replyparametersassignTo1.Add("ConversationId", conversations.ID, DbType.Int64);
                                var replysubQueryAssignToResultss = connection.Query<EmailAssignToList>(replysubQueryassignTo1, replyparametersassignTo1).ToList();


                                var replysubQueryassignCC1 = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignCC FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                                var replyparametersassignCC1 = new DynamicParameters();
                                replyparametersassignCC1.Add("ConversationId", conversations.ID, DbType.Int64);
                                var replysubQueryAssignCCResultss = connection.Query<EmailAssignToList>(replysubQueryassignCC1, replyparametersassignCC1).ToList();


                                conversations.documents = subQueryReplyDocsResultss;
                                conversations.AssignToList = replysubQueryAssignToResultss;
                                conversations.AssignCCList = replysubQueryAssignCCResultss;

                            }

                        }

                    }

                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>> GetConversationListAsync(long Id)
        {
            try
            {

                var query = @"SELECT * FROM EmailConversations FC WHERE FC.ID = @ID";

                var parameters = new DynamicParameters();
                parameters.Add("ID", Id, DbType.Int64);

                using (var connection = CreateConnection())
                {                    
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversations>> GetTopConversationListAsync(long TopicId)
        {
            try
            {

                var query = @"SELECT TOP 1 * FROM EmailConversations
                            WHERE ReplyId = 0 AND TopicID = @TopicId
                            ORDER BY ID ASC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                using (var connection = CreateConnection())
                {                    
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>>  GetReplyQuery1ListAsync(long? TopicId, long? UserId)
        {
            try
            {
                var query = @"SELECT DISTINCT FC.ID,FC.Name,FC.TopicID,FC.SessionId,FC.AddedDate,FC.Message,CONCAT(AU.FirstName,'-',AU.NickName) as UserName,AU.UserID,
                                FC.ReplyId,FC.FileData,FC.AddedByUserID,AETN.Name AS DynamicFormName,AET.Comment AS ActCommentName,AET.BackURL,
                                AET.DocumentSessionId,EMPP.FirstName AS ActUserName,FC.DueDate,FC.IsAllowParticipants,ONB.FirstName AS OnBehalfName,FC.Follow,FC.Urgent,FC.OnBehalf,
                                FC.NotifyUser,FCEP.FirstName,FCEP.LastName,AET.ActivityType,EN.IsRead,EN.ID AS EmailNotificationId,FC.NoOfDays,FC.ExpiryDueDate,DYSN.SectionName AS DynamicFormEmailSectionName,
                                FC.IsLockDueDate,CEL.EmailConversationsId AS CopyLinkEmailIds
                            FROM
                                EmailConversations FC
                            INNER JOIN EmailConversationParticipant ECPP ON ECPP.ConversationId = FC.ID AND ECPP.UserId = @UserId
                            LEFT JOIN Employee ONB ON ONB.UserID = FC.OnBehalf
                            LEFT JOIN ActivityEmailTopics AET ON AET.EmailTopicSessionId = FC.SessionId
                            INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
                            INNER JOIN Employee EMP ON EMP.UserID = AU.UserID
                            LEFT JOIN Employee EMPP ON EMPP.UserID = AET.AddedByUserID
                            LEFT JOIN Employee FCEP ON FCEP.UserID = FC.AddedByUserID
                            OUTER APPLY(SELECT DISTINCT  EmailConversationsId FROM EmailCopyLink WHERE EmailConversationsId = FC.ID)CEL
                            OUTER APPLY(select DFS.SectionName from DynamicFormSection DFS
                                        INNER JOIN EmailDynamicFormSection EDFS ON EDFS.FormSectionSessionID = DFS.SessionID AND  EDFS.FormSectionSessionID = FC.DynamicFormDataUploadSessionID AND EDFS.EmailSessionID = FC.SessionID)DYSN
                            OUTER APPLY(select TOP 1 df.Name from DynamicFormData dfd
						    inner join DynamicForm df on df.ID = dfd.DynamicFormID where dfd.SessionID =  AET.SessionId)AETN
                            LEFT JOIN EmailNotifications EN ON FC.ID=EN.ConversationId AND EN.UserId = @UserId
                            WHERE FC.ID = @TopicId AND FC.ReplyId = 0 AND ((AET.ActivityEmailTopicID IS NOT NULL AND AET.ActivityType != 'EmailFileProfileType') OR AET.ActivityEmailTopicID IS NULL)
                            ORDER BY FC.AddedDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);
                parameters.Add("UserId", UserId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailConversations>(query, parameters)).ToList();

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>> GetReplyQuery2ListAsync(long? TopicId,long? UserId)
        {
            try
            {
                var query = @"SELECT DISTINCT
                                FC.ID,FC.Name,FC.TopicID,FC.SessionId,FC.AddedDate,FC.Message,                               
                                FC.ReplyId,FC.FileData,FC.AddedByUserID,FC.DueDate,FC.IsAllowParticipants,                                
                                FC.Follow,FC.Urgent,FC.OnBehalf,FC.NotifyUser,
								CONCAT(AU.FirstName,'-',AU.NickName) as UserName,AU.UserID,ONB.FirstName AS OnBehalfName,FCEP.FirstName,FCEP.LastName,
                                EN.IsRead,EN.ID AS EmailNotificationId,FC.UserType,FC.NoOfDays,FC.ExpiryDueDate,
                                DYSN.SectionName AS DynamicFormEmailSectionName,FC.IsLockDueDate,CEL.EmailConversationsId AS CopyLinkEmailIds

                            FROM
                            EmailConversations FC
                            INNER JOIN EmailConversationParticipant ECPP ON ECPP.ConversationId = FC.ID AND ECPP.UserId = @UserId
                            LEFT JOIN Employee ONB ON ONB.UserID = FC.OnBehalf                            
                            INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
                            INNER JOIN Employee EMP ON EMP.UserID = AU.UserID       
                            OUTER APPLY(SELECT DISTINCT  EmailConversationsId FROM EmailCopyLink WHERE EmailConversationsId = FC.ID)CEL
                            OUTER APPLY(select DFS.SectionName from DynamicFormSection DFS
                                        INNER JOIN EmailDynamicFormSection EDFS ON EDFS.FormSectionSessionID = DFS.SessionID AND  EDFS.FormSectionSessionID = FC.DynamicFormDataUploadSessionID AND EDFS.EmailSessionID = FC.SessionID)DYSN
                            LEFT JOIN Employee FCEP ON FCEP.UserID = FC.AddedByUserID
                            LEFT JOIN EmailNotifications EN ON FC.ID=EN.ConversationId AND EN.UserId = @UserId
                            WHERE FC.ID = @TopicId AND FC.ReplyId = 0   
                            ORDER BY FC.AddedDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);
                parameters.Add("UserId", UserId, DbType.Int64);

                var res = new List<EmailConversations>();

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailConversations>(query, parameters)).ToList();

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<List<EmailConversations>> GetPrintReplyDiscussionListAsync(long TopicId, long UserId)
        {
            try
            {

                var Exists = await GetActivityEmailExitsAsync(TopicId);

                var res = new List<EmailConversations>();

                if (Exists > 0)
                {
                    res = await GetReplyQuery1ListAsync(TopicId, UserId);
                }
                else
                {
                    res = await GetReplyQuery2ListAsync(TopicId, UserId);
                }

                if (res != null && res.Count > 0)
                {
                    var replyIds = res.Select(s => s.ID).Distinct().ToList();
                    var sessionids = res.Select(c => c.SessionId).Distinct().ToList();

                    await Task.WhenAll(res.Select(async topic =>
                    {                       
                        topic.ReplyConversationCount = await GetReplyCount(new List<int> { topic.ID }, UserId);
                        topic.ReplyConversation = await GetReplyList(new List<int> { topic.ID }, UserId);
                        topic.documents = await GetDocumentList(new List<Guid?> { topic.SessionId });

                        if (string.IsNullOrEmpty(topic.UserType) || topic.UserType == "Users")
                        {
                            topic.AssignToList = await GetAssignToList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCList(new List<int> { topic.ID });
                        }
                        else
                        {
                            topic.AssignToList = await GetAssignToUserGroupList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCUserGroupList(new List<int> { topic.ID });
                        }


                        await Task.WhenAll(topic.ReplyConversation.Select(async conversation =>
                        {
                            conversation.documents = await GetDocumentList(new List<Guid?> { conversation.SessionId });
                            if (string.IsNullOrEmpty(conversation.UserType) || conversation.UserType == "Users")
                            {
                                conversation.AssignToList = await GetAssignToList(new List<int> { conversation.ID });
                                conversation.AssignCCList = await GetAssignCCList(new List<int> { conversation.ID });
                            }
                            else
                            {
                                conversation.AssignToList = await GetAssignToUserGroupList(new List<int> { conversation.ID });
                                conversation.AssignCCList = await GetAssignCCUserGroupList(new List<int> { conversation.ID });
                            }

                        }));
                    }));
                }
                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>> GetReplyDiscussionListAsync(long TopicId, long UserId)
        {
            try
            {                

                var Exists = await GetActivityEmailExitsAsync(TopicId);

                var res = new List<EmailConversations>();

                if (Exists > 0)
                {
                   res = await GetReplyQuery1ListAsync(TopicId, UserId);
                }
                else
                {
                    res = await GetReplyQuery2ListAsync(TopicId, UserId);
                }
               
                if (res != null && res.Count > 0)
                {
                    var replyIds = res.Select(s => s.ID).Distinct().ToList();
                    var sessionids = res.Select(c => c.SessionId).Distinct().ToList();

                    await Task.WhenAll(res.Select(async topic =>
                    {
                        var replyCount = await GetReplyCount(new List<int> { topic.ID }, UserId);
                        topic.ReplyConversationCount = replyCount;
                        //topic.ReplyConversationCount = await GetReplyCount(new List<int> { topic.ID }, UserId);
                        //topic.ReplyConversation = await GetReplyList(new List<int> { topic.ID }, UserId);
                        //topic.documents = await GetDocumentList(new List<Guid?> { topic.SessionId });
                        topic.documents = await GetCheckPermissionDocumentList(new List<Guid?> { topic.SessionId }, UserId);

                        if (string.IsNullOrEmpty(topic.UserType) || topic.UserType == "Users")
                        {
                            topic.AssignToList = await GetAssignToList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCList(new List<int> { topic.ID });
                        }
                        else
                        {
                            topic.AssignToList = await GetAssignToUserGroupList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCUserGroupList(new List<int> { topic.ID });
                        }
                            

                        //await Task.WhenAll(topic.ReplyConversation.Select(async conversation =>
                        //{
                        //    conversation.documents = await GetDocumentList(new List<Guid?> { conversation.SessionId });
                        //    if (string.IsNullOrEmpty(conversation.UserType) || conversation.UserType == "Users")
                        //    {
                        //        conversation.AssignToList = await GetAssignToList(new List<int> { conversation.ID });
                        //        conversation.AssignCCList = await GetAssignCCList(new List<int> { conversation.ID });
                        //    }
                        //    else
                        //    {
                        //        conversation.AssignToList = await GetAssignToUserGroupList(new List<int> { conversation.ID });
                        //        conversation.AssignCCList = await GetAssignCCUserGroupList(new List<int> { conversation.ID });
                        //    }
                            
                        //}));
                    }));



                    //foreach (var topic in res)
                    //{
                    //    var replyIds = new List<long?> { topic.ID };
                    //    var sessionids = new List<Guid?> { topic.SessionId };

                    //    topic.ReplyConversation = await GetReplyList(replyIds, UserId);
                    //    topic.documents = await GetDocumentList(sessionids);
                    //    topic.AssignToList = await GetAssignToList(replyIds);
                    //    topic.AssignCCList = await GetAssignCCList(replyIds);



                    //    foreach (var conversation in topic.ReplyConversation)
                    //    {
                    //        var Ids = new List<long?> { conversation.ID };
                    //        var sids = new List<Guid?> { conversation.SessionId };

                    //        conversation.documents = await GetDocumentList(sids);
                    //        conversation.AssignToList = await GetAssignToList(Ids);
                    //        conversation.AssignCCList = await GetAssignCCList(Ids);
                    //    }
                    //}
                }
                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }       
      
        public async Task<List<EmailConversations>> GetReplyListPaged(long replyId, long userId, int pageNumber, int pageSize)
        {
            try
            {
                var query = @"
            SELECT 
                FC.Name, FC.ID, FC.AddedDate, 
                CONCAT(AU.FirstName, '-', AU.NickName) AS UserName, 
                AU.UserID, FC.ReplyId, FC.SessionId, FC.FileData, 
                EN.IsRead, EN.ID AS EmailNotificationId, 
                ISNULL(FC.IsMobile, 0) AS IsMobile, FC.UserType,
                ECL.EmailConversationsId AS CopyLinkEmailIds
            FROM EmailConversations FC
            INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
            LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
            LEFT JOIN (
                SELECT DISTINCT EmailConversationsId FROM EmailCopyLink
            ) ECL ON ECL.EmailConversationsId = FC.ID
            WHERE FC.ReplyId = @ReplyId
            ORDER BY FC.AddedDate DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (var connection = CreateConnection())
                {
                    int offset = (pageNumber - 1) * pageSize;

                    var result = (await connection.QueryAsync<EmailConversations>(
                        query,
                        new { UserId = userId, ReplyId = replyId, PageSize = pageSize, Offset = offset },
                        commandTimeout: 180 
                    )).ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching replies: {ex.Message}", ex);
            }
        }

        public async Task<List<EmailConversations>> GetReplyListPaged1(long replyId, long userId, int pageNumber, int pageSize)
        {
            try
            {
                var query = @"SELECT FC.Name, FC.ID, FC.AddedDate, CONCAT(AU.FirstName, '-', AU.NickName) AS UserName, 
                                AU.UserID, FC.ReplyId, FC.SessionId, FC.FileData, EN.IsRead, 
                                EN.ID AS EmailNotificationId, ISNULL(FC.IsMobile, 0) AS IsMobile, 
                                FC.UserType, ECL.EmailConversationsId AS CopyLinkEmailIds
                                FROM EmailConversations FC
                                INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
                                LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
                                OUTER APPLY(SELECT DISTINCT EmailConversationsId FROM EmailCopyLink WHERE EmailConversationsId = FC.ID) ECL
                                WHERE FC.ReplyId = @ReplyId
                                ORDER BY FC.AddedDate DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (var connection = CreateConnection())
                {
                    int offset = (pageNumber - 1) * pageSize;

                    var result = (await connection.QueryAsync<EmailConversations>(query,
                        new { UserId = userId, ReplyId = replyId, PageSize = pageSize, Offset = offset })).ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<EmailConversations>> GetReplyListPagedAsync(long ReplyId, long UserId,int currentPage,int pageSize)
        {

            try
            {
                var res = new List<EmailConversations>();
                res = await GetReplyListPaged(ReplyId, UserId,currentPage,pageSize);


                if (res != null && res.Count > 0)
                {

                    await Task.WhenAll(res.Select(async topic =>
                    {
                        topic.documents = await GetDocumentList(new List<Guid?> { topic.SessionId });

                        if (string.IsNullOrEmpty(topic.UserType) || topic.UserType == "Users")
                        {
                            topic.AssignToList = await GetAssignToList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCList(new List<int> { topic.ID });
                        }
                        else
                        {

                        }

                    }));

                }
                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversations>> GetOnDiscussionListAsync(long ReplyId, long UserId)
        {

            try
            {
                var res = new List<EmailConversations>();
                    res = await GetonReplyList(ReplyId, UserId);


                if (res != null && res.Count > 0)
                {                   

                    await Task.WhenAll(res.Select(async topic =>
                    {                        
                        topic.documents = await GetDocumentList(new List<Guid?> { topic.SessionId });

                        if (string.IsNullOrEmpty(topic.UserType) || topic.UserType == "Users")
                        {
                            topic.AssignToList = await GetAssignToList(new List<int> { topic.ID });
                            topic.AssignCCList = await GetAssignCCList(new List<int> { topic.ID });
                        }
                        else
                        {                           
                      
                        }

                    }));

                }
                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversations>> GetReplyDiscussionListAsync_vijay(long TopicId, long UserId)
        {
            try
            {

                var query = @"SELECT FC.Name,FC.ID,FC.TopicID,FC.SessionId,FC.AddedDate,FC.Message,AU.UserName,AU.UserID,FC.ReplyId,FC.FileData,FC.AddedByUserID,
                                AET.Comment as ActCommentName,AET.BackURL,AET.DocumentSessionId,EMPP.FirstName as ActUserName,AET.AddedDate as ActAddedDate,FC.DueDate,FC.IsAllowParticipants,
                                ONB.FirstName AS OnBehalfName,FC.Follow,FC.Urgent,FC.OnBehalf,FC.NotifyUser,FCEP.FirstName,FCEP.LastName,AET.ActivityType
                                FROM EmailConversations FC  
                                LEFT JOIN Employee ONB ON ONB.UserID = FC.OnBehalf                                
                                LEFT JOIN ActivityEmailTopics AET ON AET.EmailTopicSessionId = FC.SessionId
                                INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                INNER JOIN Employee EMP ON EMP.UserID = AU.UserID      
                                LEFT JOIN Employee EMPP ON EMPP.UserID = AET.AddedByUserID 
                                LEFT JOIN Employee FCEP ON FCEP.UserID = FC.AddedByUserID 
                                WHERE FC.ID = @TopicId AND FC.ReplyId = 0 
                                AND ((AET.ActivityEmailTopicID IS NOT NULL AND AET.ActivityType != 'EmailFileProfileType') OR AET.ActivityEmailTopicID IS NULL)
                                ORDER BY FC.AddedDate DESC";


                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                var res = new List<EmailConversations>();

                using (var connection = CreateConnection())
                {
                    res = (await connection.QueryAsync<EmailConversations>(query, parameters)).ToList();
               

                if(res != null && res.Count > 0)
                {
                    foreach (var topic in res)
                    {

                        var subQuery = @"SELECT
                                        FC.Name,
                                        FC.ID,
                                        FC.AddedDate,
                                        FC.Message,
                                        AU.UserName,
                                        AU.UserID,
                                        FC.ReplyId,
                                        FC.SessionId,FC.FileData,
                                        EN.IsRead,
										EN.ID as EmailNotificationId,                                        
                                        ISNULL(FC.IsMobile, 0) AS IsMobile
                                    FROM
                                        EmailConversations FC
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                        LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
                                    WHERE
                                       FC.ReplyId = @ReplyId
                                       ORDER BY FC.AddedDate DESC";

                        var parameterss = new DynamicParameters();
                        //parameterss.Add("TopicId", TopicId, DbType.Int64);
                        parameterss.Add("UserId", UserId, DbType.Int64);
                        parameterss.Add("ReplyId", topic.ID, DbType.Int64);
                        var subQueryResults = await connection.QueryAsync<EmailConversations>(subQuery, parameterss);

                        var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";

                        var parametersDocs = new DynamicParameters();
                        parametersDocs.Add("SessionID", topic.SessionId);

                        var subQueryDocsResults = await connection.QueryAsync<EmailDocumentModel>(subQueryDocs, parametersDocs);

                        var subQueryassignTo = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignTo FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                        var parametersassignTo = new DynamicParameters();
                        parametersassignTo.Add("Id", topic.ID, DbType.Int64);
                        var subQueryAssignToResults = await connection.QueryAsync<EmailAssignToList>(subQueryassignTo, parametersassignTo);


                        var subQueryassignCC = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignCC FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                        var parametersassignCC = new DynamicParameters();
                        parametersassignCC.Add("Id", topic.ID, DbType.Int64);
                        var subQueryAssignCCResults = await connection.QueryAsync<EmailAssignToList>(subQueryassignCC, parametersassignCC);


                        topic.ReplyConversation = subQueryResults.ToList();
                        topic.documents = subQueryDocsResults.ToList();
                        topic.AssignToList = subQueryAssignToResults.ToList();
                        topic.AssignCCList = subQueryAssignCCResults.ToList();

                        foreach (var conversation in topic.ReplyConversation)
                        {
                            var subQueryReplyDocs = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                                      INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                                      WHERE D.SessionID = @SessionID";
                            var parametersReplyDocs = new DynamicParameters();
                            parametersReplyDocs.Add("SessionID", conversation.SessionId);
                            //parametersReplyDocs.Add("ReplyId", conversation.ReplyId, DbType.Int64);

                            var subQueryReplyDocsResults = await connection.QueryAsync<EmailDocumentModel>(subQueryReplyDocs, parametersReplyDocs);

                            var replysubQueryassignTo = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                              INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                              INNER JOIN Employee E ON E.UserID = FCT.UserId
                                              WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignTo = new DynamicParameters();
                            replyparametersassignTo.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignToResults = await connection.QueryAsync<EmailAssignToList>(replysubQueryassignTo, replyparametersassignTo);



                            var replysubQueryassignCC = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignCC FCT
                                              INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                              INNER JOIN Employee E ON E.UserID = FCT.UserId
                                              WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignCC = new DynamicParameters();
                            replyparametersassignCC.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignCCResults = await connection.QueryAsync<EmailAssignToList>(replysubQueryassignCC, replyparametersassignCC);



                            //var replyBysubQuery = @"SELECT
                            //                          FC.Name,
                            //                          FC.ID,
                            //                          FC.AddedDate,
                            //                          FC.Message,
                            //                          AU.UserName,
                            //                          AU.UserID,
                            //                          FC.ReplyId,
                            //                          FC.SessionId,FC.FileData
                            //                      FROM
                            //                          EmailConversations FC
                            //                          INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                            //                      WHERE
                            //                           FC.ReplyId = @ReplyId";

                            //var reparameterss = new DynamicParameters();
                            //reparameterss.Add("TopicId", TopicId, DbType.Int64);
                            //reparameterss.Add("ReplyId", conversation.ID, DbType.Int64);
                            //var replyBysubQueryResults = connection.Query<EmailConversations>(replyBysubQuery, reparameterss).ToList();


                            conversation.documents = subQueryReplyDocsResults.ToList();
                            conversation.AssignToList = replysubQueryAssignToResults.ToList();
                            conversation.AssignCCList = replysubQueryAssignCCResults.ToList();
                            //conversation.ReplyConversation = replyBysubQueryResults;
                        }

                    }
                }

                }

                return res;
                
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<List<EmployeeNameModel>> GetEmployeeNameList(List<long?> userIds)
        {
            try
            {
                var query = @"select CONCAT(FirstName,'-',NickName) as FirstName from Employee where UserID in (select UserID from ApplicationUser where UserID in @userIds);";
                var parameters = new DynamicParameters();
                parameters.Add("userIds", userIds);
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmployeeNameModel>(query, parameters)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailAssignToList>> GetAssignToUserGroupList(List<int> Ids)
        {
            try
            {
                var lists = string.Join(',', Ids.Select(i => $"'{i}'"));
                var query = @"SELECT UG.Name as FirstName, ECAU.GroupId,ECAU.TopicId FROM EmailConversationAssignToUserGroup ECAU
                                INNER JOIN UserGroup UG ON UG.UserGroupID =ECAU.GroupId
                                WHERE ECAU.ConversationId in (" + lists + ")";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Ids);

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailAssignToList>(query, parameters)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailAssignToList>> GetAssignCCUserGroupList(List<int> Ids)
        {
            try
            {
                var lists = string.Join(',', Ids.Select(i => $"'{i}'"));
                var query = @"SELECT UG.Name as FirstName, ECAU.GroupId,ECAU.TopicId FROM EmailConversationAssignCCUserGroup ECAU
                                INNER JOIN UserGroup UG ON UG.UserGroupID =ECAU.GroupId
                                WHERE ECAU.ConversationId in (" + lists + ")";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Ids);

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailAssignToList>(query, parameters)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailAssignToList>> GetAssignToList(List<int> Ids)
        {
            try
            {
                var lists = string.Join(',', Ids.Select(i => $"'{i}'"));
                var query = @"select CONCAT(E.FirstName,'-',E.NickName) as FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignTo FCA
                              INNER JOIN Employee E on E.UserID = FCA.UserId
                              where FCA.ConversationId in (" + lists + ")";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Ids);

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailAssignToList>(query, parameters)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailAssignToList>> GetAssignCCList(List<int> Ids)
        {
            try
            {
                var lists = string.Join(',', Ids.Select(i => $"'{i}'"));
                var query = @"select CONCAT(E.FirstName,'-',E.NickName) as FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignCC FCA
                              INNER JOIN Employee E on E.UserID = FCA.UserId
                              where FCA.ConversationId in (" + lists + ")";
                var parameters = new DynamicParameters();
                parameters.Add("Id", Ids);

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailAssignToList>(query, parameters)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailDocumentModel>> GetDocumentList(List<Guid?> sessionids)
        {
            try
            {
                var lists = string.Join(',', sessionids.Select(i => $"'{i}'"));
                var query = "select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID in(" + lists + ")";
               
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailDocumentModel>(query)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }       
        public async Task<List<EmailDocumentModel>> GetCheckPermissionDocumentList(List<Guid?> sessionids, long? userId)
        {
            try
            {
                var lists = string.Join(',', sessionids.Select(i => $"'{i}'"));
                var query = @"select D.DocumentID, D.FileName, D.ContentType, D.FileSize, D.FilePath, DD.DocumentID as ReplaceDocumentId, DD.FilterProfileTypeID, DD.ProfileNo 
                      from Documents D
                      LEFT JOIN ActivityEmailTopics AET ON AET.ActivityEmailTopicID = D.EmailToDMS
                      LEFT JOIN Documents DD ON DD.SessionID = AET.SessionId and DD.IsLatest = 1
                      WHERE D.SessionID in(" + lists + ")";

                using (var connection = CreateConnection())
                {
                    var documentList = (await connection.QueryAsync<EmailDocumentModel>(query)).ToList();

                    foreach (var document in documentList)
                    {
                        if (document.FilterProfileTypeID != null)
                        {
                            var permissionData = await _fileprofiletypeQueryRepository.GetDocumentUserRoleByUserIDAsync(document.FilterProfileTypeID, userId, document.ReplaceDocumentId);

                            if (permissionData != null)
                            {
                                if (permissionData.IsPermissionExits == true)
                                {
                                    document.IsView = true;
                                }
                                else
                                {
                                    if (permissionData.IsEdit == true || permissionData.IsRead == true)
                                    {
                                        document.IsView = true;
                                    }
                                    else
                                    {
                                        document.IsView = false;
                                    }
                                }
                            }
                            else
                            {
                                document.IsView = false;
                            }
                        }
                        else
                        {
                            document.IsView = true;
                        }
                    }

                    return documentList;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        private async Task<int> GetReplyCount(List<int> replyIds, long userId)
        {
            try
            {                
                var query = @"SELECT COUNT(FC.ID) FROM EmailConversations FC
                            INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
                            LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
                            WHERE FC.ReplyId IN @ReplyIds";
                
                using (var connection = CreateConnection())
                {
                    var result = await connection.QuerySingleAsync<int>(query, new { UserId = userId, ReplyIds = replyIds });
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>> GetReplyList(List<int> replyIds,long UserId)
        {
            try
            {
                var query = "SELECT FC.Name,FC.ID, FC.AddedDate,CONCAT(AU.FirstName,'-',AU.NickName) as UserName,AU.UserID,FC.ReplyId,FC.SessionId,FC.FileData,EN.IsRead,EN.ID as EmailNotificationId,ISNULL(FC.IsMobile, 0) AS IsMobile,FC.UserType FROM EmailConversations FC \r\n";
                    query += "INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId \r\n";
                    query += "LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId =" + UserId + " \r\n";
                    query += "WHERE FC.ReplyId in(" + string.Join(',', replyIds) + ") ORDER BY FC.AddedDate DESC \r\n";
           
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailConversations>(query)).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<byte[]> GetFileDataAsync(long conversationId)
        {
            // Using streaming for large file data
            using (var connection = new SqlConnection(GetConnectionString()))
            using (var command = new SqlCommand("SELECT FileData FROM EmailConversations WHERE ID = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", conversationId);
                command.CommandTimeout = 120;

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                {
                    if (await reader.ReadAsync())
                    {
                        const int bufferSize = 8192;
                        byte[] buffer = new byte[bufferSize];
                        long fieldOffset = 0;
                        long bytesRead;

                        using (var ms = new MemoryStream())
                        {
                            while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, (int)bytesRead);
                                fieldOffset += bytesRead;
                            }

                            return ms.ToArray();
                        }
                    }
                }
            }

            return null;
        }

        public async Task<byte[]> GetFileDataAsync1(long conversationId)
        {
            const string query = "SELECT FileData FROM EmailConversations WHERE ID = @Id";

            using (var connection = CreateConnection()) // IDbConnection is fine
            {
                var command = new CommandDefinition(
                    commandText: query,
                    parameters: new { Id = conversationId },
                    commandTimeout: 120 // timeout in seconds
                );

                return await connection.QueryFirstOrDefaultAsync<byte[]>(command);
            }
        }

        public async Task<byte[]> GetFileDataAsyncold(long conversationId)
        {
            const string query = "SELECT FileData FROM EmailConversations WHERE ID = @Id";

            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<byte[]>(query, new { Id = conversationId });
            }
        }

        private async Task<List<EmailConversations>> GetonReplyList(long replyId, long userId)
        {
            try
            {
                var query = @"
        SELECT 
            FC.Name, FC.ID, FC.AddedDate, CONCAT(AU.FirstName, '-', AU.NickName) as UserName, 
            AU.UserID, FC.ReplyId, FC.SessionId,
            EN.IsRead, EN.ID as EmailNotificationId, 
            ISNULL(FC.IsMobile, 0) AS IsMobile, FC.UserType,
            ECL.EmailConversationsId AS CopyLinkEmailIds
        FROM EmailConversations FC
        INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
        LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
        OUTER APPLY (
            SELECT DISTINCT EmailConversationsId 
            FROM EmailCopyLink 
            WHERE EmailConversationsId = FC.ID
        ) ECL
        WHERE FC.ReplyId = @ReplyId
        ORDER BY FC.AddedDate DESC";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailConversations>(query, new { UserId = userId, ReplyId = replyId })).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        private async Task<List<EmailConversations>> GetonReplyListold(long replyId, long UserId)
        {
            try
            {
                var query = @"
            SELECT FC.Name, FC.ID, FC.AddedDate, CONCAT(AU.FirstName, '-', AU.NickName) as UserName, 
                   AU.UserID, FC.ReplyId, FC.SessionId, FC.FileData, EN.IsRead, 
                   EN.ID as EmailNotificationId, ISNULL(FC.IsMobile, 0) AS IsMobile, FC.UserType,ECL.EmailConversationsId AS CopyLinkEmailIds
            FROM EmailConversations FC
            INNER JOIN Employee AU ON AU.UserID = FC.ParticipantId
            LEFT JOIN EmailNotifications EN ON EN.ConversationId = FC.ID AND EN.UserId = @UserId
            OUTER APPLY(SELECT DISTINCT  EmailConversationsId FROM EmailCopyLink WHERE EmailConversationsId = FC.ID)ECL
            WHERE FC.ReplyId = @ReplyId
            ORDER BY FC.AddedDate DESC";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<EmailConversations>(query, new { UserId, ReplyId = replyId })).ToList();
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversations>> GetByReplyDiscussionList(long replyId)
        {
            try
            {

                var query = @"SELECT
                                FC.Name,
                                FC.ID,
                                FC.AddedDate,
                                FC.Message,
                                AU.UserName,
                                AU.UserID,
                                FC.ReplyId,
                                FC.SessionId,FC.FileData
                            FROM
                                EmailConversations FC
                                INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                            WHERE
                                FC.ReplyId = @replyId";


                var parameters = new DynamicParameters();
                parameters.Add("replyId", replyId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    var res = (await connection.QueryAsync<EmailConversations>(query, parameters)).ToList();

                    if (res != null && res.Count > 0)
                    {
                        foreach (var conversation in res)
                        {
                            var subQueryReplyDocs = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                                      INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                                      WHERE D.SessionID = @SessionID";
                            var parametersReplyDocs = new DynamicParameters();
                            parametersReplyDocs.Add("SessionID", conversation.SessionId);
                            parametersReplyDocs.Add("ReplyId", conversation.ReplyId, DbType.Int64);

                            var subQueryReplyDocsResults = connection.Query<EmailDocumentModel>(subQueryReplyDocs, parametersReplyDocs).ToList();

                            var replysubQueryassignTo = @"SELECT FCT.ID,FCT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName from EmailConversationAssignTo FCT
                                                  INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                                  INNER JOIN Employee E ON E.UserID = FCT.UserId
                                                  WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignTo = new DynamicParameters();
                            replyparametersassignTo.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignToResults = connection.Query<EmailAssignToList>(replysubQueryassignTo, replyparametersassignTo).ToList();



                            var replysubQueryassignCC = @"SELECT FCT.ID,FCT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName from EmailConversationAssignCC FCT
                                                  INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                                  INNER JOIN Employee E ON E.UserID = FCT.UserId
                                                  WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignCC = new DynamicParameters();
                            replyparametersassignCC.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignCCResults = connection.Query<EmailAssignToList>(replysubQueryassignCC, replyparametersassignCC).ToList();

                            conversation.documents = subQueryReplyDocsResults;
                            conversation.AssignToList = replysubQueryAssignToResults;
                            conversation.AssignCCList = replysubQueryAssignCCResults;

                        }
                }

                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<int> GetActivityEmailExitsAsync(long? Id)
        {
            try
            {               
                var query = "SELECT 1 FROM ActivityEmailTopics AET \r\n" +
                                "INNER JOIN EmailConversations EC ON EC.SessionId = AET.EmailTopicSessionId AND EC.ID = @Id \r\n" +
                                "WHERE AET.ActivityType != 'EmailFileProfileType' \r\n";

                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<int>(query, parameters)).Count();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<List<Documents>> GetTopicDocListAsync(long TopicId, long UserId,string option)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("TopicId", TopicId);
                        parameters.Add("UserId", UserId);
                        parameters.Add("Option", option);                        

                        var result = await connection.QueryAsync<Documents>("sp_Select_EmailDocList", parameters, commandType: CommandType.StoredProcedure);
                        return result.ToList();
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
        public async Task<List<Documents>> GetTopicDocListAsync_old(long TopicId,long UserId)
        {
            try
            {

                var query = @"select FileIndex = ROW_NUMBER() OVER(ORDER BY D.DocumentID DESC),D.DocumentID as DocumentId,D.FileName,D.ContentType,D.FileSize,D.UploadDate,D.SessionID,D.AddedDate,D.FilePath,FC.FileData,FC.Name as SubjectName,E.FirstName AS AddedBy,D.AddedDate,EMP.FirstName as ModifiedBy,D.ModifiedDate,D.UniqueSessionId from EmailConversations FC 
                                INNER JOIN Documents D on D.SessionID = FC.SessionId
                                INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = FC.ID
								LEFT JOIN Employee E ON E.UserID = D.AddedByUserID
								LEFT JOIN Employee EMP ON EMP.UserID = D.ModifiedByUserID
                                where FC.TopicID = @TopicId AND ECP.UserId = @UserId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);
                parameters.Add("UserId", UserId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    var res = connection.Query<Documents>(query, parameters).ToList();
                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<Documents>> GetSubTopicDocListAsync(long ConversationId)
        {
            try
            {

                var query = @"SELECT FileIndex = ROW_NUMBER() OVER(ORDER BY D.DocumentID DESC),D.DocumentID as DocumentId,DD.DocumentID as ReplaceDocumentId,DD.FilterProfileTypeID,DD.ProfileNo,D.FileName,D.ContentType,D.FileSize,D.UploadDate,D.SessionID,D.AddedDate,D.FilePath,FC.FileData,FC.Name as SubjectName,E.FirstName AS AddedBy,D.AddedDate,EMP.FirstName as ModifiedBy,D.ModifiedDate,D.UniqueSessionId,D.EmailToDMS,CONCAT(AET.BackURL, '/', AET.DocumentSessionId) AS DMSBackUrl,
                            IsLatest = CASE 
                                        WHEN D.EmailToDMS IS NOT NULL AND D.FileName = (
                                            SELECT FileName 
                                            FROM Documents DS 
                                            WHERE DS.SessionID = AET.DocumentSessionId AND DS.IsLatest = 1
                                        ) THEN 'True'
                                        ELSE 'False'
                                    END
                            from EmailConversations FC 
                                INNER JOIN Documents D on D.SessionID = FC.SessionId
                                LEFT JOIN ActivityEmailTopics AET ON AET.ActivityEmailTopicID = D.EmailToDMS
                                LEFT JOIN Documents DD ON DD.SessionID = AET.SessionId and DD.IsLatest = 1
								LEFT JOIN Employee E ON E.UserID = D.AddedByUserID
								LEFT JOIN Employee EMP ON EMP.UserID = D.ModifiedByUserID
                                where FC.ID = @ConversationId
                                    UNION
                                SELECT FileIndex = ROW_NUMBER() OVER(ORDER BY D.DocumentID DESC),D.DocumentID as DocumentId,DD.DocumentID as ReplaceDocumentId,DD.FilterProfileTypeID,DD.ProfileNo,D.FileName,D.ContentType,D.FileSize,D.UploadDate,D.SessionID,D.AddedDate,D.FilePath,FC.FileData,FC.Name as SubjectName,E.FirstName AS AddedBy,D.AddedDate,EMP.FirstName as ModifiedBy,D.ModifiedDate,D.UniqueSessionId,D.EmailToDMS,CONCAT(AET.BackURL, '/', AET.DocumentSessionId) AS DMSBackUrl,
                                IsLatest = CASE 
                                            WHEN D.EmailToDMS IS NOT NULL AND D.FileName = (
                                                SELECT FileName 
                                                FROM Documents DS 
                                                WHERE DS.SessionID = AET.DocumentSessionId AND DS.IsLatest = 1
                                            ) THEN 'True'
                                            ELSE 'False'
                                        END
                                from EmailConversations FC 
                                INNER JOIN Documents D on D.SessionID = FC.SessionId
                                LEFT JOIN ActivityEmailTopics AET ON AET.ActivityEmailTopicID = D.EmailToDMS
                                LEFT JOIN Documents DD ON DD.SessionID = AET.SessionId and DD.IsLatest = 1
								LEFT JOIN Employee E ON E.UserID = D.AddedByUserID
								LEFT JOIN Employee EMP ON EMP.UserID = D.ModifiedByUserID
                                where FC.ReplyId = @ConversationId";

                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<Documents>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<EmailConversations> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM EmailConversations WHERE ID = @ID";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<EmailConversations>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<List<EmailConversations>> GetConversationTopicIdList(long TopicId)
        {
            try
            {
                var query = @"SELECT TOP 1 * FROM EmailConversations WHERE TopicID = @TopicID AND ReplyId = 0";
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

                using (var connection = CreateConnection())
                {   
                    var res = await connection.QueryAsync<EmailConversations>(query, parameters);
                    return res.ToList();                   
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversationAssignTo>> GetConversationAssignToList(long ConversationId)
        {
            try
            {
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY FCT.ID DESC), FCT.ID,FCT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<EmailConversationAssignTo>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailConversationAssignTo>> GetConversationAssignCCList(long ConversationId)
		{
			try
			{
				var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY FCT.ID DESC), FCT.ID,FCT.UserId,CONCAT(E.FirstName,'-',E.NickName) as FirstName,E.LastName from EmailConversationAssignCC FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
				var parameters = new DynamicParameters();
				parameters.Add("ConversationId", ConversationId);

				using (var connection = CreateConnection())
				{
					var res = await connection.QueryAsync<EmailConversationAssignTo>(query, parameters);
					return res.ToList();
				}
			}
			catch (Exception exp)
			{
				throw new Exception(exp.Message, exp);
			}
		}
        public async Task<List<EmailConversationAssignToUserGroup>> GetAssignCCUserGroupList(long ConversationId)
        {
            try
            {
                var query = @"SELECT FCT.ID,FCT.GroupId,FCT.AddedByUserID from EmailConversationParticipantUserGroup FCT                               
                                WHERE FCT.ConversationId = @ConversationId";
                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<EmailConversationAssignToUserGroup>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> LastUserIDUpdate(long ReplyId,long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ReplyId", ReplyId, DbType.Int64);
                            parameters.Add("UserId", UserId, DbType.Int64);
                           

                            var query = " UPDATE EmailConversations SET LastUpdateUserID = @UserId ,LastUpdateDate = GETDATE() WHERE ID = @ReplyId";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters);
                            return rowsAffected;
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

        public async Task<long> UpdateDueDateReqested(long ReplyId, long UserId,int value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ReplyId", ReplyId, DbType.Int64);
                        parameters.Add("UserId", UserId, DbType.Int64);
                        parameters.Add("value", value, DbType.Int32);


                        var query = " UPDATE EmailConversations SET IsDueDate = @value WHERE ID = @ReplyId";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return rowsAffected;
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


        public async Task<long> DocInsertDynamicFormDateUpload(Guid id, Guid DynamicFormSectionID, Guid sessionid,long userid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Id", id);
                        parameters.Add("sessionId", sessionid);
                        parameters.Add("AddedByUserID", userid);
                        parameters.Add("DynamicFormSectionID", DynamicFormSectionID);


                        //var query = @"UPDATE DynamicFormDataUpload SET EmailSessionID = @sessionId WHERE SessionID = @Id";

                        var query = @"INSERT INTO Documents(FileName,ContentType,FileSize,UploadDate,AddedByUserID,AddedDate,FilePath,SessionID,SourceFrom,EmailToDMS,IsNewPath)			
                                        SELECT D.FileName,D.ContentType,D.FileSize,GETDATE(),@AddedByUserID,GETDATE(),FilePath,@sessionId,'Email',null,
                                            CASE
                                            WHEN (D.FilePath LIKE 'AppUpload\%' OR D.FilePath LIKE 'Documents\%') AND D.IsNewPath = 1 THEN 1
                                            ELSE 0
                                            END from DynamicFormData DFD
                                        INNER JOIN DynamicFormDataUpload DFDU ON DFDU.DynamicFormDataID =DFD.DynamicFormDataID
                                        INNER JOIN DynamicFormSection DFS ON DFS.DynamicFormSectionID = DFDU.DynamicFormSectionID                                        
                                        INNER JOIN Documents D on D.SessionID = DFDU.SessionID
                                        WHERE DFD.SessionID = @Id AND DFS.SessionID = @DynamicFormSectionID";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return rowsAffected;
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

        public async Task<long> InsertEmailDynamicFormDateUploadSession(long DynamicFormID,Guid FormDataSessionID, Guid FormSectionSessionID, Guid EmailSessionID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormID", DynamicFormID);
                        parameters.Add("FormDataSessionID", FormDataSessionID);
                        parameters.Add("FormSectionSessionID", FormSectionSessionID);
                        parameters.Add("EmailSessionID", EmailSessionID);

                        var query = @"INSERT INTO EmailDynamicFormSection (DynamicFormID,FormDataSessionID,FormSectionSessionID,EmailSessionID) VALUES (@DynamicFormID,@FormDataSessionID,@FormSectionSessionID,@EmailSessionID)";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return rowsAffected;
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
        public async Task<long> LastUpdateDateEmailTopic(long TopicId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();                        
                        parameters.Add("TopicId", TopicId, DbType.Int64);


                        var query = " UPDATE EmailTopics SET LastUpdateDate = GETDATE() WHERE ID = @TopicId";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return rowsAffected;
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
        public async Task<String> SendPushNotification(long Id)
        {
            return "ok";
        }
        public async Task<long> Insert(EmailConversations forumConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();                           
                            parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                            parameters.Add("Message", forumConversations.Message, DbType.String);                          
                            parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                            parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);
                            parameters.Add("StatusCodeID", forumConversations.StatusCodeID);
                            parameters.Add("AddedByUserID", forumConversations.AddedByUserID);
                            parameters.Add("SessionId", forumConversations.SessionId,DbType.Guid);
                            parameters.Add("AddedDate", forumConversations.AddedDate,DbType.DateTime);
                            parameters.Add("FileData", forumConversations.FileData);
                            parameters.Add("Name", forumConversations.Name,DbType.String);
                            parameters.Add("DueDate", forumConversations.DueDate, DbType.DateTime);
                            parameters.Add("IsLockDueDate", forumConversations.IsLockDueDate);
                            parameters.Add("IsAllowParticipants", forumConversations.IsAllowParticipants);
                            parameters.Add("Urgent", forumConversations.Urgent);
                            parameters.Add("NotifyUser", forumConversations.NotifyUser);
                            parameters.Add("IsMobile", forumConversations.IsMobile,DbType.Int32);
                            parameters.Add("UserType", forumConversations.UserType, DbType.String);
                            parameters.Add("DynamicFormDataUploadSessionID", forumConversations.EmailFormSectionSessionID);
                            parameters.Add("UserType", forumConversations.UserType, DbType.String);
                            parameters.Add("IsDueDate", forumConversations.IsDueDate);
                            parameters.Add("DynamicFormDataUploadSessionID", forumConversations.EmailFormSectionSessionID);
                        

                            var query = "INSERT INTO EmailConversations(IsDueDate,UserType,NotifyUser,IsMobile,Urgent,DueDate,IsAllowParticipants,TopicID,Message,ParticipantId,ReplyId,StatusCodeID,AddedByUserID,SessionId,AddedDate,FileData,Name,DynamicFormDataUploadSessionID,IsLockDueDate) OUTPUT INSERTED.ID VALUES (@IsDueDate,@UserType,@NotifyUser,@IsMobile,@Urgent,@DueDate,@IsAllowParticipants,@TopicID,@Message,@ParticipantId,@ReplyId,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate,@FileData,@Name,@DynamicFormDataUploadSessionID,@IsLockDueDate)";


                            //var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);
                            var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                           

                            //var listData = forumConversations.AssigntoIds.ToList();
                            //if (listData.Count > 0)
                            //{
                            //    forumConversations.AssigntoIds.ToList().ForEach(a =>
                            //    {
                            //        var conversationAssignTo = new EmailConversationAssignTo();
                            //        conversationAssignTo.ConversationId = lastInsertedRecordId;
                            //        conversationAssignTo.TopicId = forumConversations.TopicID;
                            //        conversationAssignTo.UserId =  a;


                            //        var parameters1 = new DynamicParameters();
                            //        parameters1.Add("UserID", conversationAssignTo.UserId);
                            //        parameters1.Add("ConversationId", conversationAssignTo.ConversationId);
                            //        parameters1.Add("TopicId", conversationAssignTo.TopicId);
                            //        parameters1.Add("StatusCodeID", forumConversations.StatusCodeID);
                            //        parameters1.Add("AddedByUserID", forumConversations.AddedByUserID);
                            //        parameters1.Add("SessionId", forumConversations.SessionId);
                            //        parameters1.Add("AddedDate", forumConversations.AddedDate);

                            //        var Assigntoquery = "INSERT INTO EmailConversationAssignTo(ConversationId,TopicId,UserID,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@ConversationId,@TopicId,@UserID,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";

                            //         connection.ExecuteAsync(Assigntoquery, parameters1, transaction);


                            //        //_employeeReportToCommandRepository.AddAsync(employeeReportTo);
                            //    });
                            //}




                            // Retrieve the last inserted record ID
                            //var lastInsertedRecordId = await connection.ExecuteScalarAsync<long>("SELECT SCOPE_IDENTITY();", transaction);



                            return lastInsertedRecordId;
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
        public async Task<long> InsertAssignTo(EmailConversationAssignTo conversationAssignTo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();                            
                            parameters.Add("UserID", conversationAssignTo.UserId);
                            parameters.Add("ConversationId", conversationAssignTo.ConversationId);
                            parameters.Add("TopicId", conversationAssignTo.TopicId);
                            parameters.Add("StatusCodeID", conversationAssignTo.StatusCodeID);
                            parameters.Add("AddedByUserID", conversationAssignTo.AddedByUserID);
                            parameters.Add("SessionId", conversationAssignTo.SessionId);
                            parameters.Add("AddedDate", conversationAssignTo.AddedDate);

                            var query = "INSERT INTO EmailConversationAssignTo(ConversationId,TopicId,UserID,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@ConversationId,@TopicId,@UserID,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters);                          

                            return rowsAffected;
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
        public async Task<List<long>> GetGroupByUserIdList(string GroupIds,long TopicId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("GroupIds", GroupIds);
                        parameters.Add("TopicId", TopicId);

                        var result = await connection.QueryAsync<long>("sp_Get_UserGropUserId", parameters, commandType: CommandType.StoredProcedure);
                        return result.ToList();                        
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
        public async Task<long> InsertAssignToUserGroup_sp(EmailConversationAssignToUserGroup conversationAssignTo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ConversationId", conversationAssignTo.ConversationId);
                        parameters.Add("ReplyId", conversationAssignTo.ReplyId);
                        parameters.Add("TopicId", conversationAssignTo.TopicId);                        
                        parameters.Add("AddedByUserID", conversationAssignTo.AddedByUserID);                        
                        parameters.Add("AddedDate", conversationAssignTo.AddedDate);
                        parameters.Add("AssigntoIds", conversationAssignTo.AssigntoIds);
                        parameters.Add("AssignccIds", conversationAssignTo.AssignccIds);
                        parameters.Add("PlistIdss", conversationAssignTo.PlistIdss);
                        parameters.Add("AllowPlistids", conversationAssignTo.AllowPlistids);
                        parameters.Add("ConIds", conversationAssignTo.ConIds);
                        parameters.Add("Option", "INSERT");

                        //var query = "INSERT INTO EmailConversationAssignTo(ConversationId,TopicId,UserID,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@ConversationId,@TopicId,@UserID,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";

                        //var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                        var result = await connection.QueryFirstOrDefaultAsync<long>("sp_Ins_EmailConvAssignToUserGroup", parameters, commandType: CommandType.StoredProcedure);
                        return result;

                        //return rowsAffected;
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

        public async Task<long> InsertAssignTo_sp(EmailConversationAssignTo conversationAssignTo)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                        try
                        {
                            var parameters = new DynamicParameters();                            
                            parameters.Add("ConversationId", conversationAssignTo.ConversationId);
                            parameters.Add("ReplyId", conversationAssignTo.ReplyId);
                            parameters.Add("TopicId", conversationAssignTo.TopicId);
                            parameters.Add("StatusCodeID", conversationAssignTo.StatusCodeID);
                            parameters.Add("AddedByUserID", conversationAssignTo.AddedByUserID);
                            parameters.Add("SessionId", conversationAssignTo.SessionId);
                            parameters.Add("AddedDate", conversationAssignTo.AddedDate);
                            parameters.Add("AssigntoIds", conversationAssignTo.AssigntoIds);
                            parameters.Add("AssignccIds", conversationAssignTo.AssignccIds);
                            parameters.Add("PlistIdss", conversationAssignTo.PlistIdss);
                            parameters.Add("AllowPlistids", conversationAssignTo.AllowPlistids);
                            parameters.Add("ConIds", conversationAssignTo.ConIds);
                            parameters.Add("Option", "INSERT");
                        
                            //var query = "INSERT INTO EmailConversationAssignTo(ConversationId,TopicId,UserID,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@ConversationId,@TopicId,@UserID,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";

                            //var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            var result = await connection.QueryFirstOrDefaultAsync<long>("sp_Ins_EmailConvAssignTo", parameters, commandType: CommandType.StoredProcedure);
                            return result;

                            //return rowsAffected;
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
        public async Task<long> InsertEmailNotifications(EmailNotifications forumNotifications)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("UserID", forumNotifications.UserId);
                            parameters.Add("ConversationId", forumNotifications.ConversationId);
                            parameters.Add("TopicId", forumNotifications.TopicId);                            
                            parameters.Add("AddedByUserID", forumNotifications.AddedByUserID);                            
                            parameters.Add("AddedDate", forumNotifications.AddedDate);
                            parameters.Add("IsRead", forumNotifications.IsRead);                           

                            var query = "INSERT INTO EmailNotifications(ConversationId,TopicId,UserID,AddedByUserID,AddedDate,IsRead) VALUES (@ConversationId,@TopicId,@UserID,@AddedByUserID,@AddedDate,@IsRead)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters);

                            return rowsAffected;
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

        public async Task<long> Update(EmailConversations forumConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                   

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                            parameters.Add("Message", forumConversations.Message, DbType.String);
                            parameters.Add("ID", forumConversations.ID, DbType.Int64);
                            parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                            parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);

                            var query = " UPDATE EmailConversations SET Name = @Name ,TopicID = @TopicID ,Message=@Message,ParticipantId =@ParticipantId,ReplyId=@ReplyId WHERE ID = @ID";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);                           

                            return rowsAffected;
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

        public  async Task<List<EmailTopics>> GetEmailParticipantListAsync(long conversationID, long Userid)
        {
            try
            {
                var query = @"select ID,IsClosed from EmailConversationParticipant where ConversationId = @conversationID and UserID = @Userid";

                var parameters = new DynamicParameters();
                parameters.Add("conversationID", conversationID);
                parameters.Add("Userid", Userid);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailTopics>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailTopics>> UpdateEmailCloseAsync(long conversationID, long Userid, long Isclose)
        {
            try
            {
                var query = @"Update EmailConversationParticipant set IsClosed= @Isclose where  ConversationId =@conversationID and UserID =@Userid";

                var parameters = new DynamicParameters();
                parameters.Add("conversationID", conversationID);
                parameters.Add("Userid", Userid);
                parameters.Add("Isclose", Isclose);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailTopics>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async  Task<long> InsertCopyEmail(EmailTopics copyemail)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        Guid SessionID = Guid.NewGuid();
                        var querys = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("EmailConversationsId", copyemail.ID);
                        parameters.Add("SessionID", SessionID);
                        parameters.Add("AddedByUserID", copyemail.AddedByUserID);
                        parameters.Add("CopyEmailIds", copyemail.CopyEmailIds);
                       
                        parameters.Add("AddedDate", copyemail.AddedDate);
                       
                       

                        if (copyemail.CopyEmailIds != null)
                        {
                            var listData = copyemail.CopyEmailIds.ToList();
                            if (listData.Count > 0)
                            {
                                listData.ForEach(s =>
                                {
                                   
                                    querys += "INSERT INTO [EmailCopyLink](EmailConversationsSessionid,EmailConversationsId,SessionID,AddedByUserID,AddedDate) VALUES ( " +"'" +s + "'"+ "," + copyemail.ID + ",@SessionID,@AddedByUserID,@AddedDate);"; 
                                });

                            }
                        }

                       
                        if (!string.IsNullOrEmpty(querys))
                        {
                            await connection.ExecuteAsync(querys, parameters);
                        }

                        return copyemail.ID;
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

        public  async  Task<long> DeleteCopyEmail(long ConversationID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ConversationID", ConversationID);
                       

                        var query = "DELETE  FROM EmailCopyLink WHERE EmailConversationsId = @ConversationID";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);

                        return rowsAffected;
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

