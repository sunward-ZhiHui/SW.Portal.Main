using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class AttributeHeaderQueryRepository : QueryRepository<AttributeHeader>, IAttributeQueryRepository
    {
        public AttributeHeaderQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

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

        public async Task<IReadOnlyList<AttributeHeader>> GetAllAttributeName()
        {
            try
            {
                var query = "select t1.*,t2.CodeValue as ControlType from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId";


                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<AttributeHeader>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<AttributeHeader>> GetAllAttributeNameAsync(string? ID)
        {
            try
            {
                AttributeHeaderListModel attributeHeaderListModel = new AttributeHeaderListModel();
                var ids = string.IsNullOrEmpty(ID) ? "-1" : ID;

                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(@"select t1.*,t2.CodeValue as ControlType from AttributeHeader t1  JOIN CodeMaster t2 ON t2.CodeID=t1.ControlTypeId where " +
                   "t1.AttributeID in(" + string.Join(',', ids) + ");select * from AttributeDetails WHERE AttributeID in(" + string.Join(',', ids) + ");");
                    attributeHeaderListModel.AttributeHeader = results.Read<AttributeHeader>().ToList();
                    attributeHeaderListModel.AttributeDetails = results.Read<AttributeDetails>().ToList();
                    if (attributeHeaderListModel.AttributeHeader != null)
                    {
                        attributeHeaderListModel.AttributeHeader.ForEach(s =>
                        {
                            s.AttributeName = s.AttributeName.Replace(" ", "_");
                            if (s.ControlType == "TextBox" || s.ControlType == "ComboBox" || s.ControlType == "Memo" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                            {
                                s.DataType = "string";
                            }
                            else if (s.ControlType == "DateEdit" || s.ControlType == "TimeEdit")
                            {
                                s.DataType = "DateTime";
                            }
                            else if (s.ControlType == "SpinEdit")
                            {
                                s.DataType = "int";
                            }
                            else if (s.ControlType == "CheckBox")
                            {
                                s.DataType = "bool";
                            }
                            else if (s.ControlType == "ListBox" || s.ControlType == "TagBox")
                            {
                                s.DataType = "list";
                            }
                            else
                            {
                                s.DataType = "string";
                            }
                            s.AttributeDetails = attributeHeaderListModel.AttributeDetails.Where(w => w.AttributeID == s.AttributeID).ToList();
                        });

                    }
                    return attributeHeaderListModel.AttributeHeader;
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

        public async Task<IReadOnlyList<DynamicForm>> GetComboBoxLst()
        {
            try
            {
                var query = "select DISTINCT ScreenID from  DynamicForm";


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

        public async Task<long> Insert(AttributeHeader attributeHeader)
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
                            if (attributeHeader.AttributeID > 0)
                            {
                                parameters.Add("AttributeID", attributeHeader.AttributeID);
                                parameters.Add("AttributeName", attributeHeader.AttributeName, DbType.String);
                                parameters.Add("IsInternal", attributeHeader.IsInternal);
                                parameters.Add("Description", attributeHeader.Description, DbType.String);

                                parameters.Add("ControlType", attributeHeader.ControlType);
                                parameters.Add("EntryMask", attributeHeader.EntryMask);
                                parameters.Add("RegExp", attributeHeader.RegExp);
                                parameters.Add("ModifiedByUserID", attributeHeader.ModifiedByUserID);
                                parameters.Add("ModifiedDate", attributeHeader.ModifiedDate);
                                parameters.Add("StatusCodeID", attributeHeader.StatusCodeID);
                                parameters.Add("ControlTypeId", attributeHeader.ControlTypeId);
                                parameters.Add("IsMultiple", attributeHeader.IsMultiple);
                                parameters.Add("IsRequired", attributeHeader.IsRequired);
                                parameters.Add("RequiredMessage", attributeHeader.RequiredMessage, DbType.String);
                                var Addquerys = "UPDATE AttributeHeader SET AttributeName = @AttributeName,IsInternal=@IsInternal,Description=@Description," +
                                    "ControlType=@ControlType,EntryMask=@EntryMask, " +
                                    "RegExp=@RegExp,ModifiedByUserID=@ModifiedByUserID, " +
                                    "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID, " +
                                    "ControlTypeId=@ControlTypeId,IsMultiple=@IsMultiple, " +
                                    "IsRequired=@IsRequired,RequiredMessage=@RequiredMessage " +
                                    "WHERE  AttributeID = @AttributeID";
                                await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters, transaction);
                            }
                            else
                            {
                                parameters.Add("AttributeName", attributeHeader.AttributeName, DbType.String);
                                parameters.Add("IsInternal", attributeHeader.IsInternal);
                                parameters.Add("Description", attributeHeader.Description, DbType.String);

                                parameters.Add("ControlType", attributeHeader.ControlType);
                                parameters.Add("EntryMask", attributeHeader.EntryMask);
                                parameters.Add("RegExp", attributeHeader.RegExp);
                                parameters.Add("AddedByUserID", attributeHeader.AddedByUserID);
                                parameters.Add("AddedDate", attributeHeader.AddedDate);
                                parameters.Add("SessionId", attributeHeader.SessionId);
                                parameters.Add("StatusCodeID", attributeHeader.StatusCodeID);
                                parameters.Add("ControlTypeId", attributeHeader.ControlTypeId);
                                parameters.Add("IsMultiple", attributeHeader.IsMultiple);
                                parameters.Add("IsRequired", attributeHeader.IsRequired);
                                parameters.Add("RequiredMessage", attributeHeader.RequiredMessage, DbType.String);


                                var query = @"INSERT INTO AttributeHeader(AttributeName,IsInternal,Description,ControlType,EntryMask,RegExp,AddedByUserID,AddedDate,SessionId,StatusCodeID,ControlTypeId,IsMultiple,IsRequired,RequiredMessage) 
              OUTPUT INSERTED.AttributeID  -- Replace 'YourIDColumn' with the actual column name of your IDENTITY column
              VALUES (@AttributeName,@IsInternal,@Description,@ControlType,@EntryMask,@RegExp,@AddedByUserID,@AddedDate,@SessionId,@StatusCodeID,@ControlTypeId,@IsMultiple,@IsRequired,@RequiredMessage)";

                                var insertedId = await connection.ExecuteScalarAsync<int>(query, parameters, transaction);
                                attributeHeader.AttributeID = insertedId;
                            }
                            transaction.Commit();

                            return attributeHeader.AttributeID;

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
