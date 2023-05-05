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
using Application.Response;

namespace Infrastructure.Repository.Query
{
    public class ForumTopicsQueryRepository : QueryRepository<ForumTopics>, IForumTopicsQueryRepository
    {
        public ForumTopicsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<ForumTopics>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ForumTypes";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ForumTopics>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ForumTopics> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM ForumTypes WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumTopics>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ForumTopics> GetCustomerByEmail(string name)
        {
            try
            {              
                var query = "SELECT * FROM ForumTypes WHERE Name = @Name";
                var parameters = new DynamicParameters();
                parameters.Add("Name", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumTopics>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<ForumTopics> GetTopicListAsync()
        {
            try
            {
                long id = 1;
                var query = "SELECT * FROM ForumTypes WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ForumTopics>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public long Insert(ForumTopics forumTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    

                    try
                    {
                        var parameterss = new DynamicParameters();
                        parameterss.Add("TicketNo", forumTopics.TicketNo);
                        parameterss.Add("TopicName", forumTopics.TopicName);
                        parameterss.Add("AddedByUserID", forumTopics.AddedByUserID);
                        parameterss.Add("AddedDate", forumTopics.AddedDate);
                        parameterss.Add("StatusCodeID", forumTopics.StatusCodeID);
                        parameterss.Add("SessionId", forumTopics.SessionId);

                        connection.Open();

                        var result = connection.QueryFirstOrDefault<long>("sp_Ins_ForumTopics", parameterss, commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    catch(Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                   

                    //connection.Open();
                    //using (var transaction = connection.BeginTransaction())
                    //{

                    //    try
                    //    {
                    //        var parameters = new DynamicParameters();
                    //        parameters.Add("TicketNo", forumTopics.TicketNo, DbType.String);
                    //        parameters.Add("TopicName", forumTopics.TopicName, DbType.String);

                    //        var query = "INSERT INTO ForumTopics(TicketNo, TopicName) VALUES (@TicketNo, @TopicName)";
                    //        var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                           
                           
                    //        transaction.Commit();

                    //        return rowsAffected;
                    //    }
                    //    catch (Exception exp)
                    //    {
                    //        transaction.Rollback();
                    //        throw new Exception(exp.Message, exp);
                    //    }
                    //}
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }       
    }
}
