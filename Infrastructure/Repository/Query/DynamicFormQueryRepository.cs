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
    public class DynamicFormQueryRepository : QueryRepository<DynamicForm>, IDynamicFormQueryRepository
    {
        public DynamicFormQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public  async Task<long> Delete(long id)
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

                            var query = "DELETE  FROM DynamicForm WHERE ID = @id";


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

        public async Task<IReadOnlyList<DynamicForm>> GetAllAsync()
        {
            try
            {
              
                var query = "SELECT * FROM DynamicForm";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicForm>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<IReadOnlyList<DynamicForm>> GetAllSelectedLst(Guid sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("sessionId", sessionId);

                var query = "SELECT * FROM DynamicForm Where SessionID = @sessionId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicForm>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(DynamicForm dynamicForm)
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
                            parameters.Add("Name", dynamicForm.Name);
                            parameters.Add("ScreenID", dynamicForm.ScreenID);
                            parameters.Add("SessionID", dynamicForm.SessionID);
                            parameters.Add("AttributeID", dynamicForm.AttributeID);


                            var query = "INSERT INTO DynamicForm(Name,ScreenID,SessionID,AttributeID) VALUES (@Name,@ScreenID,@SessionID,@AttributeID)";

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
                throw new NotImplementedException();
            }
        }

        public  async Task<long> Update(DynamicForm dynamicForm)
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
                            parameters.Add("AttributeID", dynamicForm.AttributeID);
                            parameters.Add("ID", dynamicForm.ID);
                            parameters.Add("Name", dynamicForm.Name);
                            parameters.Add("ScreenID", dynamicForm.ScreenID);

                            var query = " UPDATE DynamicForm SET AttributeID = @AttributeID,Name =@Name,ScreenID =@ScreenID WHERE ID = @ID";

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
