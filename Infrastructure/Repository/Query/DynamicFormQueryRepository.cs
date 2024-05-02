using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Helpers;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Xpo.DB.Helpers;
using Google.Cloud.Firestore;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Values;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<long> Delete(long id)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("id", id);

                        //var query = "Delete from DynamicFormWorkFlowSection where DynamicFormWorkFlowID in(select DynamicFormWorkFlowID from DynamicFormWorkFlow where DynamicFormID=@id)\r\n;";
                        //query += "DELETE  FROM DynamicFormApproval WHERE DynamicFormID = @id;";
                        //query += "DELETE  FROM DynamicFormWorkFlow WHERE DynamicFormID = @id;";
                        //query += "DELETE  FROM DynamicForm WHERE ID = @id";
                        var query = "Update  DynamicForm SET IsDeleted=1 WHERE ID = @id";
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
        public async Task<IReadOnlyList<DynamicForm>> GetAllByGridFormAsync(long? userId)
        {
            List<DynamicForm> DynamicForm = new List<DynamicForm>();
            try
            {

                var query = "select t1.*,t6.Name as ProfileName,t5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
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
                        codeIds.AddRange(DynamicForm.Select(a => a.StatusCodeID).ToList());
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(DynamicForm.Select(a => a.AddedByUserID).ToList());
                        userIds.AddRange(DynamicForm.Where(a => a.ModifiedByUserID > 0).Select(a => a.ModifiedByUserID).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        codeIds = codeIds != null && codeIds.Count > 0 ? codeIds : new List<int?>() { -1 };
                        var query1 = "select UserName,UserId from ApplicationUser where userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query1 += "select * from CodeMaster where codeId in(" + string.Join(',', codeIds.Distinct()) + ");";
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

                var query = "select t1.*,t6.Name as ProfileName,t5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
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
                        codeIds.AddRange(DynamicForm.Select(a => a.StatusCodeID).ToList());
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(DynamicForm.Select(a => a.AddedByUserID).ToList());
                        userIds.AddRange(DynamicForm.Where(a => a.ModifiedByUserID > 0).Select(a => a.ModifiedByUserID).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        codeIds = codeIds != null && codeIds.Count > 0 ? codeIds : new List<int?>() { -1 };
                        var query1 = "select UserName,UserId from ApplicationUser where userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query1 += "select * from CodeMaster where codeId in(" + string.Join(',', codeIds.Distinct()) + ");";
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

                var query = "select t1.*,t6.Name as ProfileName,t5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
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
                        codeIds.AddRange(DynamicForm.Select(a => a.StatusCodeID).ToList());
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(DynamicForm.Select(a => a.AddedByUserID).ToList());
                        userIds.AddRange(DynamicForm.Where(a => a.ModifiedByUserID > 0).Select(a => a.ModifiedByUserID).ToList());
                        userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                        codeIds = codeIds != null && codeIds.Count > 0 ? codeIds : new List<int?>() { -1 };
                        var query1 = "select UserName,UserId from ApplicationUser where userId in(" + string.Join(',', userIds.Distinct()) + ");";
                        query1 += "select * from CodeMaster where codeId in(" + string.Join(',', codeIds.Distinct()) + ");";
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
                var query = "select t1.*,\r\n(Select tt2.FileName from Documents tt2 where tt2.SessionID=t1.SessionID ANd tt2.IsLatest=1) as FileName,\r\n" +
                    "(Select t22.ProfileNo from Documents t22 where t22.SessionID=t1.SessionID ANd t22.IsLatest=1) as ProfileNo,\r\n" +
                    "(Select t2.DocumentID from Documents t2 where t2.SessionID=t1.SessionID ANd t2.IsLatest=1) as DocumentID,\r\n" +
                    "(Select t4.SessionID from Documents t3 JOIN FileProfileType t4 ON t4.FileProfileTypeID=t3.FilterProfileTypeID where t3.SessionID=t1.SessionID ANd t3.IsLatest=1) as FileProfileSessionID,\r\n" +
                    "(Select tt4.Name from Documents tt3 JOIN FileProfileType tt4 ON tt4.FileProfileTypeID=tt3.FilterProfileTypeID where tt3.SessionID=t1.SessionID ANd tt3.IsLatest=1) as FileProfileName\r\n" +
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
                query = "SELECT tt1.*,(case when tt1.DynamicFormDataUploadID is NULL then  'No' ELSE 'Yes' END) as IsFileExits,\n\r" +
                     "(Select t22.ProfileNo from Documents t22 where t22.SessionID=tt1.UploadSessionID ANd t22.IsLatest=1) as ProfileNo,\r\n" +
                    " (Select tt2.FileName from Documents tt2 where tt2.SessionID=tt1.UploadSessionID ANd tt2.IsLatest=1) as FileName,\r\n" +
                    "(Select ttt2.DocumentID from Documents ttt2 where ttt2.SessionID=tt1.UploadSessionID ANd ttt2.IsLatest=1) as DocumentID,\r\n(Select ttt4.SessionID from Documents ttt3 JOIN FileProfileType ttt4 ON ttt4.FileProfileTypeID=ttt3.FilterProfileTypeID where ttt3.SessionID=tt1.UploadSessionID ANd ttt3.IsLatest=1) as FileProfileSessionID,\r\n(Select tt4.Name from Documents tt3 JOIN FileProfileType tt4 ON tt4.FileProfileTypeID=tt3.FilterProfileTypeID where tt3.SessionID=tt1.UploadSessionID ANd tt3.IsLatest=1) as FileProfileName\r\n from (select t1.*,\r\n" +
                    "(select t4.DynamicFormDataUploadID from DynamicFormDataUpload t4 WHERE t4.DynamicFormSectionID=t1.DynamicFormSectionID AND t4.DynamicFormDataID=@dynamicFormDataId) as DynamicFormDataUploadID,\r\n" +
                    "(select t2.SessionID from DynamicFormDataUpload t2 WHERE t2.DynamicFormSectionID=t1.DynamicFormSectionID AND t2.DynamicFormDataID=@dynamicFormDataId) as UploadSessionID,\r\n" +
                    "(select t3.AddedByUserID from DynamicFormDataUpload t3 WHERE t3.DynamicFormSectionID=t1.DynamicFormSectionID AND t3.DynamicFormDataID=@dynamicFormDataId) as DynamicFormDataUploadAddedUserID,\r\n" +
                    "(select t5.IsReadOnly from DynamicFormSectionSecurity t5 WHERE t5.UserID=" + UserId + " AND t1.DynamicFormSectionID=t5.DynamicFormSectionID) as UserIsReadOnly,\r\n" +
                    "(select t6.IsReadWrite from DynamicFormSectionSecurity t6 WHERE t6.UserID=" + UserId + " AND t1.DynamicFormSectionID=t6.DynamicFormSectionID) as UserIsReadWrite,\r\n" +
                    "(select t7.IsVisible from DynamicFormSectionSecurity t7 WHERE t7.UserID=" + UserId + " AND t1.DynamicFormSectionID=t7.DynamicFormSectionID) as UserIsVisible,\r\n" +
                    "(select Count(DynamicFormSectionSecurityID)  as userCounts from DynamicFormSectionSecurity t8 WHERE   t1.DynamicFormSectionID=t8.DynamicFormSectionID) as UserCount\r\n" +
                    "from DynamicFormSection t1 WHERE t1.DynamicFormID = @DynamicFormId AND  (t1.IsDeleted=0 or t1.IsDeleted is null)) tt1 where tt1.UserCount=0 OR tt1.UserIsVisible=1";
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
                var query = "select t1.*,t2.Name as DynamicFormName,t2.SessionID as DynamicFormSessionID from DynamicFormData t1 JOIN DynamicForm t2 ON t1.DynamicFormID=t2.ID WHERE t1.IsSendApproval=1 AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t2.IsDeleted=0 or t2.IsDeleted is null)";
                var result = new List<DynamicFormData>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormData>(query)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    var dynamicFormDataIDs = result.Select(a => a.DynamicFormDataId).ToList();
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
                    var dynamicFormDataIDs = result.Select(a => a.DynamicFormDataId).ToList();
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
                var query = "select t1.* from DynamicForm t1 WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.ID=@ID";

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
                var query = "select t1.*,t5.PlantCode as CompanyName from DynamicForm t1 \r\n" +
                    "LEFT JOIN Plant t5 ON t5.plantId=t1.companyId\r\n" +
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
                var query = "select t1.*,t2.PlantCode as CompanyName from Employee t1 JOIN Plant t2 ON t2.PlantID=t1.PlantID where t1.UserID=@userId";

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

                var query = "SELECT t1.*,t2.NAme as FileProfileTypeName,t4.PlantCode as CompanyName,t5.Name as ProfileName,t2.SessionID as FileProfileSessionId FROM DynamicForm t1\n\r" +
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

        public async Task<IReadOnlyList<DynamicFormSectionWorkFlow>> GetDynamicFormSectionWorkFlowByID(long? DynamicFormSectionId, long? UserId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", DynamicFormSectionId);
                parameters.Add("UserId", UserId);
                var query = "select t1.*,t3.SortOrderBy,t3.SectionName,t4.UserName as DynamicFormSectionWorkFlowUserName from DynamicFormSectionWorkFlow t1 \r\n" +
                    "JOIN DynamicFormSectionSecurity t2 ON t1.DynamicFormSectionSecurityID=t2.DynamicFormSectionSecurityID\r\n" +
                    "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t2.DynamicFormSectionID\r\n" +
                    "JOIN ApplicationUser t4 ON t4.UserID=t1.UserID WHERE t1.DynamicFormSectionId=@DynamicFormSectionId";
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

                    query = "SELECT * FROM DynamicForm Where (IsDeleted=0 or IsDeleted is null) AND ID!=@id AND ScreenID = @ScreenID";
                }
                else
                {
                    query = "SELECT * FROM DynamicForm Where (IsDeleted=0 or IsDeleted is null) AND ScreenID = @ScreenID";
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

                query = "SELECT * FROM DynamicFormSection Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId order by  SortOrderBy desc";
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
                        if (dynamicFormSection.DynamicFormSectionId > 0)
                        {
                            var query = " UPDATE DynamicFormSection SET SectionName = @SectionName,DynamicFormId =@DynamicFormId,SortOrderBy=@SortOrderBys," +
                                "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsVisible=@IsVisible," +
                                "IsReadOnly=@IsReadOnly,IsReadWrite=@IsReadWrite,Instruction=@Instruction " +
                                "WHERE DynamicFormSectionId = @DynamicFormSectionId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            parameters.Add("SortOrderBy", GeDynamicFormSectionSort(dynamicFormSection.DynamicFormId));
                            var query = "INSERT INTO DynamicFormSection(SectionName,DynamicFormId,SessionId,SortOrderBy,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsVisible,IsReadOnly,IsReadWrite,Instruction)  " +
                                "OUTPUT INSERTED.DynamicFormSectionId VALUES " +
                                "(@SectionName,@DynamicFormId,@SessionId,@SortOrderBy,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsVisible,@IsReadOnly,@IsReadWrite,@Instruction)";

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

                query = "SELECT * FROM DynamicFormSectionAttribute Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionId = @DynamicFormSectionId order by  SortOrderBy desc";
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
                        parameters.Add("IsDisplayDropDownHeader", dynamicFormSection.IsDisplayDropDownHeader);
                        parameters.Add("ApplicationMasterIds", dynamicFormSection.ApplicationMasterIdsListIds != null && dynamicFormSection.ApplicationMasterIdsListIds.Count() > 0 ? string.Join(",", dynamicFormSection.ApplicationMasterIdsListIds) : null, DbType.String);
                        parameters.Add("PlantDropDownWithOtherDataSourceIds", dynamicFormSection.PlantDropDownWithOtherDataSourceListIds != null && dynamicFormSection.PlantDropDownWithOtherDataSourceListIds.Count() > 0 ? string.Join(",", dynamicFormSection.PlantDropDownWithOtherDataSourceListIds) : null, DbType.String);
                        if (dynamicFormSection.DynamicFormSectionAttributeId > 0)
                        {

                            var query = "UPDATE DynamicFormSectionAttribute SET IsDisplayDropDownHeader=@IsDisplayDropDownHeader,ApplicationMasterIds=@ApplicationMasterIds,IsSetDefaultValue=@IsSetDefaultValue,IsDefaultReadOnly=@IsDefaultReadOnly,PlantDropDownWithOtherDataSourceIds=@PlantDropDownWithOtherDataSourceIds,IsPlantLoadDependency=@IsPlantLoadDependency,PlantDropDownWithOtherDataSourceLabelName=@PlantDropDownWithOtherDataSourceLabelName,PlantDropDownWithOtherDataSourceId=@PlantDropDownWithOtherDataSourceId,RemarksLabelName=@RemarksLabelName,IsRadioCheckRemarks=@IsRadioCheckRemarks,RadioLayout=@RadioLayout,DisplayName = @DisplayName,AttributeId =@AttributeId,DynamicFormSectionId=@DynamicFormSectionId," +
                                "SessionId =@SessionId,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,IsSpinEditType=@IsSpinEditType," +
                                "StatusCodeID=@StatusCodeID,ColSpan=@ColSpan,FormToolTips=@FormToolTips,SortOrderBy=@SortOrderBys,IsRequired=@IsRequired,IsMultiple=@IsMultiple,RequiredMessage=@RequiredMessage,IsDisplayTableHeader=@IsDisplayTableHeader,IsVisible=@IsVisible " +
                                "WHERE DynamicFormSectionAttributeId = @DynamicFormSectionAttributeId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            parameters.Add("SortOrderBy", GeDynamicFormSectionAttributeSort(dynamicFormSection.DynamicFormSectionId));
                            var query = "INSERT INTO DynamicFormSectionAttribute(IsDisplayDropDownHeader,ApplicationMasterIds,IsSetDefaultValue,IsDefaultReadOnly,PlantDropDownWithOtherDataSourceIds,IsPlantLoadDependency,PlantDropDownWithOtherDataSourceLabelName,PlantDropDownWithOtherDataSourceId,RemarksLabelName,IsRadioCheckRemarks,RadioLayout,FormToolTips,DisplayName,AttributeId,SessionId,SortOrderBy,AddedByUserID," +
                                "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,ColSpan,DynamicFormSectionId,IsRequired,IsMultiple,RequiredMessage,IsSpinEditType,IsDisplayTableHeader,IsVisible) VALUES " +
                                "(@IsDisplayDropDownHeader,@ApplicationMasterIds,@IsSetDefaultValue,@IsDefaultReadOnly,@PlantDropDownWithOtherDataSourceIds,@IsPlantLoadDependency,@PlantDropDownWithOtherDataSourceLabelName,@PlantDropDownWithOtherDataSourceId,@RemarksLabelName,@IsRadioCheckRemarks,@RadioLayout,@FormToolTips,@DisplayName,@AttributeId,@SessionId,@SortOrderBy," +
                                "@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@ColSpan,@DynamicFormSectionId,@IsRequired,@IsMultiple,@RequiredMessage,@IsSpinEditType,@IsDisplayTableHeader,@IsVisible)";

                            dynamicFormSection.DynamicFormSectionAttributeId = await connection.ExecuteAsync(query, parameters);
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
        public async Task<IReadOnlyList<DynamicFormSectionAttribute>> GetDynamicFormSectionAttributeAsync(long? dynamicFormSectionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormSectionId", dynamicFormSectionId);
                var query = "select t1.*,t9.Name DynamicGridName,t10.DataSourceTable as PlantDropDownWithOtherDataSourceTable,t6.IsFilterDataSource,t6.FilterDataSocurceID,tt4.DisplayName as FilterDataSourceDisplayName,tt4.TableName as FilterDataSourceTableName," +
                    "(case when t1.IsDisplayTableHeader is NULL then  0 ELSE t1.IsDisplayTableHeader END) as IsDisplayTableHeader,(case when t1.IsDisplayDropDownHeader is NULL then  1 ELSE t1.IsDisplayDropDownHeader END) as IsDisplayDropDownHeader,(case when t1.IsVisible is NULL then  1 ELSE t1.IsVisible END) as IsVisible," +
                    "t2.UserName as AddedBy,t8.DisplayName as DataSourceDisplayName,t8.DataSourceTable,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.SectionName,t6.ControlTypeId,t6.IsDynamicFormDropTagBox,t6.DropDownTypeId,t6.DataSourceId,t6.AttributeName,t7.CodeValue as ControlType from DynamicFormSectionAttribute t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicFormSection t5 ON t5.DynamicFormSectionId=t1.DynamicFormSectionId\r\n" +
                    "LEFT JOIN AttributeHeader t6 ON t6.AttributeID=t1.AttributeID\r\n" +
                    "LEFT JOIN CodeMaster t7 ON t7.CodeID=t6.ControlTypeID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t8 ON t8.AttributeHeaderDataSourceID=t6.DataSourceId\r\n" +
                    "LEFT JOIN DynamicForm t9 ON t9.ID=t6.DynamicFormID\r\n" +
                    "LEFT JOIN DynamicFormFilter tt4 ON tt4.DynamicFilterID=t6.FilterDataSocurceID\r\n" +
                    "LEFT JOIN AttributeHeaderDataSource t10 ON t10.AttributeHeaderDataSourceID=t1.PlantDropDownWithOtherDataSourceId\r\n" +
                    "Where (t5.IsDeleted=0 or t5.IsDeleted is null) AND (t6.IsDeleted=0 or t6.IsDeleted is null) AND (t6.AttributeIsVisible=1 or t6.AttributeIsVisible is NULL) AND (t1.IsDeleted=0 or t1.IsDeleted is null) AND (t9.IsDeleted=0 or t9.IsDeleted is null)  AND t1.DynamicFormSectionId=@DynamicFormSectionId\r\n";
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
                query = "SELECT * FROM DynamicFormSection Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId AND SortOrderBy>@SortOrderBy";
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
                query = "SELECT * FROM DynamicFormSectionAttribute Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormSectionId = @DynamicFormSectionId AND SortOrderBy>@SortOrderBy";
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
                query += "SELECT DynamicFormId,GridSortOrderByNo,DynamicFormDataId,DynamicFormDataGridId,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormDataId=@DynamicFormDataId AND DynamicFormId = @DynamicFormId  AND DynamicFormDataGridId=@DynamicFormDataGridId order by  SortOrderByNo desc;";
                query += "SELECT DynamicFormId,GridSortOrderByNo,DynamicFormDataGridId,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormId = @DynamicFormId AND DynamicFormDataGridId=@DynamicFormDataGridId   order by  GridSortOrderByNo desc";
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
        public async Task<DynamicFormData> InsertOrUpdateDynamicFormData(DynamicFormData dynamicFormData)
        {
            try
            {
                using (var connection = CreateConnection())
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
                                query += "INSERT INTO DynamicFormApproved(DynamicFormApprovalID,DynamicFormDataID,ApprovedDescription,UserID)VALUES " +
                                "(" + s.DynamicFormApprovalId + "," + dynamicFormData.DynamicFormDataId + ",'" + s.Description + "'," + s.ApprovalUserId + ");\n\r";
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
                var query = "select t1.*,(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid from DynamicFormData t1 \r\n" +
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
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid,t2.UserName as AddedBy,\r\nt3.UserName as ModifiedBy,t4.CodeValue as StatusCode,\r\nt5.IsApproval,t5.FileProfileTypeID,t6.Name as FileProfileTypeName,\r\n" +
                    "(SELECT COUNT(SessionId) from Documents t7 WHERE t7.SessionId=t1.SessionId AND t7.IsLatest=1 AND (t7.IsDelete IS NULL OR t7.IsDelete=0)) as isDocuments\r\n" +
                    "from DynamicFormData t1 \r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN DynamicForm t5 ON t5.ID=t1.DynamicFormId\r\n" +
                    "LEFT JOIN FileProfileType t6 ON t6.FileProfileTypeID=t5.FileProfileTypeID\r\nWHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.SessionId=@SessionId";
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
                dynamicFormDataIds = dynamicFormDataIds != null && dynamicFormDataIds.Count > 0 ? dynamicFormDataIds : new List<long>() { -1 };
                var query = "select t1.*,t4.UserName as ApprovedByUser,t5.DynamicFormId,\r\n" +
                   "CONCAT(case when t2.NickName is NULL then  t2.FirstName ELSE  t2.NickName END,' | ',t2.LastName) as ApprovalUser,\r\n" +
                   "CASE WHEN t1.IsApproved=1  THEN 'Approved' WHEN t1.IsApproved =0 THEN 'Rejected' ELSE 'Pending' END AS ApprovedStatus\r\n" +
                   "FROM DynamicFormApproved t1 \r\n" +
                   "JOIN Employee t2 ON t1.UserID=t2.UserID \r\n" +
                    "JOIN DynamicFormData t5 ON t5.DynamicFormDataId=t1.DynamicFormDataId \r\n" +
                   "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ApprovedByUserId WHERE (t5.IsDeleted=0 or t5.IsDeleted is null)  AND t1.DynamicFormDataId in(" + string.Join(',', dynamicFormDataIds) + ") order by t1.DynamicFormApprovedId asc;\r\n";
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
        public async Task<IReadOnlyList<DynamicFormData>> GetDynamicFormDataByIdAsync(long? id, long? userId, long? DynamicFormDataGridId, long? DynamicFormSectionGridAttributeId)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", id);
                parameters.Add("DynamicFormDataGridId", DynamicFormDataGridId);
                parameters.Add("DynamicFormSectionGridAttributeId", DynamicFormSectionGridAttributeId);
                var query = "select t1.*,t2.UserName as AddedBy,t3.UserName as ModifiedBy,t4.CodeValue as StatusCode,t5.FileProfileTypeId,t5.Name,t5.ScreenID,\r\n" +
                    "(select COUNT(t6.DocumentID) from DynamicFormDataUpload tt1 JOIN Documents t6 ON tt1.SessionID=t6.SessionID where t1.DynamicFormDataID=tt1.DynamicFormDataID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,\r\n" +
                    "(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid\r\n" +
                    "from DynamicFormData t1\r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID = t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID = t1.ModifiedByUserID\r\n" +
                    "JOIN DynamicForm t5 ON t5.ID = t1.DynamicFormID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID = t1.StatusCodeID WHERE (t1.IsDeleted=0 or t1.IsDeleted is null) AND t1.DynamicFormId =@DynamicFormId\r\n";
                if (DynamicFormDataGridId == 0 || DynamicFormDataGridId > 0)
                {
                    query += "AND t1.DynamicFormDataGridId=@DynamicFormDataGridId order by t1.GridSortOrderByNo asc;\r\n";
                    //query += "AND t1.DynamicFormDataGridId=@DynamicFormDataGridId And (t1.DynamicFormSectionGridAttributeID=@DynamicFormSectionGridAttributeId or t1.DynamicFormSectionGridAttributeID is null) order by t1.GridSortOrderByNo asc;\r\n";
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
                    var dynamicFormDataIDs = result.Select(a => a.DynamicFormDataId).ToList();
                    var resultData = await GetDynamicFormApprovedByAll(dynamicFormDataIDs);
                    var _activityEmailTopics = await GetActivityEmailTopicList(lists);
                    result.ForEach(s =>
                    {
                        var _activityEmailTopicsOne = _activityEmailTopics.FirstOrDefault(f => f.SessionId == s.SessionId);
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
        public async Task<DynamicFormData> DeleteDynamicFormData(DynamicFormData dynamicFormData)
        {
            try
            {
                using (var connection = CreateConnection())
                {


                    try
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
                        parameters.Add("ApprovedDescription", dynamicForm.ApprovedDescription);
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
            List<DynamicFormApproved> dynamicFormApprovedList = new List<DynamicFormApproved>();
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
                    dynamicFormApprovedList = (await connection.QueryAsync<DynamicFormApproved>(query, parameters)).ToList();
                    if (dynamicFormApprovedList != null && dynamicFormApprovedList.Count > 0)
                    {
                        //var empuserIds = dynamicFormApprovedList.Where(w => w.UserId > 0).Select(s => s.UserId).Distinct().ToList();
                        // var appuserIds = dynamicFormApprovedList.Where(w => w.ApprovedByUserId > 0).Select(s => s.ApprovedByUserId).Distinct().ToList();
                        // var acceptanceStatusIds = dynamicFormApprovedList.Where(w => w.ApprovedStatus > 0).Select(s => s.AcceptanceStatus).Distinct().ToList();
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
        public async Task<DynamicFormDataUpload> InsertDynamicFormDataUpload(DynamicFormDataUpload dynamicFormSection)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormDataUploadId", dynamicFormSection.DynamicFormDataUploadId);
                        parameters.Add("DynamicFormDataId", dynamicFormSection.DynamicFormDataId);
                        parameters.Add("DynamicFormSectionId", dynamicFormSection.DynamicFormSectionId);
                        parameters.Add("SessionId", dynamicFormSection.SessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", dynamicFormSection.AddedByUserId);
                        parameters.Add("ModifiedByUserID", dynamicFormSection.ModifiedByUserId);
                        parameters.Add("AddedDate", dynamicFormSection.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", dynamicFormSection.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", dynamicFormSection.StatusCodeId);
                        var query = "INSERT INTO DynamicFormDataUpload(DynamicFormDataId,DynamicFormSectionId,SessionId,AddedByUserID," +
                     "ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID) VALUES " +
                     "(@DynamicFormDataId,@DynamicFormSectionId,@SessionId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";

                        dynamicFormSection.DynamicFormDataUploadId = await connection.ExecuteAsync(query, parameters);


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
                var query = "select t1.*, \r\nt3.Name as UserGroup, \r\nt3.Description as UserGroupDescription, \r\nt4.Name as DynamicFormName,  \r\nt5.Name as LevelName, t6.NickName, t6.FirstName, t6.LastName, t7.Name as DepartmentName,  \r\nt8.Name as DesignationName,  \r\nCONCAT(case when t6.NickName is NULL  then  t6.FirstName  ELSE   t6.NickName END,' | ',t6.LastName) as FullName  \r\nfrom DynamicFormWorkFlow t1  \r\nLEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID  \r\nLEFT JOIN DynamicForm t4 ON t4.ID=t1.DynamicFormId  \r\nLEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID  \r\nJOIN Employee t6 ON t1.UserID=t6.UserID  \r\nLEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID  \r\nLEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID   where  t1.DynamicFormId=@dynamicFormId";
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
                            var listDatas = listData.Select(x => x.DynamicFormSectionId).Distinct().ToList();
                            var lists = listData.Where(w => w.DynamicFormWorkFlowId == s.DynamicFormWorkFlowId).ToList();
                            s.DynamicFormSectionIdList = listDatas;
                            if (lists != null && lists.Count() > 0)
                            {

                                s.SectionName = string.Join(',', lists.Select(z => z.SectionName).ToList());
                            }
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
                    var userExitsRoles = await GetDynamicFormWorkFlowEmptyAsync(value.DynamicFormId);
                    var userGroupUsers = await GetUserGroupUserList();
                    var LevelUsers = await GetLeveMasterUsersList(value.SelectLevelMasterIDs);

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("DynamicFormId", value.DynamicFormId);
                        parameters.Add("Type", value.Type); parameters.Add("SequenceNo", value.SequenceNo);
                        if (value.Type == "User")
                        {
                            if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                            {
                                foreach (var item in value.SelectUserIDs)
                                {
                                    var counts = userExitsRoles.Where(w => w.UserId == item).FirstOrDefault();
                                    if (counts == null)
                                    {
                                        query = "INSERT INTO [DynamicFormWorkFlow](DynamicFormId,UserId,type,SequenceNo) OUTPUT INSERTED.DynamicFormWorkFlowId " +
                                            "VALUES (@DynamicFormId," + item + ",@Type,@SequenceNo);\r\n";
                                        var dynamicFormWorkFlowId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                        if (value.SelectDynamicFormSectionIDs != null && value.SelectDynamicFormSectionIDs.Count() > 0)
                                        {
                                            var querys = string.Empty;
                                            foreach (var items in value.SelectDynamicFormSectionIDs)
                                            {
                                                querys += "INSERT INTO [DynamicFormWorkFlowSection](DynamicFormWorkFlowId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionID " +
                                           "VALUES (" + dynamicFormWorkFlowId + "," + items + ");\r\n";
                                            }
                                            await connection.QuerySingleOrDefaultAsync<long>(querys);
                                        }
                                    }
                                }
                            }
                        }
                        if (value.Type == "User Group")
                        {
                            if (value.SelectUserGroupIDs != null && value.SelectUserGroupIDs.Count() > 0)
                            {
                                var userGropuIds = userGroupUsers.Where(w => value.SelectUserGroupIDs.ToList().Contains(w.UserGroupId.Value)).ToList();
                                if (userGropuIds != null && userGropuIds.Count > 0)
                                {
                                    foreach (var s in userGropuIds)
                                    {
                                        var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                        if (counts == null)
                                        {
                                            query = "INSERT INTO [DynamicFormWorkFlow](DynamicFormId,UserId,UserGroupId,type,SequenceNo) OUTPUT INSERTED.DynamicFormWorkFlowId " +
                                                "VALUES (@DynamicFormId," + s.UserId + "," + s.UserGroupId + ",@Type,@SequenceNo);\r\n";
                                            var dynamicFormWorkFlowId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                            if (value.SelectDynamicFormSectionIDs != null && value.SelectDynamicFormSectionIDs.Count() > 0)
                                            {
                                                var querys = string.Empty;
                                                foreach (var items in value.SelectDynamicFormSectionIDs)
                                                {
                                                    querys += "INSERT INTO [DynamicFormWorkFlowSection](DynamicFormWorkFlowId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionID " +
                                               "VALUES (" + dynamicFormWorkFlowId + "," + items + ");\r\n";
                                                }
                                                await connection.QuerySingleOrDefaultAsync<long>(querys);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (value.Type == "Level")
                        {
                            if (LevelUsers != null && LevelUsers.Count() > 0)
                            {
                                foreach (var s in LevelUsers)
                                {
                                    var counts = userExitsRoles.Where(w => w.UserId == s.UserId).FirstOrDefault();
                                    if (counts == null)
                                    {
                                        query = "INSERT INTO [DynamicFormWorkFlow](DynamicFormId,UserId,LevelId,type,SequenceNo) OUTPUT INSERTED.DynamicFormWorkFlowId " +
                                           "VALUES (@DynamicFormId," + s.UserId + "," + s.LevelId + ",@Type,@SequenceNo);\r\n";
                                        var dynamicFormWorkFlowId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                                        if (value.SelectDynamicFormSectionIDs != null && value.SelectDynamicFormSectionIDs.Count() > 0)
                                        {
                                            var querys = string.Empty;
                                            foreach (var items in value.SelectDynamicFormSectionIDs)
                                            {
                                                querys += "INSERT INTO [DynamicFormWorkFlowSection](DynamicFormWorkFlowId,DynamicFormSectionId) OUTPUT INSERTED.DynamicFormWorkFlowSectionID " +
                                           "VALUES (" + dynamicFormWorkFlowId + "," + items + ");\r\n";
                                            }
                                            await connection.QuerySingleOrDefaultAsync<long>(querys);
                                        }
                                    }
                                }
                            }
                        }
                        // if (!string.IsNullOrEmpty(query))
                        //   {
                        //      await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        //  }
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
            var resultsa = await GenerateDynamicFormDataSortOrderByNo();
            List<DynamicFormDataWrokFlow> dynamicFormWorkFlowSections = new List<DynamicFormDataWrokFlow>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId);
                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                var query = "select t1.DynamicFormID,t2.FileProfileTypeId," +
                    "(select COUNT(t6.DocumentID) from DynamicFormDataUpload tt1 JOIN Documents t6 ON tt1.SessionID=t6.SessionID where t1.DynamicFormDataID=tt1.DynamicFormDataID AND t6.IsLatest = 1 AND(t6.IsDelete IS NULL OR t6.IsDelete = 0)) as IsFileprofileTypeDocument,\r\n" +
                    "(CASE WHEN t1.DynamicFormDataGridID>0  THEN 1  ELSE 0 END) AS IsDynamicFormDataGrid,t1.DynamicFormDataID,t1.SessionID,t1.ProfileID,t1.ProfileNo,t2.SessionID as DynamicFormSessionID,t1.DynamicFormDataGridId,t2.Name,t2.ScreenID,t1.SortOrderByNo from DynamicFormData t1\r\n" +
                    "JOIN DynamicForm t2 ON t2.ID=t1.DynamicFormID WHERE (t2.IsDeleted =0 OR t2.IsDeleted is null) AND (t1.IsDeleted =0 OR t1.IsDeleted is null) AND t1.DynamicFormID in( select t3.DynamicFormID from DynamicFormWorkFlow t3 where t3.UserID=@UserId)\r\n";
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
                    var SessionIds = result.Select(a => a.SessionID).ToList();
                    var lists = string.Join(',', SessionIds.Select(i => $"'{i}'"));
                    var _activityEmailTopics = await GetActivityEmailTopicList(lists);
                    foreach (var item in result)
                    {
                        var results = await GetDynamicFormWorkFlowIds(item.DynamicFormId, item.DynamicFormDataId);
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
                                foreach (var itemss in _sequenceNoList)
                                {
                                    var notdata = notCompleted.Where(w => w.SequenceNo == itemss).ToList();
                                    if (notdata != null && notdata.Count > 0)
                                    {
                                        var dataAdd = notdata.FirstOrDefault(w => w.UserId == userId);
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

        public async Task<IReadOnlyList<DynamicFormWorkFlowSection>> GetDynamicFormWorkFlowIds(long? dynamicFormId, long? dynamicFormDataId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("dynamicFormDataId", dynamicFormDataId);
                var query = "select t1.DynamicFormWorkFlowID,t1.DynamicFormSectionId,t3.SectionName,t4.UserName,t1.DynamicFormWorkFlowSectionID,t2.DynamicFormID,t2.SequenceNo,t2.UserID," +
                    "(SELECT (Count(*)) from DynamicFormWorkFlowForm t5 WHERE t5.DynamicFormWorkFlowSectionID=t1.DynamicFormWorkFlowSectionID AND t5.DynamicFormDataID=@dynamicFormDataId) as IsWorkFlowDone\r\n" +
                    "from DynamicFormWorkFlowSection t1 \r\n" +
                   "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID  \r\n" +
                    "JOIN DynamicFormWorkFlow t2 ON t2.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID \r\n" +
                     "JOIN ApplicationUser t4 ON t4.UserID=t2.UserID  \r\n" +
                    "Where  t2.DynamicFormID=@DynamicFormId order by t2.SequenceNo asc";
                var result = new List<DynamicFormWorkFlowSection>();
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowSection>(query, parameters)).ToList();
                }
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
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormId", dynamicFormId);
                parameters.Add("dynamicFormDataId", dynamicFormDataId);
                var query = "select t1.DynamicFormWorkFlowID,t3.SectionName,t1.DynamicFormWorkFlowSectionID,t1.DynamicFormSectionID,t2.DynamicFormID,t2.SequenceNo,t4.UserName,t2.UserID,t2.UserGroupID,t2.LevelID,t2.Type," +
                    "(SELECT (Count(*)) from DynamicFormWorkFlowForm t3 WHERE t3.DynamicFormWorkFlowSectionID=t1.DynamicFormWorkFlowSectionID AND t3.DynamicFormDataID=@dynamicFormDataId) as IsWorkFlowDone\r\n" +
                    "from DynamicFormWorkFlowSection t1 \r\n" +
                    "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t1.DynamicFormSectionID  \r\n" +
                    "JOIN DynamicFormWorkFlow t2 ON t2.DynamicFormWorkFlowID=t1.DynamicFormWorkFlowID \r\n" +
                     "JOIN ApplicationUser t4 ON t4.UserID=t2.UserID  \r\n" +
                    "Where  t2.DynamicFormID=@DynamicFormId order by t2.SequenceNo asc";
                var result = new List<DynamicFormWorkFlowSection>();
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DynamicFormWorkFlowSection>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
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
                            var query = string.Empty;
                            var parameters = new DynamicParameters();
                            parameters.Add("CompletedDate", DateTime.Now, DbType.DateTime);
                            dynamicFormWorkFlowSections.ForEach(s =>
                            {
                                query += "INSERT INTO [DynamicFormWorkFlowForm](DynamicFormWorkFlowSectionID,UserId,DynamicFormDataID,CompletedDate) OUTPUT INSERTED.DynamicFormWorkFlowFormID " +
                                                   "VALUES (" + s.DynamicFormWorkFlowSectionId + "," + userId + "," + dynamicFormDataId + ",@CompletedDate);\r\n";
                            });
                            if (!string.IsNullOrEmpty(query))
                            {
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
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
                var parameters = new DynamicParameters();
                parameters.Add("DynamicFormDataID", dynamicFormDataId);
                var query = "select ROW_NUMBER() OVER (ORDER BY (SELECT '1')) AS RowID,t1.DynamicFormDataID,t1.CompletedDate,t1.DynamicFormWorkFlowFormID,t1.DynamicFormWorkFlowSectionID,t1.UserID,t5.UserName as CompletedBy,t4.SequenceNo,\r\nt2.DynamicFormSectionID,t2.DynamicFormWorkFlowID,t3.SectionName,t4.UserID as DynamicFormWorkFlowUserID,t6.UserName as DynamicFormWorkFlowUser from \n\r" +
                    "DynamicFormWorkFlowForm t1 \r\n" +
                    "JOIN DynamicFormWorkFlowSection t2 ON t1.DynamicFormWorkFlowSectionID=t2.DynamicFormWorkFlowSectionID\r\n" +
                    "JOIN DynamicFormSection t3 ON t3.DynamicFormSectionID=t2.DynamicFormSectionID\r\n" +
                    "JOIN DynamicFormWorkFlow t4 ON t2.DynamicFormWorkFlowID=t4.DynamicFormWorkFlowID\r\n" +
                    "JOIN ApplicationUser t5 ON t5.UserID=t1.UserID\r\n" +
                    "JOIN ApplicationUser t6 ON t6.UserID=t4.UserID Where  t1.DynamicFormDataID=@DynamicFormDataID order by t4.SequenceNo asc";
                var result = new List<DynamicFormWorkFlowForm>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<DynamicFormWorkFlowForm>(query, parameters)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    rowCount = result.Count;
                    dynamicFormWorkFlowSections.AddRange(result);
                }
                if (listData != null && listData.Count > 0)
                {
                    var notCompleted = listData.Where(w => w.IsWorkFlowDone == 0).ToList();
                    if (notCompleted != null && notCompleted.Count > 0)
                    {
                        notCompleted.ForEach(a =>
                        {
                            DynamicFormWorkFlowForm dynamicFormWorkFlowForm = new DynamicFormWorkFlowForm();
                            dynamicFormWorkFlowForm.DynamicFormWorkFlowFormId = rowCount + 1; ;
                            dynamicFormWorkFlowForm.SectionName = a.SectionName;
                            dynamicFormWorkFlowForm.DynamicFormSectionId = a.DynamicFormSectionId;
                            dynamicFormWorkFlowForm.SequenceNo = a.SequenceNo;
                            dynamicFormWorkFlowForm.DynamicFormWorkFlowUserId = a.UserId;
                            dynamicFormWorkFlowForm.DynamicFormWorkFlowUser = a.UserName;
                            dynamicFormWorkFlowSections.Add(dynamicFormWorkFlowForm);
                            rowCount++;
                        });
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
                var query = "select t1.* from DynamicFormWorkFlowForm t1 WHERE  t1.DynamicFormDataId=@DynamicFormDataId AND t1.DynamicFormWorkFlowSectionID=@DynamicFormWorkFlowSectionID";

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
                var query = "select t1.* from ActivityEmailTopics t1 WHERE  t1.activityType='DynamicForm' AND t1.SessionId=@SessionId";

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
        public async Task<DynamicFormData> InsertCreateEmailFormData(DynamicFormData dynamicFormData)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var exitsData = await GetActivityEmailTopicsExits(dynamicFormData.SessionId);
                        if (exitsData == null)
                        {
                            var parameters = new DynamicParameters();
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
                var query = "select  * from DynamicFormSectionAttributeSecurity where  DynamicFormSectionAttributeId=@DynamicFormSectionAttributeId";

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
                var query = "select t1.*,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.DisplayName,\r\n" +
                    "t5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\n" +
                    "t8.Name as DesignationName,\r\n" +
                    "CONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\n" +
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
                query = "SELECT DynamicFormID,DynamicFormDataId,SortOrderByNo,GridSortOrderByNo,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID = @DynamicFormID   AND DynamicFormDataGridId=@DynamicFormDataGridId AND GridSortOrderByNo>@SortOrderByFrom and GridSortOrderByNo<=@SortOrderByTo order by GridSortOrderByNo asc";

                if (dynamicFormData.GridSortOrderAnotherByNo > dynamicFormData.GridSortOrderByNo)
                {
                    query = "SELECT DynamicFormID,DynamicFormDataId,SortOrderByNo,GridSortOrderByNo,DynamicFormSectionGridAttributeId FROM DynamicFormData Where (IsDeleted=0 or IsDeleted is null) AND DynamicFormID = @DynamicFormID  AND DynamicFormDataGridId=@DynamicFormDataGridId AND GridSortOrderByNo>=@SortOrderByFrom and GridSortOrderByNo<@SortOrderByTo order by GridSortOrderByNo asc";

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
        public async Task<DynamicFormDataUpload> InsertDmsDocumentDynamicFormData(DynamicFormDataUpload dynamicFormDataUpload)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var value = dynamicFormDataUpload.DocumentsModel;
                        var parameters = new DynamicParameters();
                        parameters.Add("FilterProfileTypeID", dynamicFormDataUpload.FileProfileTypeId);
                        parameters.Add("FileName", value.FileName, DbType.String);
                        parameters.Add("ContentType", value.ContentType, DbType.String);
                        parameters.Add("FileSize", value.OriginalFileSize);
                        parameters.Add("UploadDate", dynamicFormDataUpload.AddedDate, DbType.DateTime);
                        parameters.Add("AddedByUserId", dynamicFormDataUpload.AddedByUserId);
                        parameters.Add("AddedDate", dynamicFormDataUpload.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedByUserID", dynamicFormDataUpload.ModifiedByUserId);
                        parameters.Add("ModifiedDate", dynamicFormDataUpload.ModifiedDate, DbType.DateTime);
                        parameters.Add("SessionId", dynamicFormDataUpload.FileProfileSessionID, DbType.Guid);
                        parameters.Add("IsLatest", 1);
                        parameters.Add("FilePath", value.FilePath, DbType.String);
                        parameters.Add("IsNewPath", value.IsNewPath);
                        parameters.Add("IsTemp", value.IsTemp);
                        parameters.Add("ProfileNo", value.ProfileNo, DbType.String);
                        parameters.Add("SourceFrom", value.SourceFrom, DbType.String);
                        var query = "INSERT INTO [Documents](FilterProfileTypeID,FileName,ContentType,FileSize,UploadDate,AddedByUserId,AddedDate,ModifiedByUserID,ModifiedDate,SessionId,IsLatest,FilePath,IsNewPath,IsTemp,SourceFrom,ProfileNo) " +
                            "OUTPUT INSERTED.DocumentId VALUES " +
                           "(@FilterProfileTypeID,@FileName,@ContentType,@FileSize,@UploadDate,@AddedByUserId,@AddedDate,@ModifiedByUserID,@ModifiedDate,@SessionId,@IsLatest,@FilePath,@IsNewPath,@IsTemp,@SourceFrom,@ProfileNo)";
                        value.DocumentID = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        await InsertDynamicFormDataUpload(dynamicFormDataUpload);
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
    }

}
