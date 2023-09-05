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
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

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
                var query = @"SELECT CONCAT(EB.FirstName,EB.LastName) AS OnBehalfName,* FROM EmailTopics TS
                            INNER JOIN Employee E ON TS.TopicFrom = E.UserID
                            LEFT JOIN Employee EB ON TS.OnBehalf = EB.UserID
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
                    if(res.Count > 0)
                    {
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

                    }                   

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

        public async Task<List<EmailTopics>> GetBySessionTopicList(string SessionId)
        {
            try
            {                
                var query = @"SELECT * FROM EmailTopics  WHERE SessionId = @SessionId";

                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);

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
        public async Task<List<EmailTopics>> GetByIdTopicToList(long TopicId)
        {
            try
            {              

                var query = @"SELECT TS.SessionId, TS.ID, TS.TopicName,TS.Remarks,
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
                                concat(E.FirstName,',',E.LastName) as Name,
                                E.NickName,
                                E.FirstName,
                                E.LastName,
                                TP.UserId
                               
                            FROM
                                EmailTopics TS
                            INNER JOIN
                                EmailConversationAssignTo TP ON TS.ID = TP.TopicId
                            INNER JOIN
                                Employee E ON TP.UserId = E.UserId
                          
                            WHERE
                                TS.ID = @TopicId
                            ORDER BY
                                TS.StartDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

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

        public async Task<List<EmailTopics>> GetByIdTopicCCList(long TopicId)
        {
            try
            {
                var query = @"SELECT TS.SessionId, TS.ID, TS.TopicName,TS.Remarks,
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
                                concat(E.FirstName,',',E.LastName) as Name,
                                E.NickName,
                                E.FirstName,
                                E.LastName,
                                TP.UserId
                               
                            FROM
                                EmailTopics TS
                            INNER JOIN
                                EmailConversationAssignCC TP ON TS.ID = TP.TopicId
                            INNER JOIN
                                Employee E ON TP.UserId = E.UserId                          
                            WHERE
                                TS.ID = @TopicId
                            ORDER BY
                                TS.StartDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

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
        public async Task<List<EmailTopics>> GetTopicMasterSearchList(EmailSearch emailSearch)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", emailSearch.UserID);
                        parameters.Add("searchtxt", emailSearch.MSearchText);
                        parameters.Add("fromids", emailSearch.ByFrom);
                        parameters.Add("subject", emailSearch.BySubject);
                        parameters.Add("tag", emailSearch.ByTag);

                        parameters.Add("GroupTag", emailSearch.GroupTag);
                        parameters.Add("CategoryTag", emailSearch.CategoryTag);
                        parameters.Add("ActionTag", emailSearch.ActionTag);
                        parameters.Add("Name", emailSearch.Name);

                        parameters.Add("filterFrom", emailSearch.FilterFrom);
                        parameters.Add("filterTo", emailSearch.FilterTo);
                        parameters.Add("Option", "SELECT");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_MasterSearchList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailTopics>> GetTopicMasterSearchList_old(long UserId,string searchtxt)
        {
            try
            {
                var query = @"SELECT DISTINCT TS.ID, TS.SessionId,  TS.TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,
                                COALESCE(TS.Urgent, 0) AS Urgent,
                                COALESCE(TS.OverDue, 0) AS OverDue,                               
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
                                EmailConversations EC ON EC.TopicId = TS.ID
							LEFT JOIN ActivityEmailTopics AET ON AET.EmailTopicSessionId = EC.SessionId
							LEFT JOIN ApplicationMasterChild AMC ON AMC.ApplicationMasterChildID = AET.ManufacturingProcessId
							LEFT JOIN ApplicationMasterChild CAI ON CAI.ApplicationMasterChildID = AET.CategoryActionId
							LEFT JOIN ApplicationMasterChild AI ON AI.ApplicationMasterChildID = AET.ActionId
                            CROSS APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
							            FROM EmailConversations ECC 
										WHERE --ECC.TopicID=@TopicId 
										--AND 
										(ECC.ParticipantId = @UserId
										 OR
										 EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
										 OR 
										 EXISTS(SELECT * FROM EmailConversationAssignCC TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
										  OR 
										 EXISTS(SELECT * FROM EmailConversationParticipant TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
										)
							           )K
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
                                TS.OnDraft = 0 and EC.ID=K.ReplyId  /*and EC.ReplyId = 0*/
                                AND (E.FirstName LIKE '%' + @searchtxt + '%' OR TS.TopicName LIKE '%' + @searchtxt + '%' OR TS.Remarks LIKE '%' + @searchtxt + '%' OR TS.Follow LIKE '%' + @searchtxt + '%' OR AMC.Value LIKE '%' + @searchtxt + '%' OR CAI.Value LIKE '%' + @searchtxt + '%' OR AI.Value LIKE '%' + @searchtxt + '%')
                            ORDER BY
                                TS.StartDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);
                parameters.Add("searchtxt", searchtxt);                

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
        public async Task<List<EmailTopics>> GetTopicAllList(long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("Option", "SELECT_ASSIGN_ALL");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<long> SetPinTopicToList(long id)
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
                            parameters.Add("id", id);

                            var query = "UPDATE EmailTopics SET PinStatus = 'Lock' WHERE ID = @id";


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
        public async Task<long> UpdateMarkasAllReadList(long id,long userId)
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
                            parameters.Add("id", id);
                            parameters.Add("userId", userId);

                            var query = "UPDATE EmailNotifications SET IsRead = 1 where UserId = @userId and IsRead = 0 and ConversationId in (select ID from EmailConversations where ReplyId = @id)";


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

        public async Task<long> UpdateMarkasReadList(long id)
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
                            parameters.Add("id", id);

                            var query = "UPDATE EmailNotifications SET IsRead = 1 WHERE ID = @id";


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
        public async Task<long> UpdateMarkasunReadList(long id)
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
                            parameters.Add("id", id);

                            var query = "UPDATE EmailNotifications SET IsRead = 0 WHERE ID = @id";


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
        public async Task<long> UnSetPinTopicToList(long id)
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
                            parameters.Add("id", id);

                            var query = "UPDATE EmailTopics SET PinStatus = 0 WHERE ID = @id";


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
        public async Task<List<EmailTopics>> GetTopicToList(long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);                        
                        parameters.Add("Option", "SELECT_ASSIGN_TO");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetTopicCCList(long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("Option", "SELECT_ASSIGN_CC");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetTopicSentList(long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("Option", "SELECT_ASSIGN_SENT");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailTopics>> GetTopicDraftList(long UserId)
        {
            try
            {               
                var query = @"SELECT TS.SessionId, TS.ID, TS.TopicName,TS.Remarks,
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
                                TS.IsAllowParticipants
                            FROM
                                EmailTopics TS                            
                            INNER JOIN
                                Employee E ON TS.TopicFrom = E.UserId                          
                            WHERE
                                 TS.AddedByUserID = @UserId and TS.OnDraft = 1
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
        public long DeleteTopicDraftList(long TopicId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("TopicId", TopicId);
                        parameters.Add("Option", "EMAIL_DRAFT_DELETE");                       
                        connection.Open();
                        var task = connection.ExecuteAsync("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailTopics>> GetSubTopicSearchAllList(long TopicId, long UserId,string SearchTxt)
        {
            try
            {
                //var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                //                INNER JOIN EmailTopicTo TP ON TS.ID = TP.TopicId 
                //                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                         
                //                WHERE TP.UserId = @UserId order by TS.StartDate DESC";



                var query = @"SELECT TS.ID,EC.ID as ReplyId,EC.Name as TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,
                                TS.Urgent,
                                TS.OverDue,
                                TS.DueDate,
                                EC.AddedDate as StartDate,
                                TS.FileData,
                                TS.SessionId,
                                E.FirstName,
                                E.LastName,
                                COALESCE(FN.NotificationCount, 0) AS NotificationCount
                            FROM
                                EmailTopics TS
                         INNER JOIN
                                EmailConversations EC ON TS.ID = EC.TopicId
                            Cross APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
							            FROM EmailConversations ECC 
										WHERE ECC.TopicID=@TopicId 
										AND EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
							           )K
                            INNER JOIN
                                Employee E ON EC.AddedByUserID = E.UserId
                             LEFT JOIN
                            (
                                SELECT
                                    ReplyId,
                                    COUNT(*) AS NotificationCount
                                FROM
                                    EmailConversations
                                GROUP BY
                                    ReplyId
                            ) FN ON EC.ID = FN.ReplyId
                            WHERE
                                TS.ID = @TopicId and TS.OnDraft = 0 AND EC.ID=K.ReplyId 
                                AND (TS.StartDate LIKE '%' + @SearchTxt + '%' OR E.FirstName LIKE '%' + @SearchTxt + '%' OR EC.Name LIKE '%' + @SearchTxt + '%' OR TS.Remarks LIKE '%' + @SearchTxt + '%' OR TS.Follow LIKE '%' + @SearchTxt + '%')
                            ORDER BY
                                TS.StartDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);
                parameters.Add("UserId", UserId);
                parameters.Add("SearchTxt", SearchTxt);              

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

        public async Task<List<EmailTopics>> GetSubTopicToList(long TopicId, long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("TopicId", TopicId);
                        parameters.Add("Option", "SUB_SELECT_TO");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetSubTopicCCList(long TopicId, long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("TopicId", TopicId);
                        parameters.Add("Option", "SUB_SELECT_CC");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetSubTopicSentList(long TopicId, long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("TopicId", TopicId);
                        parameters.Add("Option", "SUB_SELECT_SENT");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetSubTopicAllList(long TopicId, long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("TopicId", TopicId);
                        parameters.Add("Option", "SUB_SELECT_ALL");

                        connection.Open();

                        var result = connection.Query<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetSubTopicToList_OLD(long TopicId, long UserId)
            {
            try
            {
                //var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                //                INNER JOIN EmailTopicTo TP ON TS.ID = TP.TopicId 
                //                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                         
                //                WHERE TP.UserId = @UserId order by TS.StartDate DESC";



                var query = @"SELECT TS.ID,EC.ID as ReplyId,EC.Name as TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,                                
                                TS.OverDue,
                                TS.DueDate,
                                EC.AddedDate as StartDate,
                                TS.FileData,
                                TS.SessionId,
                                E.FirstName,
                                E.LastName,
                                EC.Urgent,
                                EC.IsAllowParticipants,
                                COALESCE(FN.NotificationCount, 0) AS NotificationCount
                            FROM
                                EmailTopics TS
                         INNER JOIN
                                EmailConversations EC ON TS.ID = EC.TopicId
                                INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = EC.ID AND ECP.UserId = @UserId
                            Cross APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
							            FROM EmailConversations ECC 
										WHERE ECC.TopicID=@TopicId 
										AND EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
							           )K
                            INNER JOIN
                                Employee E ON EC.AddedByUserID = E.UserId                           
                            OUTER APPLY
							(
								SELECT									
									COUNT(*) AS NotificationCount
								FROM  EmailConversations ECC
								INNER JOIN EmailNotifications EN ON ECC.ID=EN.ConversationId and EN.IsRead = 0
								WHERE EN.TopicId=EC.TopicID AND EN.UserId=ECP.UserId AND EC.ID=ECC.ReplyId
							) FN 
                            WHERE
                                TS.ID = @TopicId and TS.OnDraft = 0 AND EC.ID=K.ReplyId 
                            ORDER BY
                                 EC.AddedDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);
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
       
        public async Task<List<EmailTopics>> GetSubTopicCCList_OLD(long TopicId, long UserId)
        {
            try
            {
                //var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                //                INNER JOIN EmailTopicTo TP ON TS.ID = TP.TopicId 
                //                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                         
                //                WHERE TP.UserId = @UserId order by TS.StartDate DESC";



                var query = @"SELECT TS.ID,EC.ID as ReplyId,EC.Name as TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,
                                TS.Urgent,
                                TS.OverDue,
                                TS.DueDate,
                                EC.AddedDate as StartDate,
                                TS.FileData,
                                TS.SessionId,
                                E.FirstName,
                                E.LastName,
                                COALESCE(FN.NotificationCount, 0) AS NotificationCount
                            FROM EmailTopics TS
                         INNER JOIN EmailConversations EC ON TS.ID = EC.TopicId
                         INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = EC.ID AND ECP.UserId = @UserId
                         Cross APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
							            FROM EmailConversations ECC 
										WHERE ECC.TopicID=@TopicId 
										AND EXISTS(SELECT * FROM EmailConversationAssignCC TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
							           )K
                            INNER JOIN
                                Employee E ON EC.AddedByUserID = E.UserId
                           OUTER APPLY
							(
								SELECT									
									COUNT(*) AS NotificationCount
								FROM  EmailConversations ECC
								INNER JOIN EmailNotifications EN ON ECC.ID=EN.ConversationId and EN.IsRead = 0
								WHERE EN.TopicId=EC.TopicID AND EN.UserId=ECP.UserId AND EC.ID=ECC.ReplyId
							) FN 
                            WHERE
                                TS.ID = @TopicId and TS.OnDraft = 0 AND EC.ID=K.ReplyId 
                            ORDER BY
                                 EC.AddedDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);
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
        public async Task<List<EmailTopics>> GetSubTopicSentList_OLD(long TopicId, long UserId)
        {
            try
            {
                //var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                //                INNER JOIN EmailTopicTo TP ON TS.ID = TP.TopicId 
                //                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                         
                //                WHERE TP.UserId = @UserId order by TS.StartDate DESC";



                var query = @"SELECT TS.ID,EC.ID as ReplyId,EC.Name as TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,
                                TS.Urgent,
                                TS.OverDue,
                                TS.DueDate,
                                EC.AddedDate as StartDate,
                                TS.FileData,
                                TS.SessionId,
                                E.FirstName,
                                E.LastName,
                                COALESCE(FN.NotificationCount, 0) AS NotificationCount
                            FROM
                                EmailTopics TS
                         INNER JOIN
                                EmailConversations EC ON TS.ID = EC.TopicId                           
                            INNER JOIN
                                Employee E ON EC.AddedByUserID = E.UserId
                             LEFT JOIN
                            (
                                SELECT
                                    ReplyId,
                                    COUNT(*) AS NotificationCount
                                FROM
                                    EmailConversations
                                GROUP BY
                                    ReplyId
                            ) FN ON EC.ID = FN.ReplyId
                            WHERE
                                TS.id = @TopicId and TS.OnDraft = 0 AND EC.ReplyId = 0 AND TS.TopicFrom = @UserId and EC.AddedByUserID = @UserId
                            ORDER BY
                                 EC.AddedDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);
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

        public async Task<List<EmailTopics>> GetSubTopicAllList_OLD(long TopicId, long UserId)
        {
            try
            {
                //var query = @"SELECT TS.ID,TS.TopicName,TS.Remarks,TS.SeqNo,TS.Status,TS.Follow,TS.OnBehalf,TS.Urgent,TS.OverDue,TS.DueDate,TS.StartDate,TS.FileData,TS.SessionId,E.FirstName,E.LastName FROM EmailTopics TS 
                //                INNER JOIN EmailTopicTo TP ON TS.ID = TP.TopicId 
                //                INNER JOIN Employee E ON TS.TopicFrom = E.UserId                         
                //                WHERE TP.UserId = @UserId order by TS.StartDate DESC";



                var query = @"SELECT TS.ID,EC.ID as ReplyId,EC.Name as TopicName,TS.Remarks,
                                TS.SeqNo,
                                TS.Status,
                                TS.Follow,
                                TS.OnBehalf,                               
                                TS.OverDue,
                                TS.DueDate,
                                EC.AddedDate as StartDate,
                                TS.FileData,
                                TS.SessionId,
                                E.FirstName,
                                E.LastName,
                                EC.Urgent,
                                EC.IsAllowParticipants,
                                COALESCE(FN.NotificationCount, 0) AS NotificationCount
                            FROM
                                EmailTopics TS
                         INNER JOIN
                                 EmailConversations EC ON EC.TopicId = TS.ID
                            INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = EC.ID AND ECP.UserId = @UserId
                            CROSS APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
							            FROM EmailConversations ECC 
										WHERE ECC.TopicID=@TopicId 
										AND 
										(ECC.OnBehalf = @UserId
										 OR
										 EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
										 OR 
										 EXISTS(SELECT * FROM EmailConversationAssignCC TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
										  OR 
										 EXISTS(SELECT * FROM EmailConversationParticipant TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
										)
							           )K
                            INNER JOIN
                                Employee E ON TS.TopicFrom = E.UserId
                             OUTER APPLY
							(
								SELECT									
									COUNT(*) AS NotificationCount
								FROM  EmailConversations ECC
								INNER JOIN EmailNotifications EN ON ECC.ID=EN.ConversationId and EN.IsRead = 0
								WHERE EN.TopicId=EC.TopicID AND EN.UserId=ECP.UserId AND EC.ID=ECC.ReplyId
							) FN 
                            WHERE
                                TS.OnDraft = 0 and EC.ID=K.ReplyId  /*and EC.ReplyId = 0*/
                            ORDER BY
                                 EC.AddedDate DESC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);
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
        public async Task<List<EmailParticipant>> GetParticipantList(long topicId,long UserId)
        {
            try
            {
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY TP.ID ASC), TP.ID,TP.TopicId,AU.UserCode,AU.UserName,TP.AddedDate,TP.SessionId,AU.UserID,
                                CASE WHEN TP.AddedByUserID = TP.UserID THEN 0 ELSE 1 END AS IsEnabled,ECO.Name as SubjectName,(E.FirstName + ' ' + E.LastName) AS Name,E.NickName,
								D.Name AS DesignationName,p.PlantCode AS CompanyName 
                                FROM EmailConversationParticipant TP 
                                INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID   
                                INNER JOIN Employee E ON E.UserID = AU.UserID
								LEFT JOIN Designation D ON D.DesignationID = E.DesignationID
								LEFT JOIN Plant P ON P.PlantID =E.PlantID
								INNER JOIN EmailConversations ECO ON ECO.ID = TP.ConversationId            
								
								CROSS APPLY(
									SELECT  EC.ID as ReplyId
                            FROM
                                EmailTopics TS
                         INNER JOIN
                                EmailConversations EC ON TS.ID = EC.TopicId
						INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = EC.ID AND ECP.UserId = @UserId
                            Cross APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END
							            FROM EmailConversations ECC 
										WHERE ECC.TopicID=@TopicId 
										AND EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = @UserId or TP.AddedByUserID = @UserId))
							           )K
                            INNER JOIN
                                Employee E ON EC.AddedByUserID = E.UserId
                             LEFT JOIN
                            (
                                SELECT
                                    ReplyId,
                                    COUNT(*) AS NotificationCount
                                FROM
                                    EmailConversations
                                GROUP BY
                                    ReplyId
                            ) FN ON EC.ID = FN.ReplyId
                            WHERE
                                TS.ID = @TopicId and TS.OnDraft = 0 AND EC.ID=K.ReplyId                            
								)R
                                WHERE TP.ConversationId =R.ReplyId order by TP.ID ASC";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", topicId);
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    return (await connection.QueryAsync<EmailParticipant>(query,parameters)).ToList();
                   // var result = connection.QueryAsync<TopicParticipant>(query, parameters).ToList();
                    //return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailParticipant>> GetConversationPList(long ConversationId)
        {
            try
            {
                var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY TP.ID ASC), TP.ID,TP.TopicId,AU.UserCode,AU.UserName,TP.AddedDate,TP.SessionId,AU.UserID,
                                CASE WHEN TP.AddedByUserID = TP.UserID THEN 0 ELSE 1 END AS IsEnabled,ECO.Name as SubjectName,(E.FirstName + ' ' + E.LastName) AS Name,E.NickName,
								D.Name AS DesignationName,p.PlantCode AS CompanyName 
                                FROM EmailConversationParticipant TP 
                                INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID   
								INNER JOIN Employee E ON E.UserID = AU.UserID
								LEFT JOIN Designation D ON D.DesignationID = E.DesignationID
								LEFT JOIN Plant P ON P.PlantID =E.PlantID
								INNER JOIN EmailConversations ECO ON ECO.ID = TP.ConversationId                               
                                WHERE TP.ConversationId = @ConversationId order by TP.ID ASC";

                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    return (await connection.QueryAsync<EmailParticipant>(query, parameters)).ToList();
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
        public long EmailTopicUpdate(EmailTopics EmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameterss = new DynamicParameters();
                        parameterss.Add("ID", EmailTopics.ID);
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
                        parameterss.Add("OnDraft", EmailTopics.OnDraft);

                        parameterss.Add("Follow", EmailTopics.Follow);
                        parameterss.Add("OnBehalf", EmailTopics.OnBehalf);
                        parameterss.Add("Urgent", EmailTopics.Urgent);
                        parameterss.Add("OverDue", EmailTopics.OverDue);
                        parameterss.Add("DueDate", EmailTopics.DueDate);


                        parameterss.Add("To", EmailTopics.To);
                        parameterss.Add("CC", EmailTopics.CC);
                        parameterss.Add("Participants", EmailTopics.Participants);

                        parameterss.Add("isValidateSession", EmailTopics.isValidateSession);
                        parameterss.Add("ActivityEmailTopicId", EmailTopics.ActivityEmailTopicId);
                        parameterss.Add("IsAllowParticipants", EmailTopics.IsAllowParticipants);                        

                        connection.Open();

                        var result = connection.QueryFirstOrDefault<long>("sp_Update_EmailTopics", parameterss, commandType: CommandType.StoredProcedure);
                        return result;
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
                        parameterss.Add("OnDraft", EmailTopics.OnDraft);
                        parameterss.Add("IsAllowParticipants", EmailTopics.IsAllowParticipants);
                        

                        parameterss.Add("Follow", EmailTopics.Follow);
                        parameterss.Add("OnBehalf", EmailTopics.OnBehalf);
                        parameterss.Add("Urgent", EmailTopics.Urgent);
                        parameterss.Add("OverDue", EmailTopics.OverDue);
                        parameterss.Add("DueDate", EmailTopics.DueDate);
                        

                        parameterss.Add("To", EmailTopics.To);
                        parameterss.Add("CC", EmailTopics.CC);
                        parameterss.Add("Participants", EmailTopics.Participants);

                        parameterss.Add("isValidateSession", EmailTopics.isValidateSession);
                        parameterss.Add("ActivityEmailTopicId", EmailTopics.ActivityEmailTopicId);

                        connection.Open();

                        var result = connection.QueryFirstOrDefault<long>("sp_Ins_EmailTopics", parameterss, commandType: CommandType.StoredProcedure);
                        return result;
                    }
                    catch(Exception exp)
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
        public long Insert_sp_Participant(TopicParticipant topicParticipant)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("TopicId", topicParticipant.TopicId);
                        parameters.Add("ConversationId", topicParticipant.ConversationId);
                        parameters.Add("SessionId", topicParticipant.SessionId);
                        parameters.Add("AddedDate", topicParticipant.AddedDate);
                        parameters.Add("AddedByUserID", topicParticipant.AddedByUserID);
                        parameters.Add("StatusCodeID", topicParticipant.StatusCodeID);
                        parameters.Add("PList", topicParticipant.PList);
                        parameters.Add("Option", "INSERT");

                        connection.Open();

                        var result = connection.QueryFirstOrDefault<long>("sp_Ins_ConversationParticipant", parameters, commandType: CommandType.StoredProcedure);
                        return result;
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
        public async Task<long> UpdateSubjectDueDate(EmailConversations emailConversations)
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
                            parameters.Add("DueDate", emailConversations.DueDate);
                            parameters.Add("ID", emailConversations.ID);

                            var query = " UPDATE EmailConversations SET DueDate = @DueDate WHERE ID = @ID";

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
        public async Task<long> UpdateTopicArchive(EmailTopics EmailTopics)
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
                            parameters.Add("ID", EmailTopics.ID);
                            parameters.Add("ModifiedByUserID", EmailTopics.ModifiedByUserID);
                            parameters.Add("ModifiedDate", EmailTopics.ModifiedDate);

                            var query = " UPDATE EmailConversationParticipant SET IsArchive = 1 where TopicId = @ID and UserId = @ModifiedByUserID";
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

        public async Task<List<ActivityEmailTopics>> GetByActivityEmailSessionList(Guid sessionId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"SELECT AMC.Value as ManufacturingProcess,CAI.Value as CategoryAction,AI.Value as ActionName from ActivityEmailTopics AET 					
							LEFT JOIN ApplicationMasterChild AMC ON AMC.ApplicationMasterChildID = AET.ManufacturingProcessId
							LEFT JOIN ApplicationMasterChild CAI ON CAI.ApplicationMasterChildID = AET.CategoryActionId
							LEFT JOIN ApplicationMasterChild AI ON AI.ApplicationMasterChildID = AET.ActionId
                            WHERE AET.EmailTopicSessionId = @SessionID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ActivityEmailTopics>(query, parameters)).ToList();
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<Documents>> GetCreateEmailDocumentListAsync(Guid sessionId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"select * from Documents where SessionID = @SessionID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<long> Delete(long id)
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
                            parameters.Add("DocumentID", id);


                            var query = "DELETE  FROM Documents WHERE DocumentID = @DocumentID";

                          
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
