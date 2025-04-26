using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Helpers;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Xpo.DB.Helpers;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Protobuf.WellKnownTypes;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Values;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using NAV;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using static Duende.IdentityServer.Models.IdentityResources;
using static iText.IO.Image.Jpeg2000ImageData;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static iTextSharp.text.pdf.AcroFields;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class DynamicFormQueryRepository : QueryRepository<DynamicForm>, IDynamicFormQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public DynamicFormQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }

        public async Task<DynamicForm> Delete(DynamicForm dynamicForm)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("id", dynamicForm.ID);
                        var query = "Update  DynamicForm SET IsDeleted=1 WHERE ID = @id";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        await DeleteDynamicFormMenu(dynamicForm);
                        return dynamicForm;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<ApplicationPermission>> GetDynamicFormMenuList()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var query = "select * from ApplicationPermission where ParentID=60248";
                    try
                    {
                        var result = (await connection.QueryAsync<ApplicationPermission>(query)).ToList();
                        return result;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<DynamicForm> DeleteDynamicFormMenu(DynamicForm dynamicForm)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("SessionID", dynamicForm.SessionID, DbType.Guid);
                    var query = "select * from ApplicationPermission where ParentID=60248 AND PermissionURL=@SessionID;";
                    try
                    {
                        var result = await connection.QueryFirstOrDefaultAsync<ApplicationPermission>(query, parameters);
                        if (result != null && result.PermissionID > 0)
                        {
                            var query1 = "Delete from ApplicationRolePermission  where PermissionID=" + result.PermissionID + ";";
                            query1 += "Delete from ApplicationPermission where PermissionID=" + result.PermissionID + ";";
                            var rowsAffected = await connection.ExecuteAsync(query1);
                            await UpdateMenuOrderDynamicFormMenu();
                        }
                        return dynamicForm;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<ApplicationPermission> UpdateMenuOrderDynamicFormMenu()
        {
            try
            {
                ApplicationPermission applicationPermission = new ApplicationPermission();
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    var query = "select * from ApplicationPermission where ParentID=60248 order by PermissionOrder asc;";
                    try
                    {
                        var result = (await connection.QueryAsync<ApplicationPermission>(query)).ToList();
                        if (result != null && result.Count() > 0)
                        {
                            int i = 1;
                            var query1 = string.Empty;
                            result.ForEach(s =>
                            {
                                query1 += "Update ApplicationPermission SET PermissionOrder=" + i + "  WHERE  PermissionID =" + s.PermissionID + ";";
                                i++;
                            });
                            if (!string.IsNullOrEmpty(query1))
                            {
                                await connection.ExecuteAsync(query1);
                            }
                        }
                        return applicationPermission;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicForm>> GetAllByGridFormAsync(long? userId)
        {
            List<DynamicForm> DynamicForm = new List<DynamicForm>();
            try
            {

                var query = "select t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted,\n\rt6.Name as ProfileName,t5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
                     "LEFT JOIN Plant t5 ON t5.plantId=t1.companyId\r\n" +
                     "LEFT JOIN DocumentProfileNoSeries t6 ON t6.profileId=t1.profileId\r\n";
                if (userId > 0)
                {
                    var dynamicIds = await GetDynamicFormDataByIdone(userId);
                    query += "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.IsGridForm=1 t1.ID in(" + string.Join(',', dynamicIds) + ")";
                }
                else
                {
                    query += "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.IsGridForm=1;";
                }
                var appUsers = new List<ApplicationUser>(); var codeUsers = new List<CodeMaster>();
                using (var connection = CreateConnection())
                {
                    DynamicForm = (await connection.QueryAsync<DynamicForm>(query)).ToList();
                    if (DynamicForm != null && DynamicForm.Count > 0)
                    {
                        List<int?> codeIds = new List<int?>();
                        codeIds.AddRange(DynamicForm.Where(w => w.StatusCodeID > 0).Select(a => a.StatusCodeID).ToList());
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(DynamicForm.Where(w => w.AddedByUserID > 0).Select(a => a.AddedByUserID).ToList());
                        userIds.AddRange(DynamicForm.Where(a => a.ModifiedByUserID > 0).Select(a => a.ModifiedByUserID).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        codeIds = codeIds != null && codeIds.Count > 0 ? codeIds : new List<int?>() { -1 };
                        var query1 = "select CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,(case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as UserName,t3.UserId from Employee t3 where t3.userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query1 += "select CodeID,\r\nCodeType,\r\nCodeValue,\r\nCodeDescription,\r\nCodeMasterID from CodeMaster where codeId in(" + string.Join(',', codeIds.Distinct()) + ");";
                        var QuerResult = await connection.QueryMultipleAsync(query1);
                        appUsers = QuerResult.Read<ApplicationUser>().ToList(); codeUsers = QuerResult.Read<CodeMaster>().ToList();
                    }
                }
                if (DynamicForm != null && DynamicForm.Count > 0)
                {
                    DynamicForm.ForEach(s =>
                    {
                        s.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.StatusCode = codeUsers.FirstOrDefault(f => f.CodeId == s.StatusCodeID)?.CodeValue;
                    });
                }
                DynamicForm = DynamicForm != null ? DynamicForm : new List<DynamicForm>();
                return DynamicForm;

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicForm>> GetAllByNoGridFormAsync(long? userId)
        {
            List<DynamicForm> DynamicForm = new List<DynamicForm>();
            try
            {

                var query = "select t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted,\n\rt6.Name as ProfileName,t5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
                     "LEFT JOIN Plant t5 ON t5.plantId=t1.companyId\r\n" +
                     "LEFT JOIN DocumentProfileNoSeries t6 ON t6.profileId=t1.profileId\r\n";
                if (userId > 0)
                {
                    var dynamicIds = await GetDynamicFormDataByIdone(userId);
                    query += "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t1.IsGridForm is null or t1.IsGridForm=0) AND t1.ID in(" + string.Join(',', dynamicIds) + ")";
                }
                else
                {
                    query += "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.IsGridForm is null or t1.IsGridForm=0;";
                }
                var appUsers = new List<ApplicationUser>(); var codeUsers = new List<CodeMaster>();
                using (var connection = CreateConnection())
                {
                    DynamicForm = (await connection.QueryAsync<DynamicForm>(query)).ToList();
                    if (DynamicForm != null && DynamicForm.Count > 0)
                    {
                        List<int?> codeIds = new List<int?>();
                        codeIds.AddRange(DynamicForm.Where(a => a.StatusCodeID > 0).Select(a => a.StatusCodeID).ToList());
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(DynamicForm.Where(a => a.AddedByUserID > 0).Select(a => a.AddedByUserID).ToList());
                        userIds.AddRange(DynamicForm.Where(a => a.ModifiedByUserID > 0).Select(a => a.ModifiedByUserID).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        codeIds = codeIds != null && codeIds.Count > 0 ? codeIds : new List<int?>() { -1 };
                        var query1 = "select CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,(case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as UserName,t3.UserId from Employee t3 where t3.userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query1 += "select CodeID,\r\nCodeType,\r\nCodeValue,\r\nCodeDescription,\r\nCodeMasterID from CodeMaster where codeId in(" + string.Join(',', codeIds.Distinct()) + ");";
                        var QuerResult = await connection.QueryMultipleAsync(query1);
                        appUsers = QuerResult.Read<ApplicationUser>().ToList(); codeUsers = QuerResult.Read<CodeMaster>().ToList();
                    }
                }
                if (DynamicForm != null && DynamicForm.Count > 0)
                {
                    DynamicForm.ForEach(s =>
                    {
                        s.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.StatusCode = codeUsers.FirstOrDefault(f => f.CodeId == s.StatusCodeID)?.CodeValue;
                    });
                }
                DynamicForm = DynamicForm != null ? DynamicForm : new List<DynamicForm>();
                return DynamicForm;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicForm>> GetAllAsync(long? userId)
        {
            List<DynamicForm> DynamicForm = new List<DynamicForm>();
            try
            {

                var query = "select t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted,\n\r" +
                    "t6.Name as ProfileName,t5.PlantCode as CompanyName,\r\n" +
                    " (select COUNT(tt1.DynamicFormDataID) from DynamicFormData tt1 where t1.ID=tt1.DynamicFormID AND (tt1.IsDeleted is NULL or tt1.IsDeleted=0)) as FormDataCount\r\n from DynamicForm t1 \r\n" +
                     "LEFT JOIN Plant t5 ON t5.plantId=t1.companyId\r\n" +
                     "LEFT JOIN DocumentProfileNoSeries t6 ON t6.profileId=t1.profileId\r\n";
                if (userId > 0)
                {
                    var dynamicIds = await GetDynamicFormDataByIdone(userId);
                    query += "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.ID in(" + string.Join(',', dynamicIds) + ")";
                }
                else
                {
                    query += "where (t1.IsDeleted=0 or t1.IsDeleted is null);";
                }
                var appUsers = new List<ApplicationUser>(); var codeUsers = new List<CodeMaster>();
                using (var connection = CreateConnection())
                {
                    DynamicForm = (await connection.QueryAsync<DynamicForm>(query)).ToList();
                    if (DynamicForm != null && DynamicForm.Count > 0)
                    {
                        List<int?> codeIds = new List<int?>();
                        codeIds.AddRange(DynamicForm.Where(a => a.StatusCodeID > 0).Select(a => a.StatusCodeID).ToList());
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(DynamicForm.Where(a => a.AddedByUserID > 0).Select(a => a.AddedByUserID).ToList());
                        userIds.AddRange(DynamicForm.Where(a => a.ModifiedByUserID > 0).Select(a => a.ModifiedByUserID).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        codeIds = codeIds != null && codeIds.Count > 0 ? codeIds : new List<int?>() { -1 };
                        var query1 = "select CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,(case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as UserName,t3.UserId from Employee t3 where t3.userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query1 += "select CodeID,\r\nCodeType,\r\nCodeValue,\r\nCodeDescription,\r\nCodeMasterID from CodeMaster where codeId in(" + string.Join(',', codeIds.Distinct()) + ");";
                        var QuerResult = await connection.QueryMultipleAsync(query1);
                        appUsers = QuerResult.Read<ApplicationUser>().ToList(); codeUsers = QuerResult.Read<CodeMaster>().ToList();
                    }
                }
                if (DynamicForm != null && DynamicForm.Count > 0)
                {
                    DynamicForm.ForEach(s =>
                    {
                        s.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.StatusCode = codeUsers.FirstOrDefault(f => f.CodeId == s.StatusCodeID)?.CodeValue;
                    });
                }
                return DynamicForm;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataUpload> GetDynamicFormDataUploadOneData(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", Id);
                var query = "select t1.DynamicFormDataUploadID,\r\nt1.DynamicFormDataID,\r\nt1.DynamicFormSectionID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsDmsLink,\r\nt1.LinkFileProfileTypeDocumentID,\r\n(Select tt2.FileName from Documents tt2 where tt2.SessionID=t1.SessionID ANd tt2.IsLatest=1) as FileName,\r\n" +
                    "(Select t22.ProfileNo from Documents t22 where t22.SessionID=t1.SessionID ANd t22.IsLatest=1) as ProfileNo,\r\n" +
                    "(Select t32.UniqueSessionId from Documents t32 where t32.SessionID=t1.SessionID ANd t32.IsLatest=1) as UniqueSessionId,\r\n" +
                    "(Select t2.DocumentID from Documents t2 where t2.SessionID=t1.SessionID ANd t2.IsLatest=1) as DocumentID,\r\n" +
                    "(case when t1.LinkFileProfileTypeDocumentID>0 then  (SELECT f2.SessionID from LinkFileProfileTypeDocument f1 JOIN FileProfileType f2 ON  f1.FileProfileTypeID=f2.FileProfileTypeID where f1.LinkFileProfileTypeDocumentID=t1.LinkFileProfileTypeDocumentID) ELSE (Select t4.SessionID from Documents t3 JOIN FileProfileType t4 ON t4.FileProfileTypeID=t3.FilterProfileTypeID where t3.SessionID=t1.SessionID ANd t3.IsLatest=1) END) as FileProfileSessionID,\r\n" +
                    "(case when t1.LinkFileProfileTypeDocumentID>0 then  (SELECT f2.Name from LinkFileProfileTypeDocument f1 JOIN FileProfileType f2 ON  f1.FileProfileTypeID=f2.FileProfileTypeID where f1.LinkFileProfileTypeDocumentID=t1.LinkFileProfileTypeDocumentID) ELSE (Select t4.Name from Documents t3 JOIN FileProfileType t4 ON t4.FileProfileTypeID=t3.FilterProfileTypeID where t3.SessionID=t1.SessionID ANd t3.IsLatest=1) END) as FileProfileName\r\n" +
                    "from DynamicFormDataUpload t1 WHERE t1.DynamicFormSectionID IS NULL AND t1.DynamicFormDataID=@ID";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicFormDataUpload>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionByIdAsync(long? id, long? UserId, long? dynamicFormDataId)
        {
            List<DynamicFormSection> DynamicFormSections = new List<DynamicFormSection>();
            DynamicFormSection dynamicFormSection = new DynamicFormSection();
            var resultData = await GetDynamicFormDataUploadOneData(dynamicFormDataId);
            dynamicFormSection.DynamicFormSectionId = -1;
            dynamicFormSection.SectionName = "No Section. Default Upload";
            dynamicFormSection.IsFileExits = "No";
            if (resultData != null)
            {
                dynamicFormSection.IsFileExits = "Yes";
                dynamicFormSection.DynamicFormDataUploadId = resultData.DynamicFormDataUploadId;
                dynamicFormSection.DynamicFormDataId = resultData.DynamicFormDataId;
                dynamicFormSection.DynamicFormDataUploadAddedUserId = resultData.AddedByUserId;
                dynamicFormSection.UploadSessionID = resultData.SessionId;
                dynamicFormSection.ProfileNo = resultData.ProfileNo;
                dynamicFormSection.DocumentId = resultData.DocumentId;
                dynamicFormSection.FileName = resultData.FileName;
                dynamicFormSection.UniqueSessionId = resultData.UniqueSessionId;
                dynamicFormSection.FileProfileName = resultData.FileProfileName;
                dynamicFormSection.FileProfileSessionID = resultData.FileProfileSessionID;
                dynamicFormSection.UserCount = 1; dynamicFormSection.UserIsVisible = true; dynamicFormSection.UserIsReadWrite = true; dynamicFormSection.UserIsReadOnly = true;
            }
            DynamicFormSections.Add(dynamicFormSection);
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("UserId", UserId);
                parameters.Add("DynamicFormDataID", dynamicFormDataId);
                query = "SELECT tt1.*,(case when tt1.DynamicFormDataUploadID is NULL then  'No' ELSE 'Yes' END) as IsFileExits,\r\n" +
                    "(Select t22.ProfileNo from Documents t22 where t22.SessionID=tt1.UploadSessionID ANd t22.IsLatest=1) as ProfileNo,\r\n " +
                    "(Select ttt2.UniqueSessionId from Documents ttt2 where ttt2.SessionID=tt1.UploadSessionID ANd ttt2.IsLatest=1) as UniqueSessionId,\r\n" +
                    "(Select tt2.FileName from Documents tt2 where tt2.SessionID=tt1.UploadSessionID ANd tt2.IsLatest=1) as FileName,\r\n" +
                    "(Select ttt2.DocumentID from Documents ttt2 where ttt2.SessionID=tt1.UploadSessionID ANd ttt2.IsLatest=1) as DocumentID,\r\n" +
                    "(Select ttt4.SessionID from Documents ttt3 JOIN FileProfileType ttt4 ON ttt4.FileProfileTypeID=ttt3.FilterProfileTypeID where ttt3.SessionID=tt1.UploadSessionID ANd ttt3.IsLatest=1) as FileProfileSessionID,\r\n (Select tt4.Name from Documents tt3 JOIN FileProfileType tt4 ON tt4.FileProfileTypeID=tt3.FilterProfileTypeID where tt3.SessionID=tt1.UploadSessionID ANd tt3.IsLatest=1) as FileProfileName from " +
                    "(select t1.DynamicFormSectionID,\r\nt1.SectionName,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.DynamicFormID,\r\nt1.SortOrderBy,\r\nt1.IsReadWrite,\r\nt1.IsReadOnly,\r\nt1.IsVisible,\r\nt1.Instruction,\r\nt1.IsDeleted,\r\nt1.SectionFileProfileTypeID,\r\n (select t4.LinkFileProfileTypeDocumentID from DynamicFormDataUpload t4 WHERE t4.DynamicFormSectionID=t1.DynamicFormSectionID AND t4.DynamicFormDataID=@dynamicFormDataId) as LinkFileProfileTypeDocumentID,\r\n (select tt4.TransactionSessionID from DynamicFormDataUpload t4 JOIN LinkFileProfileTypeDocument tt4 ON tt4.LinkFileProfileTypeDocumentID=t4.LinkFileProfileTypeDocumentID WHERE t4.DynamicFormSectionID=t1.DynamicFormSectionID AND t4.DynamicFormDataID=@dynamicFormDataId) as TransactionSessionID,\r\n  (select tt4.FileProfileTypeId from DynamicFormDataUpload t4 JOIN LinkFileProfileTypeDocument tt4 ON tt4.LinkFileProfileTypeDocumentID=t4.LinkFileProfileTypeDocumentID WHERE t4.DynamicFormSectionID=t1.DynamicFormSectionID AND t4.DynamicFormDataID=@dynamicFormDataId) as LinkFileProfileTypeId,\r\n    " +
                    "(select tt4.DocumentID from DynamicFormDataUpload t4 JOIN LinkFileProfileTypeDocument tt4 ON tt4.LinkFileProfileTypeDocumentID=t4.LinkFileProfileTypeDocumentID WHERE t4.DynamicFormSectionID=t1.DynamicFormSectionID AND t4.DynamicFormDataID=@dynamicFormDataId) as LinkDocumentID,\r\n(select t4.DynamicFormDataUploadID from DynamicFormDataUpload t4 WHERE t4.DynamicFormSectionID=t1.DynamicFormSectionID AND t4.DynamicFormDataID=@dynamicFormDataId) as DynamicFormDataUploadID,\r\n(select t2.SessionID from DynamicFormDataUpload t2 WHERE t2.DynamicFormSectionID=t1.DynamicFormSectionID AND t2.DynamicFormDataID=@dynamicFormDataId) as UploadSessionID,\r\n(select t3.AddedByUserID from DynamicFormDataUpload t3 WHERE t3.DynamicFormSectionID=t1.DynamicFormSectionID AND t3.DynamicFormDataID=@dynamicFormDataId) as DynamicFormDataUploadAddedUserID,\r\n" +
                    "(select t5.IsReadOnly from DynamicFormSectionSecurity t5 WHERE t5.UserID=" + UserId + " AND t1.DynamicFormSectionID=t5.DynamicFormSectionID) as UserIsReadOnly,\r\n" +
                    "(select t6.IsReadWrite from DynamicFormSectionSecurity t6 WHERE t6.UserID=" + UserId + " AND t1.DynamicFormSectionID=t6.DynamicFormSectionID) as UserIsReadWrite,\r\n" +
                    "(select t7.IsVisible from DynamicFormSectionSecurity t7 WHERE t7.UserID=" + UserId + " AND t1.DynamicFormSectionID=t7.DynamicFormSectionID) as UserIsVisible,\r\n(select Count(DynamicFormSectionSecurityID)  as userCounts from DynamicFormSectionSecurity t8 WHERE   t1.DynamicFormSectionID=t8.DynamicFormSectionID) as UserCount,f1.Name as SectionFileProfileTypeName\r\nfrom DynamicFormSection t1 " +
                    "LEFT JOIN FileProfileType f1 ON t1.SectionFileProfileTypeID=f1.FileProfileTypeID WHERE t1.DynamicFormID = @DynamicFormId AND  (t1.IsDeleted=0 or t1.IsDeleted is null)) tt1 where tt1.UserCount=0 OR tt1.UserIsVisible=1\r\n";
                var result = new List<DynamicFormSection>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormSection>(query, parameters)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    DynamicFormSections.AddRange(result);
                }
                return DynamicFormSections;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormData?>> GetDynamicFormDataApprovalList(long? userId)
        {
            List<long?> DynamicFormIds = new List<long?>() { -1 };
            try
            {

                var parameters = new DynamicParameters();
                var query = "select t1.DynamicFormDataID,\r\nt1.DynamicFormID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.DynamicFormItem,\r\nt1.IsSendApproval,\r\nt1.FileProfileSessionID,\r\nt1.ProfileID,\r\nt1.ProfileNo,\r\nt1.DynamicFormDataGridID,\r\nt1.IsDeleted,\r\nt1.SortOrderByNo,\r\nt1.GridSortOrderByNo,\r\nt1.DynamicFormSectionGridAttributeID,\r\nt1.IsLocked,\r\nt1.LockedUserID,t2.Name as DynamicFormName,t2.SessionID as DynamicFormSessionID from DynamicFormData t1 JOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID " +
                    "WHERE t1.DynamicFormDataID IN (select tt1.DynamicFormDataID from DynamicFormApproved tt1 Where tt1.DynamicFormDataID>0 group by tt1.DynamicFormDataID\r\n) AND t1.IsSendApproval=1 AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t2.IsDeleted=0 or t2.IsDeleted is null)";
                var result = new List<DynamicFormData>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormData>(query)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    var dynamicFormDataIDs = result.Where(w => w.IsSendApproval == true).Select(a => a.DynamicFormDataId).ToList();
                    var resultData = await GetDynamicFormApprovedByAll(dynamicFormDataIDs);
                    result.ForEach(s =>
                    {
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
                                        s.DynamicFormApprovedId = rejected.DynamicFormApprovedId;
                                        s.IsApproved = rejected.IsApproved;
                                        s.ApprovalStatusId = 3;
                                        s.ApprovalStatus = "Rejected";
                                        s.RejectedDate = rejected.ApprovedDate;
                                        s.RejectedUserId = rejected.UserId;
                                        s.RejectedUser = rejected.ApprovalUser;
                                        s.StatusName = "Rejected";
                                        s.CurrentUserId = rejected.UserId;
                                        s.CurrentUserName = rejected.ApprovalUser;
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
                                                    s.DynamicFormApprovedId = isapproved.DynamicFormApprovedId;
                                                    s.IsApproved = isapproved.IsApproved;
                                                    s.ApprovalStatusId = 2;
                                                    s.ApprovalStatus = "Approved";
                                                    s.ApprovedDate = isapproved.ApprovedDate;
                                                    s.ApprovedUserId = isapproved.UserId;
                                                    s.ApprovedUser = isapproved.ApprovalUser;
                                                    s.CurrentUserId = isapproved.UserId;
                                                    s.CurrentUserName = isapproved.ApprovalUser;
                                                }
                                            }
                                            var isapprovedPending = approvedPending.OrderBy(o => o.DynamicFormApprovedId).FirstOrDefault(w => w.IsApproved == null);
                                            if (isapprovedPending != null)
                                            {
                                                s.DynamicFormApprovedId = isapprovedPending.DynamicFormApprovedId;
                                                s.IsApproved = isapprovedPending.IsApproved;
                                                s.ApprovalStatusId = 1;
                                                s.ApprovalStatus = "Pending";
                                                s.PendingUserId = isapprovedPending.UserId;
                                                s.PendingUser = isapprovedPending.ApprovalUser;
                                                s.StatusName = "Pending";
                                                s.CurrentUserId = isapprovedPending.UserId;
                                                s.CurrentUserName = isapprovedPending.ApprovalUser;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    });
                }
                if (userId > 0 && result != null && result.Count() > 0)
                {
                    result = result.Where(w => w.CurrentUserId == userId).ToList();
                }
                return result;

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<List<long?>> GetDynamicFormDataByIdone(long? userId)
        {
            List<long?> DynamicFormIds = new List<long?>() { -1 };
            try
            {

                var parameters = new DynamicParameters();
                var query = "select t1.DynamicFormDataId,t1.IsSendApproval,t1.DynamicFormId from DynamicFormData t1 WHERE t1.IsSendApproval=1 \r\n";
                var result = new List<DynamicFormData>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormData>(query)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    var dynamicFormDataIDs = result.Where(w => w.IsSendApproval == true).Select(a => a.DynamicFormDataId).ToList();
                    var resultData = await GetDynamicFormApprovedByAll(dynamicFormDataIDs);
                    result.ForEach(s =>
                    {
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
                                        s.CurrentUserId = rejected.UserId;
                                        s.CurrentUserName = rejected.ApprovalUser;
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
                                                    s.CurrentUserId = isapproved.UserId;
                                                    s.CurrentUserName = isapproved.ApprovalUser;
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
                                                s.CurrentUserId = isapprovedPending.UserId;
                                                s.CurrentUserName = isapprovedPending.ApprovalUser;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    });
                }
                if (userId > 0 && result != null && result.Count() > 0)
                {
                    DynamicFormIds.AddRange(result.Where(w => w.CurrentUserId == userId).Select(s => s.DynamicFormId).ToList());
                }
                return DynamicFormIds;

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicForm> GetDynamicFormByIdAsync(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", Id);
                var query = "select t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted\n\r from DynamicForm t1 WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.ID=@ID";

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
        public async Task<DynamicForm> GetDynamicFormBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t6.UserName as AddedBy,t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted,\n\rt5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
                    "LEFT JOIN Plant t5 ON t5.plantId=t1.companyId\r\n" +
                    "LEFT JOIN ApplicationUser t6 ON t6.UserID=t1.AddedByUserID\r\n" +
                    "WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.SessionId=@SessionId";

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
        public async Task<ViewEmployee> GetEmployeeByUserIdIdAsync(long? userId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("userId", userId);
                var query = "select t1.EmployeeID,\r\nt1.UserID,\r\nt1.SageID,\r\nt1.PlantID,\r\nt1.LevelID,\r\nt1.DepartmentID,\r\nt1.DesignationID,\r\nt1.FirstName,\r\nt1.NickName,\r\nt1.LastName,\r\nt1.Gender,\r\nt1.JobTitle,\r\nt1.Email,\r\nt1.TypeOfEmployeement,\r\nt1.LanguageID,\r\nt1.SectionID,\r\nt1.SubSectionID,\r\nt1.CityID,\r\nt1.RegionID,\r\nt1.Signature,\r\nt1.ImageUrl,\r\nt1.DateOfEmployeement,\r\nt1.LastWorkingDate,\r\nt1.Extension,\r\nt1.SpeedDial,\r\nt1.Mobile,\r\nt1.SkypeAddress,\r\nt1.ReportID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsActive,\r\nt1.SubSectionTID,\r\nt1.HeadCount,\r\nt1.DivisionID,\r\nt1.AcceptanceLetter,\r\nt1.ExpectedJoiningDate,\r\nt1.AcceptanceStatus,\r\nt1.AcceptanceStatusDate,\r\nt1.NRICNo,t2.PlantCode as CompanyName from Employee t1 JOIN Plant t2 ON t2.PlantID=t1.PlantID where t1.UserID=@userId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ViewEmployee>(query, parameters);
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

                var query = "SELECT t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted,\n\rt2.NAme as FileProfileTypeName,t4.PlantCode as CompanyName,t5.Name as ProfileName,t2.SessionID as FileProfileSessionId FROM DynamicForm t1\n\r" +
                    " LEFT JOIN FileProfileType t2 ON t2.FileProfileTypeID=t1.FileProfileTypeID\r\n" +
                    "LEFT JOIN Plant t4 ON t4.plantId=t1.companyId\r\n" +
                     "LEFT JOIN DocumentProfileNoSeries t5 ON t5.profileId=t1.profileId\r\n" +
                    "Where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.SessionID = @sessionId";

                var result = new DynamicForm();
                using (var connection = CreateConnection())
                {
                    result = await connection.QueryFirstOrDefaultAsync<DynamicForm>(query, parameters);
                }
                if (result != null && DynamicFormDataId > 0)
                {
                    result.DynamicFormApproval = (List<DynamicFormApproval>?)await GetDynamicFormApprovalByID(result.ID, DynamicFormDataId);
                }
                return result;
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
                var query = "select t1.DynamicFormApprovalID,\r\nt1.DynamicFormID,\r\nt1.ApprovalUserID,\r\nt1.SortOrderBy,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproved,\r\nt1.Description,\r\nt1.ApprovedCountUsed,\r\n" +
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

        public async Task<IReadOnlyList<DynamicFormSectionWorkFlow>> GetDynamicFormSectionWorkFlowByID(long? DynamicFormSectionId, long? UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", DynamicFormSectionId);
                parameters.Add("UserId", UserId);
                var query = "select t1.DynamicFormSectionWorkFlowID,\r\nt1.DynamicFormSectionID,\r\nt1.DynamicFormSectionSecurityID,\r\nt1.UserID,t3.SortOrderBy,t3.SectionName," +
                    "(case when t1.UserID>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName,(case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END) as DynamicFormSectionWorkFlowUserName from DynamicFormSectionWorkFlow t1 \r\n" +
                    "JOIN DynamicFormSectionSecurity t2 ON t1.DynamicFormSectionSecurityID=t2.DynamicFormSectionSecurityID\r\n" +
                    "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t2.DynamicFormSectionID\r\n" +
                    "JOIN Employee t4 ON t4.UserID=t1.UserID WHERE t1.DynamicFormSectionId=@DynamicFormSectionId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionWorkFlow>(query, parameters)).ToList();
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
                var query = "select t1.DynamicFormApprovedID,\r\nt1.DynamicFormApprovalID,\r\nt1.DynamicFormDataID,\r\nt1.IsApproved,\r\nt1.ApprovedDescription,\r\nt1.UserID,\r\nt1.ApprovedByUserID,\r\nt1.ApprovedDate,\r\nt1.ApprovedSortBy,t2.ApprovalUserId FROM DynamicFormApproved t1 JOIN DynamicFormApproval t2  ON t2.DynamicFormApprovalID=t1.DynamicFormApprovalID " +
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
        public async Task<DynamicForm> GetDynamicFormScreenNameDataCheckValidation(string? value)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("ScreenID", value);
                query = "SELECT t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted\n\r FROM DynamicForm t1 Where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.ScreenID = @ScreenID";

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

                    query = "SELECT t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted\n\r FROM DynamicForm t1 Where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.ID!=@id AND t1.ScreenID = @ScreenID";
                }
                else
                {
                    query = "SELECT t1.ID,\r\nt1.Name,\r\nt1.ScreenID,\r\nt1.SessionID,\r\nt1.AttributeID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsApproval,\r\nt1.IsUpload,\r\nt1.FileProfileTypeID,\r\nt1.IsProfileNoGenerate,\r\nt1.IsMultipleUpload,\r\nt1.CompanyID,\r\nt1.ProfileID,\r\nt1.IsGridForm,\r\nt1.IsDeleted\n\r FROM DynamicForm t1 Where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.ScreenID = @ScreenID";
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
                        parameters.Add("CompanyId", dynamicForm.CompanyId);
                        parameters.Add("ProfileId", dynamicForm.ProfileId);
                        parameters.Add("IsGridForm", dynamicForm.IsGridForm);
                        var query = "INSERT INTO DynamicForm(IsGridForm,Name,ScreenID,SessionID,AttributeID,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsApproval,FileProfileTypeId,IsUpload,CompanyId,ProfileId) VALUES " +
                            "(@IsGridForm,@Name,@ScreenID,@SessionID,@AttributeID,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsApproval,@FileProfileTypeId,@IsUpload,@CompanyId,@ProfileId)";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
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

        public async Task<long> Update(DynamicForm dynamicForm)
        {
            try
            {
                using (var connection = CreateConnection())
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
                        parameters.Add("CompanyId", dynamicForm.CompanyId);
                        parameters.Add("ProfileId", dynamicForm.ProfileId);
                        parameters.Add("IsGridForm", dynamicForm.IsGridForm);
                        var query = " UPDATE DynamicForm SET IsGridForm=@IsGridForm,AttributeID = @AttributeID,Name =@Name,ScreenID =@ScreenID,ModifiedByUserID=@ModifiedByUserID,CompanyId=@CompanyId,ProfileId=@ProfileId," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsApproval=@IsApproval,IsUpload=@IsUpload,FileProfileTypeId=@FileProfileTypeId WHERE ID = @ID";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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

                query = "SELECT DynamicFormSectionID,DynamicFormId,SortOrderBy FROM DynamicFormSection Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId order by  SortOrderBy desc";
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
                        parameters.Add("Instruction", dynamicFormSection.Instruction, DbType.String);
                        parameters.Add("IsAutoNumberEnabled", dynamicFormSection.IsAutoNumberEnabled == null ? false : dynamicFormSection.IsAutoNumberEnabled);
                        parameters.Add("SectionFileProfileTypeId", dynamicFormSection.SectionFileProfileTypeId);
                        if (dynamicFormSection.DynamicFormSectionId > 0)
                        {
                            var query = " UPDATE DynamicFormSection SET SectionFileProfileTypeId=@SectionFileProfileTypeId,SectionName = @SectionName,DynamicFormId =@DynamicFormId,SortOrderBy=@SortOrderBys," +
                                "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsVisible=@IsVisible," +
                                "IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite,Instruction=@Instruction,IsAutoNumberEnabled=@IsAutoNumberEnabled " +
                                "WHERE DynamicFormSectionId = @DynamicFormSectionId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            parameters.Add("SortOrderBy", GeDynamicFormSectionSort(dynamicFormSection.DynamicFormId));
                            var query = "INSERT INTO DynamicFormSection(IsAutoNumberEnabled,SectionFileProfileTypeId,SectionName,DynamicFormId,SessionId,SortOrderBy,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsVisible,IsReadOnly,IsReadWrite,Instruction)  " +
                                "OUTPUT INSERTED.DynamicFormSectionId VALUES " +
                                "(@IsAutoNumberEnabled,@SectionFileProfileTypeId,@SectionName,@DynamicFormId,@SessionId,@SortOrderBy,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsVisible,@IsReadOnly,@IsReadWrite,@Instruction)";

                            dynamicFormSection.DynamicFormSectionId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }

                        return dynamicFormSection;
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


        public int? GeDynamicFormSectionAttributeSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormSectionId", id);

                query = "SELECT DynamicFormSectionAttributeID,DynamicFormSectionId,SortOrderBy FROM DynamicFormSectionAttribute Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionId = @DynamicFormSectionId order by  SortOrderBy desc";
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
        public async Task<DynamicFormSectionAttribute> UpdateFormulaTextBox(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSectionAttributeId", dynamicFormSectionAttribute.DynamicFormSectionAttributeId);
                        parameters.Add("FormulaTextBox", dynamicFormSectionAttribute.FormulaTextBox, DbType.String);
                        var query = " UPDATE DynamicFormSectionAttribute SET FormulaTextBox=@FormulaTextBox WHERE DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId";

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return dynamicFormSectionAttribute;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


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
                        parameters.Add("FormToolTips", dynamicFormSection.FormToolTips, DbType.String);
                        parameters.Add("IsVisible", dynamicFormSection.IsVisible);
                        parameters.Add("IsRadioCheckRemarks", dynamicFormSection.IsRadioCheckRemarks, DbType.String);
                        parameters.Add("RadioLayout", dynamicFormSection.RadioLayout, DbType.String);
                        parameters.Add("RemarksLabelName", dynamicFormSection.RemarksLabelName, DbType.String);
                        parameters.Add("PlantDropDownWithOtherDataSourceId", dynamicFormSection.PlantDropDownWithOtherDataSourceId);
                        parameters.Add("PlantDropDownWithOtherDataSourceLabelName", dynamicFormSection.PlantDropDownWithOtherDataSourceLabelName, DbType.String);
                        parameters.Add("IsPlantLoadDependency", dynamicFormSection.IsPlantLoadDependency == true ? true : null);
                        parameters.Add("IsDefaultReadOnly", dynamicFormSection.IsDefaultReadOnly == true ? true : null);
                        parameters.Add("IsSetDefaultValue", dynamicFormSection.IsSetDefaultValue == true ? true : null);
                        parameters.Add("IsDependencyMultiple", dynamicFormSection.IsDependencyMultiple == true ? true : null);
                        parameters.Add("IsDisplayDropDownHeader", dynamicFormSection.IsDisplayDropDownHeader);
                        parameters.Add("IsDynamicFormGridDropdown", dynamicFormSection.IsDynamicFormGridDropdown == true ? true : null);
                        parameters.Add("GridDropDownDynamicFormID", dynamicFormSection.GridDropDownDynamicFormID);
                        // parameters.Add("FormulaTextBox", dynamicFormSection.FormulaTextBox,DbType.String);
                        parameters.Add("IsDynamicFormGridDropdownMultiple", dynamicFormSection.IsDynamicFormGridDropdownMultiple == true ? true : null);
                        parameters.Add("ApplicationMasterIds", dynamicFormSection.ApplicationMasterIdsListIds != null && dynamicFormSection.ApplicationMasterIdsListIds.Count() > 0 ? string.Join(",", dynamicFormSection.ApplicationMasterIdsListIds) : null, DbType.String);
                        parameters.Add("PlantDropDownWithOtherDataSourceIds", dynamicFormSection.PlantDropDownWithOtherDataSourceListIds != null && dynamicFormSection.PlantDropDownWithOtherDataSourceListIds.Count() > 0 ? string.Join(",", dynamicFormSection.PlantDropDownWithOtherDataSourceListIds) : null, DbType.String);
                        if (dynamicFormSection.DynamicFormSectionAttributeId > 0)
                        {

                            var query = "UPDATE DynamicFormSectionAttribute SET IsDynamicFormGridDropdownMultiple=@IsDynamicFormGridDropdownMultiple,IsDynamicFormGridDropdown=@IsDynamicFormGridDropdown,GridDropDownDynamicFormID=@GridDropDownDynamicFormID,IsDependencyMultiple=@IsDependencyMultiple,IsDisplayDropDownHeader=@IsDisplayDropDownHeader,ApplicationMasterIds=@ApplicationMasterIds,IsSetDefaultValue=@IsSetDefaultValue,IsDefaultReadOnly=@IsDefaultReadOnly,PlantDropDownWithOtherDataSourceIds=@PlantDropDownWithOtherDataSourceIds,IsPlantLoadDependency=@IsPlantLoadDependency,PlantDropDownWithOtherDataSourceLabelName=@PlantDropDownWithOtherDataSourceLabelName,PlantDropDownWithOtherDataSourceId=@PlantDropDownWithOtherDataSourceId,RemarksLabelName=@RemarksLabelName,IsRadioCheckRemarks=@IsRadioCheckRemarks,RadioLayout=@RadioLayout,DisplayName = @DisplayName,AttributeId =@AttributeId,DynamicFormSectionId=@DynamicFormSectionId," +
                                "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,IsSpinEditType=@IsSpinEditType," +
                                "StatusCodeID=@StatusCodeID,ColSpan=@ColSpan,FormToolTips=@FormToolTips,SortOrderBy=@SortOrderBys,IsRequired=@IsRequired,IsMultiple=@IsMultiple,RequiredMessage=@RequiredMessage,IsDisplayTableHeader=@IsDisplayTableHeader,IsVisible=@IsVisible " +
                                "WHERE DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            parameters.Add("SortOrderBy", GeDynamicFormSectionAttributeSort(dynamicFormSection.DynamicFormSectionId));
                            var query = "INSERT INTO DynamicFormSectionAttribute(IsDynamicFormGridDropdownMultiple,IsDynamicFormGridDropdown,GridDropDownDynamicFormID,IsDependencyMultiple,IsDisplayDropDownHeader,ApplicationMasterIds,IsSetDefaultValue,IsDefaultReadOnly,PlantDropDownWithOtherDataSourceIds,IsPlantLoadDependency,PlantDropDownWithOtherDataSourceLabelName,PlantDropDownWithOtherDataSourceId,RemarksLabelName,IsRadioCheckRemarks,RadioLayout,FormToolTips,DisplayName,AttributeId,SessionId,SortOrderBy,AddedByUserID," +
                                "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,ColSpan,DynamicFormSectionId,IsRequired,IsMultiple,RequiredMessage,IsSpinEditType,IsDisplayTableHeader,IsVisible) OUTPUT INSERTED.DynamicFormSectionAttributeId VALUES " +
                                "(@IsDynamicFormGridDropdownMultiple,@IsDynamicFormGridDropdown,@GridDropDownDynamicFormID,@IsDependencyMultiple,@IsDisplayDropDownHeader,@ApplicationMasterIds,@IsSetDefaultValue,@IsDefaultReadOnly,@PlantDropDownWithOtherDataSourceIds,@IsPlantLoadDependency,@PlantDropDownWithOtherDataSourceLabelName,@PlantDropDownWithOtherDataSourceId,@RemarksLabelName,@IsRadioCheckRemarks,@RadioLayout,@FormToolTips,@DisplayName,@AttributeId,@SessionId,@SortOrderBy," +
                                "@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@ColSpan,@DynamicFormSectionId,@IsRequired,@IsMultiple,@RequiredMessage,@IsSpinEditType,@IsDisplayTableHeader,@IsVisible)";

                            dynamicFormSection.DynamicFormSectionAttributeId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        /*if (dynamicFormSection.DynamicFormSectionAttributeId > 0)
                        {
                            var querys = string.Empty;
                            querys += "Delete from DynamicFormSectionAttributeSection where DynamicFormSectionAttributeId=" + dynamicFormSection.DynamicFormSectionAttributeId + ";\r\n";

                            if (dynamicFormSection.DynamicFormSectionIds != null && dynamicFormSection.ShowSectionVisibleDataIds != null && dynamicFormSection.DynamicFormSectionIds.Count() > 0 && dynamicFormSection.ShowSectionVisibleDataIds.Count() > 0)
                            {
                                dynamicFormSection.DynamicFormSectionIds.ToList().ForEach(a =>
                                {
                                    dynamicFormSection.ShowSectionVisibleDataIds.ToList().ForEach(b =>
                                    {
                                        if (!string.IsNullOrEmpty(b))
                                        {
                                            var c = b.Split("_");
                                            var exists = c.ElementAtOrDefault(1) != null;
                                            if (exists == true)
                                            {
                                                querys += "INSERT INTO DynamicFormSectionAttributeSection(DynamicFormSectionAttributeID,DynamicFormSectionID,DynamicFormSectionSelectionByID,DynamicFormSectionSelectionID) VALUES " +
                                            "(" + dynamicFormSection.DynamicFormSectionAttributeId + "," + a + ",'" + b + "'," + c[1] + ");\n\r";
                                            }
                                        }
                                    });
                                });
                            }
                            var rowsAffected = await connection.ExecuteAsync(querys);
                        }*/
                        return dynamicFormSection;
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

        public async Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionAsync(long? dynamicFormId)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select t1.IsAutoNumberEnabled,t1.DynamicFormSectionID,\r\nt1.SectionName,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.DynamicFormID,\r\nt1.SortOrderBy,\r\nt1.IsReadWrite,\r\nt1.IsReadOnly,\r\nt1.IsVisible,\r\nt1.Instruction,\r\nt1.IsDeleted,\r\nt1.SectionFileProfileTypeID,t7.PlantID as CompanyId,t7.PlantCode,(case when t1.AddedByUserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName,(case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as AddedBy,(case when t1.ModifiedByUserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END)\r\n,t3.LastName) ELSE null END) as ModifiedBy,t4.CodeValue as StatusCode,t5.Name as SectionFileProfileTypeName,\r\n" +
                    "(select SUM(t5.FormUsedCount) from DynamicFormSectionAttribute t5 where t5.DynamicFormSectionId=t1.DynamicFormSectionId) as  FormUsedCount\r\n" +
                    "from DynamicFormSection t1 \r\n" +
                    "JOIN Employee t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN Employee t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicForm t6 ON t6.ID=t1.DynamicFormID\r\n" +
                    "LEFT JOIN Plant t7 ON t7.PlantID=t6.CompanyID\r\n" +
                     "LEFT JOIN FileProfileType t5 ON t5.FileProfileTypeID=t1.SectionFileProfileTypeID\r\n" +
                    "WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.DynamicFormId=@DynamicFormId";

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
        public async Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeForSpinEditAsync(long? dynamicFormId)
        {
            try
            {
                List<DynamicFormSectionAttribute> dynamicFormSectionAttributes = new List<DynamicFormSectionAttribute>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select t1.DynamicFormSectionAttributeID,\r\nt1.DynamicFormSectionID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.AttributeID,\r\nt1.SortOrderBy,\r\nt1.ColSpan,\r\nt1.DisplayName,\r\nt1.IsMultiple,\r\nt1.IsRequired,\r\nt1.RequiredMessage,\r\nt1.IsSpinEditType,\r\nt1.FormUsedCount,\r\nt1.IsDisplayTableHeader,\r\nt1.FormToolTips,\r\nt1.IsVisible,\r\nt1.IsDeleted,\r\nt1.IsSetDefaultValue,\r\nt1.IsDefaultReadOnly,t9.Name DynamicGridName,\r\n(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,\r\nt5.SectionName,t6.ControlTypeId,t6.AttributeName,t7.CodeValue as ControlType from \r\nDynamicFormSectionAttribute t1 \r\nJOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\nLEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t5.DynamicFormID\r\n" +
                    "Where (t9.IsDeleted=0 or t9.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null)  \r\nAND (t1.IsVisible=1 or t1.IsVisible is null) AND t6.ControlTypeID=2704 AND t9.ID=@DynamicFormId order by t1.SortOrderBy asc;\r\n";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormSectionAttributes = results.Read<DynamicFormSectionAttribute>().ToList();
                }
                return dynamicFormSectionAttributes != null ? dynamicFormSectionAttributes : new List<DynamicFormSectionAttribute>();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAllAsync(long? DynamicFormId)
        {
            try
            {
                List<DynamicFormSectionAttribute> dynamicFormSectionAttributes = new List<DynamicFormSectionAttribute>();
                var applicationMasters = new List<ApplicationMaster>(); var applicationMasterParent = new List<ApplicationMasterParent>(); var attributeHeaderDataSource = new List<AttributeHeaderDataSource>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", DynamicFormId);
                var query = "select tt1.ID as DynamicFormId,t1.GridDisplaySeqNo,t1.FormulaTextBox,t1.DynamicFormSectionAttributeID,\r\nt1.DynamicFormSectionID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.AttributeID,\r\nt1.SortOrderBy,\r\nt1.ColSpan,\r\nt1.DisplayName,\r\nt1.IsMultiple,\r\nt1.IsRequired,\r\nt1.RequiredMessage,\r\nt1.IsSpinEditType,\r\nt1.FormUsedCount,\r\nt1.IsDisplayTableHeader,\r\nt1.FormToolTips,\r\nt1.IsVisible,\r\nt1.RadioLayout,\r\nt1.IsRadioCheckRemarks,\r\nt1.RemarksLabelName,\r\nt1.IsDeleted,\r\nt1.IsPlantLoadDependency,\r\nt1.PlantDropDownWithOtherDataSourceID,\r\nt1.PlantDropDownWithOtherDataSourceLabelName,\r\nt1.PlantDropDownWithOtherDataSourceIDs,\r\nt1.IsSetDefaultValue,\r\nt1.IsDefaultReadOnly,\r\nt1.ApplicationMasterID,\r\nt1.ApplicationMasterIDs,\r\nt1.IsDisplayDropDownHeader,\r\nt1.IsDependencyMultiple,\r\nt1.IsDynamicFormGridDropdown,\r\nt1.GridDropDownDynamicFormID,\r\nt1.IsDynamicFormGridDropdownMultiple,t9.Name DynamicGridName,t10.DataSourceTable as PlantDropDownWithOtherDataSourceTable,t6.IsFilterDataSource,t6.FilterDataSocurceID,tt4.DisplayName as FilterDataSourceDisplayName,tt4.TableName as FilterDataSourceTableName," +
                    "(case when t1.IsDynamicFormGridDropdownMultiple is NULL then  0 ELSE t1.IsDynamicFormGridDropdownMultiple END) as IsDynamicFormGridDropdownMultiple,(case when t1.IsDisplayTableHeader is NULL then  0 ELSE t1.IsDisplayTableHeader END) as IsDisplayTableHeader,(case when t1.IsDisplayDropDownHeader is NULL then  0 ELSE t1.IsDisplayDropDownHeader END) as IsDisplayDropDownHeader,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible," +
                    "(case when t1.AddedByUserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName,(case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as AddedBy,(case when t1.ModifiedByUserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,',',t3.LastName) ELSE null END) as ModifiedBy,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t4.CodeValue as StatusCode,t5.SectionName,t6.ControlTypeId,t6.IsDynamicFormDropTagBox,t6.DropDownTypeId,t6.DataSourceId,t6.AttributeName,t7.CodeValue as ControlType from DynamicFormSectionAttribute t1 \r\n" +
                    "JOIN Employee t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN Employee t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t8.AttributeHeaderDataSourceID=t6.DataSourceId\r\n" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n LEFT JOIN DynamicForm tt1 ON tt1.ID=t5.DynamicFormID\n\r" +
                    "LEFT JOIN DynamicFormFilter tt4 ON tt4.DynamicFilterID=t6.FilterDataSocurceID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t10 ON t10.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                    "Where (tt1.IsDeleted=0 or tt1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null) AND tt1.ID=@DynamicFormId;\r\n";
                query += "Select ApplicationMasterId,ApplicationMasterName,ApplicationMasterDescription,ApplicationMasterCodeId from ApplicationMaster;";
                query += "Select ApplicationMasterParentId,ApplicationMasterParentCodeId,ApplicationMasterName,Description,ParentId from ApplicationMasterParent;";
                query += "Select t1.HeaderDataSourceId,t1.AttributeHeaderDataSourceId,t1.DisplayName,t1.DataSourceTable,(Select COUNT(*) as IsDynamicFormFilterBy from DynamicFormFilterBy t2 where t2.AttributeHeaderDataSourceID=t1.AttributeHeaderDataSourceID)as IsDynamicFormFilterBy from AttributeHeaderDataSource t1;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormSectionAttributes = results.Read<DynamicFormSectionAttribute>().ToList();
                    applicationMasters = results.Read<ApplicationMaster>().ToList();
                    applicationMasterParent = results.Read<ApplicationMasterParent>().ToList();
                    attributeHeaderDataSource = results.Read<AttributeHeaderDataSource>().ToList();
                }
                if (dynamicFormSectionAttributes != null && dynamicFormSectionAttributes.Count() > 0)
                {
                    dynamicFormSectionAttributes.ForEach(s =>
                    {
                        if (s.IsPlantLoadDependency == true && !string.IsNullOrEmpty(s.PlantDropDownWithOtherDataSourceIds))
                        {
                            var PlantDropDownWithOtherDataSourceListIds = s.PlantDropDownWithOtherDataSourceIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (PlantDropDownWithOtherDataSourceListIds.Count > 0)
                            {
                                var list = attributeHeaderDataSource.Where(z => z.DataSourceTable != null && PlantDropDownWithOtherDataSourceListIds.Contains(z.AttributeHeaderDataSourceId)).ToList();
                                if (list != null && list.Count() > 0)
                                {
                                    s.AttributeHeaderDataSource = list;
                                    s.MasterName = string.Join(',', list.Select(a => a.DisplayName).ToList());
                                }
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMaster" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                var list = applicationMasters.Where(z => z.ApplicationMasterId > 0 && applicationMasterIds.Contains(z.ApplicationMasterId)).ToList();
                                if (list != null && list.Count() > 0)
                                {
                                    s.ApplicationMaster = list;
                                    s.MasterName = string.Join(',', list.Select(a => a.ApplicationMasterName).ToList());
                                }
                            }
                        }
                        if (s.DataSourceTable == "ApplicationMasterParent" && !string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                var list = applicationMasterParent.Where(z => z.ApplicationMasterParentCodeId > 0 && applicationMasterIds.Contains(z.ApplicationMasterParentCodeId)).ToList();
                                if (list != null && list.Count() > 0)
                                {
                                    s.ApplicationMasterParents = list;
                                    s.MasterName = string.Join(',', list.Select(a => a.ApplicationMasterName).ToList());
                                }
                            }
                        }
                    });
                }
                return dynamicFormSectionAttributes != null ? dynamicFormSectionAttributes : new List<DynamicFormSectionAttribute>();
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
                List<DynamicFormSectionAttribute> dynamicFormSectionAttributes = new List<DynamicFormSectionAttribute>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", dynamicFormSectionId);
                var query = "select t1.GridDisplaySeqNo,t1.FormulaTextBox,t1.DynamicFormSectionAttributeID,\r\nt1.DynamicFormSectionID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.AttributeID,\r\nt1.SortOrderBy,\r\nt1.ColSpan,\r\nt1.DisplayName,\r\nt1.IsMultiple,\r\nt1.IsRequired,\r\nt1.RequiredMessage,\r\nt1.IsSpinEditType,\r\nt1.FormUsedCount,\r\nt1.IsDisplayTableHeader,\r\nt1.FormToolTips,\r\nt1.IsVisible,\r\nt1.RadioLayout,\r\nt1.IsRadioCheckRemarks,\r\nt1.RemarksLabelName,\r\nt1.IsDeleted,\r\nt1.IsPlantLoadDependency,\r\nt1.PlantDropDownWithOtherDataSourceID,\r\nt1.PlantDropDownWithOtherDataSourceLabelName,\r\nt1.PlantDropDownWithOtherDataSourceIDs,\r\nt1.IsSetDefaultValue,\r\nt1.IsDefaultReadOnly,\r\nt1.ApplicationMasterID,\r\nt1.ApplicationMasterIDs,\r\nt1.IsDisplayDropDownHeader,\r\nt1.IsDependencyMultiple,\r\nt1.IsDynamicFormGridDropdown,\r\nt1.GridDropDownDynamicFormID,\r\nt1.IsDynamicFormGridDropdownMultiple,t9.Name DynamicGridName,t10.DataSourceTable as PlantDropDownWithOtherDataSourceTable,t6.IsFilterDataSource,t6.FilterDataSocurceID,tt4.DisplayName as FilterDataSourceDisplayName,tt4.TableName as FilterDataSourceTableName," +
                    "(case when t1.IsDynamicFormGridDropdownMultiple is NULL then  0 ELSE t1.IsDynamicFormGridDropdownMultiple END) as IsDynamicFormGridDropdownMultiple,(case when t1.IsDisplayTableHeader is NULL then  0 ELSE t1.IsDisplayTableHeader END) as IsDisplayTableHeader,(case when t1.IsDisplayDropDownHeader is NULL then  0 ELSE t1.IsDisplayDropDownHeader END) as IsDisplayDropDownHeader,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible," +
                    "(case when t1.AddedByUserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName,(case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as AddedBy,(case when t1.ModifiedByUserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,(case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as ModifiedBy,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t4.CodeValue as StatusCode,t5.SectionName,t6.ControlTypeId,t6.IsDynamicFormDropTagBox,t6.DropDownTypeId,t6.DataSourceId,t6.AttributeName,t7.CodeValue as ControlType from DynamicFormSectionAttribute t1 \r\n" +
                    "JOIN Employee t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN Employee t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t8.AttributeHeaderDataSourceID=t6.DataSourceId\r\n" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n LEFT JOIN DynamicForm tt1 ON tt1.ID=t5.DynamicFormID\n\r" +
                    "LEFT JOIN DynamicFormFilter tt4 ON tt4.DynamicFilterID=t6.FilterDataSocurceID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t10 ON t10.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                    "Where (tt1.IsDeleted=0 or tt1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null)  AND t1.DynamicFormSectionId=@DynamicFormSectionId;\r\n";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormSectionAttributes = results.Read<DynamicFormSectionAttribute>().ToList();
                }
                return dynamicFormSectionAttributes != null ? dynamicFormSectionAttributes : new List<DynamicFormSectionAttribute>();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttributeSectionParent>> GetDynamicFormSectionAttributeSectionParentAsync(long? dynamicFormSectionAttributeId)
        {
            try
            {
                List<DynamicFormSectionAttributeSectionParent> _dynamicFormSectionAttributeSectionParent = new List<DynamicFormSectionAttributeSectionParent>();
                List<DynamicFormSectionAttributeSection> _dynamicFormSectionAttributeSection = new List<DynamicFormSectionAttributeSection>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionAttributeId", dynamicFormSectionAttributeId);
                var query = "select SequenceNo,DynamicFormSectionAttributeSectionParentID,DynamicFormSectionAttributeID from DynamicFormSectionAttributeSectionParent where DynamicFormSectionAttributeID=@DynamicFormSectionAttributeId;\r\n";
                query += "select t1.DynamicFormSectionAttributeSectionID,\r\nt1.DynamicFormSectionAttributeSectionParentID,\r\nt1.DynamicFormSectionID,\r\nt1.DynamicFormSectionSelectionID,\r\nt1.DynamicFormSectionSelectionByID,t2.DynamicFormSectionAttributeId from DynamicFormSectionAttributeSection t1 JOIN DynamicFormSectionAttributeSectionParent t2 ON t1.DynamicFormSectionAttributeSectionParentId=t2.DynamicFormSectionAttributeSectionParentId\r\n" +
                    "where t2.DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    _dynamicFormSectionAttributeSectionParent = results.Read<DynamicFormSectionAttributeSectionParent>().ToList();
                    _dynamicFormSectionAttributeSection = results.Read<DynamicFormSectionAttributeSection>().ToList();
                }
                if (_dynamicFormSectionAttributeSectionParent != null && _dynamicFormSectionAttributeSectionParent.Count() > 0)
                {
                    _dynamicFormSectionAttributeSectionParent.ForEach(s =>
                    {
                        s.DynamicFormSectionAttributeSections = _dynamicFormSectionAttributeSection.Where(w => w.DynamicFormSectionAttributeSectionParentId == s.DynamicFormSectionAttributeSectionParentId).ToList();
                        s.DynamicFormSectionIds = s.DynamicFormSectionAttributeSections.Select(a => a.DynamicFormSectionId).Distinct().ToList();
                        s.ShowSectionVisibleDataIds = s.DynamicFormSectionAttributeSections.Select(a => a.DynamicFormSectionSelectionById).Distinct().ToList();
                    });
                }
                return _dynamicFormSectionAttributeSectionParent;
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
                query = "SELECT DynamicFormSectionID,\r\nSectionName,\r\nSessionID,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nDynamicFormID,\r\nSortOrderBy,\r\nIsReadWrite,\r\nIsReadOnly,\r\nIsVisible,\r\nInstruction,\r\nIsDeleted,\r\nSectionFileProfileTypeID FROM DynamicFormSection Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId AND SortOrderBy>@SortOrderBy";
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


                    try
                    {
                        var result = await UpdateDynamicFormSectionSort(dynamicFormSection.DynamicFormId, dynamicFormSection.SortOrderBy);
                        var parameters = new DynamicParameters();
                        parameters.Add("id", dynamicFormSection.DynamicFormSectionId);
                        var sortby = dynamicFormSection.SortOrderBy;
                        //var query = "DELETE  FROM DynamicFormSection WHERE DynamicFormSectionID = @id;";
                        var query = "UPDATE  DynamicFormSection SET IsDeleted=1 WHERE DynamicFormSectionID = @id;";
                        if (result != null)
                        {
                            result.ForEach(s =>
                            {
                                query += "Update  DynamicFormSection SET SortOrderBy=" + sortby + "  WHERE DynamicFormSectionID =" + s.DynamicFormSectionId + ";";
                                sortby++;
                            });
                        }

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
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
                query = "SELECT DynamicFormSectionId,DynamicFormId,SortOrderBy FROM DynamicFormSection Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId  AND SortOrderBy>@SortOrderByFrom and SortOrderBy<=@SortOrderByTo order by SortOrderBy asc";

                if (dynamicFormSection.SortOrderAnotherBy > dynamicFormSection.SortOrderBy)
                {
                    query = "SELECT DynamicFormSectionId,DynamicFormId,SortOrderBy FROM DynamicFormSection Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId  AND SortOrderBy>=@SortOrderByFrom and SortOrderBy<@SortOrderByTo order by SortOrderBy asc";

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



                    try
                    {
                        var query = string.Empty;
                        int? SortOrder = dynamicFormSection.SortOrderAnotherBy > dynamicFormSection.SortOrderBy ? (dynamicFormSection.SortOrderBy + 1) : dynamicFormSection.SortOrderAnotherBy;
                        query += "Update  DynamicFormSection SET SortOrderBy=" + dynamicFormSection.SortOrderBy + "  WHERE (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionID =" + dynamicFormSection.DynamicFormSectionId + ";";
                        if (SortOrder > 0)
                        {
                            var result = await GetUpdateDynamicFormSectionSortOrder(dynamicFormSection);
                            if (result != null && result.Count > 0)
                            {

                                result.ForEach(s =>
                                {

                                    query += "Update  DynamicFormSection SET SortOrderBy=" + SortOrder + "  WHERE (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionID =" + s.DynamicFormSectionId + ";";
                                    SortOrder++;
                                });

                            }

                            var rowsAffected = await connection.ExecuteAsync(query, null);
                        }

                        return dynamicFormSection;
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



        public async Task<List<DynamicFormSectionAttribute>> UpdateDynamicFormSectionAttributeSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormSectionId", id);
                parameters.Add("SortOrderBy", SortOrderBy);
                query = "SELECT DynamicFormSectionAttributeId,DynamicFormSectionId,SortOrderBy FROM DynamicFormSectionAttribute Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionId = @DynamicFormSectionId AND SortOrderBy>@SortOrderBy";
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


                    try
                    {
                        var result = await UpdateDynamicFormSectionAttributeSort(dynamicFormSectionAttribute.DynamicFormSectionId, dynamicFormSectionAttribute.SortOrderBy);
                        var parameters = new DynamicParameters();
                        parameters.Add("id", dynamicFormSectionAttribute.DynamicFormSectionAttributeId);
                        var sortby = dynamicFormSectionAttribute.SortOrderBy;
                        //var query = "DELETE  FROM DynamicFormSectionAttribute WHERE DynamicFormSectionAttributeId = @id;";
                        var query = "UPDATE   DynamicFormSectionAttribute SET IsDeleted=1 WHERE DynamicFormSectionAttributeId = @id;";
                        if (result != null)
                        {
                            result.ForEach(s =>
                            {
                                query += "Update  DynamicFormSectionAttribute SET SortOrderBy=" + sortby + "  WHERE DynamicFormSectionAttributeId =" + s.DynamicFormSectionAttributeId + ";";
                                sortby++;
                            });
                        }

                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return rowsAffected;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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
                query = "SELECT DynamicFormSectionId,DynamicFormSectionAttributeId,SortOrderBy FROM DynamicFormSectionAttribute Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionId = @DynamicFormSectionId  AND SortOrderBy>@SortOrderByFrom and SortOrderBy<=@SortOrderByTo order by SortOrderBy asc";

                if (dynamicFormSectionAttribute.SortOrderAnotherBy > dynamicFormSectionAttribute.SortOrderBy)
                {
                    query = "SELECT DynamicFormSectionId,DynamicFormSectionAttributeId,SortOrderBy FROM DynamicFormSectionAttribute Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionId = @DynamicFormSectionId  AND SortOrderBy>=@SortOrderByFrom and SortOrderBy<@SortOrderByTo order by SortOrderBy asc";

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



                    try
                    {
                        var query = string.Empty;
                        int? SortOrder = dynamicFormSectionAttribute.SortOrderAnotherBy > dynamicFormSectionAttribute.SortOrderBy ? (dynamicFormSectionAttribute.SortOrderBy + 1) : dynamicFormSectionAttribute.SortOrderAnotherBy;
                        query += "Update  DynamicFormSectionAttribute SET SortOrderBy=" + dynamicFormSectionAttribute.SortOrderBy + "  WHERE (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionAttributeId =" + dynamicFormSectionAttribute.DynamicFormSectionAttributeId + ";";
                        if (SortOrder > 0)
                        {
                            var result = await GetUpdateDynamicFormSectionAttributeSortOrder(dynamicFormSectionAttribute);
                            if (result != null && result.Count > 0)
                            {

                                result.ForEach(s =>
                                {

                                    query += "Update  DynamicFormSectionAttribute SET SortOrderBy=" + SortOrder + "  WHERE  DynamicFormSectionAttributeId =" + s.DynamicFormSectionAttributeId + ";";
                                    SortOrder++;
                                });

                            }
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, null);
                        return dynamicFormSectionAttribute;
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
        public async Task<long> InsertDynamicFormAttributeSection(long dynamicFormSectionId, IEnumerable<AttributeHeader> attributeIds, long? UserId)
        {
            try
            {
                using (var connection = CreateConnection())
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
                            await connection.ExecuteAsync(query, parameters);
                        }

                        return dynamicFormSectionId;
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
        private async Task<string> GenerateDocumentProfileAutoNumber(DynamicFormProfile value)
        {
            DocumentNoSeriesModel documentNoSeriesModel = new DocumentNoSeriesModel();
            documentNoSeriesModel.AddedByUserID = value.UserId;
            documentNoSeriesModel.StatusCodeID = 710;
            documentNoSeriesModel.ProfileID = value.ProfileId;
            documentNoSeriesModel.PlantID = value.PlantId;
            documentNoSeriesModel.DepartmentId = value.DepartmentId;
            documentNoSeriesModel.SectionId = value.SectionId;
            documentNoSeriesModel.SubSectionId = value.SubSectionId;
            documentNoSeriesModel.DivisionId = value.DivisionId;
            return await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(documentNoSeriesModel);
        }
        public async Task<long?> GeDynamicFormDataSortOrdrByNo(DynamicFormData dynamicFormData)
        {
            try
            {
                long? SortOrderBy = null;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormData.DynamicFormId);
                parameters.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId > 0 ? dynamicFormData.DynamicFormDataId : -1);
                query += "SELECT DynamicFormId,SortOrderByNo,DynamicFormDataId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataId=@DynamicFormDataId AND DynamicFormId = @DynamicFormId order by  SortOrderByNo desc;";
                query += "SELECT DynamicFormId,SortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId order by  SortOrderByNo desc";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    var result1 = results.Read<DynamicFormData>().FirstOrDefault();
                    var result = results.Read<DynamicFormData>().FirstOrDefault();
                    if (result1 == null)
                    {
                        if (result != null)
                        {
                            if (result.SortOrderByNo == null)
                            {
                                SortOrderBy = 1;
                            }
                            else
                            {
                                SortOrderBy = result.SortOrderByNo + 1;
                            }
                        }
                        else
                        {
                            SortOrderBy = 1;
                        }
                    }
                    else
                    {
                        if (result1.SortOrderByNo == null)
                        {
                            if (result != null)
                            {
                                if (result.SortOrderByNo == null)
                                {
                                    SortOrderBy = 1;
                                }
                                else
                                {
                                    SortOrderBy = result.SortOrderByNo + 1;
                                }
                            }
                        }
                        else

                        {
                            SortOrderBy = result1.SortOrderByNo;
                        }
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long?> GeDynamicFormDataGridSortOrderByNo(DynamicFormData dynamicFormData)
        {
            try
            {
                long? SortOrderBy = null;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormData.DynamicFormId);
                parameters.Add("DynamicFormDataGridId", dynamicFormData.DynamicFormDataGridId);
                parameters.Add("DynamicFormSectionGridAttributeId", dynamicFormData.DynamicFormSectionGridAttributeId);
                parameters.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId > 0 ? dynamicFormData.DynamicFormDataId : -1);
                query += "SELECT DynamicFormId,GridSortOrderByNo,DynamicFormDataId,DynamicFormDataGridId,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataId=@DynamicFormDataId AND DynamicFormId = @DynamicFormId  AND DynamicFormSectionGridAttributeId=@DynamicFormSectionGridAttributeId AND DynamicFormDataGridId=@DynamicFormDataGridId order by  SortOrderByNo desc;";
                query += "SELECT DynamicFormId,GridSortOrderByNo,DynamicFormDataGridId,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId AND DynamicFormDataGridId=@DynamicFormDataGridId  AND DynamicFormSectionGridAttributeId=@DynamicFormSectionGridAttributeId order by  GridSortOrderByNo desc";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    var result1 = results.Read<DynamicFormData>().FirstOrDefault();
                    var result = results.Read<DynamicFormData>().FirstOrDefault();
                    if (result1 == null)
                    {
                        if (result != null)
                        {
                            if (result.GridSortOrderByNo == null)
                            {
                                SortOrderBy = 1;
                            }
                            else
                            {
                                SortOrderBy = result.GridSortOrderByNo + 1;
                            }
                        }
                        else
                        {
                            SortOrderBy = 1;
                        }
                    }
                    else
                    {
                        if (result1.GridSortOrderByNo == null)
                        {
                            if (result != null)
                            {
                                if (result.GridSortOrderByNo == null)
                                {
                                    SortOrderBy = 1;
                                }
                                else
                                {
                                    SortOrderBy = result.GridSortOrderByNo + 1;
                                }
                            }
                        }
                        else

                        {
                            SortOrderBy = result1.GridSortOrderByNo;
                        }
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowSection> InsertOrUpdateFormWorkFlowSectionNoWorkFlowStart(long? dynamicFormId, long? dynamicFormDataId, long? userId)
        {
            DynamicFormWorkFlowSection dynamicFormWorkFlowSection = new DynamicFormWorkFlowSection();
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("CompletedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("FlowStatusID", 1);
                        var dataList = await GetDynamicFormWorkFlowLists(dynamicFormId, dynamicFormDataId);
                        var _dynamicFormWorkFlows = dataList.DynamicFormWorkFlow.ToList();
                        var _dynamicFormWorkFlowForms = dataList.DynamicFormWorkFlowForm.ToList();

                        if (_dynamicFormWorkFlows != null && _dynamicFormWorkFlows.Count() > 0)
                        {
                            int i = 0;
                            _dynamicFormWorkFlows.ForEach(s =>
                            {
                                int? IsAllowDelegateUser = s.IsAllowDelegateUser == true ? 1 : null;
                                int? IsParallelWorkflow = s.IsParallelWorkflow == true ? 1 : null;
                                var query = string.Empty;
                                int FlowStatusIDs = 0;
                                var _dynamicFormWorkFlowSections = dataList.DynamicFormWorkFlowSection.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).ToList();
                                query += "INSERT INTO [DynamicFormWorkFlowForm](SequenceNo,UserId,DynamicFormDataID,FlowStatusID";
                                if (IsAllowDelegateUser == 1)
                                {
                                    query += ",IsAllowDelegateUserForm";
                                }
                                if (IsParallelWorkflow == 1)
                                {
                                    query += ",IsParallelWorkflow";
                                }
                                query += ") OUTPUT INSERTED.DynamicFormWorkFlowFormID VALUES (" + s.SequenceNo + "," + s.UserId + "," + dynamicFormDataId + "," + FlowStatusIDs;
                                if (IsAllowDelegateUser == 1)
                                {
                                    query += "," + IsAllowDelegateUser;
                                }
                                if (IsParallelWorkflow == 1)
                                {
                                    query += "," + IsParallelWorkflow;
                                }
                                query += ");\r\n";
                                var ids = connection.QuerySingleOrDefault<long>(query, parameters);
                                if (_dynamicFormWorkFlowSections != null && _dynamicFormWorkFlowSections.Count() > 0)
                                {
                                    var query1 = string.Empty;
                                    _dynamicFormWorkFlowSections.ForEach(a =>
                                    {
                                        query1 += "INSERT INTO [DynamicFormWorkFlowSectionForm](DynamicFormSectionId,DynamicFormWorkFlowFormID) OUTPUT INSERTED.DynamicFormWorkFlowSectionFormID " +
                                                  "VALUES (" + a.DynamicFormSectionId + "," + ids + ");\r\n";
                                    });
                                    if (!string.IsNullOrEmpty(query1))
                                    {
                                        connection.QuerySingleOrDefault<long>(query1, parameters);
                                    }
                                }
                                var _dynamicFormWorkFlowApprovals = dataList.DynamicFormWorkFlowApproval.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).OrderBy(o => o.SortBy).ToList();
                                if (_dynamicFormWorkFlowApprovals != null && _dynamicFormWorkFlowApprovals.Count() > 0)
                                {
                                    var query2 = string.Empty;
                                    _dynamicFormWorkFlowApprovals.ForEach(d =>
                                    {
                                        query2 += "INSERT INTO [DynamicFormWorkFlowApprovedForm](DynamicFormWorkFlowFormID,UserID,SortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormID " +
                                                  "VALUES (" + ids + "," + d.UserId + "," + d.SortBy + ");\r\n";
                                    });
                                    if (!string.IsNullOrEmpty(query2))
                                    {
                                        connection.QuerySingleOrDefault<long>(query2, parameters);
                                    }
                                }
                                i++;
                            });
                        }
                        return dynamicFormWorkFlowSection;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormData> GetDynamicFormDataOneByIdAsync(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataId", Id);
                var query = "select * from DynamicFormData Where DynamicFormDataId=@DynamicFormDataId;";
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
        public async Task<DynamicFormData> GetDynamicFormDataOneBySessionIdAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId);
                var query = "select t1.DynamicFormDataID,t1.DynamicFormSectionGridAttributeId,t1.SessionID,t1.DynamicFormDataGridID,t2.SessionID as DynamicFormSessionID,t3.SessionID as DynamicFormSectionGridAttributeSessionId from DynamicFormData t1\r\nLEFT JOIN DynamicFormData t2  ON t2.DynamicFormDataID=t1.DynamicFormDataGridID\r\nLEFT JOIN DynamicFormSectionAttribute t3  ON t3.DynamicFormSectionAttributeID=t1.DynamicFormSectionGridAttributeID\r\nwhere t1.SessionID=@SessionId;";
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
        public async Task<DynamicFormData> InsertOrUpdateDynamicFormData(DynamicFormData dynamicFormData)
        {
            try
            {
                var preData = await GetDynamicFormDataOneByIdAsync(dynamicFormData.DynamicFormDataId > 0 ? dynamicFormData.DynamicFormDataId : 0);
                using (var connection = CreateConnection())
                {
                    try
                    {
                        bool? insert = false;
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
                        parameters.Add("FileProfileSessionID", dynamicFormData.FileProfileSessionID);
                        parameters.Add("DynamicFormDataGridId", dynamicFormData.DynamicFormDataGridId);
                        parameters.Add("DynamicFormSectionGridAttributeId", dynamicFormData.DynamicFormSectionGridAttributeId);
                        var ProfileId = dynamicFormData.DynamicFormProfile.ProfileId > 0 ? dynamicFormData.DynamicFormProfile.ProfileId : null;
                        var profileNo = dynamicFormData.ProfileNo;
                        if (dynamicFormData.ProfileId > 0)
                        {
                            ProfileId = dynamicFormData.ProfileId;
                        }
                        else
                        {
                            profileNo = await GenerateDocumentProfileAutoNumber(dynamicFormData.DynamicFormProfile);
                            dynamicFormData.ProfileNo = profileNo;
                            dynamicFormData.ProfileId = dynamicFormData.DynamicFormProfile.ProfileId;

                        }
                        parameters.Add("ProfileId", ProfileId);
                        parameters.Add("ProfileNo", profileNo, DbType.String);
                        var sortNo = await GeDynamicFormDataSortOrdrByNo(dynamicFormData);
                        parameters.Add("SortOrderByNo", sortNo);
                        long? gridSortOrderByNo = null;
                        if (dynamicFormData.IsDynamicFormDataGrid == true)
                        {
                            gridSortOrderByNo = await GeDynamicFormDataGridSortOrderByNo(dynamicFormData);
                        }
                        parameters.Add("GridSortOrderByNo", gridSortOrderByNo);
                        if (dynamicFormData.DynamicFormDataId > 0)
                        {
                            var query = "UPDATE DynamicFormData SET DynamicFormSectionGridAttributeId=@DynamicFormSectionGridAttributeId,GridSortOrderByNo=@GridSortOrderByNo,SortOrderByNo=@SortOrderByNo,DynamicFormDataGridId=@DynamicFormDataGridId,DynamicFormItem = @DynamicFormItem,DynamicFormId =@DynamicFormId,ProfileId=@ProfileId," +
                               "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsSendApproval=@IsSendApproval,ProfileNo=@ProfileNo " +
                               "WHERE DynamicFormDataId = @DynamicFormDataId;\n\r";
                            query += await UpdateDynamicFormSectionAttributeCount(dynamicFormData, "Update");
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {

                            var query = "INSERT INTO DynamicFormData(DynamicFormSectionGridAttributeId,GridSortOrderByNo,SortOrderByNo,DynamicFormDataGridId,DynamicFormItem,DynamicFormId,SessionId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsSendApproval,FileProfileSessionID,ProfileId,ProfileNo)  OUTPUT INSERTED.DynamicFormDataId VALUES " +
                                "(@DynamicFormSectionGridAttributeId,@GridSortOrderByNo,@SortOrderByNo,@DynamicFormDataGridId,@DynamicFormItem,@DynamicFormId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsSendApproval,@FileProfileSessionID,@ProfileId,@ProfileNo);\n\r";
                            query += await UpdateDynamicFormSectionAttributeCount(dynamicFormData, "Add");
                            dynamicFormData.DynamicFormDataId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            insert = true;

                        }
                        if (insert == true)
                        {
                            await InsertOrUpdateFormWorkFlowSectionNoWorkFlowStart(dynamicFormData.DynamicFormId, dynamicFormData.DynamicFormDataId, dynamicFormData.AddedByUserID);
                        }
                        if (preData != null)
                        {
                            await InsertAuditTrail(dynamicFormData, preData);
                        }
                        //await CreateDynamicTable(dynamicFormData, insert);
                        return dynamicFormData;
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
        public async Task<DynamicForm> GetDynamicFormOneByIdAsync(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", Id);
                var query = "select * from DynamicForm Where ID=@DynamicFormId;";
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
        private async Task<List<object>> ExecuteQueryAsync(object sql)
        {
            List<object> result = new List<object>();
            using (var connection = CreateConnection())
            {
                using (SqlCommand command = new SqlCommand((string)sql, (SqlConnection)connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string name = string.Empty; object value = string.Empty;
                            IDictionary<string, object> objectDataList = new ExpandoObject();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                name = reader.GetName(i); value = reader.GetValue(i);
                                objectDataList[name] = value;
                            }
                            result.Add(objectDataList);
                        }
                    }
                    connection.Close();
                }

            }

            return result;
        }
        public async Task<IReadOnlyList<DynamicFormData>> GetDynamicFormDataItemList(AttributeHeaderListModel _AttributeHeader, DynamicForm _dynamicForm)
        {
            try
            {
                List<DynamicFormData> dynamicFormData = new List<DynamicFormData>();
                string query = string.Empty;
                query += "select t6.IsApproval,t6.FileProfileTypeId,t6.Name,t6.ScreenID,t1.*,t2.CodeValue as StatusCode,CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName,(case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as AddedBy,\r\nCONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) as ModifiedBy,\r\n(case when t1.LockedUserId>0 Then CONCAT(case when t5.NickName is NULL OR  t5.NickName='' then  ''\r\n ELSE  CONCAT(t5.NickName,' | ') END,t5.FirstName, (case when t5.LastName is Null OR t5.LastName='' then '' ELSE '-' END),t5.LastName) ELSE null END) as LockedUser\r\nfrom DynamicFormData t1\r\nJOIN CodeMaster t2 ON t2.CodeID=t1.StatusCodeID\r\nJOIN Employee t3 ON t3.UserID=t1.AddedByUserID\r\nJOIN Employee t4 ON t4.UserID=t1.ModifiedByUserID\r\nLEFT JOIN Employee t5 ON t5.UserID = t1.LockedUserId\r\nJOIN DynamicForm t6 ON t6.ID = t1.DynamicFormID AND (t6.IsDeleted is Null OR t6.IsDeleted=0) AND (t1.IsDeleted is Null OR t1.IsDeleted=0)\r\r";
                if (_AttributeHeader != null && _AttributeHeader.DynamicFormSectionAttribute != null && _AttributeHeader.DynamicFormSectionAttribute.Count > 0)
                {
                    _AttributeHeader.DynamicFormSectionAttribute.ForEach(s =>
                    {

                    });
                }
                using (var connection = CreateConnection())
                {

                }
                return dynamicFormData;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<DynamicFormData> CreateDynamicTable(DynamicFormData dynamicFormData, bool? insert)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var result = await GetDynamicFormOneByIdAsync(dynamicFormData.DynamicFormId);
                        if (result != null && result.ID > 0)
                        {
                            if (dynamicFormData.AddTempSectionAttributes != null && dynamicFormData.AddTempSectionAttributes.Count() > 0)
                            {
                                dynamic jsonObjs = new object();
                                if (IsValidJson(dynamicFormData.DynamicFormItem))
                                {
                                    jsonObjs = JsonConvert.DeserializeObject(dynamicFormData.DynamicFormItem);

                                }
                                string tableName = "DynamicForm_" + result.ScreenID.ToLower();
                                var parameters = new DynamicParameters();
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("tableName", tableName, DbType.String);
                                parameters.Add("DynamicFormDataID", dynamicFormData.DynamicFormDataId);
                                var query = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,NUMERIC_SCALE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME =@tableName;";
                                var res = (await connection.QueryAsync<Table_Schema>(query, parameters1)).ToList();
                                string createsql = "create table " + tableName + " (";
                                string alterSql = string.Empty;
                                createsql += "[DynamicFormDataItemID] [bigint] IDENTITY(1,1) NOT NULL,[DynamicFormDataID] [bigint] NULL,";
                                int i = 0;
                                var COLUMN_NAMEList = res.Select(s => s.COLUMN_NAME).Distinct().ToList();
                                dynamicFormData.AddTempSectionAttributes.ForEach(s =>
                                {
                                    if (!string.IsNullOrEmpty(s.DynamicAttributeName))
                                    {
                                        var Names = jsonObjs.ContainsKey(s.DynamicAttributeName);
                                        var attrName = s.DynamicAttributeName.Replace(" ", "_");
                                        if (s.DataType == typeof(long?))
                                        {
                                            createsql += "[" + attrName + "] [bigint] NULL,";
                                            long? namess = null;
                                            if (Names == true)
                                            {
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<long?>();
                                                }
                                                parameters.Add(attrName, namess);
                                            }
                                        }
                                        else if (s.DataType == typeof(DateTime?))
                                        {
                                            createsql += "[" + attrName + "] [datetime] NULL,";
                                            DateTime? namess = null;
                                            if (Names == true)
                                            {
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<DateTime?>();
                                                }
                                                parameters.Add(attrName, namess, DbType.DateTime);
                                            }
                                        }
                                        else if (s.DataType == typeof(TimeSpan?))
                                        {
                                            createsql += "[" + attrName + "] [timestamp] NULL,";
                                            TimeSpan? namess = null;
                                            if (Names == true)
                                            {
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<TimeSpan?>();
                                                }
                                                parameters.Add(attrName, namess, DbType.Time);
                                            }
                                        }
                                        else if (s.DataType == typeof(decimal?))
                                        {
                                            createsql += "[" + attrName + "] [decimal](18, 5) NULL,";
                                            decimal? namess = null;
                                            if (Names == true)
                                            {
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<decimal?>();
                                                }
                                                parameters.Add(attrName, namess, DbType.Decimal);
                                            }
                                        }
                                        else if (s.DataType == typeof(int?))
                                        {
                                            createsql += "[" + attrName + "] [int] NULL,";
                                            int? namess = null;
                                            if (Names == true)
                                            {
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<int?>();
                                                }
                                                parameters.Add(attrName, namess);
                                            }
                                        }
                                        else if (s.DataType == typeof(bool?))
                                        {
                                            createsql += "[" + attrName + "] [bit] NULL,";
                                            bool? namess = null;
                                            if (Names == true)
                                            {
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<bool?>();
                                                }
                                                parameters.Add(attrName, namess);
                                            }
                                        }
                                        else if (s.DataType == typeof(IEnumerable<long?>))
                                        {
                                            createsql += "[" + attrName + "] [nvarchar](2000) NULL,";
                                            if (Names == true)
                                            {
                                                string? namess = null;
                                                var itemDepValue = jsonObjs[s.DynamicAttributeName];
                                                var values = (JArray)itemDepValue;
                                                if (values != null)
                                                {
                                                    List<long?> listData = values.ToObject<List<long?>>();
                                                    if (listData != null && listData.Count() > 0)
                                                    {
                                                        namess = string.Join(',', listData);
                                                    }
                                                }
                                                parameters.Add(attrName, namess, DbType.String);
                                            }
                                        }
                                        else
                                        {
                                            createsql += "[" + attrName + "] [nvarchar](2000) NULL,";
                                            if (Names == true)
                                            {
                                                string? namess = null;
                                                var itemValue = jsonObjs[s.DynamicAttributeName];
                                                if (itemValue != null)
                                                {
                                                    namess = itemValue.ToObject<string>();
                                                }
                                                parameters.Add(attrName, namess, DbType.String);
                                            }
                                        }
                                        var exits = res.Where(w => w.COLUMN_NAME.ToLower() == s.DynamicAttributeName.ToLower()).FirstOrDefault();
                                        if (exits == null)
                                        {
                                            if (s.DataType == typeof(long?))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [bigint] NULL;\n";
                                            }
                                            else if (s.DataType == typeof(DateTime?))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [datetime] NULL;\n";
                                            }
                                            else if (s.DataType == typeof(TimeSpan?))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [timestamp] NULL;\n";
                                            }
                                            else if (s.DataType == typeof(decimal?))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [decimal](18, 5) NULL;\n";
                                            }
                                            else if (s.DataType == typeof(int?))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [int] NULL;\n";
                                            }
                                            else if (s.DataType == typeof(bool?))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [bit] NULL;\n";
                                            }
                                            else if (s.DataType == typeof(IEnumerable<long?>))
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [nvarchar](2000) NULL;\n";
                                            }
                                            else
                                            {
                                                alterSql += "ALTER TABLE " + tableName + " ADD [" + attrName + "] [nvarchar](2000) NULL;\n";
                                            }
                                        }
                                        i++;
                                    }
                                });
                                createsql += "CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED \r\n(\r\n\t[DynamicFormDataItemID] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY]";
                                if (i > 0)
                                {
                                    if (res.Count() > 0)
                                    {
                                        if (!string.IsNullOrEmpty(alterSql))
                                        {
                                            using (SqlCommand command = new SqlCommand((string)alterSql, (SqlConnection)connection))
                                            {
                                                connection.Open();
                                                await command.ExecuteNonQueryAsync();
                                                connection.Close();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(createsql))
                                        {
                                            using (SqlCommand command = new SqlCommand((string)createsql, (SqlConnection)connection))
                                            {
                                                connection.Open();
                                                await command.ExecuteNonQueryAsync();
                                                connection.Close();
                                            }
                                        }
                                    }
                                    await InsertOrUpdateDynamicFormDataItem(tableName, dynamicFormData, parameters, insert);
                                }
                            }
                        }
                        return dynamicFormData;
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
        public async Task<DynamicFormData> InsertOrUpdateDynamicFormDataItem(string? TableName, DynamicFormData dynamicFormData, DynamicParameters parameters, bool? insert)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameterss = new DynamicParameters();
                        parameterss.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId);
                        var querys = "select Count(*) from " + TableName + " Where DynamicFormDataId=@DynamicFormDataId;";
                        var res = await connection.ExecuteScalarAsync<int>(querys, parameterss);
                        var query = string.Empty;
                        if (res > 0)
                        {
                            if (parameters is DynamicParameters subDynamic)
                            {
                                query += "UPDATE " + TableName + " SET\r";
                                var names = string.Empty;
                                if (subDynamic.ParameterNames is not null)
                                {
                                    foreach (var keyValue in subDynamic.ParameterNames)
                                    {
                                        names += "[" + keyValue + "]=";
                                        names += "@" + keyValue + ",";
                                    }
                                }
                                query += names.TrimEnd(',') + "\rwhere DynamicFormDataId = " + dynamicFormData.DynamicFormDataId + ";";
                                await connection.ExecuteAsync(query, parameters);
                            }
                        }
                        else
                        {
                            if (parameters is DynamicParameters subDynamic)
                            {
                                query += "INSERT INTO " + TableName + "(\r";
                                var values = string.Empty;
                                var names = string.Empty;
                                if (subDynamic.ParameterNames is not null)
                                {
                                    foreach (var keyValue in subDynamic.ParameterNames)
                                    {
                                        names += "[" + keyValue + "],";
                                        values += "@" + keyValue + ",";
                                    }
                                }
                                query += names.TrimEnd(',') + ")\rOUTPUT INSERTED.DynamicFormDataItemID VALUES(" + values.TrimEnd(',') + ");";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }

                }
                return dynamicFormData;
            }
            catch (Exception exp)
            {
                throw new NotImplementedException();
            }
        }
        private async Task<DynamicFormData> InsertAuditTrail(DynamicFormData CurrentData, DynamicFormData PrevData)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {

                        Guid? SessionId = Guid.NewGuid();

                        dynamic PrevDatajsonObj = new object();
                        if (PrevData.DynamicFormItem != null && IsValidJson(PrevData.DynamicFormItem))
                        {
                            PrevDatajsonObj = JsonConvert.DeserializeObject(PrevData.DynamicFormItem);
                        }
                        dynamic CurrentDatajsonObj = new object();
                        if (CurrentData.DynamicFormItem != null && IsValidJson(CurrentData.DynamicFormItem))
                        {
                            CurrentDatajsonObj = JsonConvert.DeserializeObject(CurrentData.DynamicFormItem);
                            if (CurrentDatajsonObj != null)
                            {
                                int i = 0;
                                foreach (var item in CurrentDatajsonObj)
                                {
                                    var itemKey = (string)item.Name;
                                    string? itemValue = null;
                                    if (item.Value is JArray)
                                    {
                                        var valuesData = item.Value;
                                        List<long?> listData = valuesData.ToObject<List<long?>>();
                                        if (listData != null && listData.Count() > 0)
                                        {
                                            itemValue = string.Join(",", listData);
                                        }
                                    }
                                    else
                                    {
                                        itemValue = (string)item.Value;
                                    }
                                    var spliData = itemKey.Split("_");
                                    string? PreValueSet = null;
                                    var GetKeyName = PrevDatajsonObj?.ContainsKey(itemKey);
                                    if (GetKeyName == true)
                                    {
                                        var itemValueData = PrevDatajsonObj[itemKey];
                                        if (itemValueData is JArray)
                                        {
                                            List<long?> listDatas = itemValueData.ToObject<List<long?>>();
                                            if (listDatas != null && listDatas.Count() > 0)
                                            {
                                                PreValueSet = string.Join(",", listDatas);
                                            }
                                        }
                                        else
                                        {
                                            PreValueSet = (string)PrevDatajsonObj[itemKey];
                                        }
                                    }
                                    if (itemValue != PreValueSet)
                                    {
                                        var query = string.Empty;
                                        var parameters = new DynamicParameters();
                                        parameters.Add("DynamicFormDataId", CurrentData.DynamicFormDataId);
                                        parameters.Add("AuditUserId", CurrentData.ModifiedByUserID);
                                        parameters.Add("AuditDateTime", DateTime.Now, DbType.DateTime);
                                        parameters.Add("PreUserID", PrevData.ModifiedByUserID);
                                        parameters.Add("PreUpdateDate", PrevData.ModifiedDate, DbType.DateTime);
                                        parameters.Add("PrevData", PrevData.DynamicFormItem, DbType.String);
                                        parameters.Add("CurrentData", CurrentData.DynamicFormItem, DbType.String);
                                        parameters.Add("SessionId", SessionId, DbType.Guid);
                                        parameters.Add("CurrentValueId", itemValue, DbType.String);
                                        parameters.Add("PrevValueId", PreValueSet, DbType.String);
                                        if (i == 0)
                                        {
                                            query += "INSERT INTO DynamicFormDataAudit(DynamicFormDataId,AuditUserId,AuditDateTime,PreUserID,PreUpdateDate,PrevData,CurrentData,SessionId,CurrentValueId,PrevValueId,DynamicFormSectionAttributeId,AttributeId)  OUTPUT INSERTED.DynamicFormDataAuditId VALUES " +
                                        "(@DynamicFormDataId,@AuditUserId,@AuditDateTime,@PreUserID,@PreUpdateDate,@PrevData,@CurrentData,@SessionId,@CurrentValueId,@PrevValueId," + spliData[0] + ",'" + itemKey + "');\n\r";
                                        }
                                        else
                                        {
                                            query += "INSERT INTO DynamicFormDataAudit(DynamicFormDataId,AuditUserId,AuditDateTime,PreUserID,PreUpdateDate,SessionId,CurrentValueId,PrevValueId,DynamicFormSectionAttributeId,AttributeId)  OUTPUT INSERTED.DynamicFormDataAuditId VALUES " +
                                        "(@DynamicFormDataId,@AuditUserId,@AuditDateTime,@PreUserID,@PreUpdateDate,@SessionId,@CurrentValueId,@PrevValueId," + spliData[0] + ",'" + itemKey + "');\n\r";
                                        }
                                        if (!string.IsNullOrEmpty(query))
                                        {
                                            await connection.ExecuteAsync(query, parameters);
                                        }
                                        i++;
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
                return CurrentData;
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


                    try
                    {
                        var query = string.Empty;
                        var _dynamicFormApproval = (List<DynamicFormApproval>?)await GetDynamicFormApprovalByID(dynamicFormData.DynamicFormId, dynamicFormData.DynamicFormDataId);
                        if (_dynamicFormApproval != null)
                        {

                            _dynamicFormApproval.ForEach(s =>
                            {
                                query += "INSERT INTO DynamicFormApproved(DynamicFormDataID,ApprovedDescription,UserID,ApprovedSortBy)VALUES " +
                                "(" + dynamicFormData.DynamicFormDataId + ",'" + s.Description + "'," + s.ApprovalUserId + "," + s.SortOrderBy + ");\n\r";
                            });
                            if (!string.IsNullOrEmpty(query))
                            {
                                await connection.ExecuteAsync(query, null);
                            }
                        }

                        return dynamicFormData;
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
        public async Task<DynamicFormData> GetDynamicFormDataBySessionOneAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t2.SessionID as DynamicFormSessionID,t1.DynamicFormDataID,\r\nt1.DynamicFormID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.IsSendApproval,\r\nt1.FileProfileSessionID,\r\nt1.ProfileID,\r\nt1.ProfileNo,\r\nt1.DynamicFormDataGridID,\r\nt1.IsDeleted,\r\nt1.SortOrderByNo,\r\nt1.GridSortOrderByNo,\r\nt1.DynamicFormSectionGridAttributeID,\r\nt1.IsLocked,\r\nt1.LockedUserID,(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid from DynamicFormData t1 JOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID\r\n" +
                    "WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.SessionId=@SessionId";

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
                DynamicFormData dynamicFormData = new DynamicFormData();
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.DynamicFormDataID,\r\nt1.DynamicFormID,\r\nt1.SessionID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.DynamicFormItem,\r\nt1.IsSendApproval,\r\nt1.FileProfileSessionID,\r\nt1.ProfileID,\r\nt1.ProfileNo,\r\nt1.DynamicFormDataGridID,\r\nt1.IsDeleted,\r\nt1.SortOrderByNo,\r\nt1.GridSortOrderByNo,\r\nt1.DynamicFormSectionGridAttributeID,\r\nt1.IsLocked,\r\nt1.LockedUserID,(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid,(case when t1.AddedByUserID>0 Then CONCAT(case when t2.NickName is NULL then  t2.FirstName ELSE  t2.NickName END,' | ',t2.LastName) ELSE null END) as AddedBy,(case when t1.ModifiedByUserID>0 Then CONCAT(case when t3.NickName is NULL then  t3.FirstName ELSE  t3.NickName END,' | ',t3.LastName) ELSE null END) as ModifiedBy,t4.CodeValue as StatusCode,\r\nt5.IsApproval,t5.FileProfileTypeID,t6.Name as FileProfileTypeName,\r\n" +
                    "(SELECT COUNT(SessionId) from Documents t7 WHERE t7.SessionId=t1.SessionId AND t7.IsLatest=1 AND (t7.IsDelete IS NULL OR t7.IsDelete=0)) as isDocuments\r\n" +
                    "from DynamicFormData t1 \r\n" +
                    "JOIN Employee t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN Employee t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicForm t5 ON t5.ID=t1.DynamicFormId\r\n" +
                    "LEFT JOIN FileProfileType t6 ON t6.FileProfileTypeID=t5.FileProfileTypeID\r\nWHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.SessionId=@SessionId";
                using (var connection = CreateConnection())
                {
                    dynamicFormData = await connection.QueryFirstOrDefaultAsync<DynamicFormData>(query, parameters);
                    if (dynamicFormData != null)
                    {
                        parameters.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId);
                        var query1 = "select t1.*,(case when t1.LockedUserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName,(case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as LockedUser from DynamicFormDataSectionLock t1\r\nJOIN Employee t2 ON t2.UserID=t1.LockedUserID Where t1.DynamicFormDataId=@DynamicFormDataId;";
                        dynamicFormData.DynamicFormDataSectionLock = (await connection.QueryAsync<DynamicFormDataSectionLock>(query1, parameters)).ToList();
                        if (dynamicFormData.SessionId != null)
                        {
                            var _activityEmailTopics = await GetActivityEmailTopicList("'" + dynamicFormData.SessionId.ToString() + "'");
                            var _activityEmailTopicsOne = _activityEmailTopics.FirstOrDefault(f => f.SessionId == dynamicFormData.SessionId);
                            if (_activityEmailTopicsOne != null)
                            {
                                dynamicFormData.EmailTopicSessionId = _activityEmailTopicsOne.EmailTopicSessionId;
                                if (_activityEmailTopicsOne.EmailTopicSessionId != null)
                                {
                                    if (_activityEmailTopicsOne.IsDraft == false)
                                    {
                                        dynamicFormData.IsDraft = false;
                                    }
                                    if (_activityEmailTopicsOne.IsDraft == true)
                                    {
                                        dynamicFormData.IsDraft = true;
                                    }
                                }
                            }
                        }
                    }

                }
                return dynamicFormData;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentsModel> GetDynamicFormDataBySessionIdForDMSAsync(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select *,t2.SessionID as FileProfileTypeSessionId from Documents t1 \r\n" +
                    "LEFT JOIN FileProfileType t2 ON t2.FileProfileTypeID=t1.FilterProfileTypeID\r\n" +
                    "where (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.SessionID=@SessionId AND t1.FilterProfileTypeID>0 AND  t1.IsLatest=1 AND (t1.IsDelete IS NULL OR t1.IsDelete=0)\r\n";
                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DocumentsModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormApproved>> GetDynamicFormApprovedByAll(List<long> dynamicFormDataIds)
        {
            try
            {
                List<DynamicFormApproved> dynamicFormApproveds = new List<DynamicFormApproved>();
                if (dynamicFormDataIds != null && dynamicFormDataIds.Count > 0)
                {
                    List<DynamicFormApprovedChanged> dynamicFormApprovedChanged = new List<DynamicFormApprovedChanged>();
                    dynamicFormDataIds = dynamicFormDataIds != null && dynamicFormDataIds.Count > 0 ? dynamicFormDataIds : new List<long>() { -1 };
                    var query = "select t1.*,(case when t1.ApprovedByUserId>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END) as ApprovedByUser,t5.DynamicFormId,\r\n" +
                       "(case when t1.UserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName, (case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as ApprovalUser,\r\n" +
                       "CASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                       "FROM DynamicFormApproved t1 \r\n" +
                       "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                        "JOIN DynamicFormData t5 ON t5.DynamicFormDataId=t1.DynamicFormDataId \r\n" +
                       "LEFT JOIN Employee t4 ON t4.UserID=t1.ApprovedByUserId WHERE (t5.IsDeleted=0 or t5.IsDeleted is null)  AND t1.DynamicFormDataId in(" + string.Join(',', dynamicFormDataIds) + ") order by t1.DynamicFormApprovedId asc;\r\n";
                    query += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as UserName  from DynamicFormApprovedChanged t1 JOIN Employee t3 ON t1.UserID=t3.UserID\r\n;";
                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query, null);
                        dynamicFormApproveds = results.Read<DynamicFormApproved>().ToList();
                        dynamicFormApprovedChanged = results.Read<DynamicFormApprovedChanged>().ToList();
                    }
                    if (dynamicFormApproveds != null && dynamicFormApproveds.Count() > 0)
                    {
                        dynamicFormApproveds.ForEach(s =>
                        {
                            s.DelegateApproveAllUserName = s.ApprovalUser;
                            s.DelegateApproveAllUserId = s.UserId;
                            var list = dynamicFormApprovedChanged.Where(w => w.DynamicFormApprovedID == s.DynamicFormApprovedId).OrderByDescending(o => o.DynamicFormApprovedChangedId).ToList();
                            if (list != null && list.Count() > 0)
                            {
                                s.DynamicFormApprovedChangeds = list;
                                s.IsDelegateApproveStatus = list[0].IsApprovedStatus;
                                s.DelegateApprovedChangedId = list[0].DynamicFormApprovedChangedId;
                                s.DelegateApproveUserId = list[0].UserId;
                                s.DelegateApproveUserName = list[0].UserName;
                                s.UserId = list[0].UserId;
                                s.ApprovalUser = list[0].UserName;
                                // s.DelegateApproveAllUserName= list[0].UserName;
                            }
                        });
                    }
                }
                return dynamicFormApproveds;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ActivityEmailTopicsModel>> GetActivityEmailTopicList(string SessionIds)
        {

            try
            {
                var query = "select  * from ActivityEmailTopics where ActivityType='DynamicForm' AND SessionId in(" + SessionIds + ");";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ActivityEmailTopicsModel>(query, null)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataSectionLock>> GetDynamicFormDataSectionLockList(List<long> dynamicFormDataIds)
        {

            try
            {
                List<DynamicFormDataSectionLock> dynamicFormDataSectionLocks = new List<DynamicFormDataSectionLock>();
                dynamicFormDataIds = dynamicFormDataIds != null && dynamicFormDataIds.Count > 0 ? dynamicFormDataIds : new List<long>() { -1 };
                var query = "select  * from DynamicFormDataSectionLock where DynamicFormDataId in(" + string.Join(',', dynamicFormDataIds) + ");";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DynamicFormDataSectionLock>(query, null)).ToList();
                    dynamicFormDataSectionLocks = result != null && result.Count() > 0 ? result : new List<DynamicFormDataSectionLock>();
                }
                return dynamicFormDataSectionLocks;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormData>> GetDynamicFormDataByIdAsync(long? id, long? userId, long? DynamicFormDataGridId, long? DynamicFormSectionGridAttributeId, Guid? DynamicFormDataSessionId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", id);
                parameters.Add("DynamicFormDataGridId", DynamicFormDataGridId);
                parameters.Add("DynamicFormSectionGridAttributeId", DynamicFormSectionGridAttributeId);
                parameters.Add("DynamicFormDataSessionId", DynamicFormDataSessionId, DbType.Guid);
                var query = "select t5.IsApproval,(case when t1.LockedUserId>0 Then CONCAT(case when t33.NickName is NULL OR  t33.NickName='' then  ''\r\n ELSE  CONCAT(t33.NickName,' | ') END,t33.FirstName, (case when t33.LastName is Null OR t33.LastName='' then '' ELSE '-' END),t33.LastName) ELSE null END) as LockedUser,t1.LockedUserId,(case when t1.IsLocked is NULL then  0 ELSE t1.IsLocked END) as IsLocked,t1.DynamicFormDataID,t1.DynamicFormSectionGridAttributeID,t1.DynamicFormID,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t6.SessionID as DynamicFormSectionGridAttributeSessionId,\r\nt1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.DynamicFormItem,t1.IsSendApproval,t1.FileProfileSessionID,t1.ProfileID,t1.ProfileNo,t1.DynamicFormDataGridID,t1.IsDeleted,t1.SortOrderByNo,t1.GridSortOrderByNo,t1.DynamicFormSectionGridAttributeID," +
                    "(case when t1.AddedByUserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName, (case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as AddedBy," +
                    "(case when t1.ModifiedByUserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as ModifiedBy,t5.FileProfileTypeId,t5.Name,t5.ScreenID,\r\n" +
                    "(select COUNT(t6.DocumentID) from DynamicFormDataUpload tt1 JOIN Documents t6 ON tt1.SessionID=t6.SessionID where t1.DynamicFormDataID=tt1.DynamicFormDataID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,\r\n" +
                    "(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid\r\n" +
                    "from DynamicFormData t1\r\n" +
                    "JOIN Employee t2 ON t2.UserID = t1.AddedByUserID\r\n" +
                    "LEFT JOIN Employee t3 ON t3.UserID = t1.ModifiedByUserID\r\n" +
                     "LEFT JOIN Employee t33 ON t33.UserID = t1.LockedUserId\r\n" +
                    "JOIN DynamicForm t5 ON t5.ID = t1.DynamicFormID\r\n" +
                    "LEFT JOIN DynamicFormSectionAttribute t6 ON t6.DynamicFormSectionAttributeId = t1.DynamicFormSectionGridAttributeId\r\n" +
                    "WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.DynamicFormId =@DynamicFormId\r\n";
                if (DynamicFormDataSessionId != null)
                {
                    query += "AND t1.SessionId=@DynamicFormDataSessionId\r\n";
                }
                if (DynamicFormDataGridId == 0 || DynamicFormDataGridId > 0)
                {
                    if (DynamicFormSectionGridAttributeId > 0)
                    {
                        query += "AND t1.DynamicFormDataGridId=@DynamicFormDataGridId And t1.DynamicFormSectionGridAttributeID=@DynamicFormSectionGridAttributeId order by t1.GridSortOrderByNo asc;\r\n";
                    }
                    else
                    {


                        query += "AND t1.DynamicFormDataGridId=@DynamicFormDataGridId order by t1.GridSortOrderByNo asc;\r\n";
                    }
                }
                else
                {
                    if (DynamicFormDataGridId == -1)
                    {
                        query += "order by t1.SortOrderByNo asc;\r\n";
                    }
                    else
                    {
                        query += "AND t1.DynamicFormDataGridId is null order by t1.SortOrderByNo asc;\r\n";
                    }
                }
                var result = new List<DynamicFormData>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    var SessionIds = result.Select(a => a.SessionId).ToList();
                    var lists = string.Join(',', SessionIds.Select(i => $"'{i}'"));
                    var dynamicFormDataIDss = result.Select(a => a.DynamicFormDataId).ToList();
                    var lockItems = await GetDynamicFormDataSectionLockList(dynamicFormDataIDss);
                    var dynamicFormDataIDs = result.Where(w => w.IsSendApproval == true).Select(a => a.DynamicFormDataId).ToList();
                    var resultData = await GetDynamicFormApprovedByAll(dynamicFormDataIDs);
                    var _activityEmailTopics = await GetActivityEmailTopicList(lists);
                    result.ForEach(s =>
                    {
                        var _activityEmailTopicsOne = _activityEmailTopics.FirstOrDefault(f => f.SessionId == s.SessionId);
                        s.DynamicFormDataSectionLock = lockItems.Where(w => w.DynamicFormDataId == s.DynamicFormDataId).ToList();
                        if (_activityEmailTopicsOne != null)
                        {
                            s.EmailTopicSessionId = _activityEmailTopicsOne.EmailTopicSessionId;
                            if (_activityEmailTopicsOne.EmailTopicSessionId != null)
                            {
                                if (_activityEmailTopicsOne.IsDraft == false)
                                {
                                    s.IsDraft = false;
                                }
                                if (_activityEmailTopicsOne.IsDraft == true)
                                {
                                    s.IsDraft = true;
                                }
                            }
                        }
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
                                        s.CurrentUserId = rejected.UserId;
                                        s.CurrentUserName = rejected.ApprovalUser;
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
                                                    s.CurrentUserId = isapproved.UserId;
                                                    s.CurrentUserName = isapproved.ApprovalUser;
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
                                                s.CurrentUserId = isapprovedPending.UserId;
                                                s.CurrentUserName = isapprovedPending.ApprovalUser;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    });
                }
                if (userId > 0)
                {
                    result = result.Where(w => w.CurrentUserId == userId).ToList();
                }
                return result;

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
                                if (string.IsNullOrEmpty(s.DropDownTypeId) || s.IsDynamicFormDropTagBox != true)
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
                                        if (string.IsNullOrEmpty(s.DropDownTypeId) || s.IsDynamicFormDropTagBox != true)
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
                                    if (string.IsNullOrEmpty(s.DropDownTypeId) || s.IsDynamicFormDropTagBox != true)
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
                    var results = await connection.QueryMultipleAsync(@"select * from DynamicFormSection where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID=" + Id + " order by  SortOrderBy asc;" +
                        "select t1.*,t5.SectionName,t6.AttributeName,t7.CodeValue as ControlType,t6.IsDynamicFormDropTagBox,t6.DropDownTypeID,t6.DataSourceID,t6.DynamicFormID as DynamicFormGridDropDownID,t5.DynamicFormID,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable from\r\n" +
                        "DynamicFormSectionAttribute t1\r\n" +
                        "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                        "JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                        "LEFT JOIN AttributeHeaderDataSource t8 ON t6.DataSourceId=t8.AttributeHeaderDataSourceID\r\n" +
                        "JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\nWhere (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null)  AND t5.DynamicFormID=" + Id + " order by t1.SortOrderBy asc;");
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
        public async Task<List<DynamicFormData>> UpdateDynamicFormDataSort(long? id, long? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("SortOrderByNo", SortOrderBy);
                query = "SELECT DynamicFormDataId,DynamicFormId,SortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId AND SortOrderByNo>@SortOrderByNo order by SortOrderByNo asc";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormData>> UpdateDynamicFormDataGridSortOrderByNoSort(long? id, long? GridSortOrderByNo, long? dynamicFormDataGridId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("GridSortOrderByNo", GridSortOrderByNo);
                parameters.Add("DynamicFormDataGridId", dynamicFormDataGridId);
                query = "SELECT DynamicFormDataId,DynamicFormId,SortOrderByNo,GridSortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataGridId=@DynamicFormDataGridId AND DynamicFormId = @DynamicFormId AND GridSortOrderByNo>@GridSortOrderByNo order by GridSortOrderByNo asc";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormData>> GetForeign_Key_Table_Schema_Data(DynamicFormData dynamicFormData)
        {
            List<DynamicFormData> dynamicFormDataList = new List<DynamicFormData>();
            List<Foreign_Key_Table_Schema> Foreign_Key_Table_Schema = new List<Foreign_Key_Table_Schema>();
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                query = "SELECT\r\nKCU1.CONSTRAINT_NAME AS FK_CONSTRAINT_NAME     ,KCU1.TABLE_NAME AS FK_TABLE_NAME     ,KCU1.COLUMN_NAME AS FK_COLUMN_NAME     ,KCU2.CONSTRAINT_NAME AS REFERENCED_CONSTRAINT_NAME     ,KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME     ,KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1     ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG      AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA     AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2     ON KCU2.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG      AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA     AND KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME     AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION where KCU2.TABLE_NAME='DynamicformData';\r\n";
                using (var connection = CreateConnection())
                {
                    Foreign_Key_Table_Schema = (await connection.QueryAsync<Foreign_Key_Table_Schema>(query, parameters)).ToList();
                    var query1 = string.Empty;
                    if (Foreign_Key_Table_Schema != null && Foreign_Key_Table_Schema.Count() > 0)
                    {
                        Foreign_Key_Table_Schema.ForEach(s =>
                        {
                            if (s.FK_TABLE_NAME != "DynamicFormDataAudit")
                            {
                                query1 += " select " + s.FK_COLUMN_NAME + " as DynamicFormDataID from " + s.FK_TABLE_NAME + " where " + s.FK_COLUMN_NAME + "=" + dynamicFormData.DynamicFormDataId;
                                if (s.FK_COLUMN_NAME == "DynamicFormDataGridID" && s.FK_TABLE_NAME == "DynamicFormData")
                                {
                                    query1 += " AND (IsDeleted=0 or IsDeleted is null) ";
                                }
                                query1 += " UNION ALL";
                            }
                        });
                        if (query1.EndsWith("UNION ALL"))
                        {
                            query1 = query1.Remove(query1.Length - 9);
                        }
                        if (!string.IsNullOrEmpty(query1))
                        {
                            dynamicFormDataList = (await connection.QueryAsync<DynamicFormData>(query1, parameters)).ToList();
                        }
                    }

                }

                return dynamicFormDataList;
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


                    try
                    {
                        var res = await GetForeign_Key_Table_Schema_Data(dynamicFormData);
                        if (res != null && res.Count() > 0)
                        {
                            dynamicFormData.IsNotDelete = true;
                        }
                        else
                        {
                            var result = await UpdateDynamicFormDataSort(dynamicFormData.DynamicFormId, dynamicFormData.SortOrderByNo);
                            var parameters = new DynamicParameters();
                            parameters.Add("DynamicFormDataId", dynamicFormData.DynamicFormDataId);

                            var query = "UPDATE DynamicFormData SET IsDeleted=1 WHERE DynamicFormDataId = @DynamicFormDataId;\r\n";

                            if (dynamicFormData.SortOrderByNo > 0)
                            {
                                var sortby = dynamicFormData.SortOrderByNo;
                                if (result != null)
                                {
                                    result.ForEach(s =>
                                    {
                                        query += "Update  DynamicFormData SET SortOrderByNo=" + sortby + "  WHERE DynamicFormDataId =" + s.DynamicFormDataId + ";";
                                        sortby++;
                                    });
                                }
                            }
                            if (dynamicFormData.DynamicFormDataGridId > 0)
                            {
                                var results = await UpdateDynamicFormDataGridSortOrderByNoSort(dynamicFormData.DynamicFormId, dynamicFormData.GridSortOrderByNo, dynamicFormData.DynamicFormDataGridId);
                                if (dynamicFormData.GridSortOrderByNo > 0)
                                {
                                    var sortsby = dynamicFormData.GridSortOrderByNo;
                                    if (results != null)
                                    {
                                        results.ForEach(s =>
                                        {
                                            query += "Update  DynamicFormData SET GridSortOrderByNo=" + sortsby + "  WHERE DynamicFormDataId =" + s.DynamicFormDataId + ";";
                                            sortsby++;
                                        });
                                    }
                                }
                            }
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        }
                        return dynamicFormData;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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
                var query = "select t1.*,(case when t1.AddedByUserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName, (case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as AddedBy," +
                    "(case when t1.ModifiedByUserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as ModifiedBy," +
                    "t4.CodeValue as StatusCode," +
                    "(case when t1.ApprovalUserID>0 Then CONCAT(case when t5.NickName is NULL OR  t5.NickName='' then  ''\r\n ELSE  CONCAT(t5.NickName,' | ') END,t5.FirstName, (case when t5.LastName is Null OR t5.LastName='' then '' ELSE '-' END),t5.LastName) ELSE null END) as ApprovalUser\r\n" +
                    "from DynamicFormApproval t1 \r\n" +
                    "JOIN Employee t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN Employee t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
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
        public async Task<IReadOnlyList<ApplicationMasterParent>> GetDynamicFormApplicationMasterParentAsync(long? dynamicFormId)
        {

            try
            {
                List<ApplicationMasterParent> selectedapplicationMasterParent = new List<ApplicationMasterParent>();
                List<ApplicationMasterParent> applicationMasterParent = new List<ApplicationMasterParent>();
                List<DynamicFormSectionAttribute> dynamicFormSectionAttribute = new List<DynamicFormSectionAttribute>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select t1.ApplicationMasterIds from DynamicFormSectionAttribute t1  \r\nJOIN DynamicFormSection t2 ON t1.DynamicFormSectionID=t2.DynamicFormSectionID\r\n" +
                    "JOIN DynamicForm t3 ON t3.ID=t2.DynamicFormID \r\n" +
                    "JOIN AttributeHeader t4 ON t4.AttributeID=t1.AttributeID\r\nJOIN AttributeHeaderDataSource t5 ON t5.AttributeHeaderDataSourceID=t4.DataSourceID\n\rWHERE t3.ID=@DynamicFormId AND t1.IsVisible=1 AND (t1.IsDeleted is null OR t1.IsDeleted=0) AND t4.ControlTypeID=2702 AND \r\n" +
                    "(t3.IsDeleted is null OR t3.IsDeleted=0) AND t5.DataSourceTable='ApplicationMasterParent' AND (t2.IsDeleted is null OR t2.IsDeleted=0) AND (t3.IsDeleted is null OR t3.IsDeleted=0);";
                query += "Select * from ApplicationMasterParent where ParentID is null;\r\n";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    applicationMasterParent = results.Read<ApplicationMasterParent>().ToList();
                }
                List<long?> ids = new List<long?>();
                if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count() > 0)
                {
                    dynamicFormSectionAttribute.ForEach(s =>
                    {
                        if (!string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                ids.AddRange(applicationMasterIds);
                            }
                        }
                    });
                    ids = ids.Where(w => w > 0).Distinct().ToList();
                    selectedapplicationMasterParent = applicationMasterParent != null && applicationMasterParent.Count > 0 ? applicationMasterParent.Where(a => ids.Contains(a.ApplicationMasterParentCodeId)).ToList() : new List<ApplicationMasterParent>();
                }
                return selectedapplicationMasterParent;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationMaster>> GetDynamicFormApplicationMasterAsync(long? dynamicFormId)
        {

            try
            {
                List<ApplicationMaster> selectedapplicationMasterParent = new List<ApplicationMaster>();
                List<ApplicationMaster> applicationMasterParent = new List<ApplicationMaster>();
                List<DynamicFormSectionAttribute> dynamicFormSectionAttribute = new List<DynamicFormSectionAttribute>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select t1.ApplicationMasterIds from DynamicFormSectionAttribute t1  \r\nJOIN DynamicFormSection t2 ON t1.DynamicFormSectionID=t2.DynamicFormSectionID\r\n" +
                    "JOIN DynamicForm t3 ON t3.ID=t2.DynamicFormID \r\n" +
                    "JOIN AttributeHeader t4 ON t4.AttributeID=t1.AttributeID\r\nJOIN AttributeHeaderDataSource t5 ON t5.AttributeHeaderDataSourceID=t4.DataSourceID\n\rWHERE t3.ID=@DynamicFormId AND t1.IsVisible=1 AND (t1.IsDeleted is null OR t1.IsDeleted=0) AND t4.ControlTypeID=2702 AND \r\n" +
                    "(t3.IsDeleted is null OR t3.IsDeleted=0) AND t5.DataSourceTable='ApplicationMaster' AND (t2.IsDeleted is null OR t2.IsDeleted=0) AND (t3.IsDeleted is null OR t3.IsDeleted=0);";
                query += "Select * from ApplicationMaster;\r\n";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormSectionAttribute = results.Read<DynamicFormSectionAttribute>().ToList();
                    applicationMasterParent = results.Read<ApplicationMaster>().ToList();
                }
                List<long?> ids = new List<long?>();
                if (dynamicFormSectionAttribute != null && dynamicFormSectionAttribute.Count() > 0)
                {
                    dynamicFormSectionAttribute.ForEach(s =>
                    {
                        if (!string.IsNullOrEmpty(s.ApplicationMasterIds))
                        {
                            var applicationMasterIds = s.ApplicationMasterIds.Split(",").Select(x => (long?)Int64.Parse(x)).ToList();
                            if (applicationMasterIds.Count > 0)
                            {
                                ids.AddRange(applicationMasterIds);
                            }
                        }
                    });
                    ids = ids.Where(w => w > 0).Distinct().ToList();
                    selectedapplicationMasterParent = applicationMasterParent != null && applicationMasterParent.Count > 0 ? applicationMasterParent.Where(a => ids.Contains(a.ApplicationMasterId)).ToList() : new List<ApplicationMaster>();
                }
                return selectedapplicationMasterParent;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public int? GetDynamicFormDataApprovedSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormDataId", id);

                query = "SELECT DynamicFormApprovedID,\r\nDynamicFormApprovalID,\r\nDynamicFormDataID,\r\nIsApproved,\r\nApprovedDescription,\r\nUserID,\r\nApprovedByUserID,\r\nApprovedDate,\r\nApprovedSortBy FROM DynamicFormApproved Where DynamicFormDataId = @DynamicFormDataId order by  ApprovedSortBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormApproved>(query, parameters);
                    if (result != null)
                    {
                        SortOrderBy = result.ApprovedSortBy + 1;
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormApproved>> UpdateDynamicFormDataApprovedSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormDataId", id);
                parameters.Add("ApprovedSortBy", SortOrderBy);
                query = "SELECT DynamicFormApprovedID,\r\nDynamicFormApprovalID,\r\nDynamicFormDataID,\r\nIsApproved,\r\nApprovedDescription,\r\nUserID,\r\nApprovedByUserID,\r\nApprovedDate,\r\nApprovedSortBy FROM DynamicFormApproved Where DynamicFormDataId = @DynamicFormDataId AND ApprovedSortBy>@ApprovedSortBy";
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
        public async Task<DynamicFormApproved> DeleteDynamicFormDataApproved(DynamicFormApproved dynamicFormApproval)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var result = await UpdateDynamicFormDataApprovedSort(dynamicFormApproval.DynamicFormDataId, dynamicFormApproval.ApprovedSortBy);
                        var parameters = new DynamicParameters();
                        parameters.Add("id", dynamicFormApproval.DynamicFormApprovedId);
                        var sortby = dynamicFormApproval.ApprovedSortBy;
                        var query = "DELETE  FROM DynamicFormApproved WHERE DynamicFormApprovedId = @id;";
                        if (result != null)
                        {
                            result.ForEach(s =>
                            {
                                query += "Update  DynamicFormApproved SET ApprovedSortBy=" + sortby + "  WHERE DynamicFormApprovedId =" + s.DynamicFormApprovedId + ";";
                                sortby++;
                            });
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormApproval;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApproved> InsertOrUpdateDynamicFormDataApproved(DynamicFormApproved dynamicFormApproval)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormApprovalId", dynamicFormApproval.DynamicFormApprovalId);
                        parameters.Add("UserId", dynamicFormApproval.DelegateApproveAllUserId);
                        parameters.Add("DynamicFormDataId", dynamicFormApproval.DynamicFormDataId);
                        parameters.Add("DynamicFormApprovedId", dynamicFormApproval.DynamicFormApprovedId);
                        parameters.Add("ApprovedDescription", dynamicFormApproval.ApprovedDescription, DbType.String);
                        if (dynamicFormApproval.DynamicFormApprovedId > 0)
                        {
                            var query = "UPDATE DynamicFormApproved SET DynamicFormApprovalId = @DynamicFormApprovalId,DynamicFormDataId =@DynamicFormDataId,\n\r" +
                                "UserId=@UserId,ApprovedDescription=@ApprovedDescription\n\r" +
                                "WHERE DynamicFormApprovedId = @DynamicFormApprovedId;\n\r";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            parameters.Add("ApprovedSortBy", GetDynamicFormDataApprovedSort(dynamicFormApproval.DynamicFormDataId));
                            var query = "INSERT INTO DynamicFormApproved(DynamicFormApprovalId,DynamicFormDataId,UserId,ApprovedDescription,ApprovedSortBy)  OUTPUT INSERTED.DynamicFormApprovedId VALUES " +
                                "(@DynamicFormApprovalId,@DynamicFormDataId,@UserId,@ApprovedDescription,@ApprovedSortBy);\n\r";
                            dynamicFormApproval.DynamicFormApprovedId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }

                        return dynamicFormApproval;
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
        public async Task<DynamicFormApproval> InsertOrUpdateDynamicFormApproval(DynamicFormApproval dynamicFormApproval)
        {
            try
            {
                using (var connection = CreateConnection())
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
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            parameters.Add("SortOrderBy", GetDynamicFormApprovalSort(dynamicFormApproval.DynamicFormId));
                            var query = "INSERT INTO DynamicFormApproval(ApprovalUserId,DynamicFormId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,SortOrderBy,IsApproved,Description)  OUTPUT INSERTED.DynamicFormApprovalId VALUES " +
                                "(@ApprovalUserId,@DynamicFormId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@SortOrderBy,@IsApproved,@Description);\n\r";
                            dynamicFormApproval.DynamicFormApprovalId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);


                        }

                        return dynamicFormApproval;
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
        public int? GetDynamicFormApprovalSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);

                query = "SELECT DynamicFormApprovalID,\r\nDynamicFormID,\r\nApprovalUserID,\r\nSortOrderBy,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nIsApproved,\r\nDescription,\r\nApprovedCountUsed FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId order by  SortOrderBy desc";
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
                query = "SELECT DynamicFormApprovalID,\r\nDynamicFormID,\r\nApprovalUserID,\r\nSortOrderBy,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nIsApproved,\r\nDescription,\r\nApprovedCountUsed FROM DynamicFormApproval Where DynamicFormId = @DynamicFormId AND SortOrderBy>@SortOrderBy";
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
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormApproval;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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

                    query = "SELECT DynamicFormApprovalID,\r\nDynamicFormID,\r\nApprovalUserID,\r\nSortOrderBy,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nIsApproved,\r\nDescription,\r\nApprovedCountUsed FROM DynamicFormApproval Where ApprovalUserId=@ApprovalUserId AND DynamicFormId=@DynamicFormId AND DynamicFormApprovalId != @DynamicFormApprovalId";
                }
                else
                {
                    query = "SELECT DynamicFormApprovalID,\r\nDynamicFormID,\r\nApprovalUserID,\r\nSortOrderBy,\r\nStatusCodeID,\r\nAddedByUserID,\r\nAddedDate,\r\nModifiedByUserID,\r\nModifiedDate,\r\nIsApproved,\r\nDescription,\r\nApprovedCountUsed FROM DynamicFormApproval Where ApprovalUserId=@ApprovalUserId AND DynamicFormId = @DynamicFormId";
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
        public DynamicFormWorkFlow GetDynamicFormWorkFlowSequenceNoExitsCheckValidation(long? dynamicFormId, long? dynamicFormWorkFlowId, int? SequenceNo)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("DynamicFormWorkFlowId", dynamicFormWorkFlowId);
                parameters.Add("SequenceNo", SequenceNo);
                query = "select DynamicFormId,DynamicFormWorkFlowId,SequenceNo,UserID from DynamicFormWorkFlow where SequenceNo=@SequenceNo AND DynamicFormId=@DynamicFormId\n\r";
                if (dynamicFormWorkFlowId > 0)
                {
                    query += "\n\rAND dynamicFormWorkFlowId!=@dynamicFormWorkFlowId";
                }
                else
                {
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DynamicFormWorkFlow>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public DynamicFormWorkFlowForm GetDynamicFormDataWorkFlowSequenceNoExitsCheckValidation(long? dynamicFormId, long? dynamicFormWorkFlowId, int? SequenceNo)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormDataId", dynamicFormId);
                parameters.Add("DynamicFormWorkFlowFormId", dynamicFormWorkFlowId);
                parameters.Add("SequenceNo", SequenceNo);
                query = "select DynamicFormDataId,DynamicFormWorkFlowFormId,SequenceNo,UserID from DynamicFormWorkFlowForm where SequenceNo=@SequenceNo AND DynamicFormDataId=@DynamicFormDataId\n\r";
                if (dynamicFormWorkFlowId > 0)
                {
                    query += "\n\rAND DynamicFormWorkFlowFormId!=@DynamicFormWorkFlowFormId";
                }
                else
                {
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DynamicFormWorkFlowForm>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public DynamicFormSectionAttribute GetDynamicFormSectionAttributeCheckValidation(long? dynamicFormId, long? dynamicFormSectionAttributeId, long? attributeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("DynamicFormSectionAttributeID", dynamicFormSectionAttributeId);
                parameters.Add("AttributeId", attributeId);
                query = "select t1.*,t9.Name DynamicGridName,t6.IsFilterDataSource,t6.FilterDataSocurceID,(case when t1.IsDisplayTableHeader is NULL then  0 ELSE t1.IsDisplayTableHeader END) as IsDisplayTableHeader,(case when t1.IsDisplayDropDownHeader is NULL then  0 ELSE t1.IsDisplayDropDownHeader END) as IsDisplayDropDownHeader,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible,t5.SectionName,t6.ControlTypeId,t6.IsDynamicFormDropTagBox,t6.DropDownTypeId,t6.DataSourceId,t6.AttributeName,t7.CodeValue as ControlType \r\nfrom DynamicFormSectionAttribute t1 \r\nJOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\nLEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\nLEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\nLEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\nWhere (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null) \r\nAND t5.DynamicFormId=@DynamicFormId AND t6.ControlTypeID=2712 AND t6.AttributeID=@AttributeId";
                if (dynamicFormSectionAttributeId > 0)
                {


                    query += "\n\rAND t1.DynamicFormSectionAttributeID!=@DynamicFormSectionAttributeID";
                }
                else
                {
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DynamicFormSectionAttribute>(query, parameters);
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

                            var rowsAffected = await connection.ExecuteAsync(query, null);
                        }

                        return dynamicFormApproval;
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

                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormApprovalId", value.DynamicFormApprovalId);
                        parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                        parameters.Add("Description", value.Description, DbType.String);
                        var query = "Update DynamicFormApproval SET Description=@Description,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                            "DynamicFormApprovalId=@DynamicFormApprovalId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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
        public async Task<IReadOnlyList<LeveMasterUsersModel>> GetLeveMasterUsersList(IEnumerable<long?> SelectLevelMasterIDs)
        {
            try
            {
                var LevelIds = SelectLevelMasterIDs != null && SelectLevelMasterIDs.Count() > 0 ? SelectLevelMasterIDs : new List<long?>() { -1 };
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
                                            var counts = userExitsRoles.Where(w => w.UserId == item).FirstOrDefault();
                                            if (counts == null)
                                            {
                                                query += "INSERT INTO [DynamicFormSectionSecurity](IsVisible,IsReadOnly,IsReadWrite,DynamicFormSectionId,UserId) OUTPUT INSERTED.DynamicFormSectionSecurityId " +
                                                    "VALUES (@IsVisible,@IsReadOnly,@IsReadWrite,@DynamicFormSectionId," + item + ");\r\n";

                                            }
                                            else
                                            {
                                                query += " UPDATE DynamicFormSectionSecurity SET IsVisible=@IsVisible,IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite WHERE DynamicFormSectionSecurityId='" + counts.DynamicFormSectionSecurityId + "' AND DynamicFormSectionId = @DynamicFormSectionId;\r\n";
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
                                                var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                                if (counts == null)
                                                {
                                                    query += "INSERT INTO [DynamicFormSectionSecurity](IsVisible,IsReadOnly,IsReadWrite,DynamicFormSectionId,UserId,UserGroupId) OUTPUT INSERTED.DynamicFormSectionSecurityId " +
                                                        "VALUES (@IsVisible,@IsReadOnly,@IsReadWrite,@DynamicFormSectionId," + s.UserId + "," + s.UserGroupId + ");\r\n";
                                                }
                                                else
                                                {
                                                    query += " UPDATE DynamicFormSectionSecurity SET IsVisible=@IsVisible,IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite WHERE DynamicFormSectionSecurityId='" + counts.DynamicFormSectionSecurityId + "' AND DynamicFormSectionId = @DynamicFormSectionId;\r\n";
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
                                            var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                            if (counts == null)
                                            {
                                                query += "INSERT INTO [DynamicFormSectionSecurity](IsVisible,IsReadOnly,IsReadWrite,DynamicFormSectionId,UserId,LevelId) OUTPUT INSERTED.DynamicFormSectionSecurityId " +
                                                   "VALUES (@IsVisible,@IsReadOnly,@IsReadWrite,@DynamicFormSectionId," + s.UserId + "," + s.LevelId + ");\r\n";
                                            }
                                            else
                                            {
                                                query += " UPDATE DynamicFormSectionSecurity SET IsVisible=@IsVisible,IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite WHERE DynamicFormSectionSecurityId='" + counts.DynamicFormSectionSecurityId + "' AND DynamicFormSectionId = @DynamicFormSectionId;\r\n";
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
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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
                var query = "select t1.DynamicFormSectionSecurityID,\r\nt1.DynamicFormSectionID,\r\nt1.UserID,\r\nt1.UserGroupID,\r\nt1.LevelID,\r\nt1.IsReadWrite,\r\nt1.IsReadOnly,\r\nt1.IsVisible,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.SectionName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL OR  t6.NickName='' then  ''\r\n ELSE  CONCAT(t6.NickName,' | ') END,t6.FirstName, (case when t6.LastName is Null OR t6.LastName='' then '' ELSE '-' END),t6.LastName) as FullName\r\n" +
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

                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        await DeleteCheckDynamicFormSectionSecurity(Id);
                        return Id.GetValueOrDefault(0);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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
                                await connections.QuerySingleOrDefaultAsync<long>(query, parameters);
                            }
                        }
                        return Id.GetValueOrDefault(0);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
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



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormApprovedId", dynamicForm.DynamicFormApprovedId);
                        parameters.Add("DynamicFormApprovalId", dynamicForm.DynamicFormApprovalId);
                        parameters.Add("DynamicFormDataId", dynamicForm.DynamicFormDataId);
                        parameters.Add("IsApproved", dynamicForm.IsApproved);
                        parameters.Add("ApprovedDescription", dynamicForm.ApprovedDescription, DbType.String);
                        var query = "INSERT INTO DynamicFormApproved(DynamicFormApprovalId,IsApproved,ApprovedDescription,DynamicFormDataId)  " +
                              "OUTPUT INSERTED.DynamicFormApprovedId VALUES " +
                             "(@DynamicFormApprovalId,@IsApproved,@ApprovedDescription,@DynamicFormDataId);\n\r";
                        query += "update DynamicFormApproval set ApprovedCountUsed += 1 where DynamicFormApprovalId =" + dynamicForm.DynamicFormApprovalId + ";\n\r";
                        dynamicForm.DynamicFormApprovedId = await connection.ExecuteAsync(query, parameters);


                        return dynamicForm;
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
        public async Task<IReadOnlyList<DynamicFormApproved>> GetDynamicFormApprovedList(long? DynamicFormDataId)
        {

            try
            {
                List<DynamicFormApproved> dynamicFormApprovedList = new List<DynamicFormApproved>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataId", DynamicFormDataId);
                var query = "select t1.*,t3.Value as EmployeeStatus,(case when t1.ApprovedByUserId>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END) as ApprovedByUser,\r\n" +
                    "(case when t1.UserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName, (case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as ApprovalUser,\r\nCASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                    "FROM DynamicFormApproved t1 \r\n" +
                    "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                    "LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t2.AcceptanceStatus\r\n" +
                     "LEFT JOIN Employee t4 ON t4.UserID=t1.ApprovedByUserId\r\n" +
                    "Where t1.DynamicFormDataID=@DynamicFormDataId \r\norder by t1.DynamicFormApprovedID asc;";
                query += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as UserName  from DynamicFormApprovedChanged t1  JOIN Employee t3 ON t1.UserID=t3.UserID\r\n;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormApprovedList = results.Read<DynamicFormApproved>().ToList();
                    var dynamicFormApprovedChanged = results.Read<DynamicFormApprovedChanged>().ToList();
                    if (dynamicFormApprovedList != null && dynamicFormApprovedList.Count > 0)
                    {
                        dynamicFormApprovedList.ForEach(s =>
                        {
                            s.DelegateApproveAllUserName = s.ApprovalUser;
                            s.DelegateApproveAllUserId = s.UserId;
                            var list = dynamicFormApprovedChanged.Where(w => w.DynamicFormApprovedID == s.DynamicFormApprovedId).OrderByDescending(o => o.DynamicFormApprovedChangedId).ToList();
                            if (list != null && list.Count() > 0)
                            {
                                s.DynamicFormApprovedChangeds = list;
                                s.IsDelegateApproveStatus = list[0].IsApprovedStatus;
                                s.DelegateApprovedChangedId = list[0].DynamicFormApprovedChangedId;
                                s.DelegateApproveUserId = list[0].UserId;
                                s.DelegateApproveUserName = list[0].UserName;
                                s.UserId = list[0].UserId;
                                s.ApprovalUser = list[0].UserName;
                                // s.DelegateApproveAllUserName = list[0].UserName;
                            }
                        });
                    }
                }
                return dynamicFormApprovedList;
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
                            await connections.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return dynamicFormApproved;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataUpload> GetDynamicFormDataUploadCheckValidation(long? dynamicFormDataId, long? dynamicFormSectionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                parameters.Add("DynamicFormSectionId", dynamicFormSectionId);
                query = "select t1.* from DynamicFormDataUpload t1 where  t1.DynamicFormDataId=@DynamicFormDataId\r";
                if (dynamicFormSectionId == null)
                {
                    query += "AND t1.DynamicFormSectionId is null;";
                }
                if (dynamicFormSectionId > 0)
                {
                    query += "AND t1.DynamicFormSectionId=@DynamicFormSectionId;";
                }
                using (var connection = CreateConnection())
                {
                    return await connection.QuerySingleOrDefaultAsync<DynamicFormDataUpload>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataUpload> InsertDynamicFormDataUpload(DynamicFormDataUpload dynamicFormSection)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var exits = await GetDynamicFormDataUploadCheckValidation(dynamicFormSection.DynamicFormDataId, dynamicFormSection.DynamicFormSectionId);
                        if (exits == null)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DynamicFormDataUploadId", dynamicFormSection.DynamicFormDataUploadId);
                            parameters.Add("DynamicFormDataId", dynamicFormSection.DynamicFormDataId);
                            parameters.Add("DynamicFormSectionId", dynamicFormSection.DynamicFormSectionId);
                            parameters.Add("LinkFileProfileTypeDocumentId", dynamicFormSection.LinkFileProfileTypeDocumentId);

                            if (dynamicFormSection.UploadType == "DMS")
                            {
                                parameters.Add("SessionId", dynamicFormSection.DocumentsModel?.SessionID, DbType.Guid);
                                parameters.Add("IsDmsLink", 1);
                            }
                            else
                            {
                                parameters.Add("SessionId", dynamicFormSection.SessionId, DbType.Guid);
                                parameters.Add("IsDmsLink", null);
                            }
                            parameters.Add("AddedByUserID", dynamicFormSection.AddedByUserId);
                            parameters.Add("ModifiedByUserID", dynamicFormSection.ModifiedByUserId);
                            parameters.Add("AddedDate", dynamicFormSection.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicFormSection.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicFormSection.StatusCodeId);

                            var query = "INSERT INTO DynamicFormDataUpload(LinkFileProfileTypeDocumentId,IsDmsLink,DynamicFormDataId,DynamicFormSectionId,SessionId,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID) VALUES " +
                         "(@LinkFileProfileTypeDocumentId,@IsDmsLink,@DynamicFormDataId,@DynamicFormSectionId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";

                            dynamicFormSection.DynamicFormDataUploadId = await connection.ExecuteAsync(query, parameters);
                        }
                        else
                        {
                            dynamicFormSection = exits;
                        }

                        return dynamicFormSection;
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

        public async Task<IReadOnlyList<DynamicFormWorkFlow>> GetDynamicFormWorkFlowEmptyAsync(long? dynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select  * from DynamicFormWorkFlow where  DynamicFormId=@dynamicFormId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlow>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSection>> GetDynamicFormSectionLists(long? dynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select tt1.DynamicFormWorkFlowId,tt2.* from DynamicFormWorkFlowSection tt1 JOIN \r\nDynamicFormSection tt2 ON tt2.DynamicFormSectionID=tt1.DynamicFormSectionID  where tt2.DynamicFormId=@dynamicFormId";

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

        public async Task<IReadOnlyList<DynamicFormWorkFlow>> GetDynamicFormWorkFlowAsync(long? dynamicFormId)
        {
            try
            {
                var listData = await GetDynamicFormSectionLists(dynamicFormId);
                List<DynamicFormWorkFlow> dynamicFormWorkFlows = new List<DynamicFormWorkFlow>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                var query = "select (select Count(tt1.DynamicFormWorkFlowApprovalID) from DynamicFormWorkFlowApproval tt1 where tt1.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID ) as DynamicFormWorkFlowApprovalCount,t1.*, \r\nt3.Name as UserGroup, \r\nt3.Description as UserGroupDescription, \r\nt4.Name as DynamicFormName,  \r\nt5.Name as LevelName, t6.NickName, t6.FirstName, t6.LastName, t7.Name as DepartmentName,  \r\nt8.Name as DesignationName,  \r\n" +
                    "CONCAT(case when t6.NickName is NULL OR  t6.NickName='' then  ''\r\n ELSE  CONCAT(t6.NickName,' | ') END,t6.FirstName, (case when t6.LastName is Null OR t6.LastName='' then '' ELSE '-' END),t6.LastName) as FullName  \r\nfrom DynamicFormWorkFlow t1  \r\nLEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID  \r\nLEFT JOIN DynamicForm t4 ON t4.ID=t1.DynamicFormId  \r\nLEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID  \r\nJOIN Employee t6 ON t1.UserID=t6.UserID  \r\nLEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID  \r\nLEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID   where  t1.DynamicFormId=@dynamicFormId";
                var result = new List<DynamicFormWorkFlow>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormWorkFlow>(query, parameters)).ToList();
                }
                if (result != null && result.Count() > 0)
                {
                    result.ForEach(s =>
                    {
                        if (listData != null && listData.Count() > 0)
                        {
                            var listDatas = listData.Where(w => w.DynamicFormWorkFlowId != s.DynamicFormWorkFlowId).Select(x => x.DynamicFormSectionId).Distinct().ToList();
                            var lists = listData.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).ToList();
                            s.DynamicFormSectionIDs = listDatas != null && listDatas.Count() > 0 ? listDatas : new List<long>();
                            s.DynamicFormSectionAllIDs = listData != null && listData.Count() > 0 ? lists.Select(x => x.DynamicFormSectionId).ToList() : new List<long>();
                            s.SelectDynamicFormSectionIDs = new List<long>();
                            if (lists != null && lists.Count() > 0)
                            {
                                s.SelectDynamicFormSectionIDs = lists.Select(a => a.DynamicFormSectionId).ToList();
                                s.SectionName = string.Join(',', lists.Select(z => z.SectionName).ToList());
                            }
                        }
                        if (s.DynamicFormWorkFlowApprovalCount > 0)
                        {
                            s.IsDynamicFormWorkFlowApproval = true;
                        }
                        dynamicFormWorkFlows.Add(s);
                    });
                }
                return dynamicFormWorkFlows;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlow> InsertDynamicFormWorkFlow(DynamicFormWorkFlow value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    // var userExitsRoles = await GetDynamicFormWorkFlowEmptyAsync(value.DynamicFormId);
                    //var userGroupUsers = await GetUserGroupUserList();
                    //var LevelUsers = await GetLeveMasterUsersList(value.SelectLevelMasterIDs);

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormWorkFlowId", value.DynamicFormWorkFlowId);
                        parameters.Add("DynamicFormId", value.DynamicFormId);
                        parameters.Add("Type", "User");
                        parameters.Add("SequenceNo", value.SequenceNo);
                        parameters.Add("UserId", value.UserId);
                        parameters.Add("IsAllowDelegateUser", value.IsAllowDelegateUser == true ? true : null);
                        parameters.Add("IsParallelWorkflow", value.IsParallelWorkflow == true ? true : null);
                        if (value.DynamicFormWorkFlowId > 0)
                        {
                            query = "UPDATE DynamicFormWorkFlow SET IsParallelWorkflow=@IsParallelWorkflow,IsAllowDelegateUser=@IsAllowDelegateUser,DynamicFormId=@DynamicFormId,UserId=@UserId,SequenceNo=@SequenceNo,Type=@Type WHERE DynamicFormWorkFlowId = @DynamicFormWorkFlowId;\r\n";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        else
                        {
                            query = "INSERT INTO [DynamicFormWorkFlow](DynamicFormId,UserId,Type,SequenceNo,IsAllowDelegateUser,IsParallelWorkflow) OUTPUT INSERTED.DynamicFormWorkFlowId " +
                                               "VALUES (@DynamicFormId,@UserId,@Type,@SequenceNo,@IsAllowDelegateUser,@IsParallelWorkflow);\r\n";
                            value.DynamicFormWorkFlowId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        if (value.DynamicFormWorkFlowId > 0)
                        {
                            var querys = string.Empty;
                            querys += "DELETE  FROM DynamicFormWorkFlowSection WHERE DynamicFormWorkFlowId =" + value.DynamicFormWorkFlowId + ";";
                            if (value.SelectDynamicFormSectionIDs != null && value.SelectDynamicFormSectionIDs.Count() > 0)
                            {
                                foreach (var items in value.SelectDynamicFormSectionIDs)
                                {
                                    querys += "INSERT INTO [DynamicFormWorkFlowSection](DynamicFormWorkFlowId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionID " +
                               "VALUES (" + value.DynamicFormWorkFlowId + "," + items + ");\r\n";
                                }
                                await connection.QuerySingleOrDefaultAsync<long>(querys);
                            }
                        }
                        return value;

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlow> DeleteDynamicFormWorkFlow(DynamicFormWorkFlow dynamicFormWorkFlow)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("id", dynamicFormWorkFlow.DynamicFormWorkFlowId);
                        var query = "DELETE  FROM DynamicFormWorkFlowSection WHERE DynamicFormWorkFlowId = @id;";
                        query += "DELETE  FROM DynamicFormWorkFlowApproval WHERE DynamicFormWorkFlowId = @id;";
                        query += "DELETE  FROM DynamicFormWorkFlow WHERE DynamicFormWorkFlowId = @id;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormWorkFlow;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataWrokFlow>> GetDynamicFormWorkFlowListByUser(long? userId, long? dynamicFormDataId)
        {
            List<DynamicFormDataWrokFlow> dynamicFormWorkFlowSections = new List<DynamicFormDataWrokFlow>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId);
                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                var query = "select (case when t1.LockedUserId>0 Then CONCAT(case when t33.NickName is NULL OR  t33.NickName='' then  ''\r\n ELSE  CONCAT(t33.NickName,' | ') END,t33.FirstName, (case when t33.LastName is Null OR t33.LastName='' then '' ELSE '-' END),t33.LastName) ELSE null END) as LockedUser,t1.LockedUserId,(case when t1.IsLocked is NULL then  0 ELSE t1.IsLocked END) as IsLocked,t1.DynamicFormID,t2.FileProfileTypeId," +
                    "(select COUNT(t6.DocumentID) from DynamicFormDataUpload tt1 JOIN Documents t6 ON tt1.SessionID=t6.SessionID where t1.DynamicFormDataID=tt1.DynamicFormDataID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,\r\n" +
                    "(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid,t1.DynamicFormDataID,t1.SessionID,t1.ProfileID,t1.ProfileNo,t2.SessionID as DynamicFormSessionID,t1.DynamicFormDataGridId,t2.Name,t2.ScreenID,t1.SortOrderByNo from DynamicFormData t1\r\n" +
                    "LEFT JOIN Employee t33 ON t33.UserID = t1.LockedUserId JOIN DynamicForm t2 ON t2.ID=t1.DynamicFormID WHERE (t2.IsDeleted =0 OR t2.IsDeleted is null) AND (t1.IsDeleted =0 OR t1.IsDeleted is null)\n\r";
                query += "\n\rAND t1.DynamicFormDataID in(select a1.DynamicFormDataID from DynamicFormWorkFlowForm a1 where a1.UserID=@UserId UNION select a2.DynamicFormDataID from DynamicFormWorkFlowFormDelegate a11 JOIN DynamicFormWorkFlowForm a2 ON a11.DynamicFormWorkFlowFormID=a2.DynamicFormWorkFlowFormID where a11.UserID=@UserId)\r\n";
                if (dynamicFormDataId > 0)
                {
                    query += "AND t1.DynamicFormDataID=@DynamicFormDataId";
                }
                var result = new List<DynamicFormDataWrokFlow>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormDataWrokFlow>(query, parameters)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    List<long> DynamicFormDataIds = result.Select(a => a.DynamicFormDataId).Distinct().ToList();
                    var SessionIds = result.Select(a => a.SessionID).Distinct().ToList();
                    var lists = string.Join(',', SessionIds.Select(i => $"'{i}'"));
                    var _activityEmailTopics = await GetActivityEmailTopicList(lists);
                    var lockItems = await GetDynamicFormDataSectionLockList(DynamicFormDataIds);
                    var resultsData = await GetDynamicFormWorkFlowIds(DynamicFormDataIds);
                    foreach (var item in result)
                    {
                        item.DynamicFormDataSectionLock = lockItems.Where(w => w.DynamicFormDataId == item.DynamicFormDataId).ToList();
                        var results = resultsData.Where(w => w.DynamicFormDataId == item.DynamicFormDataId).OrderBy(o => o.SequenceNo).ToList();
                        var _activityEmailTopicsOne = _activityEmailTopics.FirstOrDefault(f => f.SessionId == item.SessionID);
                        if (results != null && results.Count > 0)
                        {
                            if (_activityEmailTopicsOne != null)
                            {
                                item.EmailTopicSessionId = _activityEmailTopicsOne.EmailTopicSessionId;
                                if (_activityEmailTopicsOne.EmailTopicSessionId != null)
                                {
                                    if (_activityEmailTopicsOne.IsDraft == false)
                                    {
                                        item.IsDraft = false;
                                    }
                                    if (_activityEmailTopicsOne.IsDraft == true)
                                    {
                                        item.IsDraft = true;
                                    }
                                }
                            }
                            var total = results.Count;
                            var notCompleted = results.Where(w => w.IsWorkFlowDone == 0).OrderBy(x => x.SequenceNo).ToList();
                            var CompletedCount = results.Where(w => w.IsWorkFlowDone > 0).OrderBy(x => x.SequenceNo).Count();
                            var notCompletedCount = notCompleted.Count();
                            var _sequenceNoList = results.Where(w => w.IsWorkFlowDone == 0).Select(q => q.SequenceNo).Distinct().OrderBy(x => x).ToList();
                            if (_sequenceNoList.Count() > 0)
                            {
                                var IsParallelWorkflows = results.Where(w => w.IsParallelWorkflow == true).Count();
                                foreach (var itemss in _sequenceNoList)
                                {
                                    var notdata = notCompleted.Where(w => w.SequenceNo == itemss).ToList();
                                    if (notdata != null && notdata.Count > 0)
                                    {
                                        var dataAdd = notdata.FirstOrDefault(w => w.UserId == userId);
                                        if (IsParallelWorkflows > 0)
                                        {
                                            if (dataAdd != null)
                                            {
                                                item.StatusName = "Pending";
                                                item.SectionName = string.Join(",", notdata.Select(s => s.SectionName).Distinct().ToList());
                                                item.DynamicFormSectionIds = notdata.Select(s => s.DynamicFormSectionId).Distinct().ToList();
                                                item.UserIds = string.Join(",", notdata.Select(s => s.UserId).Distinct().ToList());
                                                item.UserNames = string.Join(",", notdata.Select(s => s.UserName).Distinct().ToList());
                                                item.DynamicFormWorkFlowSections = notdata;
                                                dataAdd.DynamicFormDataId = item.DynamicFormDataId;
                                                dynamicFormWorkFlowSections.Add(item);
                                            }
                                        }
                                        else
                                        {
                                            if (dataAdd != null)
                                            {
                                                item.StatusName = "Pending";
                                                item.SectionName = string.Join(",", notdata.Select(s => s.SectionName).Distinct().ToList());
                                                item.DynamicFormSectionIds = notdata.Select(s => s.DynamicFormSectionId).Distinct().ToList();
                                                item.UserIds = string.Join(",", notdata.Select(s => s.UserId).Distinct().ToList());
                                                item.UserNames = string.Join(",", notdata.Select(s => s.UserName).Distinct().ToList());
                                                item.DynamicFormWorkFlowSections = notdata;
                                                dataAdd.DynamicFormDataId = item.DynamicFormDataId;
                                                dynamicFormWorkFlowSections.Add(item);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            if (total == CompletedCount)
                            {
                                item.StatusName = "Completed";
                                dynamicFormWorkFlowSections.Add(item);
                            }
                        }
                    }
                }
                return dynamicFormWorkFlowSections;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DynamicFormWorkFlowSection>> GetDynamicFormWorkFlowIds(List<long> dynamicFormDataIds)
        {
            try
            {
                List<DynamicFormWorkFlowSection> dynamicFormWorkFlowSections = new List<DynamicFormWorkFlowSection>();
                List<DynamicFormWorkFlowFormDelegate> dynamicFormWorkFlowFormDelegates = new List<DynamicFormWorkFlowFormDelegate>();
                if (dynamicFormDataIds.Count() > 0)
                {
                    var parameters = new DynamicParameters();
                    var query = "select t2.IsParallelWorkflow,t2.DynamicFormDataID,t1.DynamicFormWorkFlowFormID as DynamicFormWorkFlowID,t1.DynamicFormSectionId,t3.SectionName,(case when t2.UserID>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END) as UserName,t1.DynamicFormWorkFlowSectionFormID as DynamicFormWorkFlowSectionID,t2.SequenceNo,t2.UserID,(case when t2.FlowStatusID>0 THEN t2.FlowStatusID ELSE 0 END) as IsWorkFlowDone\r\n" +
                        "from DynamicFormWorkFlowSectionForm t1 \r\nJOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID  \r\n" +
                        "JOIN DynamicFormWorkFlowForm t2 ON t2.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID \r\n" +
                        "JOIN Employee t4 ON t4.UserID=t2.UserID  \r\n" +
                        "Where  t2.DynamicFormDataID in(" + string.Join(',', dynamicFormDataIds) + ") order by t2.SequenceNo asc";
                    using (var connection = CreateConnection())
                    {
                        var results = await connection.QueryMultipleAsync(query, parameters);
                        dynamicFormWorkFlowSections = results.Read<DynamicFormWorkFlowSection>().ToList();
                        var DynamicFormWorkFlowFormIDs = dynamicFormWorkFlowSections.Where(w => w.DynamicFormWorkFlowId > 0).Select(s => s.DynamicFormWorkFlowId).Distinct().ToList();
                        if (DynamicFormWorkFlowFormIDs.Count() > 0)
                        {
                            var query1 = string.Empty;
                            query1 += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t2.NickName is NULL OR  t2.NickName='' then  ''\r\n ELSE  CONCAT(t2.NickName,' | ') END,t2.FirstName, (case when t2.LastName is Null OR t2.LastName='' then '' ELSE '-' END),t2.LastName) ELSE null END) as UserName  from DynamicFormWorkFlowFormDelegate t1 JOIN Employee t2 ON t1.UserID=t2.UserID where t1.DynamicFormWorkFlowFormID in(" + string.Join(',', DynamicFormWorkFlowFormIDs) + ");";
                            dynamicFormWorkFlowFormDelegates = (await connection.QueryAsync<DynamicFormWorkFlowFormDelegate>(query1)).ToList();
                        }
                    }
                    if (dynamicFormWorkFlowSections != null && dynamicFormWorkFlowSections.Count() > 0)
                    {
                        dynamicFormWorkFlowSections.ForEach(s =>
                        {
                            s.ActualUserId = s.UserId;
                            s.ActualUserName = s.UserName;
                            var dynamicFormWorkFlowFormDelegateData = dynamicFormWorkFlowFormDelegates.OrderByDescending(o => o.DynamicFormWorkFlowFormDelegateID).Where(w => w.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowId).FirstOrDefault();
                            if (dynamicFormWorkFlowFormDelegateData != null)
                            {
                                s.UserName = dynamicFormWorkFlowFormDelegateData.UserName;
                                s.DelegateSectionUserId = dynamicFormWorkFlowFormDelegateData.UserID;
                                s.DelegateSectionUserName = dynamicFormWorkFlowFormDelegateData.UserName;
                                s.UserId = dynamicFormWorkFlowFormDelegateData.UserID;
                            }
                        });
                    }
                }
                return dynamicFormWorkFlowSections != null ? dynamicFormWorkFlowSections : new List<DynamicFormWorkFlowSection>();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowApprovedForm>> GetDynamicFormWorkFlowApprovedFormByList(long? userId, int? FlowStatusID)
        {
            try
            {
                var parameters = new DynamicParameters();
                List<DynamicFormWorkFlowApprovedForm> result = new List<DynamicFormWorkFlowApprovedForm>();
                var dynamicFormWorkFlowApprovedForm = new List<DynamicFormWorkFlowApprovedForm>();
                var dynamicFormWorkFlowForm = new List<DynamicFormWorkFlowForm>();
                var query = string.Empty;
                List<long?> dynamicformDataIds = new List<long?>();
                query += "SELECT * FROM DynamicFormWorkFlowForm where FlowStatusID=1;";
                query += "select tt2.*,\r\n(case when tt2.DelegateUserId>0 Then tt2.DelegateUserId ELSE tt2.UserID END) as DelegateUserIds\r\nfrom(select tt1.* from(select t1.*,(select TOP(1) t2.UserID from DynamicFormWorkFlowApprovedFormChanged t2 where t1.DynamicFormWorkFlowApprovedFormID=t2.DynamicFormWorkFlowApprovedFormID  order by t2.DynamicFormWorkFlowApprovedFormChangedID desc) as DelegateUserId from DynamicFormWorkFlowApprovedForm t1)tt1)tt2;";
                using (var connection = CreateConnection())
                {
                    var QuerResult = await connection.QueryMultipleAsync(query);
                    dynamicFormWorkFlowForm = QuerResult.Read<DynamicFormWorkFlowForm>().ToList();
                    dynamicFormWorkFlowApprovedForm = QuerResult.Read<DynamicFormWorkFlowApprovedForm>().ToList();
                }
                if (dynamicFormWorkFlowForm != null && dynamicFormWorkFlowForm.Count() > 0)
                {
                    dynamicFormWorkFlowForm.ForEach(s =>
                    {
                        var IsParallelWorkflow = dynamicFormWorkFlowForm.Where(q => q.DynamicFormDataId == s.DynamicFormDataId && q.IsParallelWorkflow == true).ToList();
                        var dynamicFormWorkFlowApprovedForms = dynamicFormWorkFlowApprovedForm.OrderBy(o => o.DynamicFormWorkFlowApprovedFormID).FirstOrDefault(w => w.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowFormId && w.IsApproved != true);
                        if (dynamicFormWorkFlowApprovedForms != null)
                        {
                            if (IsParallelWorkflow.Count() > 0)
                            {
                                dynamicFormWorkFlowApprovedForms.DynamicFormDataID = s.DynamicFormDataId;
                                if (dynamicFormWorkFlowApprovedForms.DelegateUserIds == userId)
                                {
                                    result.Add(dynamicFormWorkFlowApprovedForms);
                                }
                            }
                            else
                            {
                                dynamicFormWorkFlowApprovedForms.DynamicFormDataID = s.DynamicFormDataId;
                                if (dynamicFormWorkFlowApprovedForms.DelegateUserIds == userId)
                                {
                                    result.Add(dynamicFormWorkFlowApprovedForms);
                                }
                            }
                            //if (dynamicFormWorkFlowApprovedForms.DelegateUserId == null)
                            //{
                            //    if (dynamicFormWorkFlowApprovedForms.UserID == userId)
                            //    {
                            //        dynamicFormWorkFlowApprovedForms.DelegateUserIds = userId;
                            //        result.Add(dynamicFormWorkFlowApprovedForms);
                            //    }
                            //}
                            //else
                            //{
                            //    if (dynamicFormWorkFlowApprovedForms.DelegateUserId == userId)
                            //    {

                            //        dynamicFormWorkFlowApprovedForms.DelegateUserIds = dynamicFormWorkFlowApprovedForms.DelegateUserId;
                            //        result.Add(dynamicFormWorkFlowApprovedForms);
                            //    }
                            //}
                        }
                    });
                    result = await GetDynamicFormWorkFlowApprovedDataFormByList(result);
                }
                return result != null ? result : new List<DynamicFormWorkFlowApprovedForm>();

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormWorkFlowApprovedForm>> GetDynamicFormWorkFlowApprovedDataFormByList(List<DynamicFormWorkFlowApprovedForm> result)
        {
            try
            {
                List<long?> dynamicformDataIds = result != null && result.Count > 0 ? result.Where(w => w.DynamicFormDataID > 0).Select(s => s.DynamicFormDataID).Distinct().ToList() : new List<long?>() { -1 };
                List<DynamicFormData> dynamicformData = new List<DynamicFormData>();
                var query1 = string.Empty;
                query1 += "select t2.SessionId as DynamicFormSessionId,(case when t1.LockedUserId>0 Then CONCAT(case when t33.NickName is NULL OR  t33.NickName='' then  ''\r\n ELSE  CONCAT(t33.NickName,' | ') END,t33.FirstName, (case when t33.LastName is Null OR t33.LastName='' then '' ELSE '-' END),t33.LastName) ELSE null END) as LockedUser,t1.DynamicFormDataID,t1.DynamicFormID,t1.SessionID,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.IsSendApproval,t1.FileProfileSessionID,t1.ProfileID,t1.ProfileNo,t1.DynamicFormDataGridID,t1.IsDeleted,t1.SortOrderByNo,t1.GridSortOrderByNo,t1.DynamicFormSectionGridAttributeID,t1.IsLocked,t1.LockedUserID,t2.Name  as DynamicFormName,\n\r" +
                    "(select COUNT(t6.DocumentID) from DynamicFormDataUpload tt1 JOIN Documents t6 ON tt1.SessionID=t6.SessionID where t1.DynamicFormDataID=tt1.DynamicFormDataID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,t2.FileProfileTypeId\r\n from DynamicFormData t1\r\nJOIN DynamicForm t2 ON t2.ID=t1.DynamicFormID LEFT JOIN Employee t33 ON t33.UserID = t1.LockedUserId\r\n" +
                    "where (t1.IsDeleted is null OR t1.IsDeleted=0) AND dynamicformdataid in(" + string.Join(',', dynamicformDataIds.Distinct()) + ");";
                using (var connection = CreateConnection())
                {

                    var QuerResult = await connection.QueryMultipleAsync(query1);
                    dynamicformData = QuerResult.Read<DynamicFormData>().ToList();

                }
                if (result != null && result.Count() > 0)
                {
                    var SessionIds = dynamicformData != null && dynamicformData.Count > 0 ? dynamicformData.Select(a => a.SessionId).Distinct().ToList() : new List<Guid?>();
                    var lists = string.Join(',', SessionIds.Select(i => $"'{i}'"));
                    var _activityEmailTopics = await GetActivityEmailTopicList(lists);
                    result.ForEach(s =>
                    {
                        var dynamicformDatas = dynamicformData?.FirstOrDefault(f => f.DynamicFormDataId == s.DynamicFormDataID);
                        if (dynamicformDatas != null)
                        {
                            s.FileProfileTypeId = dynamicformDatas?.FileProfileTypeId;
                            s.DynamicFormDataSessionID = dynamicformDatas?.SessionId;
                            s.DynamicFormSessionID = dynamicformDatas?.DynamicFormSessionID;
                            s.DynamicFormName = dynamicformDatas?.DynamicFormName;
                            s.ProfileNo = dynamicformDatas?.ProfileNo;
                            s.IsLocked = dynamicformDatas?.IsLocked;
                            s.DynamicFormId = dynamicformDatas?.DynamicFormId;
                            s.LockedUserId = dynamicformDatas?.LockedUserId;
                            s.LockedUser = dynamicformDatas?.LockedUser;
                            s.IsFileprofileTypeDocument = dynamicformDatas?.IsFileprofileTypeDocument;
                            var _activityEmailTopicsOne = _activityEmailTopics.FirstOrDefault(f => f.SessionId == dynamicformDatas?.SessionId);
                            if (_activityEmailTopicsOne != null)
                            {
                                s.EmailTopicSessionId = _activityEmailTopicsOne.EmailTopicSessionId;
                                if (_activityEmailTopicsOne.EmailTopicSessionId != null)
                                {
                                    if (_activityEmailTopicsOne.IsDraft == false)
                                    {
                                        s.IsDraft = false;
                                    }
                                    if (_activityEmailTopicsOne.IsDraft == true)
                                    {
                                        s.IsDraft = true;
                                    }
                                }
                            }
                        }
                    });
                }
                return result != null ? result : new List<DynamicFormWorkFlowApprovedForm>();

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowApprovedForm>> GetDynamicFormWorkFlowApprovedFormList(long? dynamicFormDataId)
        {
            try
            {
                List<DynamicFormWorkFlowApprovedForm> dynamicFormWorkFlowApprovedForms = new List<DynamicFormWorkFlowApprovedForm>();
                List<DynamicFormWorkFlowApprovedFormChanged> dynamicFormWorkFlowApprovedFormChanged = new List<DynamicFormWorkFlowApprovedFormChanged>();
                var parameters = new DynamicParameters();
                parameters.Add("dynamicFormDataId", dynamicFormDataId);
                var query = "select t1.*,t4.FlowStatusID,(case when t1.UserID>0 Then CONCAT(case when t5.NickName is NULL OR  t5.NickName='' then  ''\r\n ELSE  CONCAT(t5.NickName,' | ') END,t5.FirstName, (case when t5.LastName is Null OR t5.LastName='' then '' ELSE '-' END),t5.LastName) ELSE null END) as UserName," +
                    "(case when t1.ApprovedByUserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as ApprovedByUser,t4.DynamicFormDataID,\r\n(CASE \r\nWHEN t1.IsApproved = 0  THEN 'Rejected'\r\nWHEN t1.IsApproved  <= 1 THEN 'Approved'\r\nELSE 'Pending' \r\nEND) AS ApprovedStatus\r\nfrom DynamicFormWorkFlowApprovedForm t1 \r\n" +
                    "JOIN DynamicFormWorkFlowForm t4 ON t4.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID\r\n" +
                    "JOIN Employee t5 ON t1.UserID=t5.UserID\r\n" +
                    "LEFT JOIN Employee t3 ON t3.UserID=t1.ApprovedByUserID  where t4.dynamicFormDataId=@dynamicFormDataId;";
                query += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END)  as UserName  from DynamicFormWorkFlowApprovedFormChanged t1  JOIN Employee t3 ON t1.UserID=t3.UserID\r\n;";

                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormWorkFlowApprovedForms = results.Read<DynamicFormWorkFlowApprovedForm>().ToList();
                    dynamicFormWorkFlowApprovedFormChanged = results.Read<DynamicFormWorkFlowApprovedFormChanged>().ToList();
                }
                if (dynamicFormWorkFlowApprovedForms != null && dynamicFormWorkFlowApprovedForms.Count() > 0)
                {
                    dynamicFormWorkFlowApprovedForms.ForEach(s =>
                    {
                        s.ActualUserId = s.UserID;
                        s.DelegateUserName = s.UserName;
                        var list = dynamicFormWorkFlowApprovedFormChanged.Where(w => w.DynamicFormWorkFlowApprovedFormID == s.DynamicFormWorkFlowApprovedFormID).OrderByDescending(o => o.DynamicFormWorkFlowApprovedFormChangedID).ToList();
                        if (list != null && list.Count() > 0)
                        {
                            s.UserID = list[0].UserId;
                            s.UserName = list[0].UserName;
                            s.DelegateUserId = list[0].UserId;
                            s.DelegateUserName = list[0].UserName;
                        }
                    });
                }
                return dynamicFormWorkFlowApprovedForms;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DynamicFormWorkFlowSection>> GetDynamicFormWorkFlowExits(long? dynamicFormId, long? userId, long? dynamicFormDataId)
        {
            try
            {
                List<DynamicFormWorkFlowFormDelegate> dynamicFormWorkFlowFormDelegates = new List<DynamicFormWorkFlowFormDelegate>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("dynamicFormDataId", dynamicFormDataId);
                var query = string.Empty;
                if (dynamicFormDataId > 0)
                {
                    query = "select t2.IsParallelWorkflow,t2.IsAllowDelegateUserForm as IsAllowDelegateUser,t1.DynamicFormWorkFlowFormID as DynamicFormWorkFlowID,(case when t2.FlowStatusID>0 THEN t2.FlowStatusID ELSE 0 END) as IsWorkFlowDone,t2.UserID,(case when t2.FlowStatusID>0 THEN t2.FlowStatusID ELSE 0 END) as FlowStatusID,t3.SectionName,t1.DynamicFormWorkFlowSectionFormID as DynamicFormWorkFlowSectionID,t2.DynamicFormDataId,t1.DynamicFormSectionID,t3.DynamicFormID,t2.SequenceNo,\r\n" +
                        "(case when t2.UserID>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END) as UserName,t1.DynamicFormWorkFlowFormID,\r\n" +
                        "(select  count(t5.DynamicFormWorkFlowApprovedFormID) from DynamicFormWorkFlowApprovedForm t5 where  t5.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID) as DynamicFormWorkFlowFormTotalCount,\r\n(select  count(t6.DynamicFormWorkFlowApprovedFormID) from DynamicFormWorkFlowApprovedForm t6 where  t6.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID AND t6.IsApproved=1) as DynamicFormWorkFlowFormCount\n\r" +
                        "from DynamicFormWorkFlowSectionForm t1 \r\n" +
                        "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID  \r\n" +
                        "JOIN DynamicFormWorkFlowForm t2 ON t2.DynamicFormWorkFlowFormID=t1.DynamicFormWorkFlowFormID \r\n" +
                        "JOIN Employee t4 ON t4.UserID=t2.UserID  \r\nWhere  t2.DynamicFormDataID=@dynamicFormDataId order by t2.SequenceNo asc";
                }
                else
                {
                    query = "select t2.IsParallelWorkflow,t2.IsAllowDelegateUser,t1.DynamicFormWorkFlowID,t3.SectionName,t1.DynamicFormWorkFlowSectionID,t1.DynamicFormSectionID,t2.DynamicFormID,t2.SequenceNo,(case when t2.UserID>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END)  as UserName,t2.UserID,t2.UserGroupID,t2.LevelID,t2.Type," +
                        "(SELECT (Count(t3.DynamicFormWorkFlowFormID)) from DynamicFormWorkFlowForm t3 WHERE t3.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID AND t3.FlowStatusID=1 AND t3.DynamicFormDataID=@dynamicFormDataId) as IsWorkFlowDone,\r\n" +
                        "(SELECT tt3.DynamicFormWorkFlowFormID from DynamicFormWorkFlowForm tt3 WHERE tt3.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID AND tt3.UserID=t2.UserID AND tt3.DynamicFormDataID=@dynamicFormDataId) as DynamicFormWorkFlowFormID\r\n \r\n" +
                        "from DynamicFormWorkFlowSection t1 \r\n" +
                        "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID  \r\n" +
                        "JOIN DynamicFormWorkFlow t2 ON t2.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID \r\n" +
                         "JOIN Employee t4 ON t4.UserID=t2.UserID  \r\n" +
                        "Where  t2.DynamicFormID=@DynamicFormId order by t2.SequenceNo asc";
                }
                var result = new List<DynamicFormWorkFlowSection>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormWorkFlowSection>(query, parameters)).ToList();
                    if (result != null && result.Count() > 0 && dynamicFormDataId > 0)
                    {
                        var DynamicFormWorkFlowFormIDs = result.Where(w => w.DynamicFormWorkFlowId > 0).Select(s => s.DynamicFormWorkFlowId).Distinct().ToList();
                        if (DynamicFormWorkFlowFormIDs.Count() > 0)
                        {
                            var query1 = string.Empty;
                            query1 += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as UserName  from DynamicFormWorkFlowFormDelegate t1  JOIN Employee t3 ON t1.UserID=t3.UserID  where t1.DynamicFormWorkFlowFormID in(" + string.Join(',', DynamicFormWorkFlowFormIDs) + ");";
                            dynamicFormWorkFlowFormDelegates = (await connection.QueryAsync<DynamicFormWorkFlowFormDelegate>(query1)).ToList();
                        }
                    }
                }
                if (dynamicFormDataId > 0)
                {
                    if (result != null && result.Count() > 0)
                    {
                        result.ForEach(s =>
                        {
                            s.ActualUserId = s.UserId;
                            s.ActualUserName = s.UserName;
                            var dynamicFormWorkFlowFormDelegateData = dynamicFormWorkFlowFormDelegates.OrderByDescending(o => o.DynamicFormWorkFlowFormDelegateID).Where(w => w.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowId).FirstOrDefault();
                            if (dynamicFormWorkFlowFormDelegateData != null)
                            {
                                s.UserName = dynamicFormWorkFlowFormDelegateData.UserName;
                                s.DelegateSectionUserId = dynamicFormWorkFlowFormDelegateData.UserID;
                                s.DelegateSectionUserName = dynamicFormWorkFlowFormDelegateData.UserName;
                                s.UserId = dynamicFormWorkFlowFormDelegateData.UserID;
                            }
                            if (s.DynamicFormWorkFlowFormTotalCount == 0)
                            {
                                if (s.IsWorkFlowDone == 1)
                                {
                                    s.IsWorkFlowFormDone = 1;
                                }
                            }
                            else
                            {
                                if (s.DynamicFormWorkFlowFormTotalCount == s.DynamicFormWorkFlowFormCount)
                                {
                                    s.IsWorkFlowFormDone = 1;
                                }
                            }
                            // if (s.DynamicFormWorkFlowFormTotalCount > 0)
                            // {

                            // }
                        });
                    }
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowItems> GetDynamicFormWorkFlowLists(long? id, long? dynamicFormDataId)
        {

            try
            {
                DynamicFormWorkFlowItems dynamicFormWorkFlowItems = new DynamicFormWorkFlowItems();
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", id);
                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                //query += "SELECT IsAllowDelegateUserForm as IsAllowDelegateUser,DynamicFormWorkFlowFormID,\r\nDynamicFormWorkFlowSectionID,\r\nDynamicFormDataID,\r\nUserID,\r\nCompletedDate,\r\nFlowStatusID,\r\nSequenceNo,\r\nDynamicFormWorkFlowID FROM DynamicFormWorkFlowForm Where DynamicFormDataId = @DynamicFormDataId;";
                query += "select tt2.*,\r\n(case when tt2.CurrentApprovalUserId>0 Then CONCAT(case when tt3.NickName is NULL OR  tt3.NickName='' then  ''\r\n ELSE  CONCAT(tt3.NickName,' | ') END,tt3.FirstName, (case when tt3.LastName is Null OR tt3.LastName='' then '' ELSE '-' END),tt3.LastName) ELSE null END) as CurrentApprovalUserName from(select tt1.*,(case when tt1.DelegateSectionUserId>0 THEN  tt1.DelegateSectionUserId ELSE  tt1.UserID END) as CurrentApprovalUserId from\r\n(select t1.*,\r\n(select TOP(1) t2.UserID from DynamicFormWorkFlowFormDelegate t2 where t1.DynamicFormWorkFlowFormID=t2.DynamicFormWorkFlowFormID  order by t2.DynamicFormWorkFlowFormDelegateID desc) as DelegateSectionUserId from DynamicFormWorkFlowForm t1   Where t1.DynamicFormDataId = @DynamicFormDataId)tt1 )tt2 JOIN  Employee tt3 ON tt3.UserID=tt2.CurrentApprovalUserId\r\n ";
                query += "SELECT IsParallelWorkflow,IsAllowDelegateUser,DynamicFormWorkFlowID,\r\nDynamicFormID,\r\nUserID,\r\nUserGroupID,\r\nLevelID,\r\nType,\r\nSequenceNo FROM DynamicFormWorkFlow Where DynamicFormId = @DynamicFormId order by SequenceNo asc;";
                query += "SELECT DynamicFormWorkFlowSectionID,\r\nDynamicFormSectionID,\r\nDynamicFormWorkFlowID,\r\nDynamicFormDataID FROM DynamicFormWorkFlowSection;";
                query += "SELECT DynamicFormWorkFlowApprovalID,\r\nDynamicFormWorkFlowID,\r\nUserID,\r\nSortBy FROM DynamicFormWorkFlowApproval;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormWorkFlowItems.DynamicFormWorkFlowForm = results.Read<DynamicFormWorkFlowForm>().ToList();
                    dynamicFormWorkFlowItems.DynamicFormWorkFlow = results.Read<DynamicFormWorkFlow>().ToList();
                    dynamicFormWorkFlowItems.DynamicFormWorkFlowSection = results.Read<DynamicFormWorkFlowSection>().ToList();
                    dynamicFormWorkFlowItems.DynamicFormWorkFlowApproval = results.Read<DynamicFormWorkFlowApproval>().ToList();

                }
                return dynamicFormWorkFlowItems;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        /*public async Task<DynamicFormWorkFlowSection> InsertOrUpdateFormWorkFlowSectionNoWorkFlow(List<DynamicFormWorkFlowSection> dynamicFormWorkFlowSections, long? dynamicFormDataId, long? userId)
        {
            DynamicFormWorkFlowSection dynamicFormWorkFlowSection = new DynamicFormWorkFlowSection();
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (dynamicFormWorkFlowSections != null && dynamicFormWorkFlowSections.Count > 0)
                        {
                            var dynamicFormId = dynamicFormWorkFlowSections[0].DynamicFormId;
                            var query = string.Empty;
                            var parameters = new DynamicParameters();
                            parameters.Add("CompletedDate", DateTime.Now, DbType.DateTime);
                            var dataList = await GetDynamicFormWorkFlowLists(dynamicFormId, dynamicFormDataId);
                            var _dynamicFormWorkFlows = dataList.DynamicFormWorkFlow.ToList();
                            var _dynamicFormWorkFlowForms = dataList.DynamicFormWorkFlowForm.ToList();
                            dynamicFormWorkFlowSections.ForEach(s =>
                            {
                                query = string.Empty;
                                query += "INSERT INTO [DynamicFormWorkFlowForm](DynamicFormWorkFlowSectionID,UserId,DynamicFormDataID,CompletedDate) OUTPUT INSERTED.DynamicFormWorkFlowFormID " +
                                                   "VALUES (" + s.DynamicFormWorkFlowSectionId + "," + userId + "," + dynamicFormDataId + ",@CompletedDate);\r\n";
                                var ids = connection.QuerySingleOrDefault<long>(query, parameters);
                                var _dynamicFormWorkFlowSections = dataList.DynamicFormWorkFlowSection.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).ToList();
                                var _dynamicFormWorkFlowApprovals = dataList.DynamicFormWorkFlowApproval.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).OrderBy(o => o.SortBy).ToList();
                                if (_dynamicFormWorkFlowSections != null && _dynamicFormWorkFlowSections.Count() > 0)
                                {
                                    var query1 = string.Empty;
                                    _dynamicFormWorkFlowSections.ForEach(a =>
                                    {
                                        query1 += "INSERT INTO [DynamicFormWorkFlowSectionForm](DynamicFormSectionId,DynamicFormWorkFlowFormID) OUTPUT INSERTED.DynamicFormWorkFlowSectionFormID " +
                                                  "VALUES (" + a.DynamicFormSectionId + "," + ids + ");\r\n";
                                    });
                                    if (!string.IsNullOrEmpty(query1))
                                    {
                                        connection.QuerySingleOrDefault<long>(query1, parameters);
                                    }
                                }
                                if (_dynamicFormWorkFlowApprovals != null && _dynamicFormWorkFlowApprovals.Count() > 0)
                                {
                                    var query2 = string.Empty;
                                    _dynamicFormWorkFlowApprovals.ForEach(d =>
                                    {
                                        query2 += "INSERT INTO [DynamicFormWorkFlowApprovedForm](DynamicFormWorkFlowFormID,UserID,SortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormID " +
                                                  "VALUES (" + ids + "," + d.UserId + "," + d.SortBy + ");\r\n";
                                    });
                                    if (!string.IsNullOrEmpty(query2))
                                    {
                                        connection.QuerySingleOrDefault<long>(query2, parameters);
                                    }
                                }
                            });
                            if (!string.IsNullOrEmpty(query))
                            {
                                //await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            }
                        }
                        return dynamicFormWorkFlowSection;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }*/
        public async Task<DynamicFormWorkFlowSection> InsertOrUpdateFormWorkFlowSectionNoWorkFlow(List<DynamicFormWorkFlowSection> dynamicFormWorkFlowSections, long? dynamicFormDataId, long? userId)
        {
            DynamicFormWorkFlowSection dynamicFormWorkFlowSection = new DynamicFormWorkFlowSection();
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (dynamicFormWorkFlowSections != null && dynamicFormWorkFlowSections.Count > 0)
                        {
                            var dynamicFormId = dynamicFormWorkFlowSections[0].DynamicFormId;

                            var parameters = new DynamicParameters();
                            parameters.Add("CompletedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("FlowStatusID", 1);
                            var dataList = await GetDynamicFormWorkFlowLists(dynamicFormId, dynamicFormDataId);
                            var _dynamicFormWorkFlows = dataList.DynamicFormWorkFlow.ToList();
                            var _dynamicFormWorkFlowForms = dataList.DynamicFormWorkFlowForm.ToList();
                            if (_dynamicFormWorkFlowForms != null && _dynamicFormWorkFlowForms.Count() > 0)
                            {
                                var exits = _dynamicFormWorkFlowForms.FirstOrDefault(f => f.CurrentApprovalUserId == userId && f.DynamicFormDataId == dynamicFormDataId && f.FlowStatusID == 0);
                                if (exits != null)
                                {
                                    parameters.Add("DynamicFormWorkFlowFormId", exits.DynamicFormWorkFlowFormId);
                                    var query2 = "UPDATE DynamicFormWorkFlowForm SET CompletedDate=@CompletedDate,FlowStatusID=@FlowStatusID WHERE DynamicFormWorkFlowFormId = @DynamicFormWorkFlowFormId;\r\n";
                                    if (!string.IsNullOrEmpty(query2))
                                    {
                                        await connection.QuerySingleOrDefaultAsync<long>(query2, parameters);
                                    }
                                }
                            }
                            else
                            {
                                if (_dynamicFormWorkFlows != null && _dynamicFormWorkFlows.Count() > 0)
                                {
                                    int i = 0;
                                    _dynamicFormWorkFlows.ForEach(s =>
                                    {
                                        var query = string.Empty;
                                        int? IsAllowDelegateUserForm = s.IsAllowDelegateUser == true ? 1 : null;
                                        int? IsParallelWorkflow = s.IsParallelWorkflow == true ? 1 : null;
                                        var _dynamicFormWorkFlowSections = dataList.DynamicFormWorkFlowSection.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).ToList();
                                        if (i == 0)
                                        {
                                            query += "INSERT INTO [DynamicFormWorkFlowForm](SequenceNo,UserId,DynamicFormDataID,FlowStatusID,CompletedDate,DynamicFormWorkFlowId";
                                            if (IsAllowDelegateUserForm == 1)
                                            {
                                                query += ",IsAllowDelegateUserForm";
                                            }
                                            if (IsParallelWorkflow == 1)
                                            {
                                                query += ",IsParallelWorkflow";
                                            }
                                            query += ") OUTPUT INSERTED.DynamicFormWorkFlowFormID VALUES (" + s.SequenceNo + "," + s.UserId + "," + dynamicFormDataId + ",@FlowStatusID,@CompletedDate," + s.DynamicFormWorkFlowId;
                                            if (IsAllowDelegateUserForm == 1)
                                            {
                                                query += "," + IsAllowDelegateUserForm;
                                            }
                                            if (IsParallelWorkflow == 1)
                                            {
                                                query += "," + IsParallelWorkflow;
                                            }
                                            query += ");\r\n";
                                        }
                                        else
                                        {
                                            query += "INSERT INTO [DynamicFormWorkFlowForm](SequenceNo,UserId,DynamicFormDataID,DynamicFormWorkFlowId";
                                            if (IsAllowDelegateUserForm == 1)
                                            {
                                                query += ",IsAllowDelegateUserForm";
                                            }
                                            if (IsParallelWorkflow == 1)
                                            {
                                                query += ",IsParallelWorkflow";
                                            }
                                            query += ") OUTPUT INSERTED.DynamicFormWorkFlowFormID VALUES (" + s.SequenceNo + "," + s.UserId + "," + dynamicFormDataId + "," + s.DynamicFormWorkFlowId;
                                            if (IsAllowDelegateUserForm == 1)
                                            {
                                                query += "," + IsAllowDelegateUserForm;
                                            }
                                            if (IsParallelWorkflow == 1)
                                            {
                                                query += "," + IsParallelWorkflow;
                                            }
                                            query += ");\r\n";

                                        }
                                        var ids = connection.QuerySingleOrDefault<long>(query, parameters);
                                        if (_dynamicFormWorkFlowSections != null && _dynamicFormWorkFlowSections.Count() > 0)
                                        {
                                            var query1 = string.Empty;
                                            _dynamicFormWorkFlowSections.ForEach(a =>
                                            {
                                                query1 += "INSERT INTO [DynamicFormWorkFlowSectionForm](DynamicFormSectionId,DynamicFormWorkFlowFormID) OUTPUT INSERTED.DynamicFormWorkFlowSectionFormID " +
                                                          "VALUES (" + a.DynamicFormSectionId + "," + ids + ");\r\n";
                                            });
                                            if (!string.IsNullOrEmpty(query1))
                                            {
                                                connection.QuerySingleOrDefault<long>(query1, parameters);
                                            }
                                        }
                                        var _dynamicFormWorkFlowApprovals = dataList.DynamicFormWorkFlowApproval.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).OrderBy(o => o.SortBy).ToList();
                                        if (_dynamicFormWorkFlowApprovals != null && _dynamicFormWorkFlowApprovals.Count() > 0)
                                        {
                                            var query2 = string.Empty;
                                            _dynamicFormWorkFlowApprovals.ForEach(d =>
                                            {
                                                query2 += "INSERT INTO [DynamicFormWorkFlowApprovedForm](DynamicFormWorkFlowFormID,UserID,SortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormID " +
                                                          "VALUES (" + ids + "," + d.UserId + "," + d.SortBy + ");\r\n";
                                            });
                                            if (!string.IsNullOrEmpty(query2))
                                            {
                                                connection.QuerySingleOrDefault<long>(query2, parameters);
                                            }
                                        }
                                        i++;
                                    });
                                }
                            }

                        }
                        return dynamicFormWorkFlowSection;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowForm>> GetDynamicFormWorkFlowFormList(long? dynamicFormDataId, long? dynamicFormId)
        {
            List<DynamicFormWorkFlowForm> dynamicFormWorkFlowSections = new List<DynamicFormWorkFlowForm>();
            try
            {
                var rowCount = 0;
                var listData = await GetDynamicFormWorkFlowExits(dynamicFormId, 0, dynamicFormDataId);
                var result = new List<DynamicFormWorkFlowForm>();
                List<DynamicFormWorkFlowFormDelegate> dynamicFormWorkFlowFormDelegates = new List<DynamicFormWorkFlowFormDelegate>();
                List<DynamicFormWorkFlowApprovedForm> dynamicFormWorkFlowApprovedForm = new List<DynamicFormWorkFlowApprovedForm>();
                var parameters = new DynamicParameters();
                if (dynamicFormDataId > 0)
                {
                    parameters.Add("DynamicFormDataID", dynamicFormDataId);
                    var query = "select t2.IsParallelWorkflow,t2.IsAllowDelegateUserForm as IsAllowDelegateUser,t2.DynamicFormWorkFlowFormID as DynamicFormWorkFlowId,(case when t2.FlowStatusID>0 THEN t2.FlowStatusID ELSE 0 END) as FlowStatusID, ROW_NUMBER() OVER (ORDER BY (SELECT '1')) AS RowID,t2.DynamicFormDataID,t2.CompletedDate,t1.DynamicFormWorkFlowFormID,t2.DynamicFormWorkFlowSectionID,t2.UserID,(case when t2.UserID>0 Then CONCAT(case when t5.NickName is NULL then  t5.FirstName ELSE  t5.NickName END,' | ',t5.LastName) ELSE null END) as  CompletedBy,t2.SequenceNo," +
                         "(select  count(tt5.DynamicFormWorkFlowApprovedFormID) from DynamicFormWorkFlowApprovedForm tt5 where  tt5.DynamicFormWorkFlowFormID=t2.DynamicFormWorkFlowFormID) as DynamicFormWorkFlowFormTotalCount,\r\n(select  count(tt6.DynamicFormWorkFlowApprovedFormID) from DynamicFormWorkFlowApprovedForm tt6 where  tt6.DynamicFormWorkFlowFormID=t2.DynamicFormWorkFlowFormID AND tt6.IsApproved=1) as DynamicFormWorkFlowFormCount,\n\r" +
                        "\r\nt1.DynamicFormSectionID,t2.DynamicFormWorkFlowID,t3.SectionName,(case when t2.UserID>0 Then CONCAT(case when t5.NickName is NULL OR  t5.NickName='' then  ''\r\n ELSE  CONCAT(t5.NickName,' | ') END,t5.FirstName, (case when t5.LastName is Null OR t5.LastName='' then '' ELSE '-' END),t5.LastName) ELSE null END) as DynamicFormWorkFlowUser from \r\n\r\n" +
                        "DynamicFormWorkFlowSectionForm t1 \r\n " +
                        "JOIN DynamicFormWorkFlowForm t2 ON t1.DynamicFormWorkFlowFormID=t2.DynamicFormWorkFlowFormID\r\n  " +
                        "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID\r\n  " +
                        "JOIN Employee t5 ON t5.UserID=t2.UserID\r\n  " +
                        "AND t2.DynamicFormDataID=@DynamicFormDataID order by t2.SequenceNo asc";

                    using (var connection = CreateConnection())
                    {
                        result = (await connection.QueryAsync<DynamicFormWorkFlowForm>(query, parameters)).ToList();
                        if (result != null && result.Count() > 0)
                        {
                            var DynamicFormWorkFlowFormIDs = result.Where(w => w.DynamicFormWorkFlowId > 0).Select(s => s.DynamicFormWorkFlowId).Distinct().ToList();
                            if (DynamicFormWorkFlowFormIDs.Count() > 0)
                            {
                                var query1 = string.Empty;
                                query1 += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as UserName  from DynamicFormWorkFlowFormDelegate t1  JOIN Employee t3 ON t1.UserID=t3.UserID  where t1.DynamicFormWorkFlowFormID in(" + string.Join(',', DynamicFormWorkFlowFormIDs) + ");";
                                //query1 += "select t1.*,(select TOP(1) t2.UserID from DynamicFormWorkFlowApprovedFormChanged t2 where t1.DynamicFormWorkFlowApprovedFormID=t2.DynamicFormWorkFlowApprovedFormID  order by t2.DynamicFormWorkFlowApprovedFormChangedID desc) as DelegateUserId from DynamicFormWorkFlowApprovedForm t1 where t1.DynamicFormWorkFlowFormID in(" + string.Join(',', DynamicFormWorkFlowFormIDs) + ");";
                                query1 += "select tt2.*,(case when tt2.ApproverUserId>0 Then CONCAT(case when tt3.NickName is NULL OR  tt3.NickName='' then  ''\r\n ELSE  CONCAT(tt3.NickName,' | ') END,tt3.FirstName, (case when tt3.LastName is Null OR tt3.LastName='' then '' ELSE '-' END),tt3.LastName) ELSE null END) as ApproverUserName from(select tt1.*,(case when tt1.DelegateUserId>0 THEN  tt1.DelegateUserId ELSE  tt1.UserID END) as ApproverUserId\r\nfrom\n\r" +
                                    "(select t1.*,(select TOP(1) t2.UserID from DynamicFormWorkFlowApprovedFormChanged t2 where t1.DynamicFormWorkFlowApprovedFormID=t2.DynamicFormWorkFlowApprovedFormID  order by t2.DynamicFormWorkFlowApprovedFormChangedID desc) as DelegateUserId \r\nfrom DynamicFormWorkFlowApprovedForm t1 where t1.DynamicFormWorkFlowFormID in(" + string.Join(',', DynamicFormWorkFlowFormIDs) + ") )tt1 )tt2\r\nJOIN  Employee tt3 ON tt3.UserID=tt2.ApproverUserId;";
                                var results = await connection.QueryMultipleAsync(query1);
                                dynamicFormWorkFlowFormDelegates = results.Read<DynamicFormWorkFlowFormDelegate>().ToList();
                                dynamicFormWorkFlowApprovedForm = results.Read<DynamicFormWorkFlowApprovedForm>().ToList();
                            }
                        }
                    }
                }
                if (result != null && result.Count > 0)
                {
                    result.ForEach(s =>
                    {
                        if (s.DynamicFormWorkFlowFormTotalCount > 0)
                        {
                            if (s.DynamicFormWorkFlowFormTotalCount == s.DynamicFormWorkFlowFormCount)
                            {
                                s.DynamicFormWorkFlowApprovalFormCompleted = true;
                            }
                            else
                            {
                                if (s.FlowStatusID == 1)
                                {
                                    var dynamicFormWorkFlowApprovedForms = dynamicFormWorkFlowApprovedForm.Where(a => a.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowId && a.IsApproved != true).OrderBy(o => o.DynamicFormWorkFlowApprovedFormID).FirstOrDefault();
                                    if (dynamicFormWorkFlowApprovedForms != null)
                                    {
                                        s.CurrentApprovalUserId = dynamicFormWorkFlowApprovedForms.ApproverUserId;
                                        s.CurrentApprovalUserName = dynamicFormWorkFlowApprovedForms.ApproverUserName;
                                        s.IsApproved = dynamicFormWorkFlowApprovedForms.IsApproved;
                                        s.ApprovedStatus = dynamicFormWorkFlowApprovedForms.IsApproved == false ? "Rejected" : (dynamicFormWorkFlowApprovedForms.IsApproved == true ? "Approved" : "");
                                    }
                                }
                            }
                        }
                        s.ActualUserId = s.UserId;
                        s.ActualUserName = s.DynamicFormWorkFlowUser;
                        var dynamicFormWorkFlowFormDelegateData = dynamicFormWorkFlowFormDelegates.OrderByDescending(o => o.DynamicFormWorkFlowFormDelegateID).Where(w => w.DynamicFormWorkFlowFormID == s.DynamicFormWorkFlowId).FirstOrDefault();
                        if (dynamicFormWorkFlowFormDelegateData != null)
                        {
                            s.CompletedBy = dynamicFormWorkFlowFormDelegateData.UserName;
                            s.DynamicFormWorkFlowUser = dynamicFormWorkFlowFormDelegateData.UserName;
                            s.DelegateSectionUserId = dynamicFormWorkFlowFormDelegateData.UserID;
                            s.DelegateSectionUserName = dynamicFormWorkFlowFormDelegateData.UserName;
                            s.UserId = dynamicFormWorkFlowFormDelegateData.UserID;
                        }
                    });
                    rowCount = result.Count;
                    dynamicFormWorkFlowSections.AddRange(result);
                }
                if (dynamicFormDataId > 0)
                {

                }
                else
                {
                    if (listData != null && listData.Count > 0)
                    {
                        var notCompleted = listData.Where(w => w.IsWorkFlowDone == 0).ToList();
                        if (notCompleted != null && notCompleted.Count > 0)
                        {
                            notCompleted.ForEach(a =>
                            {
                                DynamicFormWorkFlowForm dynamicFormWorkFlowForm = new DynamicFormWorkFlowForm();
                                dynamicFormWorkFlowForm.DynamicFormWorkFlowId = a.DynamicFormWorkFlowId;
                                //dynamicFormWorkFlowForm.DynamicFormWorkFlowFormId = rowCount + 1; ;
                                long? ids = a.DynamicFormWorkFlowFormId > 0 ? a.DynamicFormWorkFlowFormId : 0;
                                dynamicFormWorkFlowForm.DynamicFormWorkFlowFormId = (long)ids;
                                dynamicFormWorkFlowForm.SectionName = a.SectionName;
                                dynamicFormWorkFlowForm.DynamicFormSectionId = a.DynamicFormSectionId;
                                dynamicFormWorkFlowForm.SequenceNo = a.SequenceNo;
                                dynamicFormWorkFlowForm.DynamicFormWorkFlowUserId = a.UserId;
                                dynamicFormWorkFlowForm.UserId = a.UserId;
                                dynamicFormWorkFlowForm.IsAllowDelegateUser = a.IsAllowDelegateUser;
                                dynamicFormWorkFlowForm.DynamicFormWorkFlowUser = a.UserName;
                                dynamicFormWorkFlowForm.ActualUserId = a.UserId;
                                dynamicFormWorkFlowForm.ActualUserName = a.UserName;
                                dynamicFormWorkFlowForm.IsParallelWorkflow = a.IsParallelWorkflow
                                ;
                                dynamicFormWorkFlowSections.Add(dynamicFormWorkFlowForm);
                                rowCount++;
                            });
                        }
                    }
                }
                return dynamicFormWorkFlowSections;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<DynamicFormWorkFlowForm> GetDynamicFormWorkFlowFormExits(long? dynamicFormWorkFlowSectionId, long? userId, long? dynamicFormDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormWorkFlowSectionID", dynamicFormWorkFlowSectionId);
                parameters.Add("UserID", userId);
                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                var query = "select t1.DynamicFormWorkFlowFormID,\r\nt1.DynamicFormWorkFlowSectionID,\r\nt1.DynamicFormDataID,\r\nt1.UserID,\r\nt1.CompletedDate,\r\nt1.FlowStatusID,\r\nt1.SequenceNo,\r\nt1.DynamicFormWorkFlowID from DynamicFormWorkFlowForm t1 WHERE  t1.DynamicFormDataId=@DynamicFormDataId AND t1.DynamicFormWorkFlowSectionID=@DynamicFormWorkFlowSectionID";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicFormWorkFlowForm>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ActivityEmailTopics> GetActivityEmailTopicsExits(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.ActivityEmailTopicID,\r\nt1.ActivityMasterId,\r\nt1.ManufacturingProcessId,\r\nt1.CategoryActionId,\r\nt1.ActionId,\r\nt1.Comment,\r\nt1.DocumentSessionId,\r\nt1.SubjectName,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.AddedDate,\r\nt1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.SessionId,\r\nt1.EmailTopicSessionId,\r\nt1.ActivityType,\r\nt1.FromId,\r\nt1.ToIds,\r\nt1.CcIds,\r\nt1.Tags,\r\nt1.BackURL,\r\nt1.IsDraft from ActivityEmailTopics t1 WHERE  t1.activityType='DynamicForm' AND t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ActivityEmailTopics>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataUpload>> GetDynamicFormDataUploadList(long? DynamicformDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicformDataId", DynamicformDataId);
                var query = "select * from DynamicFormDataUpload where DynamicformDataId=@DynamicformDataId;";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormDataUpload>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormData> InsertCreateEmailFormData(DynamicFormData dynamicFormData)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var dynamicFormDataUploadData = await GetDynamicFormDataUploadList(dynamicFormData.DynamicFormDataId);
                        var exitsData = await GetActivityEmailTopicsExits(dynamicFormData.SessionId);
                        if (exitsData == null)
                        {
                            var parameters = new DynamicParameters();
                            // parameters.Add("DocumentSessionId", dynamicFormDataUploadData != null && dynamicFormDataUploadData.Count() > 0 ? dynamicFormDataUploadData.FirstOrDefault()?.SessionId : null, DbType.Guid);
                            parameters.Add("activityType", "DynamicForm", DbType.String);
                            parameters.Add("BackUrl", dynamicFormData.BackUrl, DbType.String);
                            parameters.Add("subjectName", dynamicFormData.ProfileNo, DbType.String);
                            parameters.Add("SessionId", dynamicFormData.SessionId, DbType.Guid);
                            parameters.Add("AddedByUserID", dynamicFormData.AddedByUserID);
                            parameters.Add("ModifiedByUserID", dynamicFormData.ModifiedByUserID);
                            parameters.Add("AddedDate", dynamicFormData.AddedDate, DbType.DateTime);
                            parameters.Add("ModifiedDate", dynamicFormData.ModifiedDate, DbType.DateTime);
                            parameters.Add("StatusCodeID", dynamicFormData.StatusCodeID);
                            var query = "INSERT INTO ActivityEmailTopics(subjectName,SessionId,AddedByUserID," +
                         "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,activityType,BackUrl) VALUES " +
                         "(@subjectName,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@activityType,@BackUrl)";

                            await connection.ExecuteAsync(query, parameters);

                        }
                        /*else
                        {
                            if (exitsData.DocumentSessionId != null)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("ActivityEmailTopicID", exitsData.ActivityEmailTopicID);
                                parameters.Add("DocumentSessionId", dynamicFormDataUploadData != null && dynamicFormDataUploadData.Count() > 0 ? dynamicFormDataUploadData.FirstOrDefault()?.SessionId : null, DbType.Guid);
                                var query2 = "UPDATE ActivityEmailTopics SET DocumentSessionId=@DocumentSessionId WHERE ActivityEmailTopicID = @ActivityEmailTopicID;\r\n";
                                if (!string.IsNullOrEmpty(query2))
                                {
                                    await connection.QuerySingleOrDefaultAsync<long>(query2, parameters);
                                }
                            }
                        }*/
                        return dynamicFormData;
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
        public async Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetDynamicFormSectionAttributeSecurityEmptyAsync(long? DynamicFormSectionAttributeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionAttributeId", DynamicFormSectionAttributeId);
                var query = "select  DynamicFormSectionAttributeSecurityID,\r\nDynamicFormSectionAttributeID,\r\nUserID,\r\nUserGroupID,\r\nLevelID,\r\nIsAccess,\r\nIsViewFormatOnly,\r\nUserType from DynamicFormSectionAttributeSecurity where  DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttributeSecurity>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetDynamicFormSectionAttributeSecurityList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionAttributeId", Id);
                var query = "select t1.DynamicFormSectionAttributeSecurityID,\r\nt1.DynamicFormSectionAttributeID,\r\nt1.UserID,\r\nt1.UserGroupID,\r\nt1.LevelID,\r\nt1.IsAccess,\r\nt1.IsViewFormatOnly,\r\nt1.UserType,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.DisplayName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL OR  t6.NickName='' then  ''\r\n ELSE  CONCAT(t6.NickName,' | ') END,t6.FirstName, (case when t6.LastName is Null OR t6.LastName='' then '' ELSE '-' END),t6.LastName) as FullName\r\n" +
                    "from DynamicFormSectionAttributeSecurity t1\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN DynamicFormSectionAttribute t4 ON t4.DynamicFormSectionAttributeId=t1.DynamicFormSectionAttributeId\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE t1.DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttributeSecurity>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionAttributeSecurity> InsertDynamicFormSectionAttributeSecurity(DynamicFormSectionAttributeSecurity value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var userExitsRoles = await GetDynamicFormSectionAttributeSecurityEmptyAsync(value.DynamicFormSectionAttributeId);
                    var userGroupUsers = await GetUserGroupUserList();
                    var LevelUsers = await GetLeveMasterUsersList(value.SelectLevelMasterIDs);

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("IsAccess", value.IsAccess);
                        parameters.Add("IsViewFormatOnly", value.IsViewFormatOnly);
                        parameters.Add("UserType", value.UserType, DbType.String);
                        parameters.Add("DynamicFormSectionAttributeId", value.DynamicFormSectionAttributeId);
                        if (value.IsAccess == true || value.IsViewFormatOnly == true)
                        {
                            if (value.UserType == "User")
                            {
                                if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                                {
                                    foreach (var item in value.SelectUserIDs)
                                    {
                                        var counts = userExitsRoles.Where(w => w.UserId == item).FirstOrDefault();
                                        if (counts == null)
                                        {
                                            query += "INSERT INTO [DynamicFormSectionAttributeSecurity](IsAccess,IsViewFormatOnly,DynamicFormSectionAttributeId,UserId,UserType) OUTPUT INSERTED.DynamicFormSectionAttributeSecurityId " +
                                                "VALUES (@IsAccess,@IsViewFormatOnly,@DynamicFormSectionAttributeId," + item + ",@UserType);\r\n";

                                        }
                                        else
                                        {
                                            query += " UPDATE DynamicFormSectionAttributeSecurity SET IsAccess=@IsAccess,IsViewFormatOnly=@IsViewFormatOnly,UserType=@UserType WHERE DynamicFormSectionAttributeSecurityId='" + counts.DynamicFormSectionAttributeSecurityId + "' AND DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId;\r\n";
                                        }
                                    }
                                }
                            }
                            if (value.UserType == "UserGroup")
                            {
                                if (value.SelectUserGroupIDs != null && value.SelectUserGroupIDs.Count() > 0)
                                {
                                    var userGropuIds = userGroupUsers.Where(w => value.SelectUserGroupIDs.ToList().Contains(w.UserGroupId.Value)).ToList();
                                    if (userGropuIds != null && userGropuIds.Count > 0)
                                    {
                                        userGropuIds.ForEach(s =>
                                        {
                                            var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                            if (counts == null)
                                            {
                                                query += "INSERT INTO [DynamicFormSectionAttributeSecurity](IsAccess,IsViewFormatOnly,DynamicFormSectionAttributeId,UserId,UserGroupId,UserType) OUTPUT INSERTED.DynamicFormSectionAttributeSecurityId " +
                                                    "VALUES (@IsAccess,@IsViewFormatOnly,@DynamicFormSectionAttributeId," + s.UserId + "," + s.UserGroupId + ",@UserType);\r\n";
                                            }
                                            else
                                            {
                                                query += " UPDATE DynamicFormSectionAttributeSecurity SET IsAccess=@IsAccess,IsViewFormatOnly=@IsViewFormatOnly,UserType=@UserType WHERE DynamicFormSectionAttributeSecurityId='" + counts.DynamicFormSectionAttributeSecurityId + "' AND DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId;\r\n";
                                            }
                                        });
                                    }
                                }
                            }
                            if (value.UserType == "Level")
                            {
                                if (LevelUsers != null && LevelUsers.Count > 0)
                                {
                                    LevelUsers.ToList().ForEach(s =>
                                    {
                                        var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                        if (counts == null)
                                        {
                                            query += "INSERT INTO [DynamicFormSectionAttributeSecurity](IsAccess,IsViewFormatOnly,DynamicFormSectionAttributeId,UserId,LevelId,UserType) OUTPUT INSERTED.DynamicFormSectionAttributeSecurityId " +
                                                "VALUES (@IsAccess,@IsViewFormatOnly,@DynamicFormSectionAttributeId," + s.UserId + "," + s.LevelId + ",@UserType);\r\n";
                                        }
                                        else
                                        {
                                            query += " UPDATE DynamicFormSectionAttributeSecurity SET IsAccess=@IsAccess,IsViewFormatOnly=@IsViewFormatOnly,UserType=@UserType WHERE DynamicFormSectionAttributeSecurityId='" + counts.DynamicFormSectionAttributeSecurityId + "' AND DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId;\r\n";
                                        }
                                    });
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteDynamicFormSectionAttributeSecurity(long? Id, List<long?> Ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSectionAttributeId", Id);
                        if (Ids != null && Ids.Count > 0)
                        {
                            string IdList = string.Join(",", Ids);
                            query += "Delete From DynamicFormSectionAttributeSecurity WHERE DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId AND  DynamicFormSectionAttributeSecurityId in (" + IdList + ");\r\n";
                        }
                        if (!string.IsNullOrEmpty(query))
                        {

                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return Id.GetValueOrDefault(0);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormData>> GetUpdateDynamicFormDataSortOrder(DynamicFormData dynamicFormData)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormID", dynamicFormData.DynamicFormId);
                parameters.Add("DynamicFormDataGridId", dynamicFormData.DynamicFormDataGridId);
                var from = dynamicFormData.SortOrderAnotherBy > dynamicFormData.SortOrderByNo ? dynamicFormData.SortOrderByNo : dynamicFormData.SortOrderAnotherBy;
                var to = dynamicFormData.SortOrderAnotherBy > dynamicFormData.SortOrderByNo ? dynamicFormData.SortOrderAnotherBy : dynamicFormData.SortOrderByNo;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT DynamicFormID,DynamicFormDataId,SortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID = @DynamicFormID  AND SortOrderByNo>@SortOrderByFrom and SortOrderByNo<=@SortOrderByTo order by SortOrderByNo asc";

                if (dynamicFormData.SortOrderAnotherBy > dynamicFormData.SortOrderByNo)
                {
                    query = "SELECT DynamicFormID,DynamicFormDataId,SortOrderByNo FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID = @DynamicFormID  AND SortOrderByNo>=@SortOrderByFrom and SortOrderByNo<@SortOrderByTo order by SortOrderByNo asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormData>> GetUpdateDynamicFormDataSortOrderByGrid(DynamicFormData dynamicFormData)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormID", dynamicFormData.DynamicFormId);
                parameters.Add("DynamicFormDataGridId", dynamicFormData.DynamicFormDataGridId);
                parameters.Add("DynamicFormSectionGridAttributeId", dynamicFormData.DynamicFormSectionGridAttributeId);
                var from = dynamicFormData.GridSortOrderAnotherByNo > dynamicFormData.GridSortOrderByNo ? dynamicFormData.GridSortOrderByNo : dynamicFormData.GridSortOrderAnotherByNo;
                var to = dynamicFormData.GridSortOrderAnotherByNo > dynamicFormData.GridSortOrderByNo ? dynamicFormData.GridSortOrderAnotherByNo : dynamicFormData.GridSortOrderByNo;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT DynamicFormID,DynamicFormDataId,SortOrderByNo,GridSortOrderByNo,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID = @DynamicFormID AND DynamicFormSectionGridAttributeId=@DynamicFormSectionGridAttributeId AND DynamicFormDataGridId=@DynamicFormDataGridId AND GridSortOrderByNo>@SortOrderByFrom and GridSortOrderByNo<=@SortOrderByTo order by GridSortOrderByNo asc";

                if (dynamicFormData.GridSortOrderAnotherByNo > dynamicFormData.GridSortOrderByNo)
                {
                    query = "SELECT DynamicFormID,DynamicFormDataId,SortOrderByNo,GridSortOrderByNo,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID = @DynamicFormID   AND DynamicFormSectionGridAttributeId=@DynamicFormSectionGridAttributeId AND DynamicFormDataGridId=@DynamicFormDataGridId AND GridSortOrderByNo>=@SortOrderByFrom and GridSortOrderByNo<@SortOrderByTo order by GridSortOrderByNo asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormData>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormData> UpdateDynamicFormDataSortOrder(DynamicFormData dynamicFormData)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var query = string.Empty;
                        if (dynamicFormData.DynamicFormDataGridId > 0)
                        {
                            if (dynamicFormData.IsDynamicFormDataGrid == true)
                            {
                                long? SortOrder = dynamicFormData.GridSortOrderAnotherByNo > dynamicFormData.GridSortOrderByNo ? (dynamicFormData.GridSortOrderByNo + 1) : dynamicFormData.GridSortOrderAnotherByNo;
                                query += "Update  DynamicFormData SET GridSortOrderByNo=" + dynamicFormData.GridSortOrderByNo + "  WHERE (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataId =" + dynamicFormData.DynamicFormDataId + ";";
                                if (SortOrder > 0)
                                {
                                    var result = await GetUpdateDynamicFormDataSortOrderByGrid(dynamicFormData);
                                    if (result != null && result.Count > 0)
                                    {

                                        result.ForEach(s =>
                                        {

                                            query += "Update  DynamicFormData SET GridSortOrderByNo=" + SortOrder + "  WHERE  DynamicFormDataId =" + s.DynamicFormDataId + ";";
                                            SortOrder++;
                                        });

                                    }
                                }
                            }
                        }
                        else
                        {
                            long? SortOrder = dynamicFormData.SortOrderAnotherBy > dynamicFormData.SortOrderByNo ? (dynamicFormData.SortOrderByNo + 1) : dynamicFormData.SortOrderAnotherBy;
                            query += "Update  DynamicFormData SET SortOrderByNo=" + dynamicFormData.SortOrderByNo + "  WHERE (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataId =" + dynamicFormData.DynamicFormDataId + ";";
                            if (SortOrder > 0)
                            {
                                var result = await GetUpdateDynamicFormDataSortOrder(dynamicFormData);
                                if (result != null && result.Count > 0)
                                {

                                    result.ForEach(s =>
                                    {

                                        query += "Update  DynamicFormData SET SortOrderByNo=" + SortOrder + "  WHERE  DynamicFormDataId =" + s.DynamicFormDataId + ";";
                                        SortOrder++;
                                    });

                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            var rowsAffected = await connection.ExecuteAsync(query, null);
                        }
                        return dynamicFormData;
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


        public async Task<long> UpdateGenerateDynamicFormDataSortOrderByNo(string? query)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(query))
                        {

                            await connection.QuerySingleOrDefaultAsync<long>(query);
                        }
                        return 1;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<long?> GenerateDynamicFormDataSortOrderByNo()
        {
            List<DynamicFormData> dynamicFormData = new List<DynamicFormData>();
            try
            {
                var query = string.Empty;
                var querys = "select DynamicFormDataID,DynamicFormID,SortOrderByNo,DynamicFormDataGridID,GridSortOrderByNo,DynamicFormSectionGridAttributeId from DynamicFormData where IsDeleted is null or IsDeleted=0";

                using (var connection = CreateConnection())
                {
                    dynamicFormData = (await connection.QueryAsync<DynamicFormData>(querys)).ToList();
                }
                if (dynamicFormData != null && dynamicFormData.Count() > 0)
                {
                    var dynamicFormIds = dynamicFormData.Where(w => w.DynamicFormId > 0).Select(d => d.DynamicFormId).Distinct().ToList();
                    var dynamicFormDataGridIds = dynamicFormData.Where(w => w.DynamicFormDataGridId > 0).Select(d => d.DynamicFormDataGridId).Distinct().ToList();
                    if (dynamicFormIds != null && dynamicFormIds.Count() > 0)
                    {
                        dynamicFormIds.ForEach(s =>
                        {
                            var dynamicFormDatas = dynamicFormData.Where(a => a.DynamicFormId == s).OrderBy(q => q.DynamicFormDataId).ToList();
                            if (dynamicFormDatas != null && dynamicFormDatas.Count() > 0)
                            {
                                long? inc = 1;
                                var SortOrderByNo = dynamicFormDatas.OrderByDescending(o => o.SortOrderByNo).FirstOrDefault();
                                if (SortOrderByNo != null && SortOrderByNo.SortOrderByNo > 0)
                                {
                                    inc = SortOrderByNo.SortOrderByNo + 1;
                                }
                                dynamicFormDatas.ForEach(d =>
                                {
                                    if (d.SortOrderByNo > 0)
                                    {
                                    }
                                    else
                                    {
                                        query += "Update DynamicFormData SET SortOrderByNo=" + inc + " WHERE DynamicFormDataId=" + d.DynamicFormDataId + ";\n\r";
                                        inc++;
                                    }
                                });
                            }
                        });
                    }
                    if (dynamicFormDataGridIds != null && dynamicFormDataGridIds.Count() > 0)
                    {
                        dynamicFormDataGridIds.ForEach(s =>
                        {
                            var dynamicFormDatas = dynamicFormData.Where(a => a.DynamicFormDataGridId == s).OrderBy(q => q.DynamicFormDataId).ToList();
                            if (dynamicFormDatas != null && dynamicFormDatas.Count() > 0)
                            {
                                long? inc = 1;
                                var SortOrderByNo = dynamicFormDatas.OrderByDescending(o => o.GridSortOrderByNo).FirstOrDefault();
                                if (SortOrderByNo != null && SortOrderByNo.GridSortOrderByNo > 0)
                                {
                                    inc = SortOrderByNo.GridSortOrderByNo + 1;
                                }
                                dynamicFormDatas.ForEach(d =>
                                {
                                    if (d.GridSortOrderByNo > 0)
                                    {
                                    }
                                    else
                                    {
                                        query += "Update DynamicFormData SET GridSortOrderByNo=" + inc + " WHERE DynamicFormDataGridId=" + s + " AND DynamicFormDataId=" + d.DynamicFormDataId + ";\n\r";
                                        inc++;
                                    }
                                });
                            }
                        });
                    }
                }
                if (!string.IsNullOrEmpty(query))
                {
                    var results = await UpdateGenerateDynamicFormDataSortOrderByNo(query);
                }
                return 1;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<LinkFileProfileTypeDocument>> GetLinkFileProfileTypeDocumentExits(DynamicFormDataUpload dynamicFormDataUpload)
        {
            try
            {
                var query = string.Empty;
                var parameters = new DynamicParameters();
                var value = dynamicFormDataUpload.DocumentsModel;
                parameters.Add("FileProfileTypeId", dynamicFormDataUpload.FileProfileTypeId);
                parameters.Add("TransactionSessionID", value.SessionID, DbType.Guid);
                query = "SELECT LinkFileProfileTypeDocumentID,\r\nTransactionSessionID,\r\nDocumentID,\r\nFileProfileTypeID,\r\nFolderID,\r\nAddedByUserID,\r\nAddedDate,\r\nDescription,\r\nIsWiki,\r\nIsWikiDraft FROM LinkFileProfileTypeDocument Where FileProfileTypeId = @FileProfileTypeId AND TransactionSessionID=@TransactionSessionID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<LinkFileProfileTypeDocument>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataUpload> InsertDmsDocumentDynamicFormData(DynamicFormDataUpload dynamicFormDataUpload)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var linkProfiiles = await GetLinkFileProfileTypeDocumentExits(dynamicFormDataUpload);
                        if (linkProfiiles != null && linkProfiiles.Count > 0)
                        {
                            dynamicFormDataUpload.LinkFileProfileTypeDocumentId = linkProfiiles[0].LinkFileProfileTypeDocumentId;
                            await InsertDynamicFormDataUpload(dynamicFormDataUpload);
                        }
                        else
                        {
                            var value = dynamicFormDataUpload.DocumentsModel;
                            var parameters = new DynamicParameters();
                            parameters.Add("FileProfileTypeId", dynamicFormDataUpload.FileProfileTypeId);
                            parameters.Add("TransactionSessionID", value.SessionID);
                            parameters.Add("DocumentId", value.DocumentID);
                            parameters.Add("AddedDate", dynamicFormDataUpload.AddedDate, DbType.DateTime);
                            parameters.Add("AddedByUserId", dynamicFormDataUpload.AddedByUserId);
                            var query = "INSERT INTO [LinkFileProfileTypeDocument](FileProfileTypeId,DocumentId,TransactionSessionID,AddedByUserId,AddedDate) " +
                                "OUTPUT INSERTED.LinkFileProfileTypeDocumentID VALUES " +
                               "(@FileProfileTypeId,@DocumentId,@TransactionSessionID,@AddedByUserId,@AddedDate)";
                            var LinkFileProfileTypeDocumentID = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            dynamicFormDataUpload.LinkFileProfileTypeDocumentId = LinkFileProfileTypeDocumentID;
                            await InsertDynamicFormDataUpload(dynamicFormDataUpload);
                        }

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                    return dynamicFormDataUpload;
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<DynamicFormReport> InsertDynamicFormReport(DynamicFormReport reportDocuments)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Name", reportDocuments.Name, DbType.String);
                    parameters.Add("Description", reportDocuments.Description, DbType.String);
                    parameters.Add("SessionId", reportDocuments.SessionId, DbType.Guid);
                    parameters.Add("FileName", reportDocuments.FileName, DbType.String);
                    parameters.Add("FilePath", reportDocuments.FilePath, DbType.String);
                    parameters.Add("AddedDate", reportDocuments.AddedDate, DbType.DateTime);
                    parameters.Add("AddedByUserId", reportDocuments.AddedByUserId);
                    parameters.Add("ModifiedDate", reportDocuments.ModifiedDate, DbType.DateTime);
                    parameters.Add("ModifiedByUserId", reportDocuments.ModifiedByUserId);
                    parameters.Add("DynamicFormId", reportDocuments.DynamicFormId);
                    parameters.Add("StatusCodeId", reportDocuments.StatusCodeId);
                    parameters.Add("DynamicFormReportId", reportDocuments.DynamicFormReportId);
                    if (reportDocuments.DynamicFormReportId > 0)
                    {
                        var query = "UPDATE DynamicFormReport SET Name=@Name,Description = @Description,FileName =@FileName,SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID," +
                            "ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,FilePath=@FilePath WHERE DynamicFormReportId = @DynamicFormReportId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO DynamicFormReport(Name,Description,FileName,SessionId,AddedByUserId,AddedDate,ModifiedByUserId,ModifiedDate,DynamicFormId,StatusCodeId,FilePath) OUTPUT INSERTED.DynamicFormReportId VALUES " +
                            "(@Name,@Description,@FileName,@SessionId,@AddedByUserId,@AddedDate,@ModifiedByUserId,@ModifiedDate,@DynamicFormId,@StatusCodeId,@FilePath)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        reportDocuments.DynamicFormReportId = rowsAffected;
                    }
                    return reportDocuments;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<DynamicFormReport> DeleteDynamicFormReport(DynamicFormReport dynamicFormReport)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormReportId", dynamicFormReport.DynamicFormReportId);
                        var query = "Update  DynamicFormReport SET IsDeleted=1 WHERE DynamicFormReportId = @DynamicFormReportId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormReport;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<DynamicFormReport> GetDynamicFormReportOneData(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.SessionID as DynamicFormSessionID from DynamicFormReport t1 JOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID where (t2.IsDeleted is null OR t2.IsDeleted=0) AND t1.SessionID=@SessionId;";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DynamicFormReport>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormReport>> GetDynamicFormReportList(long? DynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", DynamicFormId);
                var query = "select t1.* from DynamicFormReport t1 \r\n" +
                    "JOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID\r\n" +
                    "WHERE (t1.IsDeleted IS NULL OR t1.IsDeleted=0) AND t1.DynamicFormId=@DynamicFormId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormReport>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionAttributeSectionParent> InsertOrUpdateDynamicFormSectionAttributeSectionParent(DynamicFormSectionAttributeSectionParent dynamicFormSection)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        if (dynamicFormSection.DynamicFormSectionIds != null && dynamicFormSection.ShowSectionVisibleDataIds != null && dynamicFormSection.DynamicFormSectionIds.Count() > 0 && dynamicFormSection.ShowSectionVisibleDataIds.Count() > 0)
                        {
                            var parameters = new DynamicParameters();
                            if (dynamicFormSection.DynamicFormSectionAttributeId > 0)
                            {
                                var query = string.Empty;

                                parameters.Add("DynamicFormSectionAttributeId", dynamicFormSection.DynamicFormSectionAttributeId);
                                parameters.Add("DynamicFormSectionAttributeSectionParentId", dynamicFormSection.DynamicFormSectionAttributeSectionParentId);
                                if (dynamicFormSection.DynamicFormSectionAttributeSectionParentId > 0)
                                {

                                    query += "UPDATE DynamicFormSectionAttributeSectionParent SET  DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId\n\r" +
                                       "WHERE DynamicFormSectionAttributeSectionParentId = @DynamicFormSectionAttributeSectionParentId";
                                    await connection.ExecuteAsync(query, parameters);

                                }
                                else
                                {
                                    query += "INSERT INTO DynamicFormSectionAttributeSectionParent(DynamicFormSectionAttributeId) OUTPUT INSERTED.DynamicFormSectionAttributeSectionParentId VALUES " +
                                        "(@DynamicFormSectionAttributeId)";

                                    dynamicFormSection.DynamicFormSectionAttributeSectionParentId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                }
                                var querys = string.Empty;
                                querys += "Delete from DynamicFormSectionAttributeSection where DynamicFormSectionAttributeSectionParentId=" + dynamicFormSection.DynamicFormSectionAttributeSectionParentId + ";\r\n";
                                dynamicFormSection.DynamicFormSectionIds.ToList().ForEach(a =>
                                {
                                    dynamicFormSection.ShowSectionVisibleDataIds.ToList().ForEach(b =>
                                    {
                                        if (!string.IsNullOrEmpty(b))
                                        {
                                            var c = b.Split("_");
                                            var exists = c.ElementAtOrDefault(1) != null;
                                            if (exists == true)
                                            {
                                                querys += "INSERT INTO DynamicFormSectionAttributeSection(DynamicFormSectionAttributeSectionParentId,DynamicFormSectionID,DynamicFormSectionSelectionByID,DynamicFormSectionSelectionID) VALUES " +
                                            "(" + dynamicFormSection.DynamicFormSectionAttributeSectionParentId + "," + a + ",'" + b + "'," + c[1] + ");\n\r";
                                            }
                                        }
                                    });
                                });

                                var rowsAffected = await connection.ExecuteAsync(querys);
                            }
                        }
                        return dynamicFormSection;
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
        public async Task<DynamicFormSectionAttributeSectionParent> DeleteDynamicFormSectionAttributeParent(DynamicFormSectionAttributeSectionParent value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSectionAttributeSectionParentId", value.DynamicFormSectionAttributeSectionParentId);

                        var query = "Delete from DynamicFormSectionAttributeSection where DynamicFormSectionAttributeSectionParentId=@DynamicFormSectionAttributeSectionParentId\r\n;";
                        query += "DELETE  FROM DynamicFormSectionAttributeSectionParent WHERE DynamicFormSectionAttributeSectionParentId = @DynamicFormSectionAttributeSectionParentId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<DynamicFormData> UpdateDynamicFormLocked(DynamicFormData value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormDataId", value.DynamicFormDataId);
                        parameters.Add("IsLocked", value.IsLocked == true ? true : null);
                        parameters.Add("LockedUserId", value.LockedUserId);
                        var query = "update DynamicFormData set IsLocked=@IsLocked,LockedUserID=@LockedUserId where DynamicFormDataId=@DynamicFormDataId\r\n;";
                        if (value.IsLocked != true)
                        {
                            query += "Delete from DynamicFormDataSectionLock where DynamicFormDataId=@DynamicFormDataId;";
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);


                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicFormApprovedChanged>> GetDynamicFormApprovedChangedList(long? DynamicFormApprovedID)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormApprovedID", DynamicFormApprovedID);
                var query = "select  * from DynamicFormApprovedChanged where DynamicFormApprovedID=@DynamicFormApprovedID order by DynamicFormApprovedChangedId desc;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormApprovedChanged>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormApprovedChanged> InsertDynamicFormApprovedChanged(DynamicFormApprovedChanged dynamicFormApprovedChanged)
        {
            try
            {
                var appList = await GetDynamicFormApprovedChangedList(dynamicFormApprovedChanged.DynamicFormApprovedID);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DynamicFormApprovedID", dynamicFormApprovedChanged.DynamicFormApprovedID);
                    parameters.Add("UserId", dynamicFormApprovedChanged.UserId);
                    parameters.Add("DynamicFormApprovedChangedId", dynamicFormApprovedChanged.DynamicFormApprovedChangedId);
                    bool? insert = true;
                    if (appList != null && appList.Count() > 0)
                    {
                        var firstRec = appList.FirstOrDefault();
                        if (firstRec?.UserId == dynamicFormApprovedChanged.UserId)
                        {
                            insert = false;
                        }
                    }
                    if (insert == true)
                    {
                        var query = "INSERT INTO DynamicFormApprovedChanged(DynamicFormApprovedID,UserId) OUTPUT INSERTED.DynamicFormApprovedChangedId VALUES " +
                            "(@DynamicFormApprovedID,@UserId)";
                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        dynamicFormApprovedChanged.DynamicFormApprovedChangedId = rowsAffected;
                    }
                    return dynamicFormApprovedChanged;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowApproval>> GetDynamicFormWorkFlowApprovalList(long? DynamicFormWorkFlowId, long? dynamicFormDataId)
        {
            try
            {
                List<DynamicFormWorkFlowApproval> dynamicFormWorkFlowApprovals = new List<DynamicFormWorkFlowApproval>();
                List<DynamicFormWorkFlowApprovedForm> dynamicFormWorkFlowApprovedForms = new List<DynamicFormWorkFlowApprovedForm>();
                List<DynamicFormWorkFlowApprovedFormChanged> dynamicFormWorkFlowApprovedFormChanged = new List<DynamicFormWorkFlowApprovedFormChanged>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormWorkFlowId", DynamicFormWorkFlowId);
                parameters.Add("DynamicFormDataID", dynamicFormDataId);
                var query = "select  t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as UserName from DynamicFormWorkFlowApproval t1 " +
                    "JOIN Employee t3 ON t1.UserID=t3.UserID where t1.DynamicFormWorkFlowId=@DynamicFormWorkFlowId order by t1.sortby asc;";
                if (dynamicFormDataId > 0)
                {
                    query += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t4.NickName is NULL OR  t4.NickName='' then  ''\r\n ELSE  CONCAT(t4.NickName,' | ') END,t4.FirstName, (case when t4.LastName is Null OR t4.LastName='' then '' ELSE '-' END),t4.LastName) ELSE null END) as UserName,t2.DynamicFormWorkFlowID,CASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus from DynamicFormWorkFlowApprovedForm t1 \r\n" +
                       "JOIN DynamicFormWorkFlowForm t2 ON t1.DynamicFormWorkFlowFormID=t2.DynamicFormWorkFlowFormID\r\n" +
                       "\r\nJOIN Employee t4 ON t1.UserID=t4.UserID " +
                       "Where t2.DynamicFormWorkFlowFormID=@DynamicFormWorkFlowId AND t2.DynamicFormDataID=@DynamicFormDataID;";
                    query += "select t1.*,(case when t1.UserID>0 Then CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) ELSE null END) as UserName  from DynamicFormWorkFlowApprovedFormChanged t1  JOIN Employee t3 ON t1.UserID=t3.UserID\r\n;";
                }
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    dynamicFormWorkFlowApprovals = results.Read<DynamicFormWorkFlowApproval>().ToList();
                    if (dynamicFormDataId > 0)
                    {
                        dynamicFormWorkFlowApprovedForms = results.Read<DynamicFormWorkFlowApprovedForm>().ToList();
                        dynamicFormWorkFlowApprovedFormChanged = results.Read<DynamicFormWorkFlowApprovedFormChanged>().ToList();
                    }
                }
                if (dynamicFormDataId > 0 && dynamicFormWorkFlowApprovedForms != null && dynamicFormWorkFlowApprovedForms.Count() > 0)
                {
                    dynamicFormWorkFlowApprovals = new List<DynamicFormWorkFlowApproval>();
                    dynamicFormWorkFlowApprovedForms.ForEach(s =>
                    {
                        var list = dynamicFormWorkFlowApprovedFormChanged.Where(w => w.DynamicFormWorkFlowApprovedFormID == s.DynamicFormWorkFlowApprovedFormID).OrderByDescending(o => o.DynamicFormWorkFlowApprovedFormChangedID).ToList();
                        DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval = new DynamicFormWorkFlowApproval();
                        dynamicFormWorkFlowApproval.DynamicFormWorkFlowId = s.DynamicFormWorkFlowFormID;
                        dynamicFormWorkFlowApproval.DynamicFormDataId = dynamicFormDataId;
                        dynamicFormWorkFlowApproval.ApprovedStatus = s.ApprovedStatus;
                        dynamicFormWorkFlowApproval.IsApproved = s.IsApproved;
                        dynamicFormWorkFlowApproval.ApprovedDate = s.ApprovedDate;
                        dynamicFormWorkFlowApproval.ApprovedDescription = s.ApprovedDescription;
                        dynamicFormWorkFlowApproval.UserId = s.UserID;
                        dynamicFormWorkFlowApproval.ApprovedUser = s.UserName;
                        dynamicFormWorkFlowApproval.UserName = s.UserName;
                        dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId = s.DynamicFormWorkFlowApprovedFormID;
                        dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovedFormID = s.DynamicFormWorkFlowApprovedFormID;
                        dynamicFormWorkFlowApproval.DelegateApproveAllUserName = s.UserName;
                        dynamicFormWorkFlowApproval.DelegateApproveAllUserId = s.UserID;
                        if (list != null && list.Count() > 0)
                        {
                            dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovedFormChangeds = list;
                            dynamicFormWorkFlowApproval.IsDelegateApproveStatus = list[0].IsApprovedStatus;
                            dynamicFormWorkFlowApproval.DelegateApprovedChangedId = list[0].DynamicFormWorkFlowApprovedFormChangedID;
                            dynamicFormWorkFlowApproval.DelegateApproveUserId = list[0].UserId;
                            dynamicFormWorkFlowApproval.DelegateApproveUserName = list[0].UserName;
                            dynamicFormWorkFlowApproval.ApprovedByUserID = list[0].UserId;
                            dynamicFormWorkFlowApproval.ApprovedUser = list[0].UserName;
                        }
                        dynamicFormWorkFlowApprovals.Add(dynamicFormWorkFlowApproval);
                    });
                }
                return dynamicFormWorkFlowApprovals;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public int? GeDynamicFormWorkFlowApprovalSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormWorkFlowId", id);

                query = "SELECT * FROM DynamicFormWorkFlowApproval Where  DynamicFormWorkFlowId = @DynamicFormWorkFlowId order by  SortBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormWorkFlowApproval>(query, parameters);
                    if (result != null)
                    {
                        SortOrderBy = result.SortBy + 1;
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowApproval> InsertDynamicFormWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DynamicFormWorkFlowId", dynamicFormWorkFlowApproval.DynamicFormWorkFlowId);
                    parameters.Add("UserId", dynamicFormWorkFlowApproval.UserId);
                    parameters.Add("DynamicFormWorkFlowApprovalId", dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId);
                    if (dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId > 0)
                    {
                        var query = "update DynamicFormWorkFlowApproval set DynamicFormWorkFlowId=@DynamicFormWorkFlowId,UserId=@UserId where DynamicFormWorkFlowApprovalId=@DynamicFormWorkFlowApprovalId\r\n;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var sortBy = GeDynamicFormWorkFlowApprovalSort(dynamicFormWorkFlowApproval.DynamicFormWorkFlowId);
                        parameters.Add("sortBy", sortBy);
                        var query = "INSERT INTO DynamicFormWorkFlowApproval(DynamicFormWorkFlowId,UserId,sortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovalId VALUES " +
                            "(@DynamicFormWorkFlowId,@UserId,@sortBy)";
                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId = rowsAffected;
                    }
                    return dynamicFormWorkFlowApproval;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<List<DynamicFormWorkFlowApproval>> UpdateDynamicFormWorkFlowApprovalSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormWorkFlowId", id);
                parameters.Add("SortBy", SortOrderBy);
                query = "SELECT * FROM DynamicFormWorkFlowApproval Where DynamicFormWorkFlowId = @DynamicFormWorkFlowId AND SortBy>@SortBy";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowApproval> DeleteDynamicFormWorkFlowApproval(DynamicFormWorkFlowApproval value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormWorkFlowApprovalId", value.DynamicFormWorkFlowApprovalId);
                        var query = "Delete from DynamicFormWorkFlowApproval where DynamicFormWorkFlowApprovalId=@DynamicFormWorkFlowApprovalId\r\n;";
                        var sortby = value.SortBy;
                        var result = await UpdateDynamicFormWorkFlowApprovalSort(value.DynamicFormWorkFlowId, value.SortBy);
                        if (result != null)
                        {
                            result.ForEach(s =>
                            {
                                query += "Update  DynamicFormWorkFlowApproval SET SortBy=" + sortby + "  WHERE DynamicFormWorkFlowApprovalId =" + s.DynamicFormWorkFlowApprovalId + ";";
                                sortby++;
                            });
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<List<DynamicFormWorkFlowApproval>> GetUpdateDynamicFormWorkFlowApprovalSortOrder(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormWorkFlowId", dynamicFormWorkFlowApproval.DynamicFormWorkFlowId);
                var from = dynamicFormWorkFlowApproval.SortOrderAnotherBy > dynamicFormWorkFlowApproval.SortBy ? dynamicFormWorkFlowApproval.SortBy : dynamicFormWorkFlowApproval.SortOrderAnotherBy;
                var to = dynamicFormWorkFlowApproval.SortOrderAnotherBy > dynamicFormWorkFlowApproval.SortBy ? dynamicFormWorkFlowApproval.SortOrderAnotherBy : dynamicFormWorkFlowApproval.SortBy;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT * FROM DynamicFormWorkFlowApproval Where  DynamicFormWorkFlowId = @DynamicFormWorkFlowId  AND SortBy>@SortOrderByFrom and SortBy<=@SortOrderByTo order by SortBy asc";

                if (dynamicFormWorkFlowApproval.SortOrderAnotherBy > dynamicFormWorkFlowApproval.SortBy)
                {
                    query = "SELECT * FROM DynamicFormWorkFlowApproval Where DynamicFormWorkFlowId = @DynamicFormWorkFlowId  AND SortBy>=@SortOrderByFrom and SortBy<@SortOrderByTo order by SortBy asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowApproval> UpdateDynamicFormWorkFlowApprovalSortOrder(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = string.Empty;
                        long? SortOrder = dynamicFormWorkFlowApproval.SortOrderAnotherBy > dynamicFormWorkFlowApproval.SortBy ? (dynamicFormWorkFlowApproval.SortBy + 1) : dynamicFormWorkFlowApproval.SortOrderAnotherBy;
                        query += "Update  DynamicFormWorkFlowApproval SET SortBy=" + dynamicFormWorkFlowApproval.SortBy + "  WHERE DynamicFormWorkFlowApprovalId =" + dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId + ";";
                        if (SortOrder > 0)
                        {
                            var result = await GetUpdateDynamicFormWorkFlowApprovalSortOrder(dynamicFormWorkFlowApproval);
                            if (result != null && result.Count > 0)
                            {

                                result.ForEach(s =>
                                {

                                    query += "Update  DynamicFormWorkFlowApproval SET SortBy=" + SortOrder + "  WHERE  DynamicFormWorkFlowApprovalId =" + s.DynamicFormWorkFlowApprovalId + ";";
                                    SortOrder++;
                                });

                            }
                        }


                        if (!string.IsNullOrEmpty(query))
                        {
                            var rowsAffected = await connection.ExecuteAsync(query, null);
                        }
                        return dynamicFormWorkFlowApproval;
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
        public async Task<DynamicFormWorkFlowApprovedForm> UpdateDynamicFormWorkFlowApprovedFormByUser(DynamicFormWorkFlowApprovedForm dynamicFormWorkFlowApprovedForm)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormWorkFlowApprovedFormID", dynamicFormWorkFlowApprovedForm.DynamicFormWorkFlowApprovedFormID);
                        parameters.Add("IsApproved", dynamicFormWorkFlowApprovedForm.IsApproved);
                        parameters.Add("ApprovedDate", dynamicFormWorkFlowApprovedForm.ApprovedDate, DbType.DateTime);
                        parameters.Add("ApprovedDescription", dynamicFormWorkFlowApprovedForm.ApprovedDescription, DbType.String);
                        parameters.Add("ApprovedByUserID", dynamicFormWorkFlowApprovedForm.ApprovedByUserID);
                        var query = "Update DynamicFormWorkFlowApprovedForm set ApprovedByUserID=@ApprovedByUserID,IsApproved=@IsApproved,ApprovedDate=@ApprovedDate,ApprovedDescription=@ApprovedDescription where DynamicFormWorkFlowApprovedFormID=@DynamicFormWorkFlowApprovedFormID\r\n;";
                        await connection.ExecuteAsync(query, parameters);
                        return dynamicFormWorkFlowApprovedForm;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowApprovedFormChanged>> GetDynamicFormWorkFlowApprovedFormChangedList(long? DynamicFormWorkFlowApprovedFormID)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormWorkFlowApprovedFormID", DynamicFormWorkFlowApprovedFormID);
                var query = "select  * from DynamicFormWorkFlowApprovedFormChanged where DynamicFormWorkFlowApprovedFormID=@DynamicFormWorkFlowApprovedFormID order by DynamicFormWorkFlowApprovedFormChangedID desc;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowApprovedFormChanged>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowApprovedFormChanged> InsertDynamicFormWorkFlowApprovedFormChanged(DynamicFormWorkFlowApprovedFormChanged dynamicFormWorkFlowApprovedFormChanged)
        {
            try
            {
                var appList = await GetDynamicFormWorkFlowApprovedFormChangedList(dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowApprovedFormID);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DynamicFormWorkFlowApprovedFormID", dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowApprovedFormID);
                    parameters.Add("UserId", dynamicFormWorkFlowApprovedFormChanged.UserId);
                    parameters.Add("DynamicFormWorkFlowApprovedFormChangedID", dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowApprovedFormChangedID);
                    bool? insert = true;
                    if (appList != null && appList.Count() > 0)
                    {
                        var firstRec = appList.FirstOrDefault();
                        if (firstRec?.UserId == dynamicFormWorkFlowApprovedFormChanged.UserId)
                        {
                            insert = false;
                        }
                    }
                    if (insert == true)
                    {
                        var query = "INSERT INTO DynamicFormWorkFlowApprovedFormChanged(DynamicFormWorkFlowApprovedFormID,UserId) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormChangedID VALUES " +
                            "(@DynamicFormWorkFlowApprovedFormID,@UserId)";
                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowApprovedFormChangedID = rowsAffected;
                    }
                    return dynamicFormWorkFlowApprovedFormChanged;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public int? GeDynamicFormDataWorkFlowApprovalSort(long? id)
        {
            try
            {
                int? SortOrderBy = 1;
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormWorkFlowFormId", id);

                query = "SELECT * FROM DynamicFormWorkFlowApprovedForm Where  DynamicFormWorkFlowFormId = @DynamicFormWorkFlowFormId order by  SortBy desc";
                using (var connection = CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<DynamicFormWorkFlowApproval>(query, parameters);
                    if (result != null)
                    {
                        SortOrderBy = result.SortBy + 1;
                    }
                }
                return SortOrderBy;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowApproval> InsertDynamicFormDataWorkFlowApproval(DynamicFormWorkFlowApproval dynamicFormWorkFlowApproval)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DynamicFormWorkFlowFormId", dynamicFormWorkFlowApproval.DynamicFormWorkFlowId);
                    parameters.Add("UserId", dynamicFormWorkFlowApproval.UserId);
                    parameters.Add("DynamicFormWorkFlowApprovedFormID", dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId);
                    if (dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId > 0)
                    {
                        var query = "update DynamicFormWorkFlowApprovedForm set DynamicFormWorkFlowFormId=@DynamicFormWorkFlowFormId,UserId=@UserId where DynamicFormWorkFlowApprovedFormID=@DynamicFormWorkFlowApprovedFormID\r\n;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var sortBy = GeDynamicFormDataWorkFlowApprovalSort(dynamicFormWorkFlowApproval.DynamicFormWorkFlowId);
                        parameters.Add("sortBy", sortBy);
                        var query = "INSERT INTO DynamicFormWorkFlowApprovedForm(DynamicFormWorkFlowFormId,UserId,sortBy) OUTPUT INSERTED.DynamicFormWorkFlowApprovedFormID VALUES " +
                            "(@DynamicFormWorkFlowFormId,@UserId,@sortBy)";
                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        dynamicFormWorkFlowApproval.DynamicFormWorkFlowApprovalId = rowsAffected;
                    }
                    return dynamicFormWorkFlowApproval;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<List<DynamicFormWorkFlowApproval>> UpdateDynamicFormDataWorkFlowApprovalSort(long? id, int? SortOrderBy)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormWorkFlowFormId", id);
                parameters.Add("SortBy", SortOrderBy);
                query = "SELECT * FROM DynamicFormWorkFlowApprovedForm Where DynamicFormWorkFlowFormId = @DynamicFormWorkFlowFormId AND SortBy>@SortBy";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowApproval> DeleteDynamicFormDataWorkFlowApproval(DynamicFormWorkFlowApproval value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormWorkFlowApprovedFormID", value.DynamicFormWorkFlowApprovalId);
                        var query = "Delete from DynamicFormWorkFlowApprovedForm where DynamicFormWorkFlowApprovedFormID=@DynamicFormWorkFlowApprovedFormID\r\n;";
                        var sortby = value.SortBy;
                        var result = await UpdateDynamicFormDataWorkFlowApprovalSort(value.DynamicFormWorkFlowId, value.SortBy);
                        if (result != null)
                        {
                            result.ForEach(s =>
                            {
                                query += "Update  DynamicFormWorkFlowApprovedForm SET SortOrderBy=" + sortby + "  WHERE DynamicFormWorkFlowApprovedFormID =" + s.DynamicFormWorkFlowApprovedFormID + ";";
                                sortby++;
                            });
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<DynamicFormWorkFlowFormDelegate> InsertDynamicFormWorkFlowFormDelegate(DynamicFormWorkFlowFormDelegate dynamicFormWorkFlowApprovedFormChanged)
        {
            try
            {
                var appList = await GetDynamicFormWorkFlowFormDelegateItemList(dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowFormID);
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DynamicFormWorkFlowFormID", dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowFormID);
                    parameters.Add("UserId", dynamicFormWorkFlowApprovedFormChanged.UserID);
                    parameters.Add("DynamicFormWorkFlowFormDelegateID", dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowFormDelegateID);
                    bool? insert = true;
                    if (appList != null && appList.Count() > 0)
                    {
                        var firstRec = appList.FirstOrDefault();
                        if (firstRec?.UserID == dynamicFormWorkFlowApprovedFormChanged.UserID)
                        {
                            insert = false;
                        }
                    }
                    if (insert == true)
                    {
                        var query = "INSERT INTO DynamicFormWorkFlowFormDelegate(DynamicFormWorkFlowFormID,UserId) OUTPUT INSERTED.DynamicFormWorkFlowFormDelegateID VALUES " +
                            "(@DynamicFormWorkFlowFormID,@UserId)";
                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        dynamicFormWorkFlowApprovedFormChanged.DynamicFormWorkFlowFormDelegateID = rowsAffected;
                    }
                    return dynamicFormWorkFlowApprovedFormChanged;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowFormDelegate>> GetDynamicFormWorkFlowFormDelegateList(long? DynamicFormWorkFlowFormID)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormWorkFlowFormID", DynamicFormWorkFlowFormID);
                var query = string.Empty;
                query += "select t1.*,CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  ''\r\n ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName, (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as UserName  from DynamicFormWorkFlowFormDelegate t1  JOIN Employee t3 ON t1.UserID=t3.UserID  where t1.DynamicFormWorkFlowFormID=@DynamicFormWorkFlowFormID order by t1.DynamicFormWorkFlowFormDelegateID desc";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowFormDelegate>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormWorkFlowFormDelegate>> GetDynamicFormWorkFlowFormDelegateItemList(long? DynamicFormWorkFlowFormID)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormWorkFlowFormID", DynamicFormWorkFlowFormID);
                var query = string.Empty;
                query += "select  * from DynamicFormWorkFlowFormDelegate where DynamicFormWorkFlowFormID=@DynamicFormWorkFlowFormID order by DynamicFormWorkFlowFormDelegateID desc;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowFormDelegate>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowForm> InsertDynamicFormWorkFlowForm(DynamicFormWorkFlowForm value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormWorkFlowFormId", value.DynamicFormWorkFlowId);
                        parameters.Add("DynamicFormDataId", value.DynamicFormDataId);
                        parameters.Add("SequenceNo", value.SequenceNo);
                        parameters.Add("UserId", value.UserId);
                        parameters.Add("IsAllowDelegateUserForm", value.IsAllowDelegateUser == true ? true : null);
                        parameters.Add("IsParallelWorkflow", value.IsParallelWorkflow == true ? true : null);
                        if (value.DynamicFormWorkFlowId > 0)
                        {
                            query = "UPDATE DynamicFormWorkFlowForm SET IsParallelWorkflow=@IsParallelWorkflow,IsAllowDelegateUserForm=@IsAllowDelegateUserForm,DynamicFormDataId=@DynamicFormDataId,UserId=@UserId,SequenceNo=@SequenceNo WHERE DynamicFormWorkFlowFormId = @DynamicFormWorkFlowFormId;\r\n";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        else
                        {
                            query = "INSERT INTO [DynamicFormWorkFlowForm](DynamicFormDataId,UserId,SequenceNo,IsAllowDelegateUserForm,IsParallelWorkflow) OUTPUT INSERTED.DynamicFormWorkFlowFormId " +
                                               "VALUES (@DynamicFormDataId,@UserId,@SequenceNo,@IsAllowDelegateUserForm,@IsParallelWorkflow);\r\n";
                            value.DynamicFormWorkFlowId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        if (value.DynamicFormWorkFlowId > 0)
                        {
                            var querys = string.Empty;
                            querys += "DELETE  FROM DynamicFormWorkFlowSectionForm WHERE DynamicFormWorkFlowFormId =" + value.DynamicFormWorkFlowId + ";";
                            if (value.SelectDynamicFormSectionIDs != null && value.SelectDynamicFormSectionIDs.Count() > 0)
                            {
                                foreach (var items in value.SelectDynamicFormSectionIDs)
                                {
                                    querys += "INSERT INTO [DynamicFormWorkFlowSectionForm](DynamicFormWorkFlowFormId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionFormID " +
                               "VALUES (" + value.DynamicFormWorkFlowId + "," + items + ");\r\n";
                                }
                                await connection.QuerySingleOrDefaultAsync<long>(querys);
                            }
                        }
                        return value;

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormWorkFlowForm> DeleteDynamicFormWorkFlowForm(DynamicFormWorkFlowForm dynamicFormWorkFlow)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("id", dynamicFormWorkFlow.DynamicFormWorkFlowId);
                        var query = string.Empty;
                        query += "DELETE FROM DynamicFormWorkFlowApprovedFormChanged where DynamicFormWorkFlowApprovedFormID in(select DynamicFormWorkFlowApprovedFormID FROM DynamicFormWorkFlowApprovedForm where DynamicFormWorkFlowFormID=@id);\r\n";
                        query += "DELETE  FROM DynamicFormWorkFlowFormDelegate WHERE DynamicFormWorkFlowFormId = @id;\n\r";
                        query += "DELETE  FROM DynamicFormWorkFlowSectionForm WHERE DynamicFormWorkFlowFormId = @id;\n\r";
                        query += "DELETE  FROM DynamicFormWorkFlowApprovedForm WHERE DynamicFormWorkFlowFormId = @id;\n\r";
                        query += "DELETE  FROM DynamicFormWorkFlowForm WHERE DynamicFormWorkFlowFormId = @id;\n\r";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormWorkFlow;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }


            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataSectionLock> GetDynamicFormDataSectionLockList(DynamicFormDataSectionLock dynamicFormDataSectionLock)
        {

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataId", dynamicFormDataSectionLock.DynamicFormDataId);
                parameters.Add("DynamicFormSectionId", dynamicFormDataSectionLock.DynamicFormSectionId);
                var query = string.Empty;
                query += "select  * from DynamicFormDataSectionLock where DynamicFormDataId=@DynamicFormDataId AND DynamicFormSectionId=@DynamicFormSectionId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormDataSectionLock>(query, parameters)).FirstOrDefault();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataSectionLock> UpdateDynamicFormDataSectionLock(DynamicFormDataSectionLock value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var exits = await GetDynamicFormDataSectionLockList(value);
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormDataId", value.DynamicFormDataId);
                        parameters.Add("DynamicFormSectionId", value.DynamicFormSectionId);
                        parameters.Add("IsLocked", value.IsLocked == true ? true : null);
                        parameters.Add("LockedUserId", value.LockedUserId);
                        if (exits == null)
                        {
                            var query = "INSERT INTO DynamicFormDataSectionLock(DynamicFormDataId,DynamicFormSectionId,IsLocked,LockedUserId) OUTPUT INSERTED.DynamicFormDataSectionLockId VALUES " +
                            "(@DynamicFormDataId,@DynamicFormSectionId,@IsLocked,@LockedUserId)";
                            var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        else
                        {
                            var query = "Delete from DynamicFormDataSectionLock where DynamicFormSectionId=@DynamicFormSectionId AND DynamicFormDataId=@DynamicFormDataId\r\n;";
                            // var query = "update DynamicFormDataSectionLock set IsLocked=@IsLocked,LockedUserID=@LockedUserId where DynamicFormSectionId=@DynamicFormSectionId AND DynamicFormDataId=@DynamicFormDataId\r\n;";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        }

                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicFormEmailSubCont>> GetDynamicFormEmailSubCont(Guid? SessionId)
        {
            try
            {
                List<DynamicFormEmailSubCont> dynamicFormEmailSubConts = new List<DynamicFormEmailSubCont>();
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.* from DynamicFormEmailSubCont t1 WHERE t1.DynamicFormSessionID=@SessionId order by t1.DynamicFormEmailSubContId asc;";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DynamicFormEmailSubCont>(query, parameters)).ToList();
                    dynamicFormEmailSubConts = result != null ? result : new List<DynamicFormEmailSubCont>();
                }
                return dynamicFormEmailSubConts;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormEmailSubCont> DeleteDynamicFormEmailSubCont(Guid? SessionId)
        {
            try
            {
                DynamicFormEmailSubCont dynamicFormReportItems = new DynamicFormEmailSubCont();
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSessionID", SessionId, DbType.Guid);
                        var querys = "DELETE  FROM DynamicFormEmailSubCont WHERE DynamicFormSessionID = @DynamicFormSessionID";
                        var rowsAffected = await connection.ExecuteAsync(querys, parameters);
                        dynamicFormReportItems.DynamicFormSessionID = SessionId;
                        return dynamicFormReportItems;

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormReportItems> InsertDynamicFormEmailSubCont(IEnumerable<DynamicFormReportItems> subjectData, IEnumerable<DynamicFormReportItems> contentData, Guid? SessionId)
        {
            try
            {
                DynamicFormReportItems dynamicFormReportItems = new DynamicFormReportItems();
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSessionID", SessionId, DbType.Guid);
                        var querys = "DELETE  FROM DynamicFormEmailSubCont WHERE DynamicFormSessionID = @DynamicFormSessionID";
                        var rowsAffected = await connection.ExecuteAsync(querys, parameters);
                        if (subjectData != null && subjectData.Count() > 0)
                        {
                            foreach (var items in subjectData)
                            {
                                var query = string.Empty;
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("AttrId", items.AttrId, DbType.String);
                                parameters1.Add("typeName", "Subject", DbType.String);
                                parameters1.Add("DynamicFormSessionID", SessionId, DbType.Guid);
                                parameters1.Add("LabelName", items.Label, DbType.String);
                                parameters1.Add("ValueName", items.Value, DbType.String);
                                query += "INSERT INTO [DynamicFormEmailSubCont](AttrID,TypeName,DynamicFormSessionID,LabelName,ValueName) OUTPUT INSERTED.DynamicFormEmailSubContID " +
                           "VALUES (@AttrId,@typeName,@DynamicFormSessionID,@LabelName,@ValueName);\r\n";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters1);
                            }
                        }
                        if (contentData != null && contentData.Count() > 0)
                        {
                            foreach (var items in contentData)
                            {
                                var query = string.Empty;
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("AttrId", items.AttrId, DbType.String);
                                parameters1.Add("typeName", "Content", DbType.String);
                                parameters1.Add("DynamicFormSessionID", SessionId, DbType.Guid);
                                parameters1.Add("LabelName", items.Label, DbType.String);
                                parameters1.Add("ValueName", items.Value, DbType.String);
                                query += "INSERT INTO [DynamicFormEmailSubCont](AttrID,TypeName,DynamicFormSessionID,LabelName,ValueName) OUTPUT INSERTED.DynamicFormEmailSubContID " +
                           "VALUES (@AttrId,@typeName,@DynamicFormSessionID,@LabelName,@ValueName);\r\n";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters1);
                            }

                        }
                        //if (!string.IsNullOrEmpty(query))
                        //{
                        //await connection.QuerySingleOrDefaultAsync<long>(query);
                        dynamicFormReportItems.AttrId = "1";
                        //}


                        return dynamicFormReportItems;

                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        private async Task<ApplicationPermission> GetApplicationPermissionTop1Async()
        {
            try
            {
                var query = "SELECT TOP 1 * FROM ApplicationPermission ORDER BY PermissionID DESC;";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ApplicationPermission>(query));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<ApplicationPermission>> GetApplicationPermissionAsync(long? ParentId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ParentId", ParentId);
                var query = "SELECT * FROM ApplicationPermission where ParentId=@ParentId ORDER BY ParentId DESC;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicForm> InsertDynamicFormPermissionPermission(DynamicForm dynamicForm)
        {
            try
            {
                var checkLink = await GetApplicationPermissionTop1Async();
                var checkParentLink = await GetApplicationPermissionAsync(60248);
                bool? isInsert = true;
                if (checkParentLink != null && checkParentLink.Count() > 0)
                {
                    var exits = checkParentLink.Where(f => f.PermissionURL != null && f.PermissionURL.ToLower() == dynamicForm.SessionID.ToString().ToLower()).Count();
                    isInsert = exits > 0 ? false : true;
                }
                if (isInsert == true)
                {
                    using (var connection = CreateConnection())
                    {
                        long? PermissionOrder = 1;
                        if (checkParentLink != null && checkParentLink.Count() > 0)
                        {
                            var checkPer = checkParentLink.OrderByDescending(o => o.PermissionOrder).FirstOrDefault()?.PermissionOrder;
                            if (!string.IsNullOrEmpty(checkPer))
                            {
                                PermissionOrder = long.Parse(checkPer) + 1;
                            }
                        }
                        long? permissionid = 0;
                        if (checkLink != null && checkLink.PermissionID > 0)
                        {
                            permissionid = (long)checkLink.PermissionID + 1;
                        }
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("PermissionID", permissionid);
                            parameters.Add("PermissionName", dynamicForm.Name, DbType.String);
                            parameters.Add("ParentID", 60248);
                            parameters.Add("PermissionLevel", 1);
                            parameters.Add("PermissionOrder", PermissionOrder);
                            parameters.Add("IsHeader", true);
                            parameters.Add("IsNewPortal", true);
                            parameters.Add("PermissionURL", dynamicForm.SessionID);
                            parameters.Add("Name", "dynamicFormMenuList", DbType.String);
                            parameters.Add("Component", "dynamicFormMenuList", DbType.String);
                            parameters.Add("IsPermissionURL", null);

                            var query = @"INSERT INTO ApplicationPermission
                                 (PermissionID,PermissionName,ParentID,PermissionLevel,PermissionOrder,IsDisplay,IsHeader,IsNewPortal,IsCmsApp,IsMobile,IsPermissionURL,
                                  Component,Name,PermissionURL)
                                  VALUES (@PermissionID,@PermissionName,@ParentID,@PermissionLevel,@PermissionOrder,1,@IsHeader,1,1,0,@IsPermissionURL,@Component,@Name,@PermissionURL)";
                            var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        }
                        catch (Exception exp)
                        {
                            throw new Exception(exp.Message, exp);
                        }

                    }
                }
                return dynamicForm;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<List<ApplicationPermission>> GetUpdateDynamicFormMenuSortOrder(ApplicationPermission dynamicFormSectionAttribute)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("PermissionID", dynamicFormSectionAttribute.PermissionID);
                int PermissionAnotherOrder = Int32.Parse(dynamicFormSectionAttribute.PermissionAnotherOrder);
                int PermissionOrder = Int32.Parse(dynamicFormSectionAttribute.PermissionOrder);
                var from = PermissionAnotherOrder > PermissionOrder ? PermissionOrder : PermissionAnotherOrder;
                var to = PermissionAnotherOrder > PermissionOrder ? PermissionAnotherOrder : PermissionOrder;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "SELECT * FROM ApplicationPermission Where  ParentID=60248  AND PermissionOrder>@SortOrderByFrom and PermissionOrder<=@SortOrderByTo order by PermissionOrder asc";

                if (PermissionAnotherOrder > PermissionOrder)
                {
                    query = "SELECT * FROM ApplicationPermission Where  ParentID=60248  AND PermissionOrder>=@SortOrderByFrom and PermissionOrder<@SortOrderByTo order by PermissionOrder asc";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationPermission>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ApplicationPermission> UpdateDynamicFormMenuSortOrder(ApplicationPermission dynamicFormSectionAttribute)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = string.Empty;
                        int PermissionAnotherOrder = Int32.Parse(dynamicFormSectionAttribute.PermissionAnotherOrder);
                        int PermissionOrder = Int32.Parse(dynamicFormSectionAttribute.PermissionOrder);
                        int? SortOrder = PermissionAnotherOrder > PermissionOrder ? (PermissionOrder + 1) : PermissionAnotherOrder;
                        query += "Update  ApplicationPermission SET PermissionOrder=" + PermissionOrder + "  WHERE PermissionID =" + dynamicFormSectionAttribute.PermissionID + ";";
                        if (SortOrder > 0)
                        {
                            var result = await GetUpdateDynamicFormMenuSortOrder(dynamicFormSectionAttribute);
                            if (result != null && result.Count > 0)
                            {

                                result.ForEach(s =>
                                {

                                    query += "Update  ApplicationPermission SET PermissionOrder=" + SortOrder + "  WHERE  PermissionID =" + s.PermissionID + ";";
                                    SortOrder++;
                                });

                            }
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, null);
                        return dynamicFormSectionAttribute;
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

        public async Task<IReadOnlyList<DynamicFormSectionAttrFormulaFunction>> GetDynamicFormSectionAttrFormulaFunction(long? DynamicFormSectionAttributeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionAttributeId", DynamicFormSectionAttributeId);
                var query = "SELECT t1.*,t2.Type,t2.FormulaFunctionName FROM DynamicFormSectionAttrFormulaFunction t1 JOIN DynamicFormSectionAttrFormulaMasterFunction t2 ON t1.DynamicFormSectionAttrFormulaMasterFuntionId=t2.MasterID where t1.DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId ORDER BY t1.DynamicFormSectionAttrFormulaFunctionId asc;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttrFormulaFunction>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionAttrFormulaFunction> InsertOrUpdateDynamicFormSectionAttrFormulaFunction(DynamicFormSectionAttrFormulaFunction value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("DynamicFormSectionAttributeId", value.DynamicFormSectionAttributeId);
                    parameters.Add("DynamicFormSectionAttrFormulaMasterFuntionId", value.DynamicFormSectionAttrFormulaMasterFuntionId);
                    parameters.Add("AValue", value.AValue);
                    parameters.Add("BValue", value.BValue);
                    parameters.Add("ColorValue", value.ColorValue, DbType.String);
                    parameters.Add("IsBValueEnabled", value.IsBValueEnabled == true ? true : false);
                    parameters.Add("DynamicFormSectionAttrFormulaFunctionId", value.DynamicFormSectionAttrFormulaFunctionId);
                    if (value.DynamicFormSectionAttrFormulaFunctionId > 0)
                    {
                        var query = "UPDATE DynamicFormSectionAttrFormulaFunction SET ColorValue=@ColorValue,IsBValueEnabled=@IsBValueEnabled,DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId,DynamicFormSectionAttrFormulaMasterFuntionId = @DynamicFormSectionAttrFormulaMasterFuntionId,AValue =@AValue,BValue =@BValue\r" +
                            "WHERE DynamicFormSectionAttrFormulaFunctionId = @DynamicFormSectionAttrFormulaFunctionId";

                        await connection.ExecuteAsync(query, parameters);
                    }
                    else
                    {
                        var query = "INSERT INTO DynamicFormSectionAttrFormulaFunction(ColorValue,IsBValueEnabled,DynamicFormSectionAttributeId,DynamicFormSectionAttrFormulaMasterFuntionId,AValue,BValue) OUTPUT INSERTED.DynamicFormSectionAttrFormulaFunctionId VALUES " +
                            "(@ColorValue,@IsBValueEnabled,@DynamicFormSectionAttributeId,@DynamicFormSectionAttrFormulaMasterFuntionId,@AValue,@BValue)";

                        var rowsAffected = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        value.DynamicFormSectionAttrFormulaFunctionId = rowsAffected;
                    }
                    return value;

                }

            }

            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<DynamicFormSectionAttrFormulaMasterFunction>> GetDynamicFormSectionAttrFormulaMasterFunction()
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = "SELECT * FROM DynamicFormSectionAttrFormulaMasterFunction ORDER BY MasterID asc;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttrFormulaMasterFunction>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormSectionAttrFormulaFunction> DeleteDynamicFormSectionAttrFormulaFunction(DynamicFormSectionAttrFormulaFunction dynamicForm)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSectionAttrFormulaFunctionId", dynamicForm.DynamicFormSectionAttrFormulaFunctionId);
                        var query = "Delete from  DynamicFormSectionAttrFormulaFunction  WHERE DynamicFormSectionAttrFormulaFunctionId = @DynamicFormSectionAttrFormulaFunctionId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicForm;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataAssignUser>> GetDynamicFormDataAssignUserList(long? DynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", DynamicFormId);
                var query = "select t1.AddedDate,tt6.UserName as AddedBy,t1.DynamicFormDataAssignUserId,\r\nt1.DynamicFormId,\r\nt1.UserID,\r\nt1.UserGroupID,\r\nt1.LevelID,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL OR  t6.NickName='' then  ''\r\n ELSE  CONCAT(t6.NickName,' | ') END,t6.FirstName, (case when t6.LastName is Null OR t6.LastName='' then '' ELSE '-' END),t6.LastName) as FullName\r\n" +
                    "from DynamicFormDataAssignUser t1\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN DynamicForm t4 ON t4.ID=t1.DynamicFormId\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "Left JOIN ApplicationUser tt6 ON t1.AddedByUserId=tt6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE t1.DynamicFormId=@DynamicFormId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormDataAssignUser>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataAssignUser>> GetDynamicFormDataAssignUserAllList()
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = "SELECT t1.*,t2.SessionID FROM DynamicFormDataAssignUser t1 JOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID WHERE (t2.IsDeleted is null OR t2.IsDeleted=0);";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormDataAssignUser>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataAssignUser>> GetDynamicFormDataAssignUserEmptyAsync(long? DynamicFormId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", DynamicFormId);
                var query = "select  * from DynamicFormDataAssignUser where  DynamicFormId=@DynamicFormId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormDataAssignUser>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> DeleteDynamicFormDataAssignUser(long? Id, List<long?> Ids)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();

                        if (Ids != null && Ids.Count > 0)
                        {
                            string IdList = string.Join(",", Ids);
                            query += "Delete From DynamicFormDataAssignUser WHERE DynamicFormDataAssignUserId in (" + IdList + ");\r\n";
                        }
                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return Id.GetValueOrDefault(0);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataAssignUser> InsertDynamicFormDataAssignUser(DynamicFormDataAssignUser value)
        {
            try
            {
                var userGroupUsers = await GetUserGroupUserList();
                var LevelUsers = await GetLeveMasterUsersList(value.SelectLevelMasterIDs);
                var userExitsRoles = await GetDynamicFormDataAssignUserEmptyAsync(value.DynamicFormId);
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormId", value.DynamicFormId);
                        parameters.Add("Type", value.Type);
                        parameters.Add("AddedByUserId", value.AddedByUserId);
                        parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                        if (value.Type == "User")
                        {
                            if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                            {
                                foreach (var item in value.SelectUserIDs)
                                {
                                    var counts = userExitsRoles.Where(w => w.UserId == item).FirstOrDefault();
                                    if (counts == null)
                                    {
                                        query += "INSERT INTO [DynamicFormDataAssignUser](AddedByUserId,AddedDate,Type,DynamicFormId,UserId) OUTPUT INSERTED.DynamicFormDataAssignUserId " +
                                        "VALUES (@AddedByUserId,@AddedDate,@Type,@DynamicFormId," + item + ");\r\n";
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
                                        var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                        if (counts == null)
                                        {
                                            query += "INSERT INTO [DynamicFormDataAssignUser](AddedByUserId,AddedDate,Type,DynamicFormId,UserId,UserGroupId) OUTPUT INSERTED.DynamicFormDataAssignUserId " +
                                                "VALUES (@AddedByUserId,@AddedDate,@Type,@DynamicFormId," + s.UserId + "," + s.UserGroupId + ");\r\n";
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
                                    var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                    if (counts == null)
                                    {
                                        query += "INSERT INTO [DynamicFormDataAssignUser](AddedByUserId,AddedDate,Type,DynamicFormId,UserId,LevelId) OUTPUT INSERTED.DynamicFormDataAssignUserId " +
                                           "VALUES (@AddedByUserId,@AddedDate,@Type,@DynamicFormId," + s.UserId + "," + s.LevelId + ");\r\n";
                                    }
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return value;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<DynamicFormSectionAttribute>> GetUpdateDynamicFormGridSeqSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DynamicFormId", dynamicFormSectionAttribute.DynamicFormId);
                int? PermissionAnotherOrder = dynamicFormSectionAttribute.SortOrderAnotherBy;
                int? PermissionOrder = dynamicFormSectionAttribute.GridDisplaySeqNo;
                var from = PermissionAnotherOrder > PermissionOrder ? PermissionOrder : PermissionAnotherOrder;
                var to = PermissionAnotherOrder > PermissionOrder ? PermissionAnotherOrder : PermissionOrder;
                parameters.Add("SortOrderByFrom", from);
                parameters.Add("SortOrderByTo", to);
                query = "select t1.* from DynamicFormSectionAttribute t1 \r\n" +
                    "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t8.AttributeHeaderDataSourceID=t6.DataSourceId\r\n" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n LEFT JOIN DynamicForm tt1 ON tt1.ID=t5.DynamicFormID\n\r" +
                    "LEFT JOIN DynamicFormFilter tt4 ON tt4.DynamicFilterID=t6.FilterDataSocurceID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t10 ON t10.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                    "Where (tt1.IsDeleted=0 or tt1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) " +
                    "AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null) " +
                    "AND tt1.ID=@DynamicFormId AND t1.GridDisplaySeqNo>@SortOrderByFrom\n" +
                    "and t1.GridDisplaySeqNo<=@SortOrderByTo order by t1.GridDisplaySeqNo asc";
                if (PermissionAnotherOrder > PermissionOrder)
                {
                    query = "select t1.* from DynamicFormSectionAttribute t1 \r\n" +
                    "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t8.AttributeHeaderDataSourceID=t6.DataSourceId\r\n" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n LEFT JOIN DynamicForm tt1 ON tt1.ID=t5.DynamicFormID\n\r" +
                    "LEFT JOIN DynamicFormFilter tt4 ON tt4.DynamicFilterID=t6.FilterDataSocurceID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t10 ON t10.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                    "Where (tt1.IsDeleted=0 or tt1.IsDeleted is null) AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) " +
                    "AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null) " +
                    "AND tt1.ID=@DynamicFormId AND t1.GridDisplaySeqNo>=@SortOrderByFrom\n" +
                    "and t1.GridDisplaySeqNo<@SortOrderByTo order by t1.GridDisplaySeqNo asc";

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
        public async Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeGridSequenceSortOrder(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var query = string.Empty;
                        int? PermissionAnotherOrder = dynamicFormSectionAttribute.SortOrderAnotherBy;
                        int? PermissionOrder = dynamicFormSectionAttribute.GridDisplaySeqNo;
                        int? SortOrder = PermissionAnotherOrder > PermissionOrder ? (PermissionOrder + 1) : PermissionAnotherOrder;
                        query += "Update  DynamicFormSectionAttribute SET GridDisplaySeqNo=" + PermissionOrder + "  WHERE DynamicFormSectionAttributeId =" + dynamicFormSectionAttribute.DynamicFormSectionAttributeId + ";";
                        if (SortOrder > 0)
                        {
                            var result = await GetUpdateDynamicFormGridSeqSortOrder(dynamicFormSectionAttribute);
                            if (result != null && result.Count > 0)
                            {

                                result.ForEach(s =>
                                {
                                    query += "Update  DynamicFormSectionAttribute SET GridDisplaySeqNo=" + SortOrder + "  WHERE  DynamicFormSectionAttributeId =" + s.DynamicFormSectionAttributeId + ";";
                                    SortOrder++;
                                });

                            }
                        }
                        var rowsAffected = await connection.ExecuteAsync(query, null);
                        return dynamicFormSectionAttribute;
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
        public async Task<DynamicFormSectionAttribute> UpdateDynamicFormSectionAttributeAllByCheckBox(DynamicFormSectionAttribute dynamicFormSectionAttribute)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormSectionAttributeId", dynamicFormSectionAttribute.DynamicFormSectionAttributeId);
                        parameters.Add("IsDisplayTableHeader", dynamicFormSectionAttribute.IsDisplayTableHeader);
                        parameters.Add("IsVisible", dynamicFormSectionAttribute.IsVisible);
                        parameters.Add("IsDisplayDropDownHeader", dynamicFormSectionAttribute.IsDisplayDropDownHeader);
                        var query = "Update DynamicFormSectionAttribute SET IsDisplayTableHeader=@IsDisplayTableHeader,IsVisible=@IsVisible,IsDisplayDropDownHeader=@IsDisplayDropDownHeader WHERE DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormSectionAttribute;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataAudit>> GetDynamicFormDataAuditList(Guid? SessionId)
        {
            try
            {
                List<DynamicFormDataAudit> dynamicFormDataAudits = new List<DynamicFormDataAudit>();
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "Select Row_Number() Over (Order By tt1.DynamicFormDataAuditId desc) As RowNum,tt1.*,tt5.SessionID as DynamicFormSessionID,tt2.DynamicFormID,tt3.UserName as PreUser,tt4.UserName as AuditUser from (select t1.Sessionid,\r\n" +
                    "(select TOP(1) t7.AuditDateTime from DynamicFormDataAudit t7 where t7.Sessionid=t1.Sessionid order by t7.DynamicFormDataAuditId asc)as AuditDateTime,\r\n" +
                    "(select TOP(1) t6.PreUpdateDate from DynamicFormDataAudit t6 where t6.Sessionid=t1.Sessionid order by t6.DynamicFormDataAuditId asc)as PreUpdateDate,\r\n" +
                    "(select TOP(1) t4.PreUserID from DynamicFormDataAudit t4 where t4.Sessionid=t1.Sessionid order by t4.DynamicFormDataAuditId asc)as PreUserID,\r\n" +
                    "(select TOP(1) t5.AuditUserId from DynamicFormDataAudit t5 where t5.Sessionid=t1.Sessionid order by t5.DynamicFormDataAuditId asc)as AuditUserId,\r\n" +
                    "(select TOP(1) t3.DynamicFormDataId from DynamicFormDataAudit t3 where t3.Sessionid=t1.Sessionid order by t3.DynamicFormDataAuditId asc)as DynamicFormDataId,\r\n" +
                    "(select TOP(1) t2.DynamicFormDataAuditId from DynamicFormDataAudit t2 where t2.Sessionid=t1.Sessionid order by t2.DynamicFormDataAuditId asc)as DynamicFormDataAuditId from DynamicFormDataAudit t1  group by t1.Sessionid)tt1\r\n" +
                    "JOIN DynamicFormData tt2 ON tt2.DynamicFormDataID=tt1.DynamicFormDataId\r\nJOIN DynamicForm tt5 ON tt5.ID=tt2.DynamicFormID\n\r" +
                    "JOIN ApplicationUser tt3 ON tt3.UserID=tt1.PreUserID\r\n" +
                    "JOIN ApplicationUser tt4 ON tt4.UserID=tt1.AuditUserId where tt2.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    dynamicFormDataAudits = (await connection.QueryAsync<DynamicFormDataAudit>(query, parameters)).ToList();
                }
                return dynamicFormDataAudits;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataAudit>> GetDynamicFormDataAuditBySessionList(Guid? SessionId)
        {
            try
            {
                List<DynamicFormDataAudit> dynamicFormDataAudits = new List<DynamicFormDataAudit>();
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t2.IsDeleted,Row_Number() Over (Order By t1.DynamicFormDataAuditId desc) As RowNum,t1.*,t2.DisplayName,t3.UserName as AuditUser,t4.UserName as PreUser from DynamicFormDataAudit t1 JOIN DynamicFormSectionAttribute t2 ON t1.DynamicFormSectionAttributeID=t2.DynamicFormSectionAttributeID\r\nJOIN ApplicationUser t3 ON t3.UserID=t1.AuditUserID JOIN ApplicationUser t4 ON t4.UserID=t1.PreUserID where t1.SessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    dynamicFormDataAudits = (await connection.QueryAsync<DynamicFormDataAudit>(query, parameters)).ToList();
                }
                return dynamicFormDataAudits;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormFormulaMathFun>> GetDynamicFormFormulaMathFunList()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var query = "select * from DynamicFormFormulaMathFun";
                    try
                    {
                        var result = (await connection.QueryAsync<DynamicFormFormulaMathFun>(query)).ToList();
                        return result;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionSecurity>> GetDynamicFormSectionSecuritySettingList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", Id);
                var query = "select t9.Name as SectionName,t1.DynamicFormSectionSecurityID,\r\nt1.DynamicFormSectionID,\r\nt1.UserID,\r\nt1.UserGroupID,\r\nt1.LevelID,\r\nt1.IsReadWrite,\r\nt1.IsReadOnly,\r\nt1.IsVisible,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.SectionName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL OR  t6.NickName='' then  ''\r\n ELSE  CONCAT(t6.NickName,' | ') END,t6.FirstName, (case when t6.LastName is Null OR t6.LastName='' then '' ELSE '-' END),t6.LastName) as FullName\r\n" +
                    "from DynamicFormSectionSecurity t1\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN DynamicFormSection t4 ON t4.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                     "LEFT JOIN DynamicForm t9 ON t9.ID=t4.DynamicFormId\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE  (t4.IsDeleted=0 or t4.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null) AND t9.Id=@DynamicFormSectionId";
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
        public async Task<IReadOnlyList<DynamicFormWorkFlowApproval>> GetDynamicFormWorkFlowApprovalSettingList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormID", Id);
                var query = "select t1.*,t2.IsAllowDelegateUser,t2.SequenceNo,t2.IsParallelWorkflow,t2.DynamicFormID,\r\nDynamicFormSectionName = STUFF(( SELECT ',' + CAST(tt2.SectionName AS VARCHAR(MAX)) from DynamicFormWorkFlowSection tt1 JOIN DynamicFormSection tt2 ON tt2.DynamicFormSectionID=tt1.DynamicFormSectionID JOIN DynamicForm tt3 ON tt3.ID=tt2.DynamicFormID WHERE tt1.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID AND  (tt3.IsDeleted=0 OR tt3.IsDeleted IS NULL) AND (tt2.IsDeleted=0 OR tt2.IsDeleted IS NULL)\r\n   Order by tt1.DynamicFormWorkFlowSectionID asc FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''),\r\nCONCAT(case when t6.NickName is NULL  then  t6.FirstName  ELSE   t6.NickName END,' | ',t6.LastName) as UserName from DynamicFormWorkFlowApproval t1\r\nJOIN DynamicFormWorkFlow t2 ON t1.DynamicFormWorkFlowID=t2.DynamicFormWorkFlowID  \r\nJOIN Employee t6 ON t1.UserID=t6.UserID\r\nJOIN DynamicForm t3 ON t3.ID=t2.DynamicFormID\r\nwhere  t2.DynamicFormId=@DynamicFormID AND (t3.IsDeleted=0 OR IsDeleted IS NULL)\r\n\r\n";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowApproval>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttributeSecurity>> GetDynamicFormSectionAttributeSecuritySettingList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", Id);
                var query = "select tt4.SectionName,t1.DynamicFormSectionAttributeSecurityID,\r\nt1.DynamicFormSectionAttributeID,\r\nt1.UserID,\r\nt1.UserGroupID,\r\nt1.LevelID,\r\nt1.IsAccess,\r\nt1.IsViewFormatOnly,\r\nt1.UserType,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.DisplayName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL OR  t6.NickName='' then  ''\r\n ELSE  CONCAT(t6.NickName,' | ') END,t6.FirstName, (case when t6.LastName is Null OR t6.LastName='' then '' ELSE '-' END),t6.LastName) as FullName\r\n" +
                    "from DynamicFormSectionAttributeSecurity t1\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN DynamicFormSectionAttribute t4 ON t4.DynamicFormSectionAttributeId=t1.DynamicFormSectionAttributeId\r\n" +
                    "LEFT JOIN DynamicFormSection tt4 ON tt4.DynamicFormSectionID=t4.DynamicFormSectionID\r\n" +
                    "LEFT JOIN DynamicForm ttt4 ON ttt4.ID=tt4.DynamicFormId\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE (t4.IsDeleted is null OR t4.IsDeleted=0) AND (tt4.IsDeleted is null OR tt4.IsDeleted=0) AND (ttt4.IsDeleted is null OR ttt4.IsDeleted=0) AND  ttt4.ID=@DynamicFormId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormSectionAttributeSecurity>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormSectionAttributeSectionParent>> GetDynamicFormSectionAttributeSectionParentSettings(long? dynamicFormSectionAttributeId)
        {
            try
            {
                List<DynamicFormSectionAttributeSectionParent> _dynamicFormSectionAttributeSectionParent = new List<DynamicFormSectionAttributeSectionParent>();
                List<DynamicFormSectionAttributeSection> _dynamicFormSectionAttributeSection = new List<DynamicFormSectionAttributeSection>();
                List<DynamicFormSection> DynamicFormSection = new List<DynamicFormSection>();
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormSectionAttributeId);
                var query = "select t2.ApplicationMasterIDs,t5.DropDownTypeID,t5.ControlTypeID,t2.AttributeID,t5.DataSourceID,t6.DataSourceTable,t4.Name as FormName,t3.DynamicFormID,t3.DynamicFormSectionID, t3.SectionName,t2.DisplayName, t1.SequenceNo,t1.DynamicFormSectionAttributeSectionParentID,t1.DynamicFormSectionAttributeID from DynamicFormSectionAttributeSectionParent t1\r\nJOIN DynamicFormSectionAttribute t2 ON t2.DynamicFormSectionAttributeID=t1.DynamicFormSectionAttributeID\r\nJOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t2.DynamicFormSectionID\r\nJOIN DynamicForm t4 ON t4.ID=t3.DynamicFormID \r\nJOIN AttributeHeader t5 ON t5.AttributeID=t2.AttributeID\r\nJOIN AttributeHeaderDataSource t6 ON t6.AttributeHeaderDataSourceID=t5.DataSourceID\r\nWhere  t4.Id=@DynamicFormId AND (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t5.AttributeIsVisible=1 or t5.AttributeIsVisible is NULL) AND (t2.IsDeleted is null OR t2.IsDeleted=0) AND (t3.IsDeleted is null OR t3.IsDeleted=0) AND (t4.IsDeleted is null OR t4.IsDeleted=0)\r\n";
                query += "select t1.DynamicFormSectionAttributeSectionID,\r\nt1.DynamicFormSectionAttributeSectionParentID,\r\nt1.DynamicFormSectionID,\r\nt1.DynamicFormSectionSelectionID,\r\nt1.DynamicFormSectionSelectionByID,t2.DynamicFormSectionAttributeId from DynamicFormSectionAttributeSection t1 JOIN DynamicFormSectionAttributeSectionParent t2 ON t1.DynamicFormSectionAttributeSectionParentId=t2.DynamicFormSectionAttributeSectionParentId\r\n;"; ;
                query += "select t2.* from DynamicFormSection t2 where (t2.IsDeleted is null OR t2.IsDeleted=0) AND t2.DynamicFormId=@DynamicFormId;";
                using (var connection = CreateConnection())
                {
                    var results = await connection.QueryMultipleAsync(query, parameters);
                    _dynamicFormSectionAttributeSectionParent = results.Read<DynamicFormSectionAttributeSectionParent>().ToList();
                    _dynamicFormSectionAttributeSection = results.Read<DynamicFormSectionAttributeSection>().ToList();
                    DynamicFormSection = results.Read<DynamicFormSection>().ToList();
                }
                if (_dynamicFormSectionAttributeSectionParent != null && _dynamicFormSectionAttributeSectionParent.Count() > 0)
                {
                    _dynamicFormSectionAttributeSectionParent.ForEach(s =>
                    {
                        s.DynamicFormSectionAttributeSections = _dynamicFormSectionAttributeSection.Where(w => w.DynamicFormSectionAttributeSectionParentId == s.DynamicFormSectionAttributeSectionParentId).ToList();
                        s.DynamicFormSectionIds = s.DynamicFormSectionAttributeSections.Select(a => a.DynamicFormSectionId).Distinct().ToList();
                        s.DynamicSectionName = string.Join(",", DynamicFormSection.Where(w => s.DynamicFormSectionIds.Contains(w.DynamicFormSectionId)).Select(a => a.SectionName).Distinct().ToList());
                        s.ShowSectionVisibleDataIds = s.DynamicFormSectionAttributeSections.Select(a => a.DynamicFormSectionSelectionById).Distinct().ToList();
                    });
                }
                return _dynamicFormSectionAttributeSectionParent;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DynamicFormDataAttrUpload>> GetDynamicFormDataAttrUpload(long? DynamicFormSectionAttributeId, long? DynamicFormDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionAttributeID", DynamicFormSectionAttributeId);
                parameters.Add("DynamicFormDataId", DynamicFormDataId);
                var query = "select t1.*,t2.DisplayName,CONCAT(case when t3.NickName is NULL OR  t3.NickName='' then  '' ELSE  CONCAT(t3.NickName,' | ') END,t3.FirstName , (case when t3.LastName is Null OR t3.LastName='' then '' ELSE '-' END),t3.LastName) as UploadedUser\r\nfrom DynamicFormDataAttrUpload t1 " +
                    "JOIN DynamicFormSectionAttribute t2 ON t2.DynamicFormSectionAttributeID=t1.DynamicFormSectionAttributeID AND (t1.IsDeleted is null OR t1.IsDeleted=0) " +
                    "JOIN Employee t3 ON t3.UserID=t1.UploadedUserID Where t1.DynamicFormDataId=@DynamicFormDataId AND t1.DynamicFormSectionAttributeID=@DynamicFormSectionAttributeId;";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormDataAttrUpload>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataAttrUpload> InsertDynamicFormDataAttrUpload(List<DynamicFormDataAttrUpload> value)
        {
            try
            {
                DynamicFormDataAttrUpload dynamicFormDataAttrUpload = new DynamicFormDataAttrUpload();
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (value != null && value.Count() > 0)
                        {
                            foreach (var item in value)
                            {
                                var query = string.Empty;
                                var parameters = new DynamicParameters();
                                parameters.Add("DynamicFormSectionAttributeId", item.DynamicFormSectionAttributeId);
                                parameters.Add("DynamicFormDataId", item.DynamicFormDataId);
                                parameters.Add("ImageData", item.ImageData, DbType.Binary);
                                parameters.Add("UploadedUserId", item.UploadedUserId);
                                parameters.Add("ImageType", item.ImageType, DbType.String);
                                parameters.Add("FileSize", item.FileSize, DbType.Decimal);
                                parameters.Add("UploadDate", DateTime.Now, DbType.DateTime);
                                parameters.Add("FileSizes", item.FileSizes, DbType.String);
                                parameters.Add("FileName", item.FileName, DbType.String);
                                parameters.Add("SessionId", item.SessionId, DbType.Guid);
                                query += "INSERT INTO [DynamicFormDataAttrUpload](FileName,DynamicFormDataId,ImageData,UploadedUserId,ImageType,DynamicFormSectionAttributeId,FileSize,UploadDate,FileSizes,SessionId) OUTPUT INSERTED.DynamicFormDataAttrUploadId " +
                                "VALUES (@FileName,@DynamicFormDataId,@ImageData,@UploadedUserId,@ImageType,@DynamicFormSectionAttributeId,@FileSize,@UploadDate,@FileSizes,@SessionId);\r\n";
                                var result = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                dynamicFormDataAttrUpload.DynamicFormDataAttrUploadId = result;
                            }
                        }
                        return dynamicFormDataAttrUpload;
                    }
                    catch (Exception exp)
                    {
                        throw new Exception(exp.Message, exp);
                    }
                }

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DynamicFormDataAttrUpload> DeleteDynamicFormDataAttrUpload(DynamicFormDataAttrUpload dynamicFormDataAttrUpload)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormDataAttrUploadId", dynamicFormDataAttrUpload.DynamicFormDataAttrUploadId);
                        var query = "Delete from DynamicFormDataAttrUpload where DynamicFormDataAttrUploadId=@DynamicFormDataAttrUploadId;";
                        var rowsAffected = await connection.ExecuteAsync(query, parameters);
                        return dynamicFormDataAttrUpload;
                    }
                    catch (Exception exp)
                    {
                        throw (new ApplicationException(exp.Message));
                    }
                }


            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }
        }
    }
}
