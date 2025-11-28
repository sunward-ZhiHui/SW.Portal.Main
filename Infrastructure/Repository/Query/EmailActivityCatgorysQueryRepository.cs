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
        /// <summary>
        /// Retrieves the distinct list of activity category names from <c>EmailActivityCatgorys</c>.
        /// </summary>
        /// <returns>
        /// A read-only list of <see cref="EmailActivityCatgorys"/> where each record represents
        /// a distinct category name with its minimum ID.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while creating the database connection or executing the query.
        /// The original exception is included as the inner exception.
        /// </exception>
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

        /// <summary>
        /// Retrieves all activity categories for a specific topic, including group, category,
        /// and action display names and any multiple action names.
        /// </summary>
        /// <param name="TopicId">
        /// The identifier of the topic whose activity categories are being retrieved.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="EmailActivityCatgorys"/> populated with group, category,
        /// action names, and related action name list.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while querying the database or populating action names.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Retrieves all action tag IDs associated with a specific topic from <c>EmailActionTagMultiple</c>.
        /// </summary>
        /// <param name="TopicId">
        /// The identifier of the topic whose action tags are requested.
        /// </param>
        /// <returns>
        /// A list of action tag IDs (<see cref="long"/>) associated with the topic.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the query.
        /// </exception>
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
        /// <summary>
        /// Retrieves the tag lock status for a topic from the first root conversation (ReplyId = 0).
        /// </summary>
        /// <param name="TopicId">
        /// The identifier of the topic whose tag lock information is requested.
        /// </param>
        /// <returns>
        /// <c>true</c> if the tag lock is enabled; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while querying the database.
        /// </exception>
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
        /// <summary>
        /// Inserts a new activity category for a topic and manages its related action tags
        /// in the <c>EmailActionTagMultiple</c> table.
        /// </summary>
        /// <param name="emailActivityCatgorys">
        /// The <see cref="EmailActivityCatgorys"/> model containing topic, tags, description,
        /// status, and audit information, along with action tag IDs.
        /// </param>
        /// <returns>
        /// The number of rows affected by the insert operation in <c>EmailActivityCatgorys</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while deleting or inserting action tags or inserting the activity category.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Updates an existing activity category for a topic and refreshes its related action tags
        /// in the <c>EmailActionTagMultiple</c> table.
        /// </summary>
        /// <param name="emailActivityCatgorys">
        /// The <see cref="EmailActivityCatgorys"/> model containing updated tag, description,
        /// status, and audit information, along with action tag IDs.
        /// </param>
        /// <returns>
        /// The number of rows affected by the update operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while updating action tags or the activity category.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Deletes an activity category by ID and removes all related action tags for the topic.
        /// </summary>
        /// <param name="id">
        /// The identifier of the activity category to delete.
        /// </param>
        /// <param name="TopicId">
        /// The topic identifier used to delete related records in <c>EmailActionTagMultiple</c>.
        /// </param>
        /// <returns>
        /// The number of rows affected by the delete operation in <c>EmailActivityCatgorys</c>.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while deleting action tags or the activity category.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Retrieves all activity categories for a specific topic from <c>EmailActivityCatgorys</c>.
        /// </summary>
        /// <param name="TopicId">
        /// The identifier of the topic whose activity category records are requested.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="EmailActivityCatgorys"/> for the given topic.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the query.
        /// </exception>
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
        /// <summary>
        /// Retrieves user tag records for a specific topic and user from <c>EmailUserTagMultiple</c>
        /// and <c>EmailTopicUserTags</c>.
        /// </summary>
        /// <param name="TopicID">
        /// The identifier of the topic whose user tags are requested.
        /// </param>
        /// <param name="UserID">
        /// The identifier of the user who added the tags.
        /// </param>
        /// <returns>
        /// A list of <see cref="EmailActivityCatgorys"/> representing user tag mappings for the topic and user.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while querying the database.
        /// </exception>
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
        /// <summary>
        /// Retrieves all distinct user tags created by a specific user from <c>EmailTopicUserTags</c>.
        /// </summary>
        /// <param name="UserID">
        /// The identifier of the user whose user tags are requested.
        /// </param>
        /// <returns>
        /// A list of <see cref="EmailActivityCatgorys"/> where each record represents a distinct user tag
        /// and its minimum ID.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the database query.
        /// </exception>
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
        /// <summary>
        /// Updates all activity category records that have a specific "other" name,
        /// replacing it with a new name and updating audit information.
        /// </summary>
        /// <param name="othertag">
        /// The existing name value to be replaced.
        /// </param>
        /// <param name="Name">
        /// The new name to set for matching records.
        /// </param>
        /// <param name="ModifiedByUserID">
        /// The identifier of the user performing the update.
        /// </param>
        /// <returns>
        /// The number of rows affected, returned as a string.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the update query.
        /// </exception>
        public async Task<string> UpdateOtherAsync(string othertag,string Name, long ModifiedByUserID)
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

        /// <summary>
        /// Updates the name of a specific activity category by ID and sets modification audit information.
        /// </summary>
        /// <param name="id">
        /// The identifier of the activity category to update.
        /// </param>
        /// <param name="Name">
        /// The new name to set for the activity category.
        /// </param>
        /// <param name="ModifiedByUserID">
        /// The identifier of the user performing the update.
        /// </param>
        /// <returns>
        /// The number of rows affected, returned as a string.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the update query.
        /// </exception>
        public async Task<string> UpdateOtherTagAsync(long id, string Name, long ModifiedByUserID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var modifiedDate = DateTime.Now;
                        var parameters = new DynamicParameters();
                        parameters.Add("id", id);
                        parameters.Add("Name", Name);
                        parameters.Add("ModifiedByUserID", ModifiedByUserID);
                        parameters.Add("modifiedDate", modifiedDate);

                        var query = "Update EmailActivityCatgorys Set Name = @Name,ModifiedByUserID =@ModifiedByUserID,ModifiedDate = @modifiedDate where ID = @id";
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
        /// <summary>
        /// Updates all occurrences of a specific user tag value in <c>EmailTopicUserTags</c>
        /// with a new value.
        /// </summary>
        /// <param name="userTag">
        /// The existing user tag value to be updated.
        /// </param>
        /// <param name="Name">
        /// The new user tag value to set.
        /// </param>
        /// <returns>
        /// The number of rows affected, returned as a string.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the update statement.
        /// </exception>
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

        /// <summary>
        /// Deletes a user tag by its text value for a specific user in <c>EmailTopicUserTags</c>.
        /// </summary>
        /// <param name="UserID">
        /// The identifier of the user who owns the tag.
        /// </param>
        /// <param name="UserTag">
        /// The text of the user tag to delete.
        /// </param>
        /// <returns>
        /// The number of rows affected by the delete operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the delete query.
        /// </exception>
        public async Task<long> DeleteUserTagNameAsync(long UserID, string UserTag)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        var parameters = new DynamicParameters();                        
                        parameters.Add("UserID", UserID);
                        parameters.Add("UserTag", UserTag);
                        var query = "DELETE FROM EmailTopicUserTags WHERE UserTag = @UserTag and AddedByUserID = @UserID";

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
        /// <summary>
        /// Deletes a specific user tag mapping from <c>EmailUserTagMultiple</c>
        /// for a given topic, user, and tag ID.
        /// </summary>
        /// <param name="Topicid">
        /// The identifier of the topic associated with the mapping.
        /// </param>
        /// <param name="UserID">
        /// The identifier of the user who owns the mapping.
        /// </param>
        /// <param name="UserTagID">
        /// The identifier of the user tag to remove from the mapping.
        /// </param>
        /// <returns>
        /// The number of rows affected by the delete operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the delete query.
        /// </exception>
        public async Task<long> DeleteUserTagAsync(long Topicid,long UserID,long UserTagID)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                       
                        var parameters = new DynamicParameters();
                        parameters.Add("Topicid", Topicid);
                        parameters.Add("UserID", UserID);
                        parameters.Add("UserTagID", UserTagID);
                        var query = "DELETE FROM EmailUserTagMultiple WHERE TopicID = @Topicid and AddedByUserID = @UserID And UserTagID = @UserTagID";
                        
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

        /// <summary>
        /// Deletes all mappings for a specific user tag and then deletes the tag itself
        /// from <c>EmailUserTagMultiple</c> and <c>EmailTopicUserTags</c>.
        /// </summary>
        /// <param name="ID">
        /// The identifier of the user tag in <c>EmailTopicUserTags</c>.
        /// </param>
        /// <param name="UserID">
        /// The identifier of the user who owns the tag.
        /// </param>
        /// <param name="tagid">
        /// The identifier of the user tag used in <c>EmailUserTagMultiple</c>.
        /// </param>
        /// <returns>
        /// The total number of rows affected by both delete operations.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the delete statements.
        /// </exception>
        public async Task<long> DeleteUserAllTagAsync(long ID, long UserID, long tagid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = "";
                        var parameters = new DynamicParameters();
                        parameters.Add("ID", ID);
                        parameters.Add("UserID", UserID);
                        parameters.Add("tagid", tagid);



                        query += "DELETE FROM EmailUserTagMultiple WHERE UserTagID = @tagid and AddedByUserID = @UserID;";

                        query += "DELETE FROM EmailTopicUserTags WHERE  ID = @ID;";

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
        /// <summary>
        /// Retrieves all activity category records with a specific "other" name,
        /// including the associated topic name.
        /// </summary>
        /// <param name="Others">
        /// The name value used to filter activity categories.
        /// </param>
        /// <returns>
        /// A list of <see cref="EmailActivityCatgorys"/> records including topic information
        /// for the given name.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while querying the database.
        /// </exception>
        public async Task<List<EmailActivityCatgorys>> GetAllOthersAsync(string Others)
        {
            try
            {
                var query = @"select EC.ID, ET.TopicName , EC.Name From EmailActivityCatgorys EC 
                                Left join EmailTopics ET ON ET.ID = EC.TopicID 
                                 where EC.Name = @Others";
                var parameters = new DynamicParameters();
                parameters.Add("Others", Others);

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
        /// <summary>
        /// Retrieves all user tag multiple mappings for a specific user tag ID
        /// from <c>EmailUserTagMultiple</c>.
        /// </summary>
        /// <param name="UserTagID">
        /// The identifier of the user tag whose mappings are requested.
        /// </param>
        /// <returns>
        /// A list of <see cref="EmailActivityCatgorys"/> records representing mappings
        /// for the specified user tag ID.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the query.
        /// </exception>
        public async Task<List<EmailActivityCatgorys>> GetAllUserlistAsync(long UserTagID)
        {
            try
            {
                var query = @"select * From EmailUserTagMultiple where UserTagID = @UserTagID";
                var parameters = new DynamicParameters();
                parameters.Add("UserTagID", UserTagID);

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

        /// <summary>
        /// Retrieves all topic mappings for a given user tag and user,
        /// including topic and tag details.
        /// </summary>
        /// <param name="UserTag">
        /// The user tag text whose mappings are requested.
        /// </param>
        /// <param name="UserID">
        /// The identifier of the user who owns the tag.
        /// </param>
        /// <returns>
        /// A list of <see cref="EmailActivityCatgorys"/> representing topics and mappings
        /// associated with the given user tag and user.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the query.
        /// </exception>
        public async  Task<List<EmailActivityCatgorys>> GetAllUsersAsync(string UserTag, long UserID)
        {
            try
            {
                var query = @"SELECT EM.TopicID,ET.TopicName ,EM.AddedByUserID,EM.UserTagID,ETU.ID,ETU.UserTag,EM.ID as MultipleID From EmailTopicUserTags ETU
                                Inner Join EmailUserTagMultiple EM ON EM.UserTagID = ETU.ID
                                Inner Join EmailTopics ET ON ET.ID = EM.TopicID
                                Where ETU.UserTag =@UserTag and ETU.AddedByUserID = @UserID";
                var parameters = new DynamicParameters();
                parameters.Add("UserTag", UserTag);
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
    }
}
