using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
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
    public class ToDoNotesQueryRepository : QueryRepository<ToDoNotes>, IToDoNotesQueryRepository
    {
        public ToDoNotesQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<ToDoNotes>> GetAllAsync(long userId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);
                // var query = "SELECT DISTINCT Notes FROM ToDoNotes where AddedByUserID = @userId ";
                var query = "SELECT DISTINCT Notes,Min(ID)  as ID FROM TodoNotes WHERE Notes IS NOT NULL  AND  AddedByUserID =  @userId AND Notes != '' Group By Notes";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ToDoNotes>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ToDoNotes>> GetAllToDoNotesAsync(long UserID,long TopicId)
        {
            try
            {
                // var query = @"SELECT * from ToDoNotes WHERE AddedByUserID = @UserID ORDER BY Completed ASC /*AND (Completed = 0 OR Completed IS NULL)*/";
                var query = @"
                            SELECT * 
                            FROM ToDoNotes
                            WHERE (@TopicId = 0 AND ( TopicId = 0 OR TopicId IS NULL ) AND AddedByUserID = @UserID)
                            OR (@TopicId > 0 AND  TopicId = @TopicId AND TopicId IS NOT NULL AND AddedByUserID = @UserID) ORDER BY Completed DESC";
                var parameters = new DynamicParameters();
                parameters.Add("UserID", UserID);
                parameters.Add("TopicId", TopicId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ToDoNotes>(query, parameters)).ToList();                    
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(ToDoNotes ToDoNotes)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("TopicId", ToDoNotes.TopicId);
                            parameters.Add("Notes", ToDoNotes.Notes);                           
                            parameters.Add("StatusCodeID", ToDoNotes.StatusCodeID);
                            parameters.Add("AddedByUserID", ToDoNotes.AddedByUserID);
                            parameters.Add("ModifiedByUserID", ToDoNotes.ModifiedByUserID);
                            parameters.Add("AddedDate", ToDoNotes.AddedDate);
                            parameters.Add("ModifiedDate", ToDoNotes.ModifiedDate);
                            parameters.Add("SessionId", ToDoNotes.SessionId);
                       

                        var query = "INSERT INTO ToDoNotes(TopicId,Notes,StatusCodeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,SessionId,Completed) VALUES (@TopicId,@Notes,@StatusCodeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@SessionId,'Open')";

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
        public async Task<long> UpdateAsync(ToDoNotes ToDoNotes)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ID", ToDoNotes.ID);
                           
                            parameters.Add("Notes", ToDoNotes.Notes);
                            parameters.Add("ModifiedByUserID", ToDoNotes.ModifiedByUserID);
                            parameters.Add("ModifiedDate", ToDoNotes.ModifiedDate);
                       

                        var query = @"Update ToDoNotes SET Notes = @Notes,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate  WHERE ID = @ID";

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
        public async Task<string> UpdateNoteAsync(string selectNotes, string Notes, long userid)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var Date = DateTime.Now;
                        var parameters = new DynamicParameters();
                        parameters.Add("selectNotes", selectNotes);

                        parameters.Add("Notes", Notes);
                        parameters.Add("ModifiedByUserID",userid);
                        parameters.Add("Date", Date);



                        var query = @"Update ToDoNotes SET Notes = @Notes,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@Date  WHERE Notes = @selectNotes";

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

                            var query = "UPDATE ToDoNotes SET Completed = 'Completed' WHERE ID = @id";


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
        public async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("id", id);
                        var query = string.Empty;

                        query += "delete From ToDoNotes where ID  = @id;";
                        query += "Delete From ToDoNotesHistory where NotesId = @id;";

                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return id;
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
        public async Task<long> IncompleteAsync(long incompleteid)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ID", incompleteid);

                            var query = "UPDATE ToDoNotes SET Completed = 'Open' WHERE ID = @ID";


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

        public async Task<IReadOnlyList<ToDoNotes>> GetAllNotesAsync(long UserId, string notes)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("userId", UserId);
                parameters.Add("notes", notes);
                // var query = "SELECT DISTINCT Notes FROM ToDoNotes where AddedByUserID = @userId ";
                var query = @"select EC.Name as SubTopic,ET.TopicName as MainTopic,TN.ID,TN.Notes,TN.TopicId From ToDoNotes  TN 
                                inner Join EmailConversations EC on EC.ID = TN.TopicId
                                Left Join EmailTopics ET ON ET.ID = EC.TopicID
                                where TN.Notes = @notes and TN.TopicID > 0 and TN.AddedByUserID = @userId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ToDoNotes>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
