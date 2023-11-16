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
    public class ToDoNotesHistoryQueryRepository : QueryRepository<ToDoNotesHistory>, IToDoNotesHistoryQueryRepository
    {
        public ToDoNotesHistoryQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IReadOnlyList<ToDoNotesHistory>> GetAllAsync()
        {
            try
            {
                var query = "SELECT  * FROM ToDoNotesHistory";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ToDoNotesHistory>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ToDoNotesHistory>> GetTodoDueAsync(long UserId)
        {
            try
            {
                //var query = "SELECT * FROM ToDoNotesHistory WHERE AddedByUserID = @UserId AND TopicId IS NOT NULL AND CAST(DueDate AS DATE) = CAST(GETDATE() AS DATE)";
                var query = @"SELECT TNH.*,EC.Name AS SubjectName FROM ToDoNotesHistory TNH
                                INNER JOIN EmailConversations EC ON EC.ID = TNH.TopicId
                                WHERE TNH.AddedByUserID = @UserId
                                AND TNH.TopicId IS NOT NULL  AND TNH.TopicId > 0 AND TNH.Status = 'Open'
                                AND CAST(TNH.DueDate AS DATE) <= CAST(GETDATE() AS DATE)
                                ORDER BY TNH.DueDate DESC";
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    //return (await connection.QueryAsync<ToDoNotesHistory>(query,parameters)).ToList();
                    
                    var res = connection.Query<ToDoNotesHistory>(query, parameters).ToList();

                    //foreach (var items in res)
                    //{
                    //    if (items.Users != null && items.Users.Length != 0)
                    //    {
                    //        string[] userArray = items.Users.Split(',');
                    //        var subQuery = $"SELECT * FROM View_Employee WHERE UserID IN ({string.Join(",", userArray)})";
                    //        var subQueryResults = connection.Query<ViewEmployee>(subQuery).ToList();
                    //        items.participant = subQueryResults;

                    //    }

                    //}

                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ToDoNotesHistory>> GetTodoRemainderAsync(long UserId)
        {
            try
            {

                var query = @"SELECT TNH.*,EC.Name AS SubjectName , ET.TopicName as MainSubject,TD.Notes as NoteName,AP.UserName as AssignTo FROM ToDoNotesHistory TNH
                                INNER JOIN EmailConversations EC ON EC.ID = TNH.TopicId
                                INNER JOIN EmailTopics ET ON ET.ID = EC.TopicId
                                INNER JOIN ToDoNotes TD ON TD.ID = TNH.NotesId
                                LEFT JOIN ApplicationUser AP ON AP.UserID = TNH.Users
                                WHERE TNH.AddedByUserID = @UserId
                                    AND TNH.TopicId IS NOT NULL
                                    AND TNH.TopicId > 0
                                    AND TNH.Status = 'Open'
                                    AND (
                                    CAST(TNH.DueDate AS DATE) BETWEEN DATEADD(DAY, -7, CAST(GETDATE() AS DATE)) AND CAST(GETDATE() AS DATE)
                                    OR
                                    CAST(TNH.DueDate AS DATE) BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(DAY, 7, CAST(GETDATE() AS DATE))
                                    )
                                ORDER BY TNH.DueDate;";
                // var query = "SELECT * FROM ToDoNotesHistory WHERE AddedByUserID = @UserId AND TopicId IS NOT NULL AND CAST(RemainDate AS DATE) = CAST(GETDATE() AS DATE)";
                //var query = @"SELECT TNH.*,EC.Name AS SubjectName FROM ToDoNotesHistory TNH
                //                INNER JOIN EmailConversations EC ON EC.ID = TNH.TopicId 
                //                WHERE TNH.AddedByUserID = @UserId
                //                AND TNH.TopicId IS NOT NULL  AND TNH.TopicId > 0 AND TNH.Status = 'Open'
                //                AND CAST(TNH.RemainDate AS DATE) <= CAST(GETDATE() AS DATE)
                //                ORDER BY TNH.RemainDate DESC";
                var parameters = new DynamicParameters();
                parameters.Add("UserId", UserId);
                using (var connection = CreateConnection())
                {
                    //return (await connection.QueryAsync<ToDoNotesHistory>(query,parameters)).ToList();
                    
                    var res = connection.Query<ToDoNotesHistory>(query, parameters).ToList();

                    //foreach (var items in res)
                    //{
                    //    if (items.Users != null && items.Users.Length != 0)
                    //    {
                    //        string[] userArray = items.Users.Split(',');
                    //        var subQuery = $"SELECT * FROM View_Employee WHERE UserID IN ({string.Join(",", userArray)})";
                    //        var subQueryResults = connection.Query<ViewEmployee>(subQuery).ToList();
                    //        items.participant = subQueryResults;

                    //    }

                    //}

                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<IReadOnlyList<Documents>> GetToDoDocumentsAsync(string SessionId)
        {
            try
            {
                var query = @"SELECT * FROM Documents WHERE SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);                

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
        public async Task<IReadOnlyList<ToDoNotesHistory>> GetByToDoSessionIdAsync(Guid SessionId)
        {
            try
            {
                var query = @"SELECT EC.Name AS SubjectName , ET.TopicName as MainSubject,TD.Notes as NoteName,TNH.* FROM ToDoNotesHistory TNH
                                INNER JOIN EmailConversations EC ON EC.ID = TNH.TopicId
                                INNER JOIN EmailTopics ET ON ET.ID = EC.TopicId  
                                INNER JOIN ToDoNotes TD ON TD.ID = TNH.NotesId  
                                WHERE TNH.SessionId = @SessionId";
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ToDoNotesHistory>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
        public async Task<IReadOnlyList<ToDoNotesHistory>> GetAllToDoNotesHistoryAsync(long NotesId,long UserId)
        {
            try
            {

                var query = @"SELECT TNH. *, Ap.UserName as AssignToList FROM ToDoNotesHistory TNH
                    LEFT join ApplicationUser Ap on Ap.UserID = TNH.Users
                     WHERE TNH.AddedByUserID = @UserId AND TNH.NotesId = @NotesId   ORDER BY Completed ASC";
                //var query = @"SELECT * FROM ToDoNotesHistory
                //            WHERE AddedByUserID = @UserId AND NotesId = @NotesId
                //            ORDER BY Completed ASC";
                var parameters = new DynamicParameters();
                parameters.Add("NotesId", NotesId);
                parameters.Add("UserId", UserId);

                using (var connection = CreateConnection())
                {
                    //return (await connection.QueryAsync<ToDoNotesHistory>(query, parameters)).ToList();
                    //
                    
                    var res = connection.Query<ToDoNotesHistory>(query, parameters).ToList();

                    //foreach (var items in res)
                    //{
                    //    if(items.Users != null && items.Users.Length != 0)
                    //    {
                    //        string[] userArray = items.Users.Split(',');
                    //        var subQuery = $"SELECT * FROM View_Employee WHERE UserID IN ({string.Join(",", userArray)})";
                    //        var subQueryResults = connection.Query<ViewEmployee>(subQuery).ToList();
                    //        items.participant = subQueryResults;

                    //    }
                                              
                    //}

                    return res;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(ToDoNotesHistory ToDoNotesHistory)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();                           
                            parameters.Add("Description", ToDoNotesHistory.Description);
                            parameters.Add("NotesId", ToDoNotesHistory.NotesId);
                            parameters.Add("DueDate", ToDoNotesHistory.DueDate);
                            parameters.Add("RemainDate", ToDoNotesHistory.RemainDate);
                            parameters.Add("StatusCodeID", ToDoNotesHistory.StatusCodeID);
                            parameters.Add("AddedByUserID", ToDoNotesHistory.AddedByUserID);
                            parameters.Add("ModifiedByUserID", ToDoNotesHistory.ModifiedByUserID);
                            parameters.Add("AddedDate", ToDoNotesHistory.AddedDate);
                            parameters.Add("ModifiedDate", ToDoNotesHistory.ModifiedDate);
                            parameters.Add("SessionId", ToDoNotesHistory.SessionId);
                            parameters.Add("Status", ToDoNotesHistory.Status);
                            parameters.Add("ColourCode", ToDoNotesHistory.ColourCode);
                            parameters.Add("Users", ToDoNotesHistory.Users);
                            parameters.Add("TopicId", ToDoNotesHistory.TopicId);


                            var query = "INSERT INTO ToDoNotesHistory(TopicId,Users,NotesId,Description,DueDate,RemainDate,StatusCodeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,SessionId,Status,ColourCode) VALUES (@TopicId,@Users,@NotesId,@Description,@DueDate,@RemainDate,@StatusCodeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@SessionId,@Status,@ColourCode)";

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
        public async Task<long> UpdateAsync(ToDoNotesHistory ToDoNotesHistory)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ID", ToDoNotesHistory.ID);                            
                            parameters.Add("Description", ToDoNotesHistory.Description);
                            parameters.Add("DueDate", ToDoNotesHistory.DueDate);
                            parameters.Add("RemainDate", ToDoNotesHistory.RemainDate);
                            parameters.Add("ModifiedByUserID", ToDoNotesHistory.ModifiedByUserID);
                            parameters.Add("ModifiedDate", ToDoNotesHistory.ModifiedDate);
                            parameters.Add("Status", ToDoNotesHistory.Status);
                            parameters.Add("ColourCode", ToDoNotesHistory.ColourCode);
                            parameters.Add("Users", ToDoNotesHistory.Users);
                            parameters.Add("TopicId", ToDoNotesHistory.TopicId);


                            var query = @"Update ToDoNotesHistory SET TopicId = @TopicId, Users = @Users,Description = @Description,DueDate= @DueDate,RemainDate = @RemainDate,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,Status = @Status,ColourCode = @ColourCode WHERE ID = @ID";

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

                            var query = "Delete From ToDoNotesHistory  WHERE ID = @id";


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

        public async Task<IReadOnlyList<ViewEmployee>> GetUserLst(string Userid)
        {
            try
            {
                var query = "SELECT * From View_Employee where UserID In (@Userid)";
                var parameters = new DynamicParameters();
                parameters.Add("Userid", Userid);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
       

        public async Task<long> StatusUpdateAsync(long ID)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    
                    

                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ID", ID);

                            var query = "update TodoNotesHistory Set Status = 'close' Where ID =@ID";


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
