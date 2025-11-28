using Core.Entities;
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
    public class TopicTodoListQueryRepository : QueryRepository<TopicToDoList>, ITopicTodoListQueryRepository
    {
        public TopicTodoListQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        /// <summary>
        /// Retrieves all to-do list items created by the specified user,
        /// ordered by most recently added.
        /// </summary>
        /// <param name="Uid">
        /// The identifier of the user whose to-do items are requested.
        /// </param>
        /// <returns>
        /// A read-only list of <see cref="TopicToDoList"/> records belonging to the user.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while creating the database connection or executing the query.
        /// The original exception is included as the inner exception.
        /// </exception>
        public async Task<IReadOnlyList<TopicToDoList>> GetAllAsync(long Uid)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Uid", Uid);
                var query = "SELECT * FROM TopicToDoList where AddedByUserID = @Uid order by ID desc";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TopicToDoList>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        /// <summary>
        /// Inserts a new to-do item into the <c>TopicToDoList</c> table.
        /// </summary>
        /// <param name="todolist">
        /// The <see cref="TopicToDoList"/> model containing to-do name, topic reference,
        /// session, status, and audit information.
        /// </param>
        /// <returns>
        /// The number of rows affected by the insert operation (typically <c>1</c> on success).
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while connecting to the database or executing the insert statement.
        /// The original exception is included as the inner exception.
        /// </exception>
        public async Task<long> Insert(TopicToDoList todolist)
        {

            try
            {
                using (var connection = CreateConnection())
                {

                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("ToDoName", todolist.ToDoName);
                            parameters.Add("TopicId", todolist.TopicId);
                            parameters.Add("SessionId", todolist.SessionId);
                            parameters.Add("AddedByUserID", todolist.AddedByUserID);
                            parameters.Add("AddedDate", todolist.AddedDate);
                            parameters.Add("Iscompleted", todolist.Iscompleted);
                            parameters.Add("StatusCodeID", todolist.StatusCodeID);

                            var query = "INSERT INTO TopicToDoList(ToDoName,TopicId,SessionId,AddedByUserID,AddedDate,StatusCodeID,Iscompleted) VALUES (@ToDoName,@TopicId,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@Iscompleted)";

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
        /// Updates the completion status of a to-do item in the <c>TopicToDoList</c> table.
        /// </summary>
        /// <param name="todolist">
        /// The <see cref="TopicToDoList"/> model containing the item ID and updated completion status.
        /// </param>
        /// <returns>
        /// The number of rows affected by the update operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while connecting to the database or executing the update statement.
        /// The original exception is included as the inner exception.
        /// </exception>
        public async Task<long> Update(TopicToDoList todolist)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("Iscompleted", todolist.Iscompleted);
                            parameters.Add("ID", todolist.ID, DbType.Int64);

                            var query = " UPDATE TopicToDoList SET Iscompleted = @Iscompleted WHERE ID = @ID";

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
        /// Deletes a to-do item from the <c>TopicToDoList</c> table by its identifier.
        /// </summary>
        /// <param name="id">
        /// The identifier of the to-do item to delete.
        /// </param>
        /// <returns>
        /// The number of rows affected by the delete operation.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when an error occurs while creating the database connection or executing the delete statement.
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

                            var query = "DELETE  FROM TopicToDoList WHERE ID = @id";


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
