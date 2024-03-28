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
using Infrastructure.Data;
using Microsoft.VisualBasic;
using System.Data.SqlTypes;

namespace Infrastructure.Repository.Query
{
    public class EmailTopicsQueryRepository : DbConnector, IEmailTopicsQueryRepository
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
                    var result = await connection.QueryAsync<EmailTopics>(query, parameters);
                    var res = result.ToList();
                    if(res.ToList().Count > 0)
                    {
                        var subQueryDocs = @"select DocumentID,FileName,ContentType,FileSize,FilePath from Documents WHERE SessionID = @SessionID";

                        var parametersDocs = new DynamicParameters();
                        parametersDocs.Add("SessionID", res[0].SessionId);

                        var subQueryDocsResults = await connection.QueryAsync<Documents>(subQueryDocs, parametersDocs);


                        var subQueryTo = @"select E.FirstName,FT.UserId,FT.TopicId from EmailtopicTo FT
                                        INNER JOIN Employee E on E.UserID = FT.UserId
                                        where FT.TopicId = @ID";
                        var parametersTo = new DynamicParameters();
                        parametersTo.Add("ID", res[0].ID);
                        var subQueryToResults = await connection.QueryAsync<EmailAssignToList>(subQueryTo, parametersTo);


                        var subQueryCC = @"select E.FirstName,FC.UserId,FC.TopicId from EmailtopicCC FC
                                        INNER JOIN Employee E on E.UserID = FC.UserId
                                        where FC.TopicId = @ID";
                        var parametersCC = new DynamicParameters();
                        parametersCC.Add("ID", res[0].ID);
                        var subQueryCCResults = await connection.QueryAsync<EmailAssignToList>(subQueryCC, parametersCC);


                        res[0].documents = subQueryDocsResults.ToList();
                        res[0].TopicToList = subQueryToResults.ToList();
                        res[0].TopicCCList = subQueryCCResults.ToList();

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
                                INNER JOIN EmailConversationParticipant TP ON TS.ID = TP.TopicId                                
                                WHERE TP.UserId = @UserId";
                                
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<EmailTopics>(query,parameters);

                    //var result = res
                    //    .GroupBy(ps => ps.TicketNo)
                    //    .Select(g => new EmailTopics
                    //    {
                    //        Year = g.Key,
                    //        SalesByProduct = g.ToList()
                    //    })
                    //    .ToList();

                    //return result;


                    return res.ToList();

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<RequestEmail>> RequestEmailToCCList(long ConId)
        {
            try
            {
                
                var query = @"SELECT AddedByUserID as ToIds, STRING_AGG(UserId, ',') AS CcIds
                                FROM EmailConversationAssignCC
                                WHERE ConversationId = @ConId
                                GROUP BY AddedByUserID";

                var parameters = new DynamicParameters();
                parameters.Add("ConId", ConId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<RequestEmail>(query, parameters);
                    return res.ToList();

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
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);

                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailTopics>> GetByIdTopicListAsync(long id)
        {
            try
            {
                var query = @"SELECT * FROM EmailTopics  WHERE ID = @id";

                var parameters = new DynamicParameters();
                parameters.Add("id", id);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);

                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<List<long>> GetByIdUserGroupToList(long TopicId)
        {
            try
            {

                var query = @"select EAUG.GroupId from EmailTopics ET
                            INNER JOIN EmailConversationAssignToUserGroup EAUG ON EAUG.TopicId = ET.ID
                            WHERE ET.ID =@TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<long>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long>> GetByIdUserGroupCCList(long TopicId)
        {
            try
            {

                var query = @"select EAUG.GroupId from EmailTopics ET
                            INNER JOIN EmailConversationAssignCCUserGroup EAUG ON EAUG.TopicId = ET.ID
                            WHERE ET.ID =@TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

                using (var connection = CreateConnection())
                {
                    var res = await connection.QueryAsync<long>(query, parameters);
                    return res.ToList();
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
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);
                    return res.ToList();
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
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailTopics>> GetTopicAdminSearchList(EmailSearch emailSearch)
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
                        parameters.Add("UserTag", emailSearch.UserTag);

                        parameters.Add("filterFrom", emailSearch.FilterFrom);
                        parameters.Add("filterTo", emailSearch.FilterTo);
                        parameters.Add("Option", "SELECT");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_AdminSearchList", parameters, commandType: CommandType.StoredProcedure);
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
                        parameters.Add("UserTag", emailSearch.UserTag);

                        parameters.Add("filterFrom", emailSearch.FilterFrom);
                        parameters.Add("filterTo", emailSearch.FilterTo);
                        parameters.Add("Option", "SELECT");                        

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_MasterSearchList", parameters, commandType: CommandType.StoredProcedure);
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
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailTopics>> GetTopicAllList(long UserId,string searchTxt,int pageNumber,int pageSize)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("searchtxt", searchTxt);
                        parameters.Add("PageNumber", pageNumber);
                        parameters.Add("PageSize", pageSize);
                        parameters.Add("Option", "SELECT_ASSIGN_ALL");
                        //var result = connection.Query<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 300);
                        //var result = await connection.QueryAsync<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure,commandTimeout: 600);
                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_ALL_EmailTopicList", parameters, commandType: CommandType.StoredProcedure); // Increase timeout to 10 minutes
                        connection.Close();
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

        public async Task<List<EmailTopics>> GetTopicHomeList(long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("Option", "SELECT_EMAIL_HOME");
                        
                        var command = connection.CreateCommand();
                        command.CommandTimeout = 600; 

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id);

                            var query = "UPDATE EmailTopics SET PinStatus = 'Lock' WHERE ID = @id";
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
        public async Task<long> UpdateMarkasAllReadList(long id,long userId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id);
                            parameters.Add("userId", userId);

                            var query = "UPDATE EmailNotifications SET IsRead = 1 where UserId = @userId and IsRead = 0 and ConversationId in (select ID from EmailConversations EC where EC.ReplyId = @id OR EC.ID = @id)";

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

        public async Task<long> UpdateMarkasReadList(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {   
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id);

                            var query = "UPDATE EmailNotifications SET IsRead = 1 WHERE ID = @id";


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
        public async Task<long> UpdateMarkasunReadList(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {  
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id);

                            var query = "UPDATE EmailNotifications SET IsRead = 0 WHERE ID = @id";


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
        public async Task<long> UnSetPinTopicToList(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("id", id);

                            var query = "UPDATE EmailTopics SET PinStatus = 0 WHERE ID = @id";

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
        public async Task<List<EmailTopics>> GetTopicToList(long UserId, string? searchTxt, int pageNumber, int pageSize)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("searchtxt", searchTxt);
                        parameters.Add("PageNumber", pageNumber);
                        parameters.Add("PageSize", pageSize);
                        parameters.Add("Option", "SELECT_ASSIGN_TO");
                        

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
                        connection.Close();
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
        public async Task<List<EmailTopics>> GetTopicToSearchList(string SearchTxt, long UserId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("searchtxt", SearchTxt);
                        parameters.Add("Option", "SELECT_ASSIGN_TO_SEARCH");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_EmailTopicSearchList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetTopicCCList(long UserId, string searchTxt)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("searchtxt", searchTxt);                        
                        parameters.Add("Option", "SELECT_ASSIGN_CC");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetTopicSentList(long UserId,string searchTxt)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserId", UserId);
                        parameters.Add("searchtxt", searchTxt);
                        parameters.Add("Option", "SELECT_ASSIGN_SENT");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);
                    return res.ToList();
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
                    var res = await connection.QueryAsync<EmailTopics>(query, parameters);
                    return res.ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailTopics>> GetSubTopicToList(long TopicId, long UserId,string SearchTxt)
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
                        parameters.Add("searchtxt", SearchTxt);                        
                        parameters.Add("Option", "SUB_SELECT_TO");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetSubTopicCCList(long TopicId, long UserId,string SearchTxt)
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
                        parameters.Add("searchtxt", SearchTxt);
                        parameters.Add("Option", "SUB_SELECT_CC");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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

        public async Task<List<EmailTopics>> GetSubTopicSentList(long TopicId, long UserId,string SearchTxt)
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
                        parameters.Add("searchtxt", SearchTxt);
                        parameters.Add("Option", "SUB_SELECT_SENT");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailTopics>> GetSubAdminEmailTopicAllList(long TopicId, long UserId, string SearchTxt)
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
                        parameters.Add("searchtxt", SearchTxt);
                        parameters.Add("Option", "SELECT_ALL");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_Sub_Admin_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailTopics>> GetSubTopicAllList(long TopicId, long UserId,string SearchTxt)
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
                        parameters.Add("searchtxt", SearchTxt);
                        parameters.Add("Option", "SUB_SELECT_ALL");

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailTopics>> GetSubTopicHomeList(long TopicId, long UserId)
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
                        parameters.Add("Option", "SUB_SELECT_EMAIL_HOME");

                        var command = connection.CreateCommand();
                        command.CommandTimeout = 600;

                        var result = await connection.QueryAsync<EmailTopics>("sp_Select_Sub_EmailTopicList", parameters, commandType: CommandType.StoredProcedure);
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
        public async Task<List<EmailConversationAssignToUserGroup>> GetGroupParticipantList(long topicId, long UserId)
        {
            try
            {
                var query = @"select AU.Name,TP.ID,TP.TopicId,TP.AddedDate,CASE WHEN TP.AddedByUserID = TP.GroupId THEN 0 ELSE 1 END AS IsEnabled,
                            ECO.Name as SubjectName	  
                            FROM EmailConversationParticipantUserGroup TP 
                            INNER JOIN UserGroup AU ON TP.GroupId = AU.UserGroupID  
                            INNER JOIN UserGroupUser UGU ON UGU.UserGroupID = TP.GroupId AND UGU.UserID = @UserId
                            INNER JOIN EmailConversations ECO ON ECO.ID = TP.ConversationId			
                            where TP.TopicId = @TopicId";

                var parameters = new DynamicParameters();
                parameters.Add("TopicId", topicId);
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailConversationAssignToUserGroup>(query, parameters)).ToList();                   
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
                //        var query = @"SELECT RowIndex = ROW_NUMBER() OVER(ORDER BY TP.ID ASC), TP.ID,TP.TopicId,AU.UserCode,AU.UserName,TP.AddedDate,TP.SessionId,AU.UserID,
                //                        CASE WHEN TP.AddedByUserID = TP.UserID THEN 0 ELSE 1 END AS IsEnabled,ECO.Name as SubjectName,(E.FirstName + ' ' + E.LastName) AS Name,E.NickName,
                //D.Name AS DesignationName,p.PlantCode AS CompanyName 
                //                        FROM EmailConversationParticipant TP 
                //                        INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID   
                //INNER JOIN Employee E ON E.UserID = AU.UserID
                //LEFT JOIN Designation D ON D.DesignationID = E.DesignationID
                //LEFT JOIN Plant P ON P.PlantID =E.PlantID
                //INNER JOIN EmailConversations ECO ON ECO.ID = TP.ConversationId                               
                //                        WHERE TP.ConversationId = @ConversationId order by TP.ID ASC";


                var query = @";WITH CTE AS (
                                SELECT
                                    TP.UserId AS UserID,
                                    TP.ID,
                                    TP.TopicId,
                                    AU.UserCode,
                                    AU.UserName,
                                    TP.AddedDate,
                                    TP.SessionId,
                                    CASE
                                        WHEN TP.AddedByUserID = TP.UserID THEN 0
                                        ELSE 1
                                    END AS IsEnabled,
                                    ECO.Name AS SubjectName,
                                    (E.FirstName + ' ' + E.LastName) AS Name,
                                    E.NickName,
                                    D.Name AS DesignationName,
                                    P.PlantCode AS CompanyName,
                                    ROW_NUMBER() OVER (PARTITION BY TP.UserId ORDER BY TP.ID ASC) AS RowNum
                                FROM
                                    EmailConversationParticipant TP
                                    INNER JOIN ApplicationUser AU ON TP.UserId = AU.UserID
                                    INNER JOIN Employee E ON E.UserID = AU.UserID
                                    LEFT JOIN Designation D ON D.DesignationID = E.DesignationID
                                    LEFT JOIN Plant P ON P.PlantID = E.PlantID
                                    INNER JOIN EmailConversations ECO ON ECO.ID = TP.ConversationId
                                WHERE
                                    TP.ConversationId = @ConversationId
                            )

                            SELECT
                                UserID,
                                ID,
                                TopicId,
                                UserCode,
                                UserName,
                                AddedDate,
                                SessionId,
                                IsEnabled,
                                SubjectName,
                                Name,
                                NickName,
                                DesignationName,
                                CompanyName
                            FROM
                                CTE
                            WHERE
                                RowNum = 1";

                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {                    
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
        public async Task<List<EmailConversationAssignToUserGroup>> GetConversationGroupPList(long ConversationId)
        {
            try
            {
                var query = @";WITH CTE AS (
	                            SELECT
		                            TP.GroupId,                                    
		                            TP.ID,
		                            TP.TopicId,                                
		                            TP.AddedDate,                             
		                            CASE
			                            WHEN TP.AddedByUserID = UGU.UserID THEN 0
			                            ELSE 1
		                            END AS IsEnabled,
		                            ECO.Name AS SubjectName,
		                            AU.Name,
                                   
                                  
		                            ROW_NUMBER() OVER (PARTITION BY TP.GroupId ORDER BY TP.ID ASC) AS RowNum
	                            FROM
		                            EmailConversationParticipantUserGroup TP
		                            INNER JOIN UserGroup AU ON TP.GroupId = AU.UserGroupID
		                            INNER JOIN UserGroupUser UGU ON UGU.UserGroupID = TP.GroupId 
		                            INNER JOIN EmailConversations ECO ON ECO.ID = TP.ConversationId
	                            WHERE
		                            TP.ConversationId = @ConversationId
	                            )

	                            SELECT
	                            GroupId,
	                            ID,
	                            TopicId,
	                            AddedDate,
	                            IsEnabled,
	                            SubjectName,
	                            Name
	                            FROM
	                            CTE
	                            WHERE
	                            RowNum = 1";

                var parameters = new DynamicParameters();
                parameters.Add("ConversationId", ConversationId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailConversationAssignToUserGroup>(query, parameters)).ToList();
                    // var result = connection.QueryAsync<TopicParticipant>(query, parameters).ToList();
                    //return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<EmailParticipant>> GetParticipantbysessionidList(Guid sessionId)
        {
            try
            {
                var query = @"SELECT * FROM EmailConversationParticipant WHERE SessionId = @sessionId";

                var parameters = new DynamicParameters();
                parameters.Add("sessionId", sessionId);

                using (var connection = CreateConnection())
                {                    
                    return (await connection.QueryAsync<EmailParticipant>(query, parameters)).ToList();                    
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
                    var result =  await connection.QueryFirstOrDefaultAsync<EmailTopics>(query, parameters);
                    return result;
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
                        parameterss.Add("NotifyUser", EmailTopics.NotifyUser);

                        parameterss.Add("GroupTag", EmailTopics.GroupTag);
                        parameterss.Add("CategoryTag", EmailTopics.CategoryTag);
                        parameterss.Add("ActionTag", EmailTopics.ActionTag);
                        parameterss.Add("actName", EmailTopics.actName);
                        parameterss.Add("UserTag", EmailTopics.UserTag);
                        parameterss.Add("UserTagId", EmailTopics.UserTagId);

                        parameterss.Add("UserType", EmailTopics.UserType);
                        parameterss.Add("ToUserGroup", EmailTopics.ToUserGroup);
                        parameterss.Add("CCUserGroup", EmailTopics.CCUserGroup);
                        parameterss.Add("ParticipantsUserGroup", EmailTopics.ParticipantsUserGroup);

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
                        parameterss.Add("NotifyUser", EmailTopics.NotifyUser);

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

                        parameterss.Add("GroupTag", EmailTopics.GroupTag);
                        parameterss.Add("CategoryTag", EmailTopics.CategoryTag);
                        parameterss.Add("ActionTag", EmailTopics.ActionTag);
                        parameterss.Add("ActName", EmailTopics.actName);
                        parameterss.Add("ActivityType", EmailTopics.ActivityType);
                        parameterss.Add("UserTag", EmailTopics.UserTag);

                        parameterss.Add("UserType", EmailTopics.UserType);
                        parameterss.Add("ToUserGroup", EmailTopics.ToUserGroup);
                        parameterss.Add("CCUserGroup", EmailTopics.CCUserGroup);
                        parameterss.Add("ParticipantsUserGroup", EmailTopics.ParticipantsUserGroup);
                        parameterss.Add("NoOfDays", EmailTopics.NoOfDays);
                        parameterss.Add("ExpiryDueDate", EmailTopics.ExpiryDueDate);

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
        
        public async Task<long> CreateActivityEmailAsync(ActivityEmailTopics activityEmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    
                   

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("SubjectName", activityEmailTopics.SubjectName);
                            parameters.Add("Comment", activityEmailTopics.Comment);
                            parameters.Add("ActivityType", activityEmailTopics.ActivityType);
                            parameters.Add("SessionId", activityEmailTopics.SessionId);
                            parameters.Add("BackURL", activityEmailTopics.BackURL);
                            parameters.Add("SessionId", activityEmailTopics.SessionId);
                            parameters.Add("DocumentSessionId", activityEmailTopics.DocumentSessionId);
                            parameters.Add("AddedByUserID", activityEmailTopics.AddedByUserID);
                            parameters.Add("AddedDate", activityEmailTopics.AddedDate);
                            parameters.Add("ManufacturingProcessId", activityEmailTopics.ManufacturingProcessId);
                            parameters.Add("CategoryActionId", activityEmailTopics.CategoryActionId);
                            parameters.Add("EmailTopicSessionId", activityEmailTopics.EmailTopicSessionId);
                            

                            var query = "INSERT INTO ActivityEmailTopics(SubjectName,Comment,ActivityType,SessionId,BackURL,DocumentSessionId,AddedByUserID,AddedDate,ManufacturingProcessId,CategoryActionId,EmailTopicSessionId) OUTPUT INSERTED.ActivityEmailTopicID VALUES (@SubjectName,@Comment,@ActivityType,@SessionId,@BackURL,@DocumentSessionId,@AddedByUserID,@AddedDate,@ManufacturingProcessId,@CategoryActionId,@EmailTopicSessionId)";

                            //var rowsAffected = await connection.ExecuteAsync(query, parameters);
                            var insertedId = await connection.ExecuteScalarAsync<int>(query, parameters);                          

                            return insertedId;
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

        public async Task<long> UpdateActivityEmailAsync(ActivityEmailTopics activityEmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    
                   

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("SubjectName", activityEmailTopics.SubjectName);
                            parameters.Add("Comment", activityEmailTopics.Comment);
                            parameters.Add("ActivityType", activityEmailTopics.ActivityType);
                            parameters.Add("SessionId", activityEmailTopics.SessionId);
                            parameters.Add("BackURL", activityEmailTopics.BackURL);
                            parameters.Add("SessionId", activityEmailTopics.SessionId);
                            parameters.Add("DocumentSessionId", activityEmailTopics.DocumentSessionId);
                            parameters.Add("AddedByUserID", activityEmailTopics.AddedByUserID);
                            parameters.Add("AddedDate", activityEmailTopics.AddedDate);
                            parameters.Add("ManufacturingProcessId", activityEmailTopics.ManufacturingProcessId);
                            parameters.Add("CategoryActionId", activityEmailTopics.CategoryActionId);
                            parameters.Add("ActivityEmailTopicID", activityEmailTopics.ActivityEmailTopicID);                            

                            var query = "UPDATE ActivityEmailTopics SET SubjectName = @SubjectName,Comment = @Comment,ActivityType = @ActivityType,AddedByUserID = @AddedByUserID,AddedDate = @AddedDate,ManufacturingProcessId = @ManufacturingProcessId,CategoryActionId = @CategoryActionId WHERE ActivityEmailTopicID = @ActivityEmailTopicID";

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
        public async Task<long> InsertParticipant(TopicParticipant topicParticipant)
        {
            var rowsAffected = 0;
            var result = 1;
            try
            {
                using (var connection = CreateConnection())
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
                            

                            var query = "INSERT INTO EmailConversationParticipant(TopicID, UserId,StatusCodeID,AddedByUserID,AddedDate,SessionId) VALUES (@TopicID, @UserId,@StatusCodeID,@AddedByUserID,@AddedDate,@SessionId)";
                            rowsAffected = await connection.ExecuteAsync(query, parameters);
                           
                            //return rowsAffected;
                        }
                        
                        return result;
                    
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }        
        public async Task<long> CreateUserTagAsync(EmailActivityCatgorys emailActivityCatgorys)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserTag", emailActivityCatgorys.UserTag);
                        parameters.Add("TopicId", emailActivityCatgorys.TopicId);
                        parameters.Add("AddedByUserID", emailActivityCatgorys.AddedByUserID);
                        parameters.Add("AddedDate", emailActivityCatgorys.AddedDate);

                        var query = " INSERT INTO EmailTopicUserTags (UserTag,TopicId,AddedByUserID,AddedDate,ModifiedByUserID,ModifiedDate) VALUES (@UserTag,@TopicId,@AddedByUserID,@AddedDate,@AddedByUserID,@AddedDate)";

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
        public async Task<long> UpdateUserTagAsync(EmailActivityCatgorys emailActivityCatgorys)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("UserTag", emailActivityCatgorys.UserTag);
                        parameters.Add("ID", emailActivityCatgorys.UserTagId);

                        var query = " UPDATE EmailTopicUserTags SET UserTag = @UserTag WHERE ID = @ID";

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

        public async Task<long> CreateEmailTimelineEventAsync(EmailTimelineEvent emailTimelineEvent)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentID", emailTimelineEvent.DocumentID);
                        parameters.Add("TopicID", emailTimelineEvent.TopicID);
                        parameters.Add("ConversationID", emailTimelineEvent.ConversationID);
                        parameters.Add("Description", emailTimelineEvent.Description);                        
                        parameters.Add("AddedByUserID", emailTimelineEvent.AddedByUserID);
                        parameters.Add("AddedDate", emailTimelineEvent.AddedDate);

                        var query = " INSERT INTO EmailTimelineEvent (DocumentID,TopicID,ConversationID,Description,AddedByUserID,AddedDate,ModifiedByUserID,ModifiedDate) VALUES (@DocumentID,@TopicID,@ConversationID,@Description,@AddedByUserID,@AddedDate,@AddedByUserID,@AddedDate)";

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
        public async Task<long> UpdateEmailTimelineEventAsync(EmailTimelineEvent emailTimelineEvent)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentID", emailTimelineEvent.DocumentID);
                        parameters.Add("TopicID", emailTimelineEvent.TopicID);
                        parameters.Add("ConversationID", emailTimelineEvent.ConversationID);
                        parameters.Add("Description", emailTimelineEvent.Description);
                        parameters.Add("ModifiedByUserID", emailTimelineEvent.ModifiedByUserID);
                        parameters.Add("ModifiedDate", emailTimelineEvent.ModifiedDate);
                        parameters.Add("ID", emailTimelineEvent.ID);

                        var query = " UPDATE EmailTimelineEvent SET Description = @Description,ModifiedByUserID = @ModifiedByUserID,ModifiedDate=@ModifiedDate WHERE ID = @ID";

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
        public async Task<List<EmailTimelineEvent>> GetEmailTimelineEvent(long DocumentId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("DocumentId", DocumentId);

                var query = @"select ETE.*, E.FirstName AS UserName from EmailTimelineEvent ETE
                                INNER JOIN Employee E ON E.UserID = ETE.AddedByUserID
                                WHERE ETE.DocumentID =  @DocumentId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailTimelineEvent>(query, parameters)).ToList();
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

                    

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DueDate", EmailTopics.DueDate);
                            parameters.Add("ID", EmailTopics.ID);                           

                            var query = " UPDATE EmailTopics SET DueDate = @DueDate WHERE ID = @ID";

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
        public async Task<long> UpdateSubjectDueDate(EmailConversations emailConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DueDate", emailConversations.DueDate);
                            parameters.Add("NoOfDays", emailConversations.NoOfDays);                        
                            parameters.Add("ID", emailConversations.ID);
                            parameters.Add("TopicId", emailConversations.TopicID);
                            parameters.Add("ModifiedByUserID", emailConversations.ModifiedByUserID);
                            parameters.Add("ModifiedDate", emailConversations.ModifiedDate);

                        DateTime? expiryDueDate = null; // Use nullable DateTime

                        if (emailConversations.DueDate.HasValue)
                        {
                            DateTime dueDate = emailConversations.DueDate.Value;
                            expiryDueDate = dueDate.AddDays(emailConversations.NoOfDays);

                            // Check if expiryDueDate falls within the valid range for SQL datetime
                            if (expiryDueDate < SqlDateTime.MinValue.Value || expiryDueDate > SqlDateTime.MaxValue.Value)
                            {
                                throw new Exception("Expiry due date is out of range for SQL datetime.");
                            }
                        }

                        parameters.Add("ExpiryDueDate", expiryDueDate, DbType.DateTime); // Use DbType.DateTime for null values




                        var query = "";
                        var emailquery = "";

                        if ((emailConversations.ModifiedByUserID == emailConversations.UserId) || emailConversations.openAccessUserLink == true)
                        {
                            query = " UPDATE EmailConversations SET ExpiryDueDate = @ExpiryDueDate, NoOfDays = @NoOfDays, DueDate = @DueDate,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ID = @ID";
                            emailquery = "UPDATE EmailTopics SET ExpiryDueDate = @ExpiryDueDate, NoOfDays = @NoOfDays, DueDate = @DueDate,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ID = @TopicId";

                        }
                        else
                        {
                            query = " UPDATE EmailConversations SET NoOfDays = @NoOfDays, DueDate = @DueDate,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ID = @ID";
                            emailquery = "UPDATE EmailTopics SET NoOfDays = @NoOfDays, DueDate = @DueDate,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ID = @TopicId";
                           
                        }


                        var ckquery = "SELECT TOP 1 ID FROM EmailConversations WHERE TopicID = @TopicId AND ReplyId = 0 ORDER BY ID";
                            var actresult = await connection.QueryAsync<EmailConversations>(ckquery, parameters);


                            var rowsAffected = await connection.ExecuteAsync(query, parameters);

                            // Check if there are any results in the actresult
                            if (actresult != null && actresult.Any())
                            {
                                var firstResult = actresult.FirstOrDefault();

                                // Check if the first record's ID matches the ID from emailConversations
                                if (firstResult != null && firstResult.ID == emailConversations.ID)
                                {
                                    // Execute another SQL query (emailquery) with the same parameters
                                    var rowsAffected2 = await connection.ExecuteAsync(emailquery, parameters);
                                }
                            }

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
        public async Task<long> UpdateSubjectName(EmailConversations emailConversations)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    
                    

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Name", emailConversations.Name);
                            parameters.Add("ID", emailConversations.ID);
                            parameters.Add("TopicId", emailConversations.TopicID);

                            parameters.Add("ModifiedByUserID", emailConversations.ModifiedByUserID);
                            parameters.Add("ModifiedDate", emailConversations.ModifiedDate);


                            var query = " UPDATE EmailConversations SET Name = @Name,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ID = @ID";
                            var subquery = "UPDATE EmailConversations SET Name = @Name,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ReplyId = @ID";
                            var emailquery = "UPDATE EmailTopics SET TopicName = @Name,ModifiedByUserID = @ModifiedByUserID,ModifiedDate = @ModifiedDate WHERE ID = @TopicId";


                            var ckquery = "SELECT TOP 1 ID FROM EmailConversations WHERE TopicID = @TopicId AND ReplyId = 0 ORDER BY ID";
                            var actresult = await connection.QueryAsync<EmailConversations>(ckquery, parameters);



                            var rowsAffected = await connection.ExecuteAsync(query, parameters);
                            var rowsAffected1 = await connection.ExecuteAsync(subquery, parameters);

                            // Check if there are any results in the actresult
                            if (actresult != null && actresult.Any())
                            {
                                var firstResult = actresult.FirstOrDefault();

                                // Check if the first record's ID matches the ID from emailConversations
                                if (firstResult != null && firstResult.ID == emailConversations.ID)
                                {
                                    // Execute another SQL query (emailquery) with the same parameters
                                    var rowsAffected2 = await connection.ExecuteAsync(emailquery, parameters);
                                }
                            }                           

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
        public async Task<long> UpdateTopicClose(EmailTopics EmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Remarks", EmailTopics.Remarks);
                            parameters.Add("ID", EmailTopics.ID);

                            var query = " UPDATE EmailTopics SET Remarks = @Remarks, Status ='closed' WHERE ID = @ID";
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
        public async Task<long> UpdateTopicGroupArchive(EmailTopics EmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ID", EmailTopics.ID);
                        parameters.Add("ModifiedByUserID", EmailTopics.ModifiedByUserID);
                        parameters.Add("ModifiedDate", EmailTopics.ModifiedDate);

                        var query = " UPDATE EmailConversationParticipant SET IsArchive = 1 where TopicId = @ID and UserId = @ModifiedByUserID";
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
        public async Task<long> UpdateTopicArchive(EmailTopics EmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    

                        try
                        {
                            var parameters = new DynamicParameters();                           
                            parameters.Add("ID", EmailTopics.ID);
                            parameters.Add("ModifiedByUserID", EmailTopics.ModifiedByUserID);
                            parameters.Add("ModifiedDate", EmailTopics.ModifiedDate);

                            var query = " UPDATE EmailConversationParticipant SET IsArchive = 1 where TopicId = @ID and UserId = @ModifiedByUserID";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);

                            var query2 = "UPDATE EmailNotifications SET IsRead = 1 WHERE IsRead = 0 AND TopicId = @ID AND UserId = @ModifiedByUserID";
                            var rowsAffected2 = await connection.ExecuteAsync(query2, parameters);


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
        public async Task<long> UpdateTopicUnArchive(EmailTopics EmailTopics)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ID", EmailTopics.ID);
                            parameters.Add("ModifiedByUserID", EmailTopics.ModifiedByUserID);
                            parameters.Add("ModifiedDate", EmailTopics.ModifiedDate);

                            var query = " UPDATE EmailConversationParticipant SET IsArchive = 0 where TopicId = @ID and UserId = @ModifiedByUserID";
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
        public async Task<List<ActivityEmailTopics>> GetActivityEmailListBySession(Guid sessionId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"SELECT * from ActivityEmailTopics WHERE SessionId = @SessionID";

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
        public async Task<List<ActivityEmailTopics>> GetActivityEmailDocListBySession(Guid sessionId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"SELECT * from ActivityEmailTopics WHERE SessionId = @SessionID AND ActivityType='FileProfileType'";

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
        
        public async Task<List<Documents>> GetPATypeDocLstAsync(long Id,string Type)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);
                var query = "";

                if (Type == "ProductionActivity")
                {
                    query = @"SELECT D.FileName,D.FileSize from Documents D
                            INNER JOIN ProductionActivityAppLineDoc PAD ON PAD.DocumentID = D.DocumentID
                            WHERE PAD.ProductionActivityAppLineID = @Id";
                }
                else
                {
                    query = @"SELECT D.FileName,D.FileSize from Documents D
                            INNER JOIN ProductionActivityRoutineAppLineDoc PARD ON PARD.DocumentID = D.DocumentID
                            WHERE PARD.ProductionActivityRoutineAppLineID = @Id";
                }               

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
        
        public async Task<List<DynamicFormData>> GetDynamicFormNameAsync(Guid sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"select df.Name from DynamicFormData dfd
                            inner join DynamicForm df on df.ID = dfd.DynamicFormID
                            where dfd.SessionID =  @SessionID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }        
        public async Task<List<DynamicFormSectionSecurity>> GetUserListByDynamicFormAsync(long Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);

                var query = @"select * from DynamicFormSectionSecurity where DynamicFormSectionID = @Id";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionSecurity>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormSection>> GetDynamicFormEmailSectionListAsync(Guid sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"SELECT DFS.*,DFD.SessionID as EmailFormDataSessionID,DFS.SessionID as EmailFormSectionSessionID from ActivityEmailTopics AET
                            INNER JOIN DynamicFormData DFD ON DFD.SessionID= AET.SessionId
                            INNER JOIN DynamicFormSection DFS ON DFS.DynamicFormID = DFD.DynamicFormID
                            LEFT JOIN EmailDynamicFormSection EDFS ON EDFS.FormSectionSessionID = DFS.SessionID AND EDFS.FormDataSessionID = DFD.SessionID
                            where EDFS.FormSectionSessionID IS NULL AND AET.ActivityType = 'DynamicForm' AND AET.EmailTopicSessionId = @SessionID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSection>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<List<Documents>> GetDynamicFormDocumentListAsync(Guid sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionID", sessionId, DbType.Guid);

                var query = @"select D.* from Documents D
                                INNER JOIN DynamicFormDataUpload DFU ON DFU.SessionID = D.SessionID
                                INNER JOIN dynamicformdata DFD ON DFD.DynamicFormDataID = DFU.DynamicFormDataID
                                INNER JOIN ActivityEmailTopics AET ON AET.SessionId = DFD.SessionID AND AET.ActivityType = 'DynamicForm' AND AET.SessionId =  @SessionID";

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
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentID", id);


                            var query = "DELETE  FROM Documents WHERE DocumentID = @DocumentID";

                          
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
