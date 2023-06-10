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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Application.Queries;
using Microsoft.AspNetCore.Http;
using DevExpress.Data.Filtering.Helpers;

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

        public async Task<List<ForumTopics>> GetByIdAsync(long id)
        {
            try
            {
                //var query = "SELECT * FROM ForumTopics WHERE ID = @Id";
                var query = @"SELECT * FROM ForumTopics TS 
                                INNER JOIN ForumTypes TP ON TS.TypeId = TP.ID                                
                                WHERE TS.ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<ForumTopics>(query, parameters).ToList();
                    return res;

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<ForumTopics>> GetUserTopicList(long UserId)
        {
            try
            {
                //var query = "SELECT * FROM ForumTypes WHERE ID = @UserId";
                var query = @"SELECT TS.ID,TS.TicketNo,TS.TopicName,TS.TypeId,TS.CategoryId,TS.Remarks,TS.SeqNo,TS.Status FROM ForumTopics TS 
                                INNER JOIN ForumTopicParticipant TP ON TS.ID = TP.TopicId                                
                                WHERE TP.UserId = @UserId";
                                
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    connection.Open();                    

                    var res = connection.Query<ForumTopics>(query,parameters).ToList();

                    //var result = res
                    //    .GroupBy(ps => ps.TicketNo)
                    //    .Select(g => new ForumTopics
                    //    {
                    //        Year = g.Key,
                    //        SalesByProduct = g.ToList()
                    //    })
                    //    .ToList();

                    //return result;


                    return res;

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<ForumTopics>> GetTreeTopicList(long UserId)
        {
            List<ForumTopics> tfrom = new List<ForumTopics>();
            try
            {
                //var query = "SELECT * FROM ForumTypes WHERE ID = @UserId";
                var query = @"SELECT TS.ID,TS.TicketNo,TS.EndDate,TS.TopicName,TS.TypeId,TS.CategoryId,TS.Remarks,TS.SeqNo,TPS.Name as TypeName FROM ForumTopics TS 
                                INNER JOIN ForumTopicParticipant TP ON TS.ID = TP.TopicId 
                                INNER JOIN ForumTypes TPS ON TS.TypeId = TPS.ID 
                                WHERE TP.UserId = @UserId";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    connection.Open();

                    var res = connection.Query<ForumTopics>(query, parameters).ToList();

                    var result = res.AsEnumerable().GroupBy(x => new { x.TicketNo }).Select(y => new ForumTopics
                        {
                            Label = y.Key.TicketNo != "" ? y.Key.TicketNo : "N/A",
                            children = y == null || !y.Any() ? tfrom : y.GroupBy(x => new { x.TypeName}).Select(z => new ForumTopics
                            {
                                Label = z.Key.TypeName,
                                children = z == null || !z.Any() ? tfrom : z.Select(x => new ForumTopics
                                {
                                    ID = x.ID,
                                    SeqNo = x.SeqNo,
                                    Label = x.TopicName,
                                    TopicName = x.TopicName,
                                    Description = x.Description,
                                    Status = x.Status,
                                    EndDate = x.EndDate,                                  
                                    Remarks = x.Remarks,

                                }).OrderBy(z => z.SeqNo).ToList()
                                }).ToList()
                            }).ToList();

                    return result;


                    //return res;

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<TopicParticipant>> GetParticipantList(long topicId)
        {
            try
            {
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY TP.ID DESC), TP.ID,TP.TopicId,AU.UserCode,AU.UserName,TP.AddedDate,TP.SessionId,AU.UserID, CASE WHEN FT.AddedByUserID = TP.UserID THEN 0 ELSE 1 END AS IsEnabled FROM ForumTopicParticipant TP 
                                INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID   
								INNER JOIN ForumTopics FT ON FT.ID = TP.TopicId                               
                                WHERE TP.TopicId = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", topicId);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    return (await connection.QueryAsync<TopicParticipant>(query,parameters)).ToList();
                   // var result = connection.QueryAsync<TopicParticipant>(query, parameters).ToList();
                    //return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<ForumCategorys>> GetCategoryByTypeId(long id)
        {
            try
            {
                var query = "SELECT * FROM ForumCategorys WHERE TypeID = @TypeID";
                var parameters = new DynamicParameters();
                parameters.Add("TypeID", id);
                
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<ForumCategorys>(query,parameters)).ToList();
                    return result;
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
                        parameterss.Add("TypeId", forumTopics.TypeId);
                        parameterss.Add("CategoryId", forumTopics.CategoryId);                        
                        parameterss.Add("StartDate", forumTopics.StartDate);
                        parameterss.Add("Description", forumTopics.Description);
                        //parameterss.Add("EndDate", forumTopics.EndDate);
                        //parameterss.Add("DueDate", forumTopics.DueDate);
                        parameterss.Add("AddedByUserID", forumTopics.AddedByUserID);
                        parameterss.Add("AddedDate", forumTopics.AddedDate);
                        parameterss.Add("StatusCodeID", forumTopics.StatusCodeID);
                        parameterss.Add("SessionId", forumTopics.SessionId);
                        parameterss.Add("FileData", forumTopics.FileData);

                        parameterss.Add("Follow", forumTopics.Follow);
                        parameterss.Add("OnBehalf", forumTopics.OnBehalf);
                        parameterss.Add("Urgent", forumTopics.Urgent);
                        parameterss.Add("OverDue", forumTopics.OverDue);
                        parameterss.Add("DueDate", forumTopics.DueDate);
                        

                        parameterss.Add("To", forumTopics.To);
                        parameterss.Add("CC", forumTopics.CC);
                        parameterss.Add("Participants", forumTopics.Participants);

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
        public async Task<long> InsertParticipant(TopicParticipant topicParticipant)
        {
            var rowsAffected = 0;
            var result = 1;
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        string[] values = topicParticipant.PList.Split(',');
                      
                        foreach (var item in values)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("UserId", item, DbType.Int64);
                            parameters.Add("TopicId", topicParticipant.TopicId);
                            parameters.Add("SessionId", topicParticipant.SessionId);
                            parameters.Add("AddedDate", topicParticipant.AddedDate);
                            parameters.Add("AddedByUserID", topicParticipant.AddedByUserID);
                            parameters.Add("StatusCodeID", topicParticipant.StatusCodeID);
                            

                            var query = "INSERT INTO ForumTopicParticipant(TopicID, UserId,StatusCodeID,AddedByUserID,AddedDate,SessionId) VALUES (@TopicID, @UserId,@StatusCodeID,@AddedByUserID,@AddedDate,@SessionId)";
                            rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);
                           
                            //return rowsAffected;
                        }
                        transaction.Commit();
                        return result;
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> UpdateDueDate(ForumTopics forumTopics)
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
                            parameters.Add("DueDate", forumTopics.DueDate);
                            parameters.Add("ID", forumTopics.ID);                           

                            var query = " UPDATE ForumTopics SET DueDate = @DueDate WHERE ID = @ID";

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
        public async Task<long> UpdateTopicClose(ForumTopics forumTopics)
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
                            parameters.Add("Remarks", forumTopics.Remarks);
                            parameters.Add("ID", forumTopics.ID);

                            var query = " UPDATE ForumTopics SET Remarks = @Remarks, Status ='closed' WHERE ID = @ID";

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
