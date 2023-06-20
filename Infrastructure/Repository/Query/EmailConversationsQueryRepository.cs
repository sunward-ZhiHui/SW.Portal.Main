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
    public class EmailConversationsQueryRepository : QueryRepository<EmailConversations>, IEmailConversationsQueryRepository
    {
        public EmailConversationsQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<long> Delete(EmailConversations forumConversations)
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
                            var parameters = new DynamicParameters();                            
                            parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                            parameters.Add("Message", forumConversations.Message, DbType.String);
                            parameters.Add("ID", forumConversations.ID, DbType.Int64);
                            parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                            parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);

                            var query = "DELETE  FROM EmailConversations WHERE ID = @ID";


                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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

        public async Task<IReadOnlyList<ViewEmployee>> GetAllParticipantAsync(long topicId)
        {
            try
            {
                var query = @"select  * from View_Employee where (StatusName!='Resign' or StatusName is null) and UserID NOT IN (SELECT AU.UserID FROM EmailTopicParticipant TP INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID WHERE TP.TopicId = @TopicId)";
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
        public async Task<List<EmailAssignToList>> GetAllAssignToListAsync(long topicId)
        {
            try
            {
                //var query = @"SELECT FT.UserId,E.FirstName FROM EmailTopicTo FT
                //                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                //                INNER JOIN Employee E ON E.UserID = FT.UserId
                //                WHERE TopicId = @TopicId
                //UNION
                //SELECT FC.UserId,E.FirstName FROM EmailTopicCC FC
                //                INNER JOIN ApplicationUser AU ON AU.UserID = FC.UserId
                //                INNER JOIN Employee E ON E.UserID = FC.UserId
                //                WHERE TopicId = @TopicId";

                var query = @"SELECT FT.UserId,E.FirstName FROM EmailTopicParticipant FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
                                WHERE TopicId = @TopicId";

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
                var query = @"SELECT FT.UserId,E.FirstName FROM EmailTopicTo FT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FT.UserId
                                INNER JOIN Employee E ON E.UserID = FT.UserId
                                WHERE TopicId = @TopicId";
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

        public async Task<long> DeleteParticipant(TopicParticipant topicParticipant)
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
                            var parameters = new DynamicParameters();
                            parameters.Add("ID", topicParticipant.ID, DbType.Int64);                           

                            var query = "DELETE  FROM EmailTopicParticipant WHERE ID = @ID";


                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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
                    connection.Open();
                    var res = connection.Query<EmailConversations>(query, parameters).ToList();
                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailConversations>> GetDiscussionListAsync(long TopicId)
        {
            try
            {

                var query = @"SELECT FC.Name,FC.ID,FC.SessionId,FC.AddedDate,FC.Message,AU.UserName,AU.UserID,FC.ReplyId,FC.FileData FROM EmailConversations FC                                
                                INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                INNER JOIN Employee EMP ON EMP.UserID = AU.UserID                               
                                WHERE FC.TopicId = @TopicId AND FC.ReplyId = 0 ORDER BY FC.AddedDate ASC";


                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    connection.Open();
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
                                        FC.SessionId,FC.FileData
                                    FROM
                                        EmailConversations FC
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                    WHERE
                                        FC.TopicId = @TopicId AND FC.ReplyId = @ReplyId";

                            var parameterss = new DynamicParameters();
                            parameterss.Add("TopicId", TopicId, DbType.Int64);
                            parameterss.Add("ReplyId", topic.ID, DbType.Int64);
                        var subQueryResults = connection.Query<EmailConversations>(subQuery, parameterss).ToList();                     


                        var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";
                                   
                        var parametersDocs = new DynamicParameters();
                        parametersDocs.Add("SessionID", topic.SessionId);                                       

                        var subQueryDocsResults = connection.Query<Documents>(subQueryDocs, parametersDocs).ToList();

                        var subQueryassignTo = @"select E.FirstName,FCA.UserId,FCA.TopicId from EmailConversationAssignTo FCA
                                        INNER JOIN Employee E on E.UserID = FCA.UserId
                                        where FCA.ConversationId = @Id";
                        var parametersassignTo = new DynamicParameters();
                        parametersassignTo.Add("Id", topic.ID, DbType.Int64);
                        var subQueryAssignToResults = connection.Query<EmailAssignToList>(subQueryassignTo, parametersassignTo).ToList();


                        topic.ReplyConversation = subQueryResults;
                        topic.documents = subQueryDocsResults;
                        topic.AssignToList = subQueryAssignToResults;

                        foreach (var conversation in topic.ReplyConversation)
                        {
                            var subQueryReplyDocs = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                        INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                        WHERE D.SessionID = @SessionID";
                            var parametersReplyDocs = new DynamicParameters();
                            parametersReplyDocs.Add("SessionID", conversation.SessionId);
                            parametersReplyDocs.Add("ReplyId", conversation.ReplyId, DbType.Int64);                          

                            var subQueryReplyDocsResults = connection.Query<Documents>(subQueryReplyDocs, parametersReplyDocs).ToList();

                            var replysubQueryassignTo = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                            var replyparametersassignTo = new DynamicParameters();
                            replyparametersassignTo.Add("ConversationId", conversation.ID, DbType.Int64);
                            var replysubQueryAssignToResults = connection.Query<EmailAssignToList>(replysubQueryassignTo, replyparametersassignTo).ToList();



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

                            conversation.ReplyConversation = replyBysubQueryResults;


                            foreach (var conversations in conversation.ReplyConversation)
                            {


                                var subQueryReplyDocs1 = @"select D.DocumentID,D.FileName,D.ContentType,D.FileSize,D.FilePath,FC.FileData,FC.Name from EmailConversations FC
                                                        INNER JOIN Documents D ON D.SessionID = FC.SessionId
                                                        WHERE D.SessionID = @SessionID";
                                var parametersReplyDocs1 = new DynamicParameters();
                                parametersReplyDocs1.Add("SessionID", conversations.SessionId);
                                parametersReplyDocs1.Add("ReplyId", conversations.ReplyId, DbType.Int64);

                                var subQueryReplyDocsResultss = connection.Query<Documents>(subQueryReplyDocs1, parametersReplyDocs1).ToList();

                                var replysubQueryassignTo1 = @"SELECT FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                                var replyparametersassignTo1 = new DynamicParameters();
                                replyparametersassignTo1.Add("ConversationId", conversations.ID, DbType.Int64);
                                var replysubQueryAssignToResultss = connection.Query<EmailAssignToList>(replysubQueryassignTo1, replyparametersassignTo1).ToList();

                                conversations.documents = subQueryReplyDocsResultss;
                                conversations.AssignToList = replysubQueryAssignToResultss;

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

        public async Task<List<Documents>> GetTopicDocListAsync(long TopicId)
        {
            try
            {

                var query = @"select FileIndex = ROW_NUMBER() OVER(ORDER BY D.DocumentID DESC),D.DocumentID as DocumentId,D.FileName,D.ContentType,D.FileSize,D.UploadDate,D.SessionID,D.AddedDate,D.FilePath,FC.FileData,FC.Name from EmailConversations FC 
                                INNER JOIN Documents D on D.SessionID = FC.SessionId
                                where FC.TopicID = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<Documents>(query, parameters).ToList();
                    return res;
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

        public async Task<List<EmailConversationAssignTo>> GetConversationAssignToList(long ConversationId)
        {
            try
            {
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY FCT.ID DESC), FCT.ID,FCT.UserId,E.FirstName,E.LastName from EmailConversationAssignTo FCT
                                INNER JOIN ApplicationUser AU ON AU.UserID = FCT.UserId
                                INNER JOIN Employee E ON E.UserID = FCT.UserId
                                WHERE FCT.ConversationId = @ConversationId";
                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<EmailConversationAssignTo>(query, parameters).ToList();
                    return res;                   
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(EmailConversations forumConversations)
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
                            parameters.Add("Name", forumConversations.Name);

                            var query = "INSERT INTO EmailConversations(TopicID,Message,ParticipantId,ReplyId,StatusCodeID,AddedByUserID,SessionId,AddedDate,FileData,Name) OUTPUT INSERTED.ID VALUES (@TopicID,@Message,@ParticipantId,@ReplyId,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate,@FileData,@Name)";


                            //var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);
                            var lastInsertedRecordId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                           

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


                            transaction.Commit();

                            return lastInsertedRecordId;
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
        public async Task<long> InsertAssignTo(EmailConversationAssignTo conversationAssignTo)
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
                            var parameters = new DynamicParameters();                            
                            parameters.Add("UserID", conversationAssignTo.UserId);
                            parameters.Add("ConversationId", conversationAssignTo.ConversationId);
                            parameters.Add("TopicId", conversationAssignTo.TopicId);
                            parameters.Add("StatusCodeID", conversationAssignTo.StatusCodeID);
                            parameters.Add("AddedByUserID", conversationAssignTo.AddedByUserID);
                            parameters.Add("SessionId", conversationAssignTo.SessionId);
                            parameters.Add("AddedDate", conversationAssignTo.AddedDate);

                            var query = "INSERT INTO EmailConversationAssignTo(ConversationId,TopicId,UserID,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@ConversationId,@TopicId,@UserID,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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


        public async Task<long> InsertEmailNotifications(EmailNotifications forumNotifications)
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
                            var parameters = new DynamicParameters();
                            parameters.Add("UserID", forumNotifications.UserId);
                            parameters.Add("ConversationId", forumNotifications.ConversationId);
                            parameters.Add("TopicId", forumNotifications.TopicId);                            
                            parameters.Add("AddedByUserID", forumNotifications.AddedByUserID);                            
                            parameters.Add("AddedDate", forumNotifications.AddedDate);
                            parameters.Add("IsRead", forumNotifications.IsRead);                           

                            var query = "INSERT INTO EmailNotifications(ConversationId,TopicId,UserID,AddedByUserID,AddedDate,IsRead) VALUES (@ConversationId,@TopicId,@UserID,@AddedByUserID,@AddedDate,@IsRead)";

                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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

        public async Task<long> Update(EmailConversations forumConversations)
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
                            var parameters = new DynamicParameters();
                            parameters.Add("TopicID", forumConversations.TopicID, DbType.Int64);
                            parameters.Add("Message", forumConversations.Message, DbType.String);
                            parameters.Add("ID", forumConversations.ID, DbType.Int64);
                            parameters.Add("ParticipantId", forumConversations.ParticipantId, DbType.Int64);
                            parameters.Add("ReplyId", forumConversations.ReplyId, DbType.Int64);

                            var query = " UPDATE EmailConversations SET Name = @Name ,TopicID = @TopicID ,Message=@Message,ParticipantId =@ParticipantId,ReplyId=@ReplyId WHERE ID = @ID";


                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return rowsAffected;
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

