using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Data.Edm.Library;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
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
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from DynamicForm t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID";

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
        public async Task<DynamicForm> GetDynamicFormBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from DynamicForm t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID WHERE t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicForm>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicForm> GetAllSelectedList(Guid? sessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("sessionId", sessionId);

                var query = "SELECT * FROM DynamicForm Where SessionID = @sessionId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<DynamicForm>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public DynamicForm GetDynamicFormScreenNameCheckValidation(string? value, long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("ScreenID", value);
                if (id > 0)
                {
                    parameters.Add("ID", id);
                    parameters.Add("ScreenID", value);

                    query = "SELECT * FROM DynamicForm Where ID!=@id AND ScreenID = @ScreenID";
                }
                else
                {
                    query = "SELECT * FROM DynamicForm Where ScreenID = @ScreenID";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DynamicForm>(query, parameters);
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
                            parameters.Add("AddedByUserID", dynamicForm.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicForm.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicForm.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicForm.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicForm.StatusCodeID);

                            var query = "INSERT INTO DynamicForm(Name,ScreenID,SessionID,AttributeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID) VALUES " +
                                "(@Name,@ScreenID,@SessionID,@AttributeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";

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

        public async Task<long> Update(DynamicForm dynamicForm)
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
                            parameters.Add("AddedByUserID", dynamicForm.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicForm.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicForm.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicForm.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicForm.StatusCodeID);
                            var query = " UPDATE DynamicForm SET AttributeID = @AttributeID,Name =@Name,ScreenID =@ScreenID,ModifiedByUserID=@ModifiedByUserID," +
                                "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID WHERE ID = @ID";

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
        public int? GeDynamicFormSectionSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);

                query = "SELECT * FROM DynamicFormSection Where DynamicFormId = @DynamicFormId order by  SortOrderBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormSection>(query, parameters);
                    if (result != null)
                    {
                        SortOrderBy = result.SortOrderBy + 1;
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSection> InsertOrUpdateDynamicFormSection(DynamicFormSection dynamicFormSection)
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
                            parameters.Add("DynamicFormSectionId", dynamicFormSection.DynamicFormSectionId);
                            parameters.Add("SectionName", dynamicFormSection.SectionName, DbType.String);
                            parameters.Add("DynamicFormId", dynamicFormSection.DynamicFormId);
                            parameters.Add("SortOrderBys", dynamicFormSection.SortOrderBy);
                            parameters.Add("SessionId", dynamicFormSection.SessionId, DbType.Guid);
                            parameters.Add("AddedByUserID", dynamicFormSection.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicFormSection.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicFormSection.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicFormSection.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicFormSection.StatusCodeID);
                            if (dynamicFormSection.DynamicFormSectionId > 0)
                            {
                                var query = " UPDATE DynamicFormSection SET SectionName = @SectionName,DynamicFormId =@DynamicFormId,SortOrderBy=@SortOrderBys," +
                                    "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID " +
                                    "WHERE DynamicFormSectionId = @DynamicFormSectionId";
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                parameters.Add("SortOrderBy", GeDynamicFormSectionSort(dynamicFormSection.DynamicFormId));
                                var query = "INSERT INTO DynamicFormSection(SectionName,DynamicFormId,SessionId,SortOrderBy,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID)  OUTPUT INSERTED.DynamicFormSectionId VALUES " +
                                    "(@SectionName,@DynamicFormId,@SessionId,@SortOrderBy,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";

                                dynamicFormSection.DynamicFormSectionId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            }
                            transaction.Commit();

                            return dynamicFormSection;
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


        public int? GeDynamicFormSectionAttributeSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormSectionId", id);

                query = "SELECT * FROM DynamicFormSectionAttribute Where DynamicFormSectionId = @DynamicFormSectionId order by  SortOrderBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormSectionAttribute>(query, parameters);
                    if (result != null)
                    {
                        SortOrderBy = result.SortOrderBy + 1;
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionAttribute> InsertOrUpdateDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSection)
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
                            parameters.Add("DynamicFormSectionAttributeId", dynamicFormSection.DynamicFormSectionAttributeId);
                            parameters.Add("DynamicFormSectionId", dynamicFormSection.DynamicFormSectionId);
                            parameters.Add("DisplayName", dynamicFormSection.DisplayName, DbType.String);
                            parameters.Add("AttributeId", dynamicFormSection.AttributeId);
                            parameters.Add("SortOrderBys", dynamicFormSection.SortOrderBy);
                            parameters.Add("SessionId", dynamicFormSection.SessionId, DbType.Guid);
                            parameters.Add("AddedByUserID", dynamicFormSection.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicFormSection.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicFormSection.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicFormSection.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicFormSection.StatusCodeID);
                            parameters.Add("ColSpan", dynamicFormSection.ColSpan);
                            parameters.Add("IsRequired", dynamicFormSection.IsRequired);
                            parameters.Add("IsMultiple", dynamicFormSection.IsMultiple);
                            parameters.Add("RequiredMessage", dynamicFormSection.RequiredMessage, DbType.String);
                            parameters.Add("IsSpinEditType", dynamicFormSection.IsSpinEditType, DbType.String);
                            if (dynamicFormSection.DynamicFormSectionAttributeId > 0)
                            {

                                var query = "UPDATE DynamicFormSectionAttribute SET DisplayName = @DisplayName,AttributeId =@AttributeId,DynamicFormSectionId=@DynamicFormSectionId," +
                                    "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,IsSpinEditType=@IsSpinEditType," +
                                    "StatusCodeID=@StatusCodeID,ColSpan=@ColSpan,SortOrderBy=@SortOrderBys,IsRequired=@IsRequired,IsMultiple=@IsMultiple,RequiredMessage=@RequiredMessage " +
                                    "WHERE DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId";
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                parameters.Add("SortOrderBy", GeDynamicFormSectionAttributeSort(dynamicFormSection.DynamicFormSectionId));
                                var query = "INSERT INTO DynamicFormSectionAttribute(DisplayName,AttributeId,SessionId,SortOrderBy,AddedByUserID," +
                                    "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,ColSpan,DynamicFormSectionId,IsRequired,IsMultiple,RequiredMessage,IsSpinEditType) VALUES " +
                                    "(@DisplayName,@AttributeId,@SessionId,@SortOrderBy," +
                                    "@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@ColSpan,@DynamicFormSectionId,@IsRequired,@IsMultiple,@RequiredMessage,@IsSpinEditType)";

                                dynamicFormSection.DynamicFormSectionAttributeId = await connection.ExecuteAsync(query, parameters, transaction);
                            }
                            transaction.Commit();

                            return dynamicFormSection;
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

        public async Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionAsync(long? dynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from DynamicFormSection t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID " +
                    "WHERE t1.DynamicFormId=@DynamicFormId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSection>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAsync(long? dynamicFormSectionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", dynamicFormSectionId);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.SectionName,t6.ControlTypeId,t6.AttributeName,t7.CodeValue as ControlType from DynamicFormSectionAttribute t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "Where t1.DynamicFormSectionId=@DynamicFormSectionId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttribute>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormSection>> UpdateDynamicFormSectionSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("SortOrderBy", SortOrderBy);
                query = "SELECT * FROM DynamicFormSection Where DynamicFormId = @DynamicFormId AND SortOrderBy>@SortOrderBy";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSection>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteDynamicFormSection(DynamicFormSection dynamicFormSection)
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
                            var result = await UpdateDynamicFormSectionSort(dynamicFormSection.DynamicFormId, dynamicFormSection.SortOrderBy);
                            var parameters = new DynamicParameters();
                            parameters.Add("id", dynamicFormSection.DynamicFormSectionId);
                            var sortby = dynamicFormSection.SortOrderBy;
                            var query = "DELETE  FROM DynamicFormSection WHERE DynamicFormSectionID = @id;";
                            if (result != null)
                            {
                                result.ForEach(s =>
                                {
                                    query += "Update  DynamicFormSection SET SortOrderBy=" + sortby + "  WHERE DynamicFormSectionID =" + s.DynamicFormSectionId + ";";
                                    sortby++;
                                });
                            }

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
        public async Task<List<DynamicFormSection>> GetUpdateDynamicFormSectionSortOrder(DynamicFormSection dynamicFormSection)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormSection.DynamicFormId);
                var from = dynamicFormSection.SortOrderAnotherBy > dynamicFormSection.SortOrderBy ? dynamicFormSection.SortOrderBy : dynamicFormSection.SortOrderAnotherBy;
                var to = dynamicFormSection.SortOrderAnotherBy > dynamicFormSection.SortOrderBy ? dynamicFormSection.SortOrderAnotherBy : dynamicFormSection.SortOrderBy;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT DynamicFormSectionId,DynamicFormId,SortOrderBy FROM DynamicFormSection Where DynamicFormId = @DynamicFormId  AND SortOrderBy>@SortOrderByFrom and SortOrderBy<=@SortOrderByTo order by SortOrderBy asc";

                if (dynamicFormSection.SortOrderAnotherBy > dynamicFormSection.SortOrderBy)
                {
                    query = "SELECT DynamicFormSectionId,DynamicFormId,SortOrderBy FROM DynamicFormSection Where DynamicFormId = @DynamicFormId  AND SortOrderBy>=@SortOrderByFrom and SortOrderBy<@SortOrderByTo order by SortOrderBy asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSection>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSection> UpdateDynamicFormSectionSortOrder(DynamicFormSection dynamicFormSection)
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
                            var query = string.Empty;
                            int? SortOrder = dynamicFormSection.SortOrderAnotherBy > dynamicFormSection.SortOrderBy ? (dynamicFormSection.SortOrderBy + 1) : dynamicFormSection.SortOrderAnotherBy;
                            query += "Update  DynamicFormSection SET SortOrderBy=" + dynamicFormSection.SortOrderBy + "  WHERE DynamicFormSectionID =" + dynamicFormSection.DynamicFormSectionId + ";";
                            if (SortOrder > 0)
                            {
                                var result = await GetUpdateDynamicFormSectionSortOrder(dynamicFormSection);
                                if (result != null && result.Count > 0)
                                {

                                    result.ForEach(s =>
                                    {

                                        query += "Update  DynamicFormSection SET SortOrderBy=" + SortOrder + "  WHERE DynamicFormSectionID =" + s.DynamicFormSectionId + ";";
                                        SortOrder++;
                                    });

                                }

                                var rowsAffected = await connection.ExecuteAsync(query, null, transaction);
                            }
                            transaction.Commit();

                            return dynamicFormSection;
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



        public async Task<List<DynamicFormSectionAttribute>> UpdateDynamicFormSectionAttributeSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormSectionId", id);
                parameters.Add("SortOrderBy", SortOrderBy);
                query = "SELECT * FROM DynamicFormSectionAttribute Where DynamicFormSectionId = @DynamicFormSectionId AND SortOrderBy>@SortOrderBy";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttribute>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteDynamicFormSectionAttribute(DynamicFormSectionAttribute dynamicFormSectionAttribute)
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
                            var result = await UpdateDynamicFormSectionAttributeSort(dynamicFormSectionAttribute.DynamicFormSectionId, dynamicFormSectionAttribute.SortOrderBy);
                            var parameters = new DynamicParameters();
                            parameters.Add("id", dynamicFormSectionAttribute.DynamicFormSectionAttributeId);
                            var sortby = dynamicFormSectionAttribute.SortOrderBy;
                            var query = "DELETE  FROM DynamicFormSectionAttribute WHERE DynamicFormSectionAttributeId = @id;";
                            if (result != null)
                            {
                                result.ForEach(s =>
                                {
                                    query += "Update  DynamicFormSectionAttribute SET SortOrderBy=" + sortby + "  WHERE DynamicFormSectionAttributeId =" + s.DynamicFormSectionAttributeId + ";";
                                    sortby++;
                                });
                            }

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



        public async Task<List<DynamicFormSectionAttribute>> GetUpdateDynamicFormSectionAttributeSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormSectionId", dynamicFormSectionAttribute.DynamicFormSectionId);
                var from = dynamicFormSectionAttribute.SortOrderAnotherBy > dynamicFormSectionAttribute.SortOrderBy ? dynamicFormSectionAttribute.SortOrderBy : dynamicFormSectionAttribute.SortOrderAnotherBy;
                var to = dynamicFormSectionAttribute.SortOrderAnotherBy > dynamicFormSectionAttribute.SortOrderBy ? dynamicFormSectionAttribute.SortOrderAnotherBy : dynamicFormSectionAttribute.SortOrderBy;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT DynamicFormSectionId,DynamicFormSectionAttributeId,SortOrderBy FROM DynamicFormSectionAttribute Where DynamicFormSectionId = @DynamicFormSectionId  AND SortOrderBy>@SortOrderByFrom and SortOrderBy<=@SortOrderByTo order by SortOrderBy asc";

                if (dynamicFormSectionAttribute.SortOrderAnotherBy > dynamicFormSectionAttribute.SortOrderBy)
                {
                    query = "SELECT DynamicFormSectionId,DynamicFormSectionAttributeId,SortOrderBy FROM DynamicFormSectionAttribute Where DynamicFormSectionId = @DynamicFormSectionId  AND SortOrderBy>=@SortOrderByFrom and SortOrderBy<@SortOrderByTo order by SortOrderBy asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttribute>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute)
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
                            var query = string.Empty;
                            int? SortOrder = dynamicFormSectionAttribute.SortOrderAnotherBy > dynamicFormSectionAttribute.SortOrderBy ? (dynamicFormSectionAttribute.SortOrderBy + 1) : dynamicFormSectionAttribute.SortOrderAnotherBy;
                            query += "Update  DynamicFormSectionAttribute SET SortOrderBy=" + dynamicFormSectionAttribute.SortOrderBy + "  WHERE DynamicFormSectionAttributeId =" + dynamicFormSectionAttribute.DynamicFormSectionAttributeId + ";";
                            if (SortOrder > 0)
                            {
                                var result = await GetUpdateDynamicFormSectionAttributeSortOrder(dynamicFormSectionAttribute);
                                if (result != null && result.Count > 0)
                                {

                                    result.ForEach(s =>
                                    {

                                        query += "Update  DynamicFormSectionAttribute SET SortOrderBy=" + SortOrder + "  WHERE DynamicFormSectionAttributeId =" + s.DynamicFormSectionAttributeId + ";";
                                        SortOrder++;
                                    });

                                }
                            }
                            var rowsAffected = await connection.ExecuteAsync(query, null, transaction);
                            transaction.Commit();
                            return dynamicFormSectionAttribute;
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
        public async Task<long> InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds, long? UserId)
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

                            var query = string.Empty;
                            int? SortOrderBy = GeDynamicFormSectionAttributeSort(dynamicFormSectionId);
                            var parameters = new DynamicParameters();
                            parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                            if (dynamicFormSectionId > 0 && attributeIds != null && attributeIds.Count() > 0)
                            {
                                attributeIds.ToList().ForEach(s =>
                                {

                                    query += "INSERT INTO DynamicFormSectionAttribute(DisplayName,AttributeId,SessionId,SortOrderBy,AddedByUserID," +
                                    "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,ColSpan,DynamicFormSectionId,IsRequired,IsMultiple) VALUES " +
                                    "('" + s.Description + "'," + s.AttributeID + ",'" + Guid.NewGuid() + "'," + SortOrderBy + "," +
                                    "" + UserId + "," + UserId + ",@AddedDate,@AddedDate," +
                                    "1,6," + dynamicFormSectionId + ",0,0);";
                                    SortOrderBy++;
                                });
                                await connection.ExecuteAsync(query, parameters, transaction);
                            }
                            transaction.Commit();

                            return dynamicFormSectionId;
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

        public async Task<DynamicFormData> InsertOrUpdateDynamicFormData(DynamicFormData dynamicFormData)
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
                            parameters.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId);
                            parameters.Add("DynamicFormItem", dynamicFormData.DynamicFormItem, DbType.String);
                            parameters.Add("DynamicFormId", dynamicFormData.DynamicFormId);
                            parameters.Add("SessionId", dynamicFormData.SessionId, DbType.Guid);
                            parameters.Add("AddedByUserID", dynamicFormData.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicFormData.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicFormData.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicFormData.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicFormData.StatusCodeID);
                            if (dynamicFormData.DynamicFormDataId > 0)
                            {
                                var query = "UPDATE DynamicFormData SET DynamicFormItem = @DynamicFormItem,DynamicFormId =@DynamicFormId," +
                                    "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID " +
                                    "WHERE DynamicFormDataId = @DynamicFormDataId";
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                var query = "INSERT INTO DynamicFormData(DynamicFormItem,DynamicFormId,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID)  OUTPUT INSERTED.DynamicFormDataId VALUES " +
                                    "(@DynamicFormItem,@DynamicFormId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";

                                dynamicFormData.DynamicFormDataId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            }
                            transaction.Commit();

                            return dynamicFormData;
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
        public async Task<DynamicFormData> GetDynamicFormDataBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode from DynamicFormData t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID WHERE t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicFormData>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
