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

        public async Task<IReadOnlyList<TopicToDoList>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM TopicToDoList";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TopicToDoList>(query)).ToList();
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
                            parameters.Add("TopicName", todolist.TopicName, DbType.String);
                            var query = "INSERT INTO TopicToDoList(TopicName) VALUES (@TopicName)";

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
                            parameters.Add("TopicName", todolist.TopicName, DbType.String);
                            parameters.Add("ID", todolist.ID, DbType.Int64);

                            var query = " UPDATE TopicToDoList SET TopicName = @TopicName WHERE ID = @ID";


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


        public async Task<long> Delete(TopicToDoList todolist)
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
                            parameters.Add("TopicName", todolist.TopicName, DbType.String);
                            parameters.Add("ID", todolist.ID, DbType.Int64);

                            var query = "DELETE  FROM TopicToDoList WHERE ID = @ID";


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
