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

        public async Task<IReadOnlyList<ToDoNotes>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM ToDoNotes";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ToDoNotes>(query)).ToList();
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
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
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
        public async Task<long> UpdateAsync(ToDoNotes ToDoNotes)
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
                            parameters.Add("ID", ToDoNotes.ID);
                           
                            parameters.Add("Notes", ToDoNotes.Notes);
                            parameters.Add("ModifiedByUserID", ToDoNotes.ModifiedByUserID);
                            parameters.Add("ModifiedDate", ToDoNotes.ModifiedDate);                           

                            var query = @"Update ToDoNotes SET Notes = @Notes,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate WHERE ID = @ID";

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

                            var query = "UPDATE ToDoNotes SET Completed = 'Completed' WHERE ID = @id";


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
        public async Task<long> IncompleteAsync(long incompleteid)
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
                            parameters.Add("ID", incompleteid);

                            var query = "UPDATE ToDoNotes SET Completed = 'Open' WHERE ID = @ID";


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
