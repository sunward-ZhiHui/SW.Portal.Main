using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Infrastructure.Repository.Query
{
    public class EmailActivityCatgorysQueryRepository : DbConnector, IEmailActivityCatgorysQueryRepository
    {
        public EmailActivityCatgorysQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<EmailActivityCatgorys>> GetAllAsync()
        {
            try
            {
                //var query = "SELECT DISTINCT Name FROM EmailActivityCatgorys WHERE Name IS NOT NULL AND Name != ''";
                var query = "SELECT DISTINCT Name,Min(ID)  as ID FROM EmailActivityCatgorys WHERE Name IS NOT NULL AND Name != '' Group By Name";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailActivityCatgorys>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<EmailActivityCatgorys>> GetAllTopicCategoryAsync(long TopicId)
        {
            try
            {
                var query = @"SELECT t3.Value as GroupName,t4.Value as CategoryName,t5.Value as ActionName, t2.* FROM EmailActivityCatgorys t2
                                LEFT JOIN ApplicationMasterChild t3 ON t3.ApplicationMasterChildID=t2.GroupTag
                                LEFT JOIN ApplicationMasterChild t4 ON t4.ApplicationMasterChildID=t2.CategoryTag
                                LEFT JOIN ApplicationMasterChild t5 ON t5.ApplicationMasterChildID=t2.ActionTag
                                WHERE t2.TopicId = @TopicId";
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

                List<EmailActivityCatgorys> res = new List<EmailActivityCatgorys>();

                using (var connection = CreateConnection())
                {
                    res = (await connection.QueryAsync<EmailActivityCatgorys>(query, parameters)).ToList();
                }


                foreach (var items in res)
                {
                    var query1 = @"select AMC.Value from EmailActionTagMultiple ETM
                                    LEFT JOIN ApplicationMasterChild AMC on AMC.ApplicationMasterChildID = ETM.ActionID
                                    WHERE ETM.TopicID  = @TopicId";
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("TopicId", TopicId);

                    using (var connection = CreateConnection())
                    {                        
                        var subQueryResults  = (await connection.QueryAsync<string>(query1, parameters1)).ToList();
                        items.ActionNames = subQueryResults;
                    }
                }

                return res;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<long>> GetActionTagMultipleAsync(long TopicId)
        {
            try
            {
                var query = @"SELECT ActionID FROM EmailActionTagMultiple WHERE TopicId = @TopicId";
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

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
        
        public async Task<bool> GetTagLockInfoAsync(long TopicId)
        {
            try
            {               
                var query = @"select top 1 ISNULL(TagLock, 0) as TagLock from EmailConversations where TopicID = @TopicId and ReplyId = 0 order by ID asc";
                var parameters = new DynamicParameters();
                parameters.Add("TopicId", TopicId);

                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parameters);
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(EmailActivityCatgorys emailActivityCatgorys)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {

                        var deleteQuery = "DELETE FROM EmailActionTagMultiple WHERE TopicID = @TopicID";
                        await connection.ExecuteAsync(deleteQuery, new { TopicID = emailActivityCatgorys.TopicId });

                        if (emailActivityCatgorys.ActionTagIds != null && emailActivityCatgorys.ActionTagIds.Any())
                        {
                            var insertActionTagQuery = @"
                            INSERT INTO EmailActionTagMultiple (TopicID, ActionID)
                            VALUES (@TopicID, @ActionID)";

                            foreach (var actionId in emailActivityCatgorys.ActionTagIds)
                            {
                                if (actionId.HasValue) // Check if the ActionID is not null
                                {
                                    await connection.ExecuteAsync(insertActionTagQuery,
                                        new { TopicID = emailActivityCatgorys.TopicId, ActionID = actionId });
                                }
                            }
                        }


                        var parameters = new DynamicParameters();
                            parameters.Add("TopicId", emailActivityCatgorys.TopicId);
                            parameters.Add("GroupTag", emailActivityCatgorys.GroupTag);
                            parameters.Add("CategoryTag", emailActivityCatgorys.CategoryTag);
                            parameters.Add("ActionTag", emailActivityCatgorys.ActionTag);
                            parameters.Add("Name", emailActivityCatgorys.Name);
                            parameters.Add("Description", emailActivityCatgorys.Description);
                            parameters.Add("StatusCodeID", emailActivityCatgorys.StatusCodeID);
                            parameters.Add("AddedByUserID", emailActivityCatgorys.AddedByUserID);
                            parameters.Add("ModifiedByUserID", emailActivityCatgorys.ModifiedByUserID);
                            parameters.Add("AddedDate", emailActivityCatgorys.AddedDate);
                            parameters.Add("ModifiedDate", emailActivityCatgorys.ModifiedDate);
                            parameters.Add("SessionId", emailActivityCatgorys.SessionId);

                            var query = "INSERT INTO EmailActivityCatgorys(Name,GroupTag,CategoryTag,ActionTag,TopicId,Description,StatusCodeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,SessionId) VALUES (@Name,@GroupTag,@CategoryTag,@ActionTag,@TopicId,@Description,@StatusCodeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@SessionId)";

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
            };
        }
        public async Task<long> UpdateAsync(EmailActivityCatgorys emailActivityCatgorys)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                        {

                        var deleteQuery = "DELETE FROM EmailActionTagMultiple WHERE TopicID = @TopicID";
                        await connection.ExecuteAsync(deleteQuery, new { TopicID = emailActivityCatgorys.TopicId });

                        if (emailActivityCatgorys.ActionTagIds != null && emailActivityCatgorys.ActionTagIds.Any())
                        {
                            var insertActionTagQuery = @"
                            INSERT INTO EmailActionTagMultiple (TopicID, ActionID)
                            VALUES (@TopicID, @ActionID)";

                            foreach (var actionId in emailActivityCatgorys.ActionTagIds)
                            {
                                if (actionId.HasValue) // Check if the ActionID is not null
                                {
                                    await connection.ExecuteAsync(insertActionTagQuery,
                                        new { TopicID = emailActivityCatgorys.TopicId, ActionID = actionId });
                                }
                            }
                        }

                        var parameters = new DynamicParameters();
                            parameters.Add("ID", emailActivityCatgorys.ID);
                            parameters.Add("TopicId", emailActivityCatgorys.TopicId);
                            parameters.Add("GroupTag", emailActivityCatgorys.GroupTag);
                            parameters.Add("CategoryTag", emailActivityCatgorys.CategoryTag);
                            parameters.Add("ActionTag", emailActivityCatgorys.ActionTag);
                            parameters.Add("Name", emailActivityCatgorys.Name);
                            parameters.Add("Description", emailActivityCatgorys.Description);
                            parameters.Add("StatusCodeID", emailActivityCatgorys.StatusCodeID);
                            parameters.Add("AddedByUserID", emailActivityCatgorys.AddedByUserID);
                            parameters.Add("ModifiedByUserID", emailActivityCatgorys.ModifiedByUserID);
                            parameters.Add("AddedDate", emailActivityCatgorys.AddedDate);
                            parameters.Add("ModifiedDate", emailActivityCatgorys.ModifiedDate);
                            parameters.Add("SessionId", emailActivityCatgorys.SessionId);

                            var query = @"Update EmailActivityCatgorys SET Name = @Name,GroupTag = @GroupTag,CategoryTag = @CategoryTag,ActionTag = @ActionTag,TopicId=@TopicId,Description=@Description,StatusCodeID=@StatusCodeID,AddedByUserID=@AddedByUserID,ModifiedByUserID=@ModifiedByUserID,AddedDate=@AddedDate,ModifiedDate=@ModifiedDate,SessionId=@SessionId WHERE ID = @ID";

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
            };
        }
        public async Task<long> DeleteAsync(long id, long TopicId)
        {
            try
            {
                using (var connection = CreateConnection())
                {  
                    try
                        {

                        var deleteQuery = "DELETE FROM EmailActionTagMultiple WHERE TopicID = @TopicID";
                        await connection.ExecuteAsync(deleteQuery, new { TopicID = TopicId });


                        var parameters = new DynamicParameters();
                            parameters.Add("id", id);

                            var query = "DELETE  FROM EmailActivityCatgorys WHERE ID = @id";
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

        public async Task<IReadOnlyList<EmailActivityCatgorys>> GetAllemailCategoryAsync(long TopicId)
        {
            
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("TopicId", TopicId);
                    var query = "select * from EmailActivityCatgorys where TopicId = @TopicId";

                    using (var connection = CreateConnection())
                    {
                        return (await connection.QueryAsync<EmailActivityCatgorys>(query, parameters)).ToList();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message, exp);
                }
            
        }
        
        public async Task<List<EmailActivityCatgorys>> GetByUserTagAsync(long TopicID,long UserID)
        {
            try
            {
                // var query = "SELECT UserTag, ID as UserTagId,TopicId as UserTagTopicId,AddedByUserID as UserTagAddedByUserID FROM EmailTopicUserTags where TopicID = @TopicID and AddedByUserID = @UserID";
                var query = @"
                            SELECT ETU.UserTag , ET.UserTagID as UserTagId, ET.TopicID as UserTagTopicId,ET.AddedByUserID as UserTagAddedByUserID FROM EmailUserTagMultiple ET
                            Left join EmailTopicUserTags ETU ON ETU.ID = ET.UserTagID
                            where ET.TopicID = @TopicID and ET.AddedByUserID = @UserID";
                var parameters = new DynamicParameters();
                parameters.Add("TopicID", TopicID);
                parameters.Add("UserID", UserID);
                

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailActivityCatgorys>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<EmailActivityCatgorys>> GetAllUserTagAsync(long UserID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                //  var query = "SELECT DISTINCT UserTag FROM EmailTopicUserTags where AddedByUserID = @UserID and UserTag IS NOT NULL AND UserTag != ''";
                var query = @"SELECT DISTINCT UserTag ,Min(ID)  as ID FROM EmailTopicUserTags where AddedByUserID = @UserID and UserTag IS NOT NULL AND UserTag != ''Group By UserTag";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<EmailActivityCatgorys>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<string> UpdateOtherAsync(string othertag,string Name, long ModifiedByUserID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var modifiedDate = DateTime.Now;
                        var parameters = new DynamicParameters();
                        parameters.Add("othertag", othertag);
                        parameters.Add("Name", Name);
                        parameters.Add("ModifiedByUserID", ModifiedByUserID);
                        parameters.Add("modifiedDate", modifiedDate);

                        var query = "Update EmailActivityCatgorys Set Name = @Name,ModifiedByUserID =@ModifiedByUserID,ModifiedDate = @modifiedDate where Name = @othertag";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);

                        return rowsAffected.ToString();
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

        public async Task<string> UpdateuserAsync(string userTag, string Name)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("userTag", userTag);
                        parameters.Add("Name", Name);

                        var query = "Update EmailTopicUserTags Set UserTag = @Name where UserTag = @userTag";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);

                        return rowsAffected.ToString();
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

        public async Task<long> DeleteUserTagAsync(long ID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = "";
                        var parameters = new DynamicParameters();
                        parameters.Add("id", ID);


                        query += "DELETE FROM EmailUserTagMultiple WHERE UserTagID = @id;";
                         query += "DELETE  FROM EmailTopicUserTags WHERE ID = @id;";
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
