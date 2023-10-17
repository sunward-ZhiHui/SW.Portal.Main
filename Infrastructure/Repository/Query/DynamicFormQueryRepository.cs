using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Xpo.DB.Helpers;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Values;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<DynamicForm> GetAllSelectedList(Guid? sessionId, long? DynamicFormDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("sessionId", sessionId);

                var query = "SELECT t1.*,t2.NAme as FileProfileTypeName FROM DynamicForm t1 LEFT JOIN FileProfileType t2 ON t2.FileProfileTypeID=t1.FileProfileTypeID  Where t1.SessionID = @sessionId";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryFirstOrDefaultAsync<DynamicForm>(query, parameters));
                    if (result != null)
                    {
                        result.DynamicFormApproval = (List<DynamicFormApproval>?)await GetDynamicFormApprovalByID(result.ID, DynamicFormDataId);
                    }
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormApproval>> GetDynamicFormApprovalByID(long? dynamicFormId, long? DynamicFormDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("DynamicFormDataId", DynamicFormDataId);
                var query = "select t1.*,\r\n" +
                    "(select t2.IsApproved from DynamicFormApproved t2 WHERE t2.DynamicFormApprovalID=t1.DynamicFormApprovalID AND t2.DynamicFormDataID=@DynamicFormDataId) as Approved\r\n" +
                    " FROM DynamicFormApproval t1 WHERE t1.DynamicFormId=@DynamicFormId order by t1.sortorderby asc;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproved> GetDynamicFormApprovedByID(long? dynamicFormDataId, long? approvalUserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                parameters.Add("ApprovalUserId", approvalUserId);
                var query = "select t1.*,t2.ApprovalUserId FROM DynamicFormApproved t1 JOIN DynamicFormApproval t2  ON t2.DynamicFormApprovalID=t1.DynamicFormApprovalID " +
                    "Where t1.DynamicFormDataId=@DynamicFormDataId AND t2.ApprovalUserId=@ApprovalUserId order by t2.sortorderby asc";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicFormApproved>(query, parameters);
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
                            parameters.Add("IsApproval", dynamicForm.IsApproval);
                            parameters.Add("FileProfileTypeId", dynamicForm.FileProfileTypeId);
                            parameters.Add("IsUpload", dynamicForm.IsUpload);
                            var query = "INSERT INTO DynamicForm(Name,ScreenID,SessionID,AttributeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsApproval,FileProfileTypeId,IsUpload) VALUES " +
                                "(@Name,@ScreenID,@SessionID,@AttributeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsApproval,@FileProfileTypeId,@IsUpload)";

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
                            parameters.Add("IsApproval", dynamicForm.IsApproval);
                            parameters.Add("FileProfileTypeId", dynamicForm.FileProfileTypeId);
                            parameters.Add("IsUpload", dynamicForm.IsUpload);
                            var query = " UPDATE DynamicForm SET AttributeID = @AttributeID,Name =@Name,ScreenID =@ScreenID,ModifiedByUserID=@ModifiedByUserID," +
                                "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsApproval=@IsApproval,IsUpload=@IsUpload,FileProfileTypeId=@FileProfileTypeId WHERE ID = @ID";

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
                            parameters.Add("IsVisible", dynamicFormSection.IsVisible);
                            parameters.Add("IsReadOnly", dynamicFormSection.IsReadOnly);
                            parameters.Add("IsReadWrite", dynamicFormSection.IsReadWrite);
                            if (dynamicFormSection.DynamicFormSectionId > 0)
                            {
                                var query = " UPDATE DynamicFormSection SET SectionName = @SectionName,DynamicFormId =@DynamicFormId,SortOrderBy=@SortOrderBys," +
                                    "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsVisible=@IsVisible," +
                                    "IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite " +
                                    "WHERE DynamicFormSectionId = @DynamicFormSectionId";
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                parameters.Add("SortOrderBy", GeDynamicFormSectionSort(dynamicFormSection.DynamicFormId));
                                var query = "INSERT INTO DynamicFormSection(SectionName,DynamicFormId,SessionId,SortOrderBy,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsVisible,IsReadOnly,IsReadWrite)  " +
                                    "OUTPUT INSERTED.DynamicFormSectionId VALUES " +
                                    "(@SectionName,@DynamicFormId,@SessionId,@SortOrderBy,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsVisible,@IsReadOnly,@IsReadWrite)";

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
                            parameters.Add("IsDisplayTableHeader", dynamicFormSection.IsDisplayTableHeader);
                            if (dynamicFormSection.DynamicFormSectionAttributeId > 0)
                            {

                                var query = "UPDATE DynamicFormSectionAttribute SET DisplayName = @DisplayName,AttributeId =@AttributeId,DynamicFormSectionId=@DynamicFormSectionId," +
                                    "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,IsSpinEditType=@IsSpinEditType," +
                                    "StatusCodeID=@StatusCodeID,ColSpan=@ColSpan,SortOrderBy=@SortOrderBys,IsRequired=@IsRequired,IsMultiple=@IsMultiple,RequiredMessage=@RequiredMessage,IsDisplayTableHeader=@IsDisplayTableHeader " +
                                    "WHERE DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId";
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                parameters.Add("SortOrderBy", GeDynamicFormSectionAttributeSort(dynamicFormSection.DynamicFormSectionId));
                                var query = "INSERT INTO DynamicFormSectionAttribute(DisplayName,AttributeId,SessionId,SortOrderBy,AddedByUserID," +
                                    "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,ColSpan,DynamicFormSectionId,IsRequired,IsMultiple,RequiredMessage,IsSpinEditType,IsDisplayTableHeader) VALUES " +
                                    "(@DisplayName,@AttributeId,@SessionId,@SortOrderBy," +
                                    "@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@ColSpan,@DynamicFormSectionId,@IsRequired,@IsMultiple,@RequiredMessage,@IsSpinEditType,@IsDisplayTableHeader)";

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
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,\r\n" +
                    "(select SUM(t5.FormUsedCount) from DynamicFormSectionAttribute t5 where t5.DynamicFormSectionId=t1.DynamicFormSectionId) as  FormUsedCount\r\n" +
                    "from DynamicFormSection t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
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
                var query = "select t1.*," +
                    "(case when t1.IsDisplayTableHeader is NULL then  0 ELSE t1.IsDisplayTableHeader END) as IsDisplayTableHeader," +
                    "t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.SectionName,t6.ControlTypeId,t6.AttributeName,t7.CodeValue as ControlType from DynamicFormSectionAttribute t1 \r\n" +
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
                            parameters.Add("IsSendApproval", dynamicFormData.IsSendApproval);
                            if (dynamicFormData.DynamicFormDataId > 0)
                            {
                                var query = "UPDATE DynamicFormData SET DynamicFormItem = @DynamicFormItem,DynamicFormId =@DynamicFormId," +
                                    "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsSendApproval=@IsSendApproval " +
                                    "WHERE DynamicFormDataId = @DynamicFormDataId;\n\r";
                                query += await UpdateDynamicFormSectionAttributeCount(dynamicFormData, "Update");
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                var query = "INSERT INTO DynamicFormData(DynamicFormItem,DynamicFormId,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsSendApproval)  OUTPUT INSERTED.DynamicFormDataId VALUES " +
                                    "(@DynamicFormItem,@DynamicFormId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsSendApproval);\n\r";
                                query += await UpdateDynamicFormSectionAttributeCount(dynamicFormData, "Add");
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
        public async Task<DynamicFormData> InsertOrUpdateDynamicFormApproved(DynamicFormData dynamicFormData)
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
                            var _dynamicFormApproval = (List<DynamicFormApproval>?)await GetDynamicFormApprovalByID(dynamicFormData.DynamicFormId, dynamicFormData.DynamicFormDataId);
                            if (_dynamicFormApproval != null)
                            {

                                _dynamicFormApproval.ForEach(s =>
                                {
                                    query += "INSERT INTO DynamicFormApproved(DynamicFormApprovalID,DynamicFormDataID,ApprovedDescription,UserID)VALUES " +
                                    "(" + s.DynamicFormApprovalId + "," + dynamicFormData.DynamicFormDataId + ",'" + s.Description + "'," + s.ApprovalUserId + ");\n\r";
                                });
                                if (!string.IsNullOrEmpty(query))
                                {
                                    await connection.ExecuteAsync(query, null, transaction);
                                }
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
        public async Task<DynamicFormData> GetDynamicFormDataBySessionOneAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.* from DynamicFormData t1 \r\n" +
                    "WHERE t1.SessionId=@SessionId";

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
        public async Task<DynamicFormData> GetDynamicFormDataBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.UserName as AddedBy,\r\nt3.UserName as ModifiedBy,t4.CodeValue as StatusCode,\r\nt5.IsApproval,t5.FileProfileTypeID,t6.Name as FileProfileTypeName,\r\n" +
                    "(SELECT COUNT(SessionId) from Documents t7 WHERE t7.SessionId=t1.SessionId) as isDocuments\r\n" +
                    "from DynamicFormData t1 \r\nJOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\nJOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicForm t5 ON t5.ID=t1.DynamicFormId\r\n" +
                    "LEFT JOIN FileProfileType t6 ON t6.FileProfileTypeID=t5.FileProfileTypeID\r\nWHERE t1.SessionId=@SessionId";
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
        public async Task<IReadOnlyList<DynamicFormApproved>> GetDynamicFormApprovedByAll()
        {
            try
            {
                var query = "select t1.*,t4.UserName as ApprovedByUser,\r\n" +
                   "CONCAT(case when t2.NickName is NULL then  t2.FirstName ELSE  t2.NickName END,' | ',t2.LastName) as ApprovalUser,\r\n" +
                   "CASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                   "FROM DynamicFormApproved t1 \r\n" +
                   "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                   "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ApprovedByUserId order by t1.DynamicFormApprovedId asc;\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproved>(query, null)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormData>> GetDynamicFormDataByIdAsync(long? id)
        {
            try
            {
                var resultData = await GetDynamicFormApprovedByAll();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", id);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.Name,\r\nt5.ScreenID from DynamicFormData t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN DynamicForm t5 ON t5.ID=t1.DynamicFormID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID WHERE t1.DynamicFormId=@DynamicFormId";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                    if (result != null && result.Count > 0)
                    {
                        result.ForEach(s =>
                        {
                            if (s.DynamicFormItem != null && IsValidJson(s.DynamicFormItem))
                            {
                                s.ObjectData = JsonConvert.DeserializeObject(s.DynamicFormItem);
                            }
                            if (s.IsSendApproval == true)
                            {
                                var approvedList = resultData.Where(w => w.DynamicFormDataId == s.DynamicFormDataId).ToList();
                                if (approvedList != null && approvedList.Count() > 0)
                                {
                                    s.DynamicFormApproved = approvedList;
                                    var approved = approvedList.Where(w => w.IsApproved == true).ToList();
                                    var approvedPending = approvedList.Where(w => w.IsApproved == null).ToList();
                                    if (approved != null && approved.Count() > 0 && approvedList.Count() == approved.Count())
                                    {
                                        s.ApprovalStatusId = 4;
                                        s.ApprovalStatus = "Approved Done";
                                        s.StatusName = "Completed";
                                    }
                                    else
                                    {
                                        var rejected = approvedList.Where(w => w.IsApproved == false).FirstOrDefault();
                                        if (rejected != null)
                                        {
                                            s.IsApproved = rejected.IsApproved;
                                            s.ApprovalStatusId = 3;
                                            s.ApprovalStatus = "Rejected";
                                            s.RejectedDate = rejected.ApprovedDate;
                                            s.RejectedUserId = rejected.UserId;
                                            s.RejectedUser = rejected.ApprovalUser;
                                            s.StatusName = "Rejected";
                                        }
                                        else
                                        {
                                            if (approvedPending != null && approvedPending.Count > 0)
                                            {
                                                if (approved != null && approved.Count() > 0)
                                                {
                                                    var isapproved = approved.OrderByDescending(o => o.DynamicFormApprovedId).FirstOrDefault(w => w.IsApproved == true);
                                                    if (isapproved != null)
                                                    {
                                                        s.IsApproved = isapproved.IsApproved;
                                                        s.ApprovalStatusId = 2;
                                                        s.ApprovalStatus = "Approved";
                                                        s.ApprovedDate = isapproved.ApprovedDate;
                                                        s.ApprovedUserId = isapproved.UserId;
                                                        s.ApprovedUser = isapproved.ApprovalUser;
                                                    }
                                                }
                                                var isapprovedPending = approvedPending.OrderBy(o => o.DynamicFormApprovedId).FirstOrDefault(w => w.IsApproved == null);
                                                if (isapprovedPending != null)
                                                {
                                                    s.IsApproved = isapprovedPending.IsApproved;
                                                    s.ApprovalStatusId = 1;
                                                    s.ApprovalStatus = "Pending";
                                                    s.PendingUserId = isapprovedPending.UserId;
                                                    s.PendingUser = isapprovedPending.ApprovalUser;
                                                    s.StatusName = "Pending";
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        });
                    }
                    return result;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<string> UpdateDynamicFormCurrentSectionAttributeCount(DynamicFormData dynamicFormData)
        {

            var query = string.Empty;
            var dynamicCurrentData = await GetDynamicFormDataBySessionOneAsync(dynamicFormData.SessionId);
            if (dynamicCurrentData != null && !string.IsNullOrEmpty(dynamicCurrentData.DynamicFormItem))
            {
                if (IsValidJson(dynamicCurrentData.DynamicFormItem))
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(dynamicCurrentData.DynamicFormItem);
                    dynamicFormData.AttributeHeader.DynamicFormSection.ForEach(a =>
                    {
                        var dynamicFormSectionAttributeData = dynamicFormData.AttributeHeader.DynamicFormSectionAttribute.Where(w => w.DynamicFormSectionId == a.DynamicFormSectionId).OrderBy(o => o.SortOrderBy).ToList();
                        if (dynamicFormSectionAttributeData != null && dynamicFormSectionAttributeData.Count > 0)
                        {
                            dynamicFormSectionAttributeData.ForEach(s =>
                            {
                                var Names = s.DynamicFormSectionAttributeId;
                                var itemValue = jsonObj[s.DynamicAttributeName];
                                if (itemValue is JArray)
                                {
                                    var values = (JArray)itemValue;
                                    if (values != null)
                                    {
                                        List<long?> listData = values.ToObject<List<long?>>();
                                        if (listData != null && listData.Count > 0)
                                        {
                                            listData.ForEach(l =>
                                            {
                                                query += "update AttributeDetails set FormUsedCount -= 1 where FormUsedCount>0 AND AttributeDetailID =" + l + ";\n\r";
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                    {
                                        long? Svalues = itemValue == null ? null : (long)itemValue;
                                        if (Svalues != null)
                                        {
                                            query += "update AttributeDetails set FormUsedCount -= 1 where FormUsedCount>0 AND AttributeDetailID =" + Svalues + ";\n\r";
                                        }
                                    }
                                    if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                    {
                                        long? Svalues = itemValue == null ? null : (long)itemValue;
                                        if (Svalues != null)
                                        {
                                            query += "update AttributeDetails set FormUsedCount -= 1 where FormUsedCount>0 AND AttributeDetailID =" + Svalues + ";\n\r";
                                        }
                                    }
                                }
                            });
                        }
                    });
                }
            }
            return query;
        }
        private async Task<string> UpdateDynamicFormSectionAttributeCount(DynamicFormData dynamicFormData, string? Type)
        {
            var query = await UpdateDynamicFormCurrentSectionAttributeCount(dynamicFormData);
            if (dynamicFormData.AttributeHeader != null && dynamicFormData.AttributeHeader.DynamicFormSection != null && dynamicFormData.AttributeHeader.DynamicFormSectionAttribute.Count > 0)
            {
                if (!string.IsNullOrEmpty(dynamicFormData.DynamicFormItem))
                {
                    if (IsValidJson(dynamicFormData.DynamicFormItem))
                    {
                        dynamic jsonObj = JsonConvert.DeserializeObject(dynamicFormData.DynamicFormItem);
                        dynamicFormData.AttributeHeader.DynamicFormSection.ForEach(a =>
                        {
                            var dynamicFormSectionAttributeData = dynamicFormData.AttributeHeader.DynamicFormSectionAttribute.Where(w => w.DynamicFormSectionId == a.DynamicFormSectionId).OrderBy(o => o.SortOrderBy).ToList();
                            if (dynamicFormSectionAttributeData != null && dynamicFormSectionAttributeData.Count > 0)
                            {
                                dynamicFormSectionAttributeData.ForEach(s =>
                                {
                                    var Names = jsonObj.ContainsKey(s.DynamicAttributeName);
                                    if (Names == true)
                                    {
                                        var itemValue = jsonObj[s.DynamicAttributeName];
                                        if (itemValue is JArray)
                                        {
                                            var values = (JArray)itemValue;
                                            if (values != null)
                                            {
                                                List<long?> listData = values.ToObject<List<long?>>();
                                                if (listData != null && listData.Count > 0)
                                                {
                                                    listData.ForEach(l =>
                                                    {
                                                        query += "update AttributeDetails set FormUsedCount += 1 where AttributeDetailID =" + l + ";\n\r";
                                                    });

                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                            {
                                                long? Svalues = itemValue == null ? null : (long)itemValue;
                                                if (Svalues != null)
                                                {
                                                    query += "update AttributeDetails set FormUsedCount += 1 where AttributeDetailID =" + Svalues + ";\n\r";
                                                }
                                            }
                                            if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                            {
                                                long? Svalues = itemValue == null ? null : (long)itemValue;
                                                if (Svalues != null)
                                                {
                                                    query += "update AttributeDetails set FormUsedCount += 1 where AttributeDetailID =" + Svalues + ";\n\r";
                                                }
                                            }
                                        }
                                        if (Type == "Add")
                                        {
                                            query += "update DynamicFormSectionAttribute set FormUsedCount += 1 where DynamicFormSectionAttributeID =" + s.DynamicFormSectionAttributeId + ";\n\r";
                                            query += "update AttributeHeader set FormUsedCount += 1 where AttributeID =" + s.AttributeId + ";\n\r";
                                        }
                                    }
                                });
                            }
                        });
                    }
                }
            }
            return query;
        }
        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private async Task<string> DeleteDynamicFormCurrentSectionAttribute(DynamicFormData dynamicFormData)
        {

            var query = string.Empty;
            var dynamicCurrentData = await GetAllAttributeNameByID(dynamicFormData.DynamicFormId);
            if (dynamicCurrentData != null && !string.IsNullOrEmpty(dynamicFormData.DynamicFormItem))
            {
                if (IsValidJson(dynamicFormData.DynamicFormItem))
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(dynamicFormData.DynamicFormItem);
                    dynamicCurrentData.DynamicFormSection.ForEach(a =>
                    {
                        var dynamicFormSectionAttributeData = dynamicCurrentData.DynamicFormSectionAttribute.Where(w => w.DynamicFormSectionId == a.DynamicFormSectionId).OrderBy(o => o.SortOrderBy).ToList();
                        if (dynamicFormSectionAttributeData != null && dynamicFormSectionAttributeData.Count > 0)
                        {
                            dynamicFormSectionAttributeData.ForEach(s =>
                            {
                                var Names = jsonObj.ContainsKey(s.DynamicAttributeName);
                                if (Names == true)
                                {
                                    var itemValue = jsonObj[s.DynamicAttributeName];
                                    if (itemValue is JArray)
                                    {
                                        var values = (JArray)itemValue;
                                        if (values != null)
                                        {
                                            List<long?> listData = values.ToObject<List<long?>>();
                                            if (listData != null && listData.Count > 0)
                                            {
                                                listData.ForEach(l =>
                                                {
                                                    query += "update AttributeDetails set FormUsedCount -= 1 where FormUsedCount>0 AND AttributeDetailID =" + l + ";\n\r";
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (s.ControlType == "ComboBox" || s.ControlType == "Radio" || s.ControlType == "RadioGroup")
                                        {
                                            long? Svalues = itemValue == null ? null : (long)itemValue;
                                            if (Svalues != null)
                                            {
                                                query += "update AttributeDetails set FormUsedCount -= 1 where FormUsedCount>0 AND AttributeDetailID =" + Svalues + ";\n\r";
                                            }
                                        }
                                        else if (s.ControlType == "ListBox" && s.IsMultiple == false)
                                        {
                                            long? Svalues = itemValue == null ? null : (long)itemValue;
                                            if (Svalues != null)
                                            {
                                                query += "update AttributeDetails set FormUsedCount -= 1 where FormUsedCount>0 AND AttributeDetailID =" + Svalues + ";\n\r";
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    query += "update DynamicFormSectionAttribute set FormUsedCount -= 1 where DynamicFormSectionAttributeID =" + s.DynamicFormSectionAttributeId + ";\n\r";
                                    query += "update AttributeHeader set FormUsedCount -= 1 where AttributeID =" + s.AttributeId + ";\n\r";
                                }
                            });
                        }
                    });
                }
            }
            return query;
        }
        private async Task<AttributeHeaderListModel> GetAllAttributeNameByID(long? Id)
        {
            try
            {
                AttributeHeaderListModel attributeHeaderListModel = new AttributeHeaderListModel();
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(@"select * from DynamicFormSection where DynamicFormID=" + Id + " order by  SortOrderBy asc;" +
                        "select t1.*,t5.SectionName,t6.AttributeName,t7.CodeValue as ControlType,t5.DynamicFormID from DynamicFormSectionAttribute t1\r\n" +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\nWhere t5.DynamicFormID=" + Id + " order by t1.SortOrderBy asc;");
                    attributeHeaderListModel.DynamicFormSection = results.Read<DynamicFormSection>().ToList();
                    attributeHeaderListModel.DynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    if (attributeHeaderListModel.DynamicFormSectionAttribute != null)
                    {
                        List<long?> attributeIds = attributeHeaderListModel.DynamicFormSectionAttribute.Where(a => a.AttributeId > 0).Select(a => a.AttributeId).ToList();
                        attributeHeaderListModel.DynamicFormSectionAttribute.ForEach(s =>
                        {
                            s.AttributeName = string.IsNullOrEmpty(s.AttributeName) ? string.Empty : char.ToUpper(s.AttributeName[0]) + s.AttributeName.Substring(1);
                            s.DynamicAttributeName = s.DynamicFormSectionAttributeId + "_" + s.AttributeName;
                        });

                    }
                    return attributeHeaderListModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<string> DeleteDynamicFormApproved(DynamicFormData dynamicFormData)
        {
            var stringData = string.Empty;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId);
                var query = "select  * from DynamicFormApproved where  DynamicFormDataId=@DynamicFormDataId";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DynamicFormApproved>(query, parameters)).ToList();
                    if (result != null && result.Count > 0)
                    {
                        result.ForEach(s =>
                        {
                            stringData += "update DynamicFormApproval set ApprovedCountUsed -= 1 where ApprovedCountUsed>0 AND DynamicFormApprovalId =" + s.DynamicFormApprovalId + ";\n\r";
                        });
                    }
                }
                return stringData;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormData> DeleteDynamicFormData(DynamicFormData dynamicFormData)
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

                            var query = await DeleteDynamicFormCurrentSectionAttribute(dynamicFormData);
                            query += await DeleteDynamicFormApproved(dynamicFormData);
                            query += "DELETE  FROM DynamicFormApproved WHERE DynamicFormDataId = @DynamicFormDataId;\r\n";
                            query += "DELETE  FROM DynamicFormData WHERE DynamicFormDataId = @DynamicFormDataId;\r\n";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);
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
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DynamicFormApproval>> GetDynamicFormApprovalAsync(long? dynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode," +
                    "CONCAT(case when t5.NickName is NULL\r\n then  t5.FirstName\r\n ELSE\r\n  t5.NickName END,' | ',t5.LastName) as ApprovalUser\r\n" +
                    "from DynamicFormApproval t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN Employee t5 ON t5.UserID=t1.ApprovalUserID\r\n" +
                    "WHERE t1.DynamicFormId=@DynamicFormId order by t1.sortorderby asc;";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproval> InsertOrUpdateDynamicFormApproval(DynamicFormApproval dynamicFormApproval)
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
                            parameters.Add("DynamicFormApprovalId", dynamicFormApproval.DynamicFormApprovalId);
                            parameters.Add("ApprovalUserId", dynamicFormApproval.ApprovalUserId);
                            parameters.Add("DynamicFormId", dynamicFormApproval.DynamicFormId);
                            parameters.Add("AddedByUserID", dynamicFormApproval.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicFormApproval.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicFormApproval.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicFormApproval.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicFormApproval.StatusCodeID);
                            parameters.Add("IsApproved", dynamicFormApproval.IsApproved);
                            parameters.Add("SortOrderBys", dynamicFormApproval.SortOrderBy);
                            parameters.Add("Description", dynamicFormApproval.Description, DbType.String);
                            if (dynamicFormApproval.DynamicFormApprovalId > 0)
                            {
                                var query = "UPDATE DynamicFormApproval SET ApprovalUserId = @ApprovalUserId,DynamicFormId =@DynamicFormId,SortOrderBy=@SortOrderBys,\n\r" +
                                    "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsApproved=@IsApproved,Description=@Description\n\r" +
                                    "WHERE DynamicFormApprovalId = @DynamicFormApprovalId;\n\r";
                                await connection.ExecuteAsync(query, parameters, transaction);

                            }
                            else
                            {
                                parameters.Add("SortOrderBy", GetDynamicFormApprovalSort(dynamicFormApproval.DynamicFormId));
                                var query = "INSERT INTO DynamicFormApproval(ApprovalUserId,DynamicFormId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SortOrderBy,IsApproved,Description)  OUTPUT INSERTED.DynamicFormApprovalId VALUES " +
                                    "(@ApprovalUserId,@DynamicFormId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SortOrderBy,@IsApproved,@Description);\n\r";
                                dynamicFormApproval.DynamicFormApprovalId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);


                            }
                            transaction.Commit();

                            return dynamicFormApproval;
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
        public int? GetDynamicFormApprovalSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);

                query = "SELECT * FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId order by  SortOrderBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormApproval>(query, parameters);
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
        public async Task<List<DynamicFormApproval>> UpdateDynamicFormApprovalSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("SortOrderBy", SortOrderBy);
                query = "SELECT * FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId AND SortOrderBy>@SortOrderBy";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproval> DeleteDynamicFormApproval(DynamicFormApproval dynamicFormApproval)
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
                            var result = await UpdateDynamicFormApprovalSort(dynamicFormApproval.DynamicFormId, dynamicFormApproval.SortOrderBy);
                            var parameters = new DynamicParameters();
                            parameters.Add("id", dynamicFormApproval.DynamicFormApprovalId);
                            var sortby = dynamicFormApproval.SortOrderBy;
                            var query = "DELETE  FROM DynamicFormApproval WHERE DynamicFormApprovalId = @id;";
                            if (result != null)
                            {
                                result.ForEach(s =>
                                {
                                    query += "Update  DynamicFormApproval SET SortOrderBy=" + sortby + "  WHERE DynamicFormApprovalId =" + s.DynamicFormApprovalId + ";";
                                    sortby++;
                                });
                            }
                            var rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);
                            transaction.Commit();
                            return dynamicFormApproval;
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
        public DynamicFormApproval GetDynamicFormApprovalUserCheckValidation(long? dynamicFormId, long? dynamicFormApprovalId, long? approvalUserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("ApprovalUserId", approvalUserId);
                if (dynamicFormApprovalId > 0)
                {
                    parameters.Add("DynamicFormApprovalId", dynamicFormApprovalId);

                    query = "SELECT * FROM DynamicFormApproval Where ApprovalUserId=@ApprovalUserId AND DynamicFormId=@DynamicFormId AND DynamicFormApprovalId != @DynamicFormApprovalId";
                }
                else
                {
                    query = "SELECT * FROM DynamicFormApproval Where ApprovalUserId=@ApprovalUserId AND DynamicFormId = @DynamicFormId";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DynamicFormApproval>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproval> UpdateDynamicFormApprovalSortOrder(DynamicFormApproval dynamicFormApproval)
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
                            int? SortOrder = dynamicFormApproval.SortOrderAnotherBy > dynamicFormApproval.SortOrderBy ? (dynamicFormApproval.SortOrderBy + 1) : dynamicFormApproval.SortOrderAnotherBy;
                            query += "Update  DynamicFormApproval SET SortOrderBy=" + dynamicFormApproval.SortOrderBy + "  WHERE DynamicFormApprovalId =" + dynamicFormApproval.DynamicFormApprovalId + ";";
                            if (SortOrder > 0)
                            {
                                var result = await GetUpdateDynamicFormApprovalSortOrder(dynamicFormApproval);
                                if (result != null && result.Count > 0)
                                {

                                    result.ForEach(s =>
                                    {

                                        query += "Update  DynamicFormApproval SET SortOrderBy=" + SortOrder + "  WHERE DynamicFormApprovalId =" + s.DynamicFormApprovalId + ";";
                                        SortOrder++;
                                    });

                                }

                                var rowsAffected = await connection.ExecuteAsync(query, null, transaction);
                            }
                            transaction.Commit();

                            return dynamicFormApproval;
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
        public async Task<List<DynamicFormApproval>> GetUpdateDynamicFormApprovalSortOrder(DynamicFormApproval dynamicFormApproval)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormApproval.DynamicFormId);
                var from = dynamicFormApproval.SortOrderAnotherBy > dynamicFormApproval.SortOrderBy ? dynamicFormApproval.SortOrderBy : dynamicFormApproval.SortOrderAnotherBy;
                var to = dynamicFormApproval.SortOrderAnotherBy > dynamicFormApproval.SortOrderBy ? dynamicFormApproval.SortOrderAnotherBy : dynamicFormApproval.SortOrderBy;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT DynamicFormApprovalId,DynamicFormId,SortOrderBy FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId  AND SortOrderBy>@SortOrderByFrom and SortOrderBy<=@SortOrderByTo order by SortOrderBy asc";

                if (dynamicFormApproval.SortOrderAnotherBy > dynamicFormApproval.SortOrderBy)
                {
                    query = "SELECT DynamicFormApprovalId,DynamicFormId,SortOrderBy FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId  AND SortOrderBy>=@SortOrderByFrom and SortOrderBy<@SortOrderByTo order by SortOrderBy asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproval> UpdateDescriptionDynamicFormApprovalField(DynamicFormApproval value)
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
                            parameters.Add("DynamicFormApprovalId", value.DynamicFormApprovalId);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                            parameters.Add("Description", value.Description, DbType.String);
                            var query = "Update DynamicFormApproval SET Description=@Description,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                "DynamicFormApprovalId=@DynamicFormApprovalId";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
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
        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUserList()
        {
            try
            {
                var query = "select  * from UserGroupUser";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserGroupUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<LeveMasterUsersModel>> GetLeveMasterUsersList(IEnumerable<long>? SelectLevelMasterIDs)
        {
            try
            {
                var LevelIds = SelectLevelMasterIDs != null && SelectLevelMasterIDs.Count() > 0 ? SelectLevelMasterIDs : new List<long>() { -1 };
                var query = "select  t1.LevelID,t1.DesignationID,t3.UserID from Designation t1 \r\n" +
                    "JOIN LevelMaster t2 ON t1.LevelID=t2.LevelID\r\n" +
                    "JOIN Employee t3 ON t3.DesignationID=t1.DesignationID " +
                    "where t1.LevelID in(" + string.Join(',', LevelIds) + ")"; ;

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<LeveMasterUsersModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionSecurity>> GetDynamicFormSectionSecurityEmptyAsync(long? dynamicFormSectionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", dynamicFormSectionId);
                var query = "select  * from DynamicFormSectionSecurity where  DynamicFormSectionId=@DynamicFormSectionId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionSecurity>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionSecurity> InsertDynamicFormSectionSecurity(DynamicFormSectionSecurity value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var userExitsRoles = await GetDynamicFormSectionSecurityEmptyAsync(value.DynamicFormSectionId);
                    var userGroupUsers = await GetUserGroupUserList();
                    var LevelUsers = await GetLeveMasterUsersList(value.SelectLevelMasterIDs);
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = string.Empty;
                            var parameters = new DynamicParameters();
                            parameters.Add("IsReadWrite", value.IsReadWrite);
                            parameters.Add("IsReadOnly", value.IsReadOnly);
                            parameters.Add("IsVisible", value.IsVisible);
                            parameters.Add("DynamicFormSectionId", value.DynamicFormSectionId);
                            query += " UPDATE DynamicFormSection SET IsVisible=@IsVisible,IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite WHERE DynamicFormSectionId = @DynamicFormSectionId;\r\n";
                            if (value.IsVisible == true)
                            {
                                if (value.IsReadOnly == true || value.IsReadWrite == true)
                                {
                                    if (value.Type == "User")
                                    {
                                        if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                                        {
                                            foreach (var item in value.SelectUserIDs)
                                            {
                                                var counts = userExitsRoles.Where(w => w.UserId == item).Count();
                                                if (counts == 0)
                                                {
                                                    query += "INSERT INTO [DynamicFormSectionSecurity](DynamicFormSectionId,UserId) OUTPUT INSERTED.DynamicFormSectionSecurityId " +
                                                        "VALUES (@DynamicFormSectionId," + item + ");\r\n";

                                                }
                                            }
                                        }
                                    }
                                    if (value.Type == "UserGroup")
                                    {
                                        if (value.SelectUserGroupIDs != null && value.SelectUserGroupIDs.Count() > 0)
                                        {
                                            var userGropuIds = userGroupUsers.Where(w => value.SelectUserGroupIDs.ToList().Contains(w.UserGroupId.Value)).ToList();
                                            if (userGropuIds != null && userGropuIds.Count > 0)
                                            {
                                                userGropuIds.ForEach(s =>
                                                {
                                                    var counts = userExitsRoles.Where(w => w.UserId == s.UserId).Count();
                                                    if (counts == 0)
                                                    {
                                                        query += "INSERT INTO [DynamicFormSectionSecurity](DynamicFormSectionId,UserId,UserGroupId) OUTPUT INSERTED.DynamicFormSectionSecurityId " +
                                                            "VALUES (@DynamicFormSectionId," + s.UserId + "," + s.UserGroupId + ");\r\n";
                                                    }
                                                });
                                            }
                                        }
                                    }
                                    if (value.Type == "Level")
                                    {
                                        if (LevelUsers != null && LevelUsers.Count > 0)
                                        {
                                            LevelUsers.ToList().ForEach(s =>
                                            {
                                                var counts = userExitsRoles.Where(w => w.UserId == s.UserId).Count();
                                                if (counts == 0)
                                                {
                                                    query += "INSERT INTO [DynamicFormSectionSecurity](DynamicFormSectionId,UserId,LevelId) OUTPUT INSERTED.DynamicFormSectionSecurityId " +
                                                       "VALUES (@DynamicFormSectionId," + s.UserId + "," + s.LevelId + ");\r\n";
                                                }
                                            });
                                        }
                                    }
                                }

                            }
                            else
                            {

                            }
                            if (!string.IsNullOrEmpty(query))
                            {
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            }
                            transaction.Commit();
                            return value;
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
        public async Task<IReadOnlyList<DynamicFormSectionSecurity>> GetDynamicFormSectionSecurityList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", Id);
                var query = "select t1.*,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.SectionName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\n" +
                    "from DynamicFormSectionSecurity t1\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN DynamicFormSection t4 ON t4.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE t1.DynamicFormSectionId=@DynamicFormSectionId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionSecurity>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteDynamicFormSectionSecurity(long? Id, List<long?> Ids)
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
                            var parameters = new DynamicParameters();
                            parameters.Add("IsReadWrite", false);
                            parameters.Add("IsReadOnly", false);
                            parameters.Add("IsVisible", false);
                            parameters.Add("DynamicFormSectionId", Id);
                            //query += " UPDATE DynamicFormSection SET IsVisible=@IsVisible,IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite WHERE DynamicFormSectionId = @DynamicFormSectionId;\r\n";

                            if (Ids != null && Ids.Count > 0)
                            {
                                string IdList = string.Join(",", Ids);
                                query += "Delete From DynamicFormSectionSecurity WHERE DynamicFormSectionSecurityId in (" + IdList + ");\r\n";
                            }
                            if (!string.IsNullOrEmpty(query))
                            {

                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            }
                            transaction.Commit();
                            await DeleteCheckDynamicFormSectionSecurity(Id);
                            return Id.GetValueOrDefault(0);
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
        public async Task<long> DeleteCheckDynamicFormSectionSecurity(long? Id)
        {
            try
            {
                using (var connections = CreateConnection())
                {

                    connections.Open();
                    using (var transactions = connections.BeginTransaction())
                    {
                        try
                        {
                            var userExitsRoles = await GetDynamicFormSectionSecurityEmptyAsync(Id);
                            if (userExitsRoles == null || userExitsRoles.Count == 0)
                            {
                                var query = string.Empty;
                                var parameters = new DynamicParameters();
                                parameters.Add("IsReadWrite", false);
                                parameters.Add("IsReadOnly", false);
                                parameters.Add("IsVisible", false);
                                parameters.Add("DynamicFormSectionId", Id);
                                query += " UPDATE DynamicFormSection SET IsVisible=@IsVisible,IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite WHERE DynamicFormSectionId = @DynamicFormSectionId;\r\n";
                                if (!string.IsNullOrEmpty(query))
                                {
                                    await connections.QuerySingleOrDefaultAsync<long>(query, parameters, transactions);
                                }
                            }
                            transactions.Commit();
                            return Id.GetValueOrDefault(0);
                        }
                        catch (Exception exp)
                        {
                            transactions.Rollback();
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
        public async Task<DynamicFormApproved> InsertDynamicFormApproved(DynamicFormApproved dynamicForm)
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
                            parameters.Add("DynamicFormApprovedId", dynamicForm.DynamicFormApprovedId);
                            parameters.Add("DynamicFormApprovalId", dynamicForm.DynamicFormApprovalId);
                            parameters.Add("DynamicFormDataId", dynamicForm.DynamicFormDataId);
                            parameters.Add("IsApproved", dynamicForm.IsApproved);
                            parameters.Add("ApprovedDescription", dynamicForm.ApprovedDescription);
                            var query = "INSERT INTO DynamicFormApproved(DynamicFormApprovalId,IsApproved,ApprovedDescription,DynamicFormDataId)  " +
                                  "OUTPUT INSERTED.DynamicFormApprovedId VALUES " +
                                 "(@DynamicFormApprovalId,@IsApproved,@ApprovedDescription,@DynamicFormDataId);\n\r";
                            query += "update DynamicFormApproval set ApprovedCountUsed += 1 where DynamicFormApprovalId =" + dynamicForm.DynamicFormApprovalId + ";\n\r";
                            dynamicForm.DynamicFormApprovedId = await connection.ExecuteAsync(query, parameters, transaction);

                            transaction.Commit();

                            return dynamicForm;
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
        public async Task<IReadOnlyList<DynamicFormApproved>> GetDynamicFormApprovedList(long? DynamicFormDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataId", DynamicFormDataId);
                var query = "select t1.*,t3.Value as EmployeeStatus,t4.UserName as ApprovedByUser,\r\n" +
                    "CONCAT(case when t2.NickName is NULL then  t2.FirstName ELSE  t2.NickName END,' | ',t2.LastName) as ApprovalUser,\r\nCASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                    "FROM DynamicFormApproved t1 \r\n" +
                    "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                    "LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t2.AcceptanceStatus\r\n" +
                     "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ApprovedByUserId\r\n" +
                    "Where t1.DynamicFormDataID=@DynamicFormDataId \r\norder by t1.DynamicFormApprovedID asc";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApproved>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproved> UpdateDynamicFormApprovedByStaus(DynamicFormApproved dynamicFormApproved)
        {
            try
            {
                using (var connections = CreateConnection())
                {

                    connections.Open();
                    using (var transactions = connections.BeginTransaction())
                    {
                        try
                        {
                            var query = string.Empty;
                            var parameters = new DynamicParameters();
                            parameters.Add("DynamicFormDataId", dynamicFormApproved.DynamicFormDataId);
                            parameters.Add("DynamicFormApprovedId", dynamicFormApproved.DynamicFormApprovedId);
                            parameters.Add("ApprovedDescription", dynamicFormApproved.ApprovedDescription);
                            parameters.Add("IsApproved", dynamicFormApproved.IsApproved);
                            parameters.Add("ApprovedByUserId", dynamicFormApproved.ApprovedByUserId);
                            parameters.Add("ApprovedDate", dynamicFormApproved.ApprovedDate);
                            query += " UPDATE DynamicFormApproved SET DynamicFormDataId=@DynamicFormDataId,IsApproved=@IsApproved,ApprovedDescription=@ApprovedDescription,ApprovedByUserId=@ApprovedByUserId,ApprovedDate=@ApprovedDate WHERE DynamicFormApprovedId = @DynamicFormApprovedId;\r\n";
                            if (!string.IsNullOrEmpty(query))
                            {
                                await connections.QuerySingleOrDefaultAsync<long>(query, parameters, transactions);
                            }
                            transactions.Commit();
                            return dynamicFormApproved;
                        }
                        catch (Exception exp)
                        {
                            transactions.Rollback();
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
