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
                var query = "SELECT DISTINCT Name FROM EmailActivityCatgorys where Name IS NOT NULL";

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

        public async Task<long> Insert(EmailActivityCatgorys emailActivityCatgorys)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
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
        public async Task<long> DeleteAsync(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {  
                    try
                        {
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
                var query = "SELECT UserTag, ID as UserTagId,TopicId as UserTagTopicId,AddedByUserID as UserTagAddedByUserID FROM EmailTopicUserTags where TopicID = @TopicID and AddedByUserID = @UserID";
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
                var query = "SELECT DISTINCT UserTag FROM EmailTopicUserTags where AddedByUserID = @UserID and UserTag IS NOT NULL";

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
