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
    public class EmailTopicsQueryRepository : QueryRepository<EmailTopics>, IEmailTopicsQueryRepository
    {
        public EmailTopicsQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public async Task<IReadOnlyList<EmailTopics>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM EmailTypes";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailTopics>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailTopics>> GetByIdAsync(long id)
        {
            try
            {
                var query = @"SELECT * FROM EmailTopics TS
                            INNER JOIN Employee E ON TS.TopicFrom = E.UserID
                            WHERE ID = @Id";
                //var query = @"SELECT * FROM EmailTopics TS 
                //                INNER JOIN EmailTypes TP ON TS.TypeId = TP.ID                                
                //                WHERE TS.ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<EmailTopics>(query, parameters).ToList();

                    var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";

                    var parametersDocs = new DynamicParameters();
                    parametersDocs.Add("SessionID", res[0].SessionId);

                    var subQueryDocsResults = connection.Query<Documents>(subQueryDocs, parametersDocs).ToList();


                    var subQueryTo = @"select E.FirstName,FT.UserId,FT.TopicId from EmailtopicTo FT
                                        INNER JOIN Employee E on E.UserID = FT.UserId
                                        where FT.TopicId = @ID";
                    var parametersTo = new DynamicParameters();
                    parametersTo.Add("ID", res[0].ID);
                    var subQueryToResults = connection.Query<EmailAssignToList>(subQueryTo, parametersTo).ToList();


                    var subQueryCC = @"select E.FirstName,FC.UserId,FC.TopicId from EmailtopicCC FC
                                        INNER JOIN Employee E on E.UserID = FC.UserId
                                        where FC.TopicId = @ID";
                    var parametersCC = new DynamicParameters();
                    parametersCC.Add("ID", res[0].ID);
                    var subQueryCCResults = connection.Query<EmailAssignToList>(subQueryCC, parametersCC).ToList();


                    res[0].documents = subQueryDocsResults;
                    res[0].TopicToList = subQueryToResults;
                    res[0].TopicCCList = subQueryCCResults;

                    return res;

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailTopics>> GetUserTopicList(long UserId)
        {
            try
            {
                //var query = "SELECT * FROM EmailTypes WHERE ID = @UserId";
                var query = @"SELECT TS.ID,TS.TicketNo,TS.TopicName,TS.TypeId,TS.CategoryId,TS.Remarks,TS.SeqNo,TS.Status FROM EmailTopics TS 
                                INNER JOIN EmailTopicParticipant TP ON TS.ID = TP.TopicId                                
                                WHERE TP.UserId = @UserId";
                                
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    connection.Open();                    

                    var res = connection.Query<EmailTopics>(query,parameters).ToList();

                    //var result = res
                    //    .GroupBy(ps => ps.TicketNo)
                    //    .Select(g => new EmailTopics
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
        public async Task<List<EmailTopics>> GetTopicToList(long UserId)
        {
            try
            {
                //var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                //                INNER JOIN EmailTopicTo TP ON TS.ID = TP.TopicId 
                //                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                         
                //                WHERE TP.UserId = @UserId order by TS.StartDate DESC";



                var query = @"SELECT TS.ID, TS.TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,
                                TS.Urgent,
                                TS.OverDue,
                                TS.DueDate,
                                TS.StartDate,
                                TS.FileData,
                                TS.SessionId,
                                E.FirstName,
                                E.LastName,
                                COALESCE(FN.NotificationCount, 0) AS NotificationCount
                            FROM
                                EmailTopics TS
                            INNER JOIN
                                EmailTopicParticipant TP ON TS.ID = TP.TopicId
                            INNER JOIN
                                Employee E ON TS.TopicFrom = E.UserId
                            LEFT JOIN
                                (
                                    SELECT
                                        TopicId,
                                        COUNT(*) AS NotificationCount
                                    FROM
                                        EmailNotifications WHERE UserId = @UserId
                                    GROUP BY
                                        TopicId
                                ) FN ON TS.ID = FN.TopicId
                            WHERE
                                TP.UserId = @UserId
                            ORDER BY
                                TS.StartDate DESC";




                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<EmailTopics>(query, parameters).ToList();
                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailTopics>> GetTopicCCList(long UserId)
        {
            try
            {
                var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                                INNER JOIN EmailTopicCC TP ON TS.ID = TP.TopicId
                                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                                    
                                WHERE TP.UserId = @UserId order by TS.StartDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    var res = connection.Query<EmailTopics>(query, parameters).ToList();
                    return res;
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
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY TP.ID DESC), TP.ID,TP.TopicId,AU.UserCode,AU.UserName,TP.AddedDate,TP.SessionId,AU.UserID, CASE WHEN FT.AddedByUserID = TP.UserID THEN 0 ELSE 1 END AS IsEnabled FROM EmailTopicParticipant TP 
                                INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID   
								INNER JOIN EmailTopics FT ON FT.ID = TP.TopicId                               
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

      
        public async Task<EmailTopics> GetCustomerByEmail(string name)
        {
            try
            {              
                var query = "SELECT * FROM EmailTypes WHERE Name = @Name";
                var parameters = new DynamicParameters();
                parameters.Add("Name", name, DbType.String);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<EmailTopics>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<EmailTopics> GetTopicListAsync()
        {
            try
            {
                long id = 1;
                var query = "SELECT * FROM EmailTypes WHERE ID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("ID", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<EmailTopics>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public long Insert(EmailTopics EmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {                   

                    try
                    {
                        var parameterss = new DynamicParameters();
                        parameterss.Add("TicketNo", EmailTopics.TicketNo);
                        parameterss.Add("TopicName", EmailTopics.TopicName);
                        parameterss.Add("TypeId", EmailTopics.TypeId);
                        parameterss.Add("CategoryId", EmailTopics.CategoryId);                        
                        parameterss.Add("StartDate", EmailTopics.StartDate);
                        parameterss.Add("Description", EmailTopics.Description);
                        //parameterss.Add("EndDate", EmailTopics.EndDate);
                        //parameterss.Add("DueDate", EmailTopics.DueDate);
                        parameterss.Add("AddedByUserID", EmailTopics.AddedByUserID);
                        parameterss.Add("AddedDate", EmailTopics.AddedDate);
                        parameterss.Add("StatusCodeID", EmailTopics.StatusCodeID);
                        parameterss.Add("SessionId", EmailTopics.SessionId);
                        parameterss.Add("FileData", EmailTopics.FileData);

                        parameterss.Add("Follow", EmailTopics.Follow);
                        parameterss.Add("OnBehalf", EmailTopics.OnBehalf);
                        parameterss.Add("Urgent", EmailTopics.Urgent);
                        parameterss.Add("OverDue", EmailTopics.OverDue);
                        parameterss.Add("DueDate", EmailTopics.DueDate);
                        

                        parameterss.Add("To", EmailTopics.To);
                        parameterss.Add("CC", EmailTopics.CC);
                        parameterss.Add("Participants", EmailTopics.Participants);

                        connection.Open();

                        var result = connection.QueryFirstOrDefault<long>("sp_Ins_EmailTopics", parameterss, commandType: CommandType.StoredProcedure);
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
                    //        parameters.Add("TicketNo", EmailTopics.TicketNo, DbType.String);
                    //        parameters.Add("TopicName", EmailTopics.TopicName, DbType.String);

                    //        var query = "INSERT INTO EmailTopics(TicketNo, TopicName) VALUES (@TicketNo, @TopicName)";
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
                            

                            var query = "INSERT INTO EmailTopicParticipant(TopicID, UserId,StatusCodeID,AddedByUserID,AddedDate,SessionId) VALUES (@TopicID, @UserId,@StatusCodeID,@AddedByUserID,@AddedDate,@SessionId)";
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

        public async Task<long> UpdateDueDate(EmailTopics EmailTopics)
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
                            parameters.Add("DueDate", EmailTopics.DueDate);
                            parameters.Add("ID", EmailTopics.ID);                           

                            var query = " UPDATE EmailTopics SET DueDate = @DueDate WHERE ID = @ID";

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
        public async Task<long> UpdateTopicClose(EmailTopics EmailTopics)
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
                            parameters.Add("Remarks", EmailTopics.Remarks);
                            parameters.Add("ID", EmailTopics.ID);

                            var query = " UPDATE EmailTopics SET Remarks = @Remarks, Status ='closed' WHERE ID = @ID";

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
