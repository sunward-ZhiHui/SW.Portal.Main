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

                            var query = "DELETE  FROM ForumConversations WHERE ID = @ID";


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

                            var query = "DELETE  FROM ForumTopicParticipant WHERE ID = @ID";


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

                var query = @"SELECT FC.ID,FC.AddedDate,FC.Message,AU.UserName,AU.UserID,FC.ReplyId,FCS.Message as ReplyMessage,FCS.AddedDate as ReplyDateTime,RAU.UserName as ReplyUserName FROM ForumConversations FC 
                                LEFT JOIN ForumConversations FCS ON FCS.ID = FC.ReplyId
                                LEFT JOIN ApplicationUser RAU ON RAU.UserID = FCS.AddedByUserID
                                INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                INNER JOIN Employee EMP ON EMP.UserID = AU.UserID
                                WHERE FC.TopicId = @TopicId AND FC.ReplyId = 0 ORDER BY FC.AddedDate ASC;";


                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<ForumConversations>(query,parameters).ToList();

                    foreach (var topic in res)
                    {
                       
                            var subQuery = @"SELECT
                                        FC.ID,
                                        FC.AddedDate,
                                        FC.Message,
                                        AU.UserName,
                                        AU.UserID,
                                        FC.ReplyId
                                    FROM
                                        ForumConversations FC
                                        INNER JOIN ApplicationUser AU ON AU.UserID = FC.ParticipantId
                                    WHERE
                                        FC.TopicId = @TopicId AND FC.ReplyId = @ReplyId";

                            var parameterss = new DynamicParameters();
                            parameterss.Add("TopicId", TopicId, DbType.Int64);
                            parameterss.Add("ReplyId", topic.ID, DbType.Int64);

                            // Execute the subquery using Dapper's Query method and pass in the topic ID as a parameter
                            var subQueryResults = connection.Query<ForumConversations>(subQuery, parameterss).ToList();

                            // Assign the subquery results to the topic's Conversations property
                            topic.ReplyConversation = subQueryResults;
                        
                       

                    }

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

        public async Task<long> Insert(ForumConversations forumConversations)
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

                            var query = "INSERT INTO ForumConversations(TopicID,Message,ParticipantId,ReplyId,StatusCodeID,AddedByUserID,SessionId,AddedDate) VALUES (@TopicID,@Message,@ParticipantId,@ReplyId,@StatusCodeID,@AddedByUserID,@SessionId,@AddedDate)";


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

        public async Task<long> Update(ForumConversations forumConversations)
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

                            var query = " UPDATE ForumConversations SET Name = @Name ,TopicID = @TopicID ,Message=@Message,ParticipantId =@ParticipantId,ReplyId=@ReplyId WHERE ID = @ID";


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

