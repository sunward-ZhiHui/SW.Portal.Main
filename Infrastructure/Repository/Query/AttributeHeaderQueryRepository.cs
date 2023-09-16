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
    public class AttributeHeaderQueryRepository : QueryRepository<AttributeHeader>, IAttributeQueryRepository
    {
        public AttributeHeaderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }

        public  async Task<long> DeleteAsync(long id)
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

                            var query = "DELETE  FROM AttributeHeader WHERE AttributeID = @id";


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

        public async Task<IReadOnlyList<AttributeHeader>> GetAllAsync(long ID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", ID, DbType.Int64);

                var query = "SELECT * FROM AttributeHeader Where AttributeID = @ID ";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeader>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<AttributeHeader> GetByIdAsync(long id)
        {
            try
            {
                var query = "SELECT * FROM AttributeHeader WHERE AttrubuteID = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int64);

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<AttributeHeader>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public  async Task<long> Insert(AttributeHeader attributeHeader)
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
                            parameters.Add("AttributeName", attributeHeader.AttributeName);
                            parameters.Add("IsInternal", attributeHeader.IsInternal);
                            parameters.Add("Description", attributeHeader.Description);
                            parameters.Add("ControlType", attributeHeader.ControlType);
                            parameters.Add("EntryMask", attributeHeader.EntryMask);
                            parameters.Add("RegExp", attributeHeader.RegExp);
                            parameters.Add("AddedByUserID", attributeHeader.AddedByUserID);
                            parameters.Add("AddedDate", attributeHeader.AddedDate);
                            parameters.Add("SessionId", attributeHeader.SessionId);
                            parameters.Add("StatusCodeID", attributeHeader.StatusCodeID);

                            //var query = "INSERT INTO AttributeHeader(AttributeName,IsInternal,Description,ControlType,EntryMask,RegExp,AddedByUserID,AddedDate,SessionId,StatusCodeID) VALUES (@AttributeName,@IsInternal,@Description,@ControlType,@EntryMask,@RegExp,@AddedByUserID,@AddedDate,@SessionId,@StatusCodeID)";

                            //var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                            //transaction.Commit();

                            //return rowsAffected;

                            var query = @"INSERT INTO AttributeHeader(AttributeName,IsInternal,Description,ControlType,EntryMask,RegExp,AddedByUserID,AddedDate,SessionId,StatusCodeID) 
              OUTPUT INSERTED.AttributeID  -- Replace 'YourIDColumn' with the actual column name of your IDENTITY column
              VALUES (@AttributeName,@IsInternal,@Description,@ControlType,@EntryMask,@RegExp,@AddedByUserID,@AddedDate,@SessionId,@StatusCodeID)";

                            var insertedId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);

                            transaction.Commit();

                            return insertedId;

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

        public Task<long> UpdateAsync(AttributeHeader attributeHeader)
        {
            throw new NotImplementedException();
        }
    }
}
