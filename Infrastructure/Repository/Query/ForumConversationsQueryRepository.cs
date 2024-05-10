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

namespace Infrastructure.Repository.Query
{
    public class ForumConversationsQueryRepository : QueryRepository<ForumConversations>, IForumConversationsQueryRepository
    {
        public ForumConversationsQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<long> Delete(ForumConversations forumConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();                            
                    parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                    parameters.Add("Message", forumConversations.Message, DbType.String);
                    parameters.Add("ID", forumConversations.ID, DbType.Int64);
                    parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                    parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);

                    var query = "DELETE  FROM ForumConversations WHERE ID = @ID";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;
                        
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
                var query = @"select  * from View_Employee where (StatusName!='Resign' or StatusName is null) and UserID NOT IN (SELECT AU.UserID FROM ForumTopicParticipant TP INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID WHERE TP.TopicId = @TopicId)";
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
        public async Task<List<ForumAssignToList>> GetAllAssignToListAsync(long topicId)
        {
            try
            {
                var query = @"SELECT FT.UserId,E.FirstName FROM ForumTopicParticipant FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
                                WHERE TopicId = @TopicId";

                                //UNION

                                //SELECT FC.UserId,E.FirstName FROM ForumTopicCC FC
                                //INNER JOIN ApplicationUser AU ON AU.UserID = FC.UserId
                                //INNER JOIN Employee E ON E.UserID = FC.UserId
                                //WHERE TopicId = @TopicId";
                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumAssignToList>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<ForumTopicTo>> GetTopicToListAsync(long topicId)
        {
            try
            {
                var query = @"SELECT FT.UserId,E.FirstName FROM ForumTopicTo FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
                                WHERE TopicId = @TopicId";
                var parameters = new DynamicParameters();
                parameters.Add("TopicID", topicId);
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumTopicTo>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> DeleteParticipant(TopicParticipant topicParticipant)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("ID", topicParticipant.ID, DbType.Int64);

                    var query = "DELETE  FROM ForumTopicParticipant WHERE ID = @ID";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;
                       
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ForumConversations>> GetAllAsync()
        {
            try
            {
                var query1 = "SELECT * FROM ForumConversations";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumConversations>(query1)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<ForumConversations>> GetDiscussionListAsync(long TopicId)
        {
            try
            {

                var query = @"SELECT FC.ID,FC.SessionId,FC.AddedDate,FC.Message,AU.UserName,AU.UserID,FC.ReplyId,FC.FileData FROM ForumConversations FC                                
                                INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                INNER JOIN Employee EMP ON EMP.UserID = AU.UserID                               
                                WHERE FC.TopicId = @TopicId AND FC.ReplyId = 0 ORDER BY FC.AddedDate ASC;";


                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    
                    var res = connection.Query<ForumConversations>(query,parameters).ToList();

                    foreach (var topic in res)
                    {
                       
                            var subQuery = @"SELECT
                                        FC.ID,
                                        FC.AddedDate,
                                        FC.Message,
                                        AU.UserName,
                                        AU.UserID,
                                        FC.ReplyId,
                                        FC.SessionId,FC.FileData
                                    FROM
                                        ForumConversations FC
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                    WHERE
                                        FC.TopicId = @TopicId AND FC.ReplyId = @ReplyId";

                            var parameterss = new DynamicParameters();
                            parameterss.Add("TopicId", TopicId, DbType.Int64);
                            parameterss.Add("ReplyId", topic.ID, DbType.Int64);
                        var subQueryResults = connection.Query<ForumConversations>(subQuery, parameterss).ToList();                     


                        var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";
                                   
                        var parametersDocs = new DynamicParameters();
                        parametersDocs.Add("SessionID", topic.SessionId);                                       

                        var subQueryDocsResults = connection.Query<Documents>(subQueryDocs, parametersDocs).ToList();


                        var subQueryassignTo = @"select E.FirstName,FCA.UserId,FCA.TopicId from ForumConversationAssignTo FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                        var parametersassignTo = new DynamicParameters();  
                        parametersassignTo.Add("Id", topic.ID, DbType.Int64);
                        var subQueryAssignToResults = connection.Query<ForumAssignToList>(subQueryassignTo, parametersassignTo).ToList();

                        topic.ReplyConversation = subQueryResults;
                        topic.documents = subQueryDocsResults;
                        topic.AssignToList = subQueryAssignToResults;

                        foreach (var conversation in topic.ReplyConversation)
                        {
                            var subQueryReplyDocs = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData from ForumConversations FC
                                                        INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                        WHERE D.SessionID = @SessionID";
                            var parametersReplyDocs = new DynamicParameters();
                            parametersReplyDocs.Add("SessionID", conversation.SessionId);
                            parameterss.Add("ReplyId", conversation.ReplyId, DbType.Int64);

                            //var subQueryReplyDocsResults = connection.Query<Documents>(subQueryReplyDocs, parametersReplyDocs).ToList();
                            //topic.ReplyConversation.documents = subQueryReplyDocsResults;


                            var subQueryReplyDocsResults = connection.Query<Documents>(subQueryReplyDocs, parametersReplyDocs).ToList();


                            var replysubQueryassignTo = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from ForumConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignTo = new DynamicParameters();
                            replyparametersassignTo.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignToResults = connection.Query<ForumAssignToList>(replysubQueryassignTo, replyparametersassignTo).ToList();

                            conversation.documents = subQueryReplyDocsResults;
                            conversation.AssignToList = replysubQueryAssignToResults;

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

        public async Task<List<Documents>> GetTopicDocListAsync(long TopicId)
        {
            try
            {

                var query = @"select FileIndex = ROW_NUMBER() OVER(ORDER BY D.DocumentID DESC),D.DocumentID as DocumentId,D.FileName,D.ContentType,D.FileSize,D.UploadDate,D.SessionID,D.AddedDate,D.FilePath,FC.FileData from ForumConversations FC 
                                INNER JOIN Documents D on D.SessionID = FC.SessionId
                                where FC.TopicID = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

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

        public async Task<ForumConversations> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ForumConversations WHERE ID = @ID";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumConversations>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<ForumConversationAssignTo>> GetConversationAssignToList(long ConversationId)
        {
            try
            {
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY FCT.ID DESC), FCT.ID,FCT.UserId,E.FirstName,E.LastName from ForumConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {                    
                    var res = connection.Query<ForumConversationAssignTo>(query, parameters).ToList();
                    return res;                   
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(ForumConversations forumConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                   
                    var parameters = new DynamicParameters();                           
                    parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                    parameters.Add("Message", forumConversations.Message, DbType.String);
                    //  parameters.Add("ID", forumConversations.ID, DbType.Int64);
                    parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                    parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);
                    parameters.Add("StatusCodeID", forumConversations.StatusCodeID);
                    parameters.Add("AddedByUserID", forumConversations.AddedByUserID);
                    parameters.Add("SessionId", forumConversations.SessionId);
                    parameters.Add("AddedDate", forumConversations.AddedDate);
                    parameters.Add("FileData", forumConversations.FileData);

                    var query = "INSERT INTO ForumConversations(TopicID,Message,ParticipantId,ReplyId,StatusCodeID,AddedByUserID,SessionId,AddedDate,FileData) OUTPUT INSERTED.ID VALUES (@TopicID,@Message,@ParticipantId,@ReplyId,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate,@FileData)";


                    //var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);
                    var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                    return lastInsertedRecordId;                       
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertAssignTo(ForumConversationAssignTo conversationAssignTo)
        {
            try
            {
                using (var connection = CreateConnection())
                {                   
                    var parameters = new DynamicParameters();                            
                    parameters.Add("UserID", conversationAssignTo.UserId);
                    parameters.Add("ConversationId", conversationAssignTo.ConversationId);
                    parameters.Add("TopicId", conversationAssignTo.TopicId);
                    parameters.Add("StatusCodeID", conversationAssignTo.StatusCodeID);
                    parameters.Add("AddedByUserID", conversationAssignTo.AddedByUserID);
                    parameters.Add("SessionId", conversationAssignTo.SessionId);
                    parameters.Add("AddedDate", conversationAssignTo.AddedDate);

                    var query = "INSERT INTO ForumConversationAssignTo(ConversationId,TopicId,UserID,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@ConversationId,@TopicId,@UserID,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;
                       
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<long> InsertForumNotifications(ForumNotifications forumNotifications)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                   
                    var parameters = new DynamicParameters();
                    parameters.Add("UserID", forumNotifications.UserId);
                    parameters.Add("ConversationId", forumNotifications.ConversationId);
                    parameters.Add("TopicId", forumNotifications.TopicId);                            
                    parameters.Add("AddedByUserID", forumNotifications.AddedByUserID);                            
                    parameters.Add("AddedDate", forumNotifications.AddedDate);
                    parameters.Add("IsRead", forumNotifications.IsRead);                           

                    var query = "INSERT INTO ForumNotifications(ConversationId,TopicId,UserID,AddedByUserID,AddedDate,IsRead) VALUES (@ConversationId,@TopicId,@UserID,@AddedByUserID,@AddedDate,@IsRead)";

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);                         

                    return rowsAffected;                        
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Update(ForumConversations forumConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                  
                    var parameters = new DynamicParameters();
                    parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                    parameters.Add("Message", forumConversations.Message, DbType.String);
                    parameters.Add("ID", forumConversations.ID, DbType.Int64);
                    parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                    parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);

                    var query = " UPDATE ForumConversations SET Name = @Name ,TopicID = @TopicID ,Message=@Message,ParticipantId =@ParticipantId,ReplyId=@ReplyId WHERE ID = @ID";

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;
                        
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

    }
}

