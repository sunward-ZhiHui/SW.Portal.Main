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
        public async Task<long> Insert(TopicToDoList todolist)
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
                            parameters.Add("ToDoName", todolist.ToDoName);
                            parameters.Add("TopicId", todolist.TopicId);
                            parameters.Add("SessionId", todolist.SessionId);
                            parameters.Add("AddedByUserID", todolist.AddedByUserID);
                            parameters.Add("AddedDate", todolist.AddedDate);
                            parameters.Add("Iscompleted", todolist.Iscompleted);
                            parameters.Add("StatusCodeID", todolist.StatusCodeID);

                            var query = "INSERT INTO TopicToDoList(ToDoName,TopicId,SessionId,AddedByUserID,AddedDate,StatusCodeID,Iscompleted) VALUES (@ToDoName,@TopicId,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@Iscompleted)";

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
        public async Task<long> Update(TopicToDoList todolist)
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
                            parameters.Add("Iscompleted", todolist.Iscompleted);
                            parameters.Add("ID", todolist.ID, DbType.Int64);

                            var query = " UPDATE TopicToDoList SET Iscompleted = @Iscompleted WHERE ID = @ID";

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


        public async Task<long> Delete(long id)
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

                            var query = "DELETE  FROM TopicToDoList WHERE ID = @id";


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
