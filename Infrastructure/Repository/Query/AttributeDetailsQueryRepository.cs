using Core.Entities;
using Core.EntityModels;
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

                            //var query = "DELETE  FROM AttributeDetails WHERE AttributeDetailID = @id";
                            var query = "Update AttributeDetails SET Disabled=1 WHERE  AttributeDetailID = @id";


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

        public async Task<IReadOnlyList<AttributeDetails>> GetAllAsync()
        {
            try
            {
                var query = "SELECT * FROM AttributeDetails WHERE Disabled=0 OR Disabled IS NULL";
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
                var query = "SELECT * FROM AttributeDetails WHERE (Disabled=0 OR Disabled IS NULL) AND AttributeDetailID = @Id";
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


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("AttributeID", attributeDetails.AttributeID);
                        parameters.Add("AttributeDetailName", attributeDetails.AttributeDetailName);
                        parameters.Add("Description", attributeDetails.Description);
                        parameters.Add("Disabled", attributeDetails.Disabled);
                        parameters.Add("SessionId", attributeDetails.SessionId);
                        parameters.Add("AddedByUserID", attributeDetails.AddedByUserID);
                        parameters.Add("AddedDate", DateTime.Now);

                        parameters.Add("StatusCodeID", attributeDetails.StatusCodeID);

                        var query = "INSERT INTO AttributeDetails(AttributeID,AttributeDetailName,Description,Disabled,SessionId,AddedByUserID,AddedDate,StatusCodeID)  OUTPUT INSERTED.AttributeDetailID  VALUES (@AttributeID,@AttributeDetailName,@Description,@Disabled,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID)";

                        var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);


                        return insertedId;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }


                }

            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<IReadOnlyList<AttributeDetails>> LoadAttributelst(long Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("id", Id);

                var query = "SELECT * FROM AttributeDetails Where (Disabled=0 OR Disabled IS NULL) AND AttributeID = @id";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeDetails>(query, parameters)).ToList();
                }
            }


            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
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
                            parameters.Add("Disabled", attributeDetails.Disabled);
                            parameters.Add("ModifiedByUserID", attributeDetails.ModifiedByUserID);
                            parameters.Add("ModifiedDate", attributeDetails.ModifiedDate);
                            parameters.Add("AttributeDetailName", attributeDetails.AttributeDetailName);

                            parameters.Add("AttributeDetailID", attributeDetails.AttributeDetailID, DbType.Int64);

                            var query = " UPDATE AttributeDetails SET Description=@Description,Disabled = @Disabled,ModifiedByUserID =@ModifiedByUserID,ModifiedDate =@ModifiedDate,AttributeDetailName =@AttributeDetailName WHERE AttributeDetailID = @AttributeDetailID";

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
        public AttributeDetails AttributeDetailsValueCheckValidation(string? value, long attributeId, long? attributeDetailId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("AttributeID", attributeId);
                parameters.Add("AttributeDetailName", value);
                if (attributeDetailId > 0)
                {
                    parameters.Add("AttributeDetailID", attributeDetailId);
                    query = "SELECT * FROM AttributeDetails Where (Disabled=0 OR Disabled IS NULL) AND AttributeDetailID!=@AttributeDetailID AND AttributeDetailName=@AttributeDetailName AND AttributeID = @AttributeID";
                }
                else
                {
                    query = "SELECT * FROM AttributeDetails Where (Disabled=0 OR Disabled IS NULL) AND AttributeDetailName=@AttributeDetailName AND AttributeID = @AttributeID";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<AttributeDetails>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
