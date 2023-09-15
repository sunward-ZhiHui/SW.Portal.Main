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
    public class AttributeDetailsQueryRepository : QueryRepository<AttributeDetails>, IAttributeDetailsQueryRepository
    {
        public AttributeDetailsQueryRepository(IConfiguration configuration)
           : base(configuration)
        {

        }

        public Task<long> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<AttributeDetails>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM AttributeDetail";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeDetails>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<AttributeDetails> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM AttributeDetail WHERE AttrubuteID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<AttributeDetails>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long> Insert(AttributeDetails attributeDetails)
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
                            parameters.Add("Description", attributeDetails.Description);
                            parameters.Add("Disabled", attributeDetails.Disabled);
                            parameters.Add("SessionId", attributeDetails.SessionId);
                            parameters.Add("AddedByUserID", attributeDetails.AddedByUserID);
                            parameters.Add("AddedDate", DateTime.Now);
                          
                            parameters.Add("StatusCodeID", attributeDetails.StatusCodeID);

                            var query = "INSERT INTO AttributeDetails(Description,Disabled,SessionId,AddedByUserID,AddedDate,StatusCodeID,SortOrder) VALUES (@Description,@Disabled,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@SortOrder)";

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

        public async Task<long> UpdateAsync(AttributeDetails attributeDetails)
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
                            parameters.Add("Description", attributeDetails.Description);
                          
                            parameters.Add("AttributeDetailID", attributeDetails.AttributeDetailID, DbType.Int64);

                            var query = " UPDATE AttributeDetails SET Description=@Description WHERE AttributeDetailID = @AttributeDetailID";

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
            throw new NotImplementedException();
        }
    }
}
