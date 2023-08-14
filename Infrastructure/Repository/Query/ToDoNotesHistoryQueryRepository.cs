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
        public async Task<IReadOnlyList<ToDoNotesHistory>> GetAllToDoNotesHistoryAsync(long NotesId,long UserId)
        {
            try
            {
                var query = @"SELECT * FROM ToDoNotesHistory
                            WHERE AddedByUserID = @UserId AND NotesId = @NotesId
                            ORDER BY Completed ASC";
                var parameters = new DynamicParameters();
                parameters.Add("NotesId", NotesId);
                parameters.Add("UserId", UserId);

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

        public async Task<long> Insert(ToDoNotesHistory ToDoNotesHistory)
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

                            var query = "INSERT INTO ToDoNotesHistory(NotesId,Description,DueDate,RemainDate,StatusCodeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,SessionId) VALUES (@NotesId,@Description,@DueDate,@RemainDate,@StatusCodeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@SessionId)";

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
            };
        }
        public async Task<long> UpdateAsync(ToDoNotesHistory ToDoNotesHistory)
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
                            parameters.Add("ID", ToDoNotesHistory.ID);                            
                            parameters.Add("Description", ToDoNotesHistory.Description);
                            parameters.Add("DueDate", ToDoNotesHistory.DueDate);
                            parameters.Add("RemainDate", ToDoNotesHistory.RemainDate);
                            parameters.Add("ModifiedByUserID", ToDoNotesHistory.ModifiedByUserID);
                            parameters.Add("ModifiedDate", ToDoNotesHistory.ModifiedDate);                           

                            var query = @"Update ToDoNotesHistory SET Description = @Description,DueDate= @DueDate,RemainDate = @RemainDate,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate WHERE ID = @ID";

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
            };
        }
        public async Task<long> DeleteAsync(long id)
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

                            var query = "UPDATE ToDoNotesHistory SET Completed=1 WHERE ID = @id";


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
