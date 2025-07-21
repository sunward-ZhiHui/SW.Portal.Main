using Core.Entities;
using Core.Entities.Views;
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

                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    //var query = "DELETE  FROM AttributeDetails WHERE AttributeDetailID = @id";
                    var query = "Update AttributeDetails SET Disabled=1 WHERE  AttributeDetailID = @id";
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;
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
                    var parameters = new DynamicParameters();
                    parameters.Add("Description", attributeDetails.Description);
                    parameters.Add("Disabled", attributeDetails.Disabled);
                    parameters.Add("ModifiedByUserID", attributeDetails.ModifiedByUserID);
                    parameters.Add("ModifiedDate", attributeDetails.ModifiedDate);
                    parameters.Add("AttributeDetailName", attributeDetails.AttributeDetailName);

                    parameters.Add("AttributeDetailID", attributeDetails.AttributeDetailID, DbType.Int64);

                    var query = " UPDATE AttributeDetails SET Description=@Description,Disabled = @Disabled,ModifiedByUserID =@ModifiedByUserID,ModifiedDate =@ModifiedDate,AttributeDetailName =@AttributeDetailName WHERE AttributeDetailID = @AttributeDetailID";

                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return rowsAffected;

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
        public IEnumerable<AttributeGroupCheckBox> GetChild(long? id, List<AttributeGroupCheckBox> attributeGroupCheckBoxes)
        {
            var locations = attributeGroupCheckBoxes.Where(x => x.ParentId == id).ToList();

            var child = locations.AsEnumerable().Union(attributeGroupCheckBoxes.AsEnumerable().Where(x => x.ParentId == id).SelectMany(y => GetChild(y.AttributeGroupCheckBoxId, attributeGroupCheckBoxes))).ToList();
            return child;
        }
        public async Task<AttributeGroupCheckBox> DeleteAttributeGroupCheckBox(AttributeGroupCheckBox attributeGroupCheckBox)
        {

            try
            {
                var result = await GetAttributeGroupCheckBoxList(attributeGroupCheckBox.AttributeId);
                var childLists = GetChild(attributeGroupCheckBox.AttributeGroupCheckBoxId, result.ToList());
                using (var connection = CreateConnection())
                {

                    var parameters = new DynamicParameters();
                    parameters.Add("AttributeGroupCheckBoxId", attributeGroupCheckBox.AttributeGroupCheckBoxId);
                    var query = "Update AttributeGroupCheckBox SET IsDeleted=1 WHERE  AttributeGroupCheckBoxId = @AttributeGroupCheckBoxId;";
                    var ids = childLists.Select(x => x.AttributeGroupCheckBoxId).ToList();
                    if (ids.Count > 0)
                    {
                        query += "Update AttributeGroupCheckBox SET IsDeleted=1 WHERE  AttributeGroupCheckBoxId IN(" + string.Join(',', ids) + ");";
                    }
                    var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    return attributeGroupCheckBox;
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<AttributeGroupCheckBox>> GetAttributeGroupCheckBoxList(long? AttributeId)
        {
            try
            {
                List<AttributeGroupCheckBox> attributeGroupCheckBoxes = new List<AttributeGroupCheckBox>();
                var parameters = new DynamicParameters();
                parameters.Add("AttributeId", AttributeId);

                var query = "SELECT t1.*,t2.UserName as AddedByUser FROM AttributeGroupCheckBox t1 \r\nJOIN ApplicationUser t2 ON t1.AddedByUserID=t2.UserID\r\n" +
                    "Where (t1.IsDeleted=0 OR t1.IsDeleted IS NULL) AND t1.AttributeID = @AttributeId";
                using (var connection = CreateConnection())
                {
                    attributeGroupCheckBoxes = (await connection.QueryAsync<AttributeGroupCheckBox>(query, parameters)).ToList();
                }
                return attributeGroupCheckBoxes;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }

        public async Task<AttributeGroupCheckBox> InsertOrUpdateAttributeGroupCheckBox(AttributeGroupCheckBox attributeGroupCheckBox)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("AttributeGroupCheckBoxId", attributeGroupCheckBox.AttributeGroupCheckBoxId);
                        parameters.Add("AttributeId", attributeGroupCheckBox.AttributeId);
                        parameters.Add("Value", attributeGroupCheckBox.Value, DbType.String);
                        parameters.Add("Description", attributeGroupCheckBox.Description, DbType.String);
                        parameters.Add("IsDeleted", attributeGroupCheckBox.IsDeleted == true ? true : null);
                        parameters.Add("SessionId", attributeGroupCheckBox.SessionId);
                        parameters.Add("AddedByUserID", attributeGroupCheckBox.AddedByUserID);
                        parameters.Add("AddedDate", attributeGroupCheckBox.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", attributeGroupCheckBox.ModifiedByUserID);
                        parameters.Add("ModifiedDate", attributeGroupCheckBox.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", attributeGroupCheckBox.StatusCodeID);
                        parameters.Add("ParentId", attributeGroupCheckBox.ParentId);
                        parameters.Add("IsTextBox", attributeGroupCheckBox.IsTextBox == true ? true : false);
                        if (attributeGroupCheckBox.AttributeGroupCheckBoxId > 0)
                        {
                            var query = " UPDATE AttributeGroupCheckBox SET IsTextBox=@IsTextBox,ParentId=@ParentId,Value=@Value,Description=@Description,IsDeleted = @IsDeleted,ModifiedByUserID =@ModifiedByUserID,ModifiedDate =@ModifiedDate WHERE AttributeGroupCheckBoxId = @AttributeGroupCheckBoxId";

                            await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            var query = "INSERT INTO AttributeGroupCheckBox(IsTextBox,ParentId,AttributeId,Value,Description,IsDeleted,SessionId,AddedByUserID,AddedDate,StatusCodeID)  OUTPUT INSERTED.AttributeGroupCheckBoxId  VALUES " +
                                "(@IsTextBox,@ParentId,@AttributeId,@Value,@Description,@IsDeleted,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID)";

                            attributeGroupCheckBox.AttributeGroupCheckBoxId = await connection.ExecuteScalarAsync<long>(query, parameters);
                        }

                        return attributeGroupCheckBox;
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
    }
}
