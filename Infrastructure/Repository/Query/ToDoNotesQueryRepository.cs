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
        /// <summary>
        /// Retrieves the distinct list of notes created by a specific user,
        /// returning one entry per unique note text.
        /// </summary>
        /// <param name="userId">
        /// The identifier of the user whose notes are requested.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="ToDoNotes"/> where each record represents
        /// a distinct note with its minimum ID.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while creating the database connection or executing the query.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Retrieves all to-do notes for a user, optionally filtered by topic.
        /// </summary>
        /// <param name="UserID">
        /// The identifier of the user whose to-do notes are requested.
        /// </param>
        /// <param name="TopicId">
        /// The topic identifier to filter notes by.  
        /// If <c>0</c>, only notes without a topic (or topic ID 0) are returned.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="ToDoNotes"/> matching the user and topic criteria,
        /// ordered by completion status (completed last).
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the query or opening the connection.
        /// </exception>
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
        /// <summary>
        /// Inserts a new to-do note into the <c>ToDoNotes</c> table, with initial status set to <c>Open</c>.
        /// </summary>
        /// <param name="ToDoNotes">
        /// The <see cref="ToDoNotes"/> model containing topic, note text, status, session,
        /// and audit information.
        /// </param>
        /// <returns>
        /// The number of rows affected by the insert operation (typically <c>1</c> on success).
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while connecting to the database or executing the insert statement.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Updates the text and audit information of an existing to-do note.
        /// </summary>
        /// <param name="ToDoNotes">
        /// The <see cref="ToDoNotes"/> model containing the note ID and updated values.
        /// </param>
        /// <returns>
        /// The number of rows affected by the update operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the update query.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Updates all notes that match a specific existing note value,
        /// replacing it with a new note text and updating audit information.
        /// </summary>
        /// <param name="selectNotes">
        /// The current note text to search for and update.
        /// </param>
        /// <param name="Notes">
        /// The new note text to save.
        /// </param>
        /// <param name="userid">
        /// The identifier of the user performing the update.
        /// </param>
        /// <returns>
        /// The number of rows affected by the update operation, returned as a string.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the update query.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Marks a to-do note as completed by updating its <c>Completed</c> status to <c>Completed</c>.
        /// </summary>
        /// <param name="id">
        /// The identifier of the note to mark as completed.
        /// </param>
        /// <returns>
        /// The number of rows affected by the update operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while updating the record in the database.
        /// </exception>
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
        /// <summary>
        /// Permanently deletes a to-do note and its history from the database.
        /// </summary>
        /// <param name="id">
        /// The identifier of the note to delete.
        /// </param>
        /// <returns>
        /// The ID of the deleted note, returned after the delete operation completes.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the delete statements.
        /// The original exception is included as the inner exception.
        /// </exception>
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
        /// <summary>
        /// Marks a completed to-do note as incomplete by setting its <c>Completed</c>
        /// status back to <c>Open</c>.
        /// </summary>
        /// <param name="incompleteid">
        /// The identifier of the note to reopen.
        /// </param>
        /// <returns>
        /// The number of rows affected by the update operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while updating the note status.
        /// </exception>
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
        /// <summary>
        /// Retrieves all notes for a user that match the specified note text,
        /// including related topic and subtopic information.
        /// </summary>
        /// <param name="UserId">
        /// The identifier of the user whose notes are requested.
        /// </param>
        /// <param name="notes">
        /// The note text to filter by.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="ToDoNotes"/> including topic and subtopic details
        /// for matching notes.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while executing the query.
        /// </exception>
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
