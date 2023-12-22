using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NAV;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repository.Query
{
    public class ProductionActivityAppLineQueryRepository : QueryRepository<ProductionActivityAppLine>, IProductionActivityQueryRepository
    {
        public ProductionActivityAppLineQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
        private string DocumentQueryString()
        {
            var query = "select  " +
                    "SessionId," +
                    "DocumentId," +
                    "FileName," +
                    "ContentType," +
                    "FileSize," +
                    "UploadDate," +
                    "FilterProfileTypeId," +
                    "CloseDocumentId," +
                    "DocumentParentId," +
                    "IsMobileUpload," +
                    "TableName," +
                    "ExpiryDate," +
                    "AddedByUserId," +
                    "ModifiedByUserId," +
                    "ModifiedDate," +
                    "IsLocked," +
                    "LockedByUserId," +
                    "LockedDate," +
                    "AddedDate," +
                    "IsCompressed," +
                    "FileIndex," +
                    "ProfileNo," +
                    "IsLatest," +
                    "ArchiveStatusId," +
                    "Description," +
                    "IsWikiDraft," +
                    "IsWiki," +
                    "FilePath, " +
                    "IsNewPath, " +
                    "IsDelete, " +
                    "DeleteByUserID, " +
                    "DeleteByDate, " +
                    "SourceFrom, " +
                    "UniqueSessionId " +
                    "from Documents ";
            return query;
        }
        public async Task<IReadOnlyList<ViewEmployee>> GetAllUserWithoutStatussAsync()
        {
            try
            {
                var query = "select * from Employee";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<MultipleProductioAppLineItemLists> GetMultipleQueryAsync(List<long?> ProductionActivityAppLineIds, List<Guid?> SessionIds, List<long?> navprodOrderLineIds, List<int?> statusCodeIds, List<long?> masterChildIds, List<long?> masterDetaildChildIds, List<long?> manufacturingProcessChildIds)
        {
            MultipleProductioAppLineItemLists multipleProductioAppLineItemLists = new MultipleProductioAppLineItemLists();
            try
            {
                var query = "select ProductActivityCaseResponsId,ProductActivityCaseId from ProductActivityCaseRespons;";
                query += "select ProductActivityCaseResponsDutyId, ProductActivityCaseResponsId from ProductActivityCaseResponsDuty;";
                query += "select EmployeeId, ProductActivityCaseResponsDutyId from ProductActivityCaseResponsResponsible;";
                query += "select t1.AcitivityMasterID,t1.ActivityMasterMultipleId,t1.ProductionActivityAppLineID,t2.Value as AcitivityMasterName from ActivityMasterMultiple t1 JOIN ApplicationMasterDetail t2 ON t1.AcitivityMasterID=t2.ApplicationMasterDetailID;";
                ProductionActivityAppLineIds = ProductionActivityAppLineIds != null && ProductionActivityAppLineIds.Count > 0 ? ProductionActivityAppLineIds : new List<long?>() { -1 };
                query += "select * from ProductionActivityAppLineQaChecker where ProductionActivityAppLineId in(" + string.Join(',', ProductionActivityAppLineIds) + ");";
                SessionIds = SessionIds != null && SessionIds.Count > 0 ? SessionIds : new List<Guid?>() { Guid.NewGuid() };
                query += DocumentQueryString() + " where  SessionId in(" + string.Join(",", SessionIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ") AND IsLatest=1 AND (IsDelete is null or IsDelete=0);";
                query += "select UserName,UserId,SessionId from ApplicationUser;";
                navprodOrderLineIds = navprodOrderLineIds != null && navprodOrderLineIds.Count > 0 ? navprodOrderLineIds : new List<long?>() { -1 };
                query += "select t11.NAVProdOrderLineId,t11.ItemNo,t11.Description,t11.Description1,t11.BatchNo,t11.RePlanRefNo,CONCAT(t11.ProdOrderNo,'|',t11.Description,(case when ISNULL(NULLIF(t11.Description1, ''), null) is NULL then  t11.Description1 ELSE  CONCAT(' | ',Description1) END)) as ProdOrderNoDesc from NavprodOrderLine t11 where t11.NAVProdOrderLineId in(" + string.Join(',', navprodOrderLineIds) + ");";
                statusCodeIds = statusCodeIds != null && statusCodeIds.Count > 0 ? statusCodeIds : new List<int?>() { -1 };
                query += "select * from CodeMaster where CodeId in(" + string.Join(',', statusCodeIds) + ");";
                masterChildIds = masterChildIds != null && masterChildIds.Count > 0 ? masterChildIds : new List<long?>() { -1 };
                query += "select ApplicationMasterChildID,value from ApplicationMasterChild where ApplicationMasterChildID in(" + string.Join(',', masterChildIds) + ");";
                masterDetaildChildIds = masterDetaildChildIds != null && masterDetaildChildIds.Count > 0 ? masterDetaildChildIds : new List<long?>() { -1 };
                query += "select ApplicationMasterDetailID,value from ApplicationMasterDetail where ApplicationMasterDetailID in(" + string.Join(',', masterDetaildChildIds) + ");";
                query += "select ActivityEmailTopicID,ActivityType,EmailTopicSessionId,ActivityMasterId from ActivityEmailTopics where documentsessionid is not null AND ActivityType='productionactivity' AND ActivityMasterId in(" + string.Join(',', ProductionActivityAppLineIds) + ");";
                query += "select * from ProductActivityPermission;";
                query += "select * from ProductActivityCaseCategoryMultiple;";
                query += "select * from ProductActivityCaseActionMultiple;";
                manufacturingProcessChildIds = manufacturingProcessChildIds != null && manufacturingProcessChildIds.Count > 0 ? manufacturingProcessChildIds : new List<long?>() { -1 };
                query += "select ManufacturingProcessChildId,ProductActivityCaseId from ProductActivityCase where ManufacturingProcessChildId in(" + string.Join(',', manufacturingProcessChildIds) + ");";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    multipleProductioAppLineItemLists.ProductActivityCaseRespons = result.Read<ProductActivityCaseRespons>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCaseResponsDuty = result.Read<ProductActivityCaseResponsDuty>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCaseResponsResponsible = result.Read<ProductActivityCaseResponsResponsible>().ToList();
                    multipleProductioAppLineItemLists.ActivityMasterMultiple = result.Read<ActivityMasterMultiple>().ToList();
                    multipleProductioAppLineItemLists.ProductionActivityAppLineQaCheckerModel = result.Read<ProductionActivityAppLineQaCheckerModel>().ToList();
                    multipleProductioAppLineItemLists.Documents = result.Read<Documents>().ToList();
                    multipleProductioAppLineItemLists.ApplicationUser = result.Read<ApplicationUser>().ToList();
                    multipleProductioAppLineItemLists.NavprodOrderLine = result.Read<NavprodOrderLine>().ToList();
                    multipleProductioAppLineItemLists.CodeMaster = result.Read<CodeMaster>().ToList();
                    multipleProductioAppLineItemLists.ApplicationMasterChild = result.Read<ApplicationMasterChild>().ToList();
                    multipleProductioAppLineItemLists.ApplicationMasterDetail = result.Read<ApplicationMasterDetail>().ToList();
                    multipleProductioAppLineItemLists.ActivityEmailTopics = result.Read<ActivityEmailTopicsModel>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityPermission = result.Read<ProductActivityPermissionModel>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCaseCategoryMultiple = result.Read<ProductActivityCaseCategoryMultiple>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCaseActionMultiple = result.Read<ProductActivityCaseActionMultiple>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCase = result.Read<ProductActivityCase>().ToList();
                }
                return multipleProductioAppLineItemLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductActivityAppModel> GetProductActivityAppLineOneItem(long? ProductionActivityAppLineID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductionActivityAppLineID", ProductionActivityAppLineID);
                var query = "select t1.ProductionActivityAppLineID,SessionID,(select COUNT(*) from Documents t2 where t2.SessionID=t1.SessionID AND (t2.IsDelete IS NULL OR t2.IsDelete=0))as IsDocuments\r\nfrom ProductionActivityAppline t1 where t1.ProductionActivityAppLineID=@ProductionActivityAppLineID;";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ProductActivityAppModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ProductActivityAppModel>> GetAllAsync(ProductActivityAppModel value)
        {
            List<ProductActivityAppModel> productActivityAppModels = new List<ProductActivityAppModel>();
            try
            {
                var userId = value.AddedByUserID;
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", value.CompanyId);
                parameters.Add("ProdOrderNo", value.ProdOrderNo, DbType.String);
                parameters.Add("LocationID", value.LocationId);
                parameters.Add("ProdActivityResultId", value.ProdActivityResultId);
                parameters.Add("ManufacturingProcessChildId", value.ManufacturingProcessChildId);
                parameters.Add("ProdActivityCategoryChildId", value.ProdActivityCategoryChildId);
                parameters.Add("ProdActivityActionChildD", value.ProdActivityActionChildD);
                parameters.Add("ActivityStatusId", value.ActivityStatusId);
                parameters.Add("ActivityMasterId", value.ActivityMasterId);
                parameters.Add("NavprodOrderLineId", value.NavprodOrderLineId);
                parameters.Add("StartDate", value.StartDate, DbType.DateTime);
                parameters.Add("EndDate", value.EndDate, DbType.DateTime);
                var query = @"select CASE WHEN  t1.ProfileNo IS NULL THEN '' ELSE  t1.ProfileNo END AS ProfileNo,t1.ProfileId,
                    t1.ProductionActivityAppLineID,t1.ProductionActivityAppID,t1.ActionDropdown,t1.ProdActivityActionID,t1.ProdActivityCategoryID,t1.ManufacturingProcessID,t1.IsTemplateUpload,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID as LineSessionId,t1.ProductActivityCaseLineID,t1.NavprodOrderLineID,t14.Name as LocationName,t1.Comment as LineComment,t1.QaCheck,t1.IsOthersOptions,t1.ProdActivityResultID,t1.ManufacturingProcessChildID,t1.ProdActivityCategoryChildID,t1.ProdActivityActionChildD,t1.TopicID,t1.QaCheckUserID,t1.QaCheckDate,t1.LocationID,t1.ProductActivityCaseID,t1.ActivityMasterID,t1.ActivityStatusID,t2.CompanyID,t10.PlantCode as CompanyName,t2.ProdOrderNo,t2.Comment,t2.SessionId,t2.LocationID,
                    'Production Activity' as Type,
                    (case when t1.IsTemplateUpload=1 then 'Yes' ELSE 'No' END) as IsTemplateUploadFlag,
                    (select COUNT(tt1.ProductionActivityAppLineDocId) from ProductionActivityAppLineDoc tt1 WHERE tt1.Type = 'Production Activity' AND tt1.ProductionActivityAppLineID=t1.ProductionActivityAppLineID) as SupportDocCount,t12.NameOfTemplate,t12.Link,t12.LocationToSaveId
                    from ProductionActivityAppLine t1 
                    JOIN ProductionActivityApp t2 ON t1.ProductionActivityAppId=t2.ProductionActivityAppId
                    JOIN Plant as t10 ON t10.PlantID = t2.CompanyID 
                    LEFT JOIN ICTMaster t14 ON t14.ictMasterId=t2.LocationId
                    LEFT JOIN ProductActivityCaseLine as t12 ON t12.ProductActivityCaseLineId = t1.ProductActivityCaseLineId
                    WHERE t1.ProductionActivityAppLineID>0";
                if (value.NavprodOrderLineId > 0)
                {
                    query += "\n\rAND t1.NavprodOrderLineId=@NavprodOrderLineId";
                }
                if (value.ProdActivityActionChildD > 0)
                {
                    query += "\n\rAND t1.ProdActivityActionChildD=@ProdActivityActionChildD";
                }
                if (value.ProdActivityCategoryChildId > 0)
                {
                    query += "\n\rAND t1.ProdActivityCategoryChildID=@ProdActivityCategoryChildId";
                }
                if (value.ManufacturingProcessChildId > 0)
                {
                    query += "\n\rAND t1.ManufacturingProcessChildId=@ManufacturingProcessChildId";
                }
                if (!string.IsNullOrEmpty(value.ProdOrderNo))
                {
                    query += "\n\rAND t2.ProdOrderNo=@ProdOrderNo";
                }
                if (value.CompanyId > 0)
                {
                    query += "\n\rAND t2.CompanyId=@CompanyID";
                }
                if (value.LocationId > 0)
                {
                    query += "\n\rAND t2.LocationID=@LocationID";
                }
                if (value.ProdActivityResultId > 0)
                {
                    query += "\n\rAND t1.ProdActivityResultID=@ProdActivityResultId";
                }
                if (value.ActivityStatusId > 0)
                {
                    query += "\n\rAND t1.ActivityStatusId=@ActivityStatusId";
                }
                if (value.StartDate != null && value.EndDate == null)
                {
                    var from = value.StartDate.Value.ToString("yyyy-MM-dd");
                    query += "\n\rAND CAST(t1.AddedDate AS Date) >='" + from + "'\r\n";
                }
                if (value.EndDate != null && value.StartDate == null)
                {
                    var to = value.EndDate.Value.ToString("yyyy-MM-dd");
                    query += "\n\rAND CAST(t1.AddedDate AS Date)<='" + to + "'\r\n";
                }
                if (value.StartDate != null && value.EndDate != null)
                {
                    var from = value.StartDate.Value.ToString("yyyy-MM-dd");
                    var to = value.EndDate.Value.ToString("yyyy-MM-dd");
                    query += "\n\rAND CAST(t1.AddedDate AS Date) >='" + from + "'\r\n";
                    query += "\n\rAND CAST(t1.AddedDate AS Date)<='" + to + "'\r\n";
                }
                var employeeAll = await GetAllUserWithoutStatussAsync();
                var productActivityApps = new List<ProductActivityAppModel>();
                using (var connection = CreateConnection())
                {
                    productActivityApps = (await connection.QueryAsync<ProductActivityAppModel>(query, parameters)).ToList();
                }
                if (productActivityApps.Count > 0)
                {
                    var masterChildIds = new List<long?>();
                    var masterDetaildChildIds = new List<long?>();
                    masterDetaildChildIds.AddRange(productActivityApps.ToList().Where(w => w.ActivityStatusId > 0).Select(s => s.ActivityStatusId).Distinct().ToList());
                    masterDetaildChildIds.AddRange(productActivityApps.ToList().Where(w => w.ProdActivityResultId > 0).Select(s => s.ProdActivityResultId).Distinct().ToList());
                    masterChildIds.AddRange(productActivityApps.ToList().Where(w => w.ManufacturingProcessChildId > 0).Select(s => s.ManufacturingProcessChildId).Distinct().ToList());
                    masterChildIds.AddRange(productActivityApps.ToList().Where(w => w.ProdActivityCategoryChildId > 0).Select(s => s.ProdActivityCategoryChildId).Distinct().ToList());
                    masterChildIds.AddRange(productActivityApps.ToList().Where(w => w.ProdActivityActionChildD > 0).Select(s => s.ProdActivityActionChildD).Distinct().ToList());
                    var manufacturingProcessChildIds = productActivityApps.ToList().Where(w => w.ManufacturingProcessChildId > 0).Select(s => s.ManufacturingProcessChildId).Distinct().ToList();
                    var statusCodeIds = productActivityApps.ToList().Where(w => w.StatusCodeID > 0).Select(s => s.StatusCodeID).Distinct().ToList();
                    var navprodOrderLineIds = productActivityApps.ToList().Where(w => w.NavprodOrderLineId > 0).Select(s => s.NavprodOrderLineId).Distinct().ToList();
                    var productionActivityAppLineIds = productActivityApps.ToList().Select(s => s.ProductionActivityAppLineId).Distinct().ToList();
                    var sessionIds = productActivityApps.ToList().Where(w => w.LineSessionId != null).Select(s => s.LineSessionId).ToList();
                    var addedIds = productActivityApps.ToList().Select(s => s.AddedByUserID).Distinct().ToList();
                    addedIds.Add(userId);
                    var employee = employeeAll != null && employeeAll.Count() > 0 ? employeeAll.Where(w => addedIds.Contains(w.UserID)).ToList() : null;
                    var loginUser = employee != null && employee.Count() > 0 ? employee.FirstOrDefault(w => w.UserID == userId)?.DepartmentID : null;

                    var templateTestCaseCheckList = await GetMultipleQueryAsync(productionActivityAppLineIds, sessionIds, navprodOrderLineIds, statusCodeIds, masterChildIds, masterDetaildChildIds, manufacturingProcessChildIds);
                    var productionActivityAppLineQaChecker = templateTestCaseCheckList.ProductionActivityAppLineQaCheckerModel.ToList();
                    var templateTestCaseCheckListResponse = templateTestCaseCheckList.ProductActivityCaseRespons.ToList();
                    var templateTestCaseCheckListResponseDuty = templateTestCaseCheckList.ProductActivityCaseResponsDuty.ToList();
                    var templateTestCaseCheckListResponseResponsible = templateTestCaseCheckList.ProductActivityCaseResponsResponsible.ToList();
                    var activityEmailTopicList = templateTestCaseCheckList.ActivityEmailTopics.ToList();
                    var codeMasters = templateTestCaseCheckList.CodeMaster.ToList();
                    var applicationMasterChild = templateTestCaseCheckList.ApplicationMasterChild.ToList();
                    var documents = templateTestCaseCheckList.Documents.ToList();
                    var docIds = documents.Select(s => s.DocumentId).ToList();
                    var userIds = documents != null && documents.Count > 0 ? documents.Where(x => x.LockedByUserId > 0).Select(s => s.LockedByUserId).Distinct().ToList() : new List<long?>();
                    var filterProfileTypeIds = documents != null && documents.Count > 0 ? documents.Where(x => x.FilterProfileTypeId > 0).Select(s => s.FilterProfileTypeId).Distinct().ToList() : new List<long?>();
                    var appUser = templateTestCaseCheckList.ApplicationUser.ToList();
                    var applicationMasterDetail = templateTestCaseCheckList.ApplicationMasterDetail.ToList();
                    var navprodOrderLines = templateTestCaseCheckList.NavprodOrderLine.ToList();
                    var activityMasterMultiple = templateTestCaseCheckList.ActivityMasterMultiple.ToList();
                    var templateTestCaseCheckListIds = productActivityApps.Where(w => w.ProductActivityCaseId != null).Select(s => s.ProductActivityCaseId).ToList();
                    var activityCaseList = templateTestCaseCheckList.ProductActivityCase.ToList();
                    var prodactivityCategoryMultiplelist = templateTestCaseCheckList.ProductActivityCaseCategoryMultiple.ToList();
                    var prodactivityActionMultiplelist = templateTestCaseCheckList.ProductActivityCaseActionMultiple.ToList();
                    var productActivityPermissionList = templateTestCaseCheckList.ProductActivityPermission.ToList();
                    productActivityApps.ForEach(s =>
                    {
                        List<ProductActivityPermissionModel> ProductActivityPermissions = new List<ProductActivityPermissionModel>();
                        /* var checkSameDept = false;

                         if (s.AddedByUserID == userId)
                         {
                             checkSameDept = true;
                         }
                         else
                         {
                             var employeeDep = employee != null && employee.Count() > 0 ? employee.FirstOrDefault(w => w.UserID == s.AddedByUserID)?.DepartmentID : null;
                             if (loginUser != null && employeeDep != null && loginUser == employeeDep)
                             {
                                 checkSameDept = true;
                             }
                         }
                         if (checkSameDept == true)
                         {*/
                        List<long> responsibilityUsers = new List<long>();
                        /*if (s.ProductActivityCaseId > 0)
                        {
                            var templateTestCaseCheckListResponses = templateTestCaseCheckListResponse.Where(w => w.ProductActivityCaseId == s.ProductActivityCaseId && s.ProductActivityCaseId != null).Select(s => new { s.ProductActivityCaseResponsId, s.ProductActivityCaseId }).ToList();
                            var templateTestCaseCheckListResponseIdss = templateTestCaseCheckListResponses.Select(s => s.ProductActivityCaseResponsId).ToList();
                            var templateTestCaseCheckListResponseDutys = templateTestCaseCheckListResponseDuty.Where(w => templateTestCaseCheckListResponseIdss.Contains(w.ProductActivityCaseResponsId.Value)).Select(s => new { s.ProductActivityCaseResponsDutyId, s.ProductActivityCaseResponsId }).ToList();
                            var templateTestCaseCheckListResponseDutyIdss = templateTestCaseCheckListResponseDutys.Select(s => s.ProductActivityCaseResponsDutyId).ToList();
                            var templateTestCaseCheckListResponseResponsibles = templateTestCaseCheckListResponseResponsible.Where(w => templateTestCaseCheckListResponseDutyIdss.Contains(w.ProductActivityCaseResponsDutyId.Value)).Select(s => new { s.EmployeeId, s.ProductActivityCaseResponsDutyId }).ToList();
                            var empIdss = templateTestCaseCheckListResponseResponsibles.Select(s => s.EmployeeId).ToList();
                            var userIdss = employeeAll != null && employeeAll.Count() > 0 ? employeeAll.Where(w => empIdss.Contains(w.EmployeeID)).Select(s => s.UserID.GetValueOrDefault(0)).ToList() : new List<long>();
                            var appUserss = appUser.Select(s => new { s.UserID, s.SessionId }).Where(w => userIdss.Contains(w.UserID) && w.SessionId != null).ToList();

                            if (appUserss != null && appUserss.Count > 0)
                            {
                                var usersIdsBy = appUserss.Select(s => s.UserID).ToList();
                                if (usersIdsBy != null && usersIdsBy.Count > 0)
                                {
                                    responsibilityUsers.AddRange(usersIdsBy);
                                }
                            }
                        }*/

                        var currentActivityResponseIds = templateTestCaseCheckListResponseDuty.Where(r => r.ProductActivityCaseResponsId == s.ProductionActivityAppLineId).Select(r => r.ProductActivityCaseResponsDutyId).ToList();

                        var activityCaseManufacturingProcessList = activityCaseList.Where(a => a.ManufacturingProcessChildId == s.ManufacturingProcessChildId).Select(s => s.ProductActivityCaseId).ToList();
                        var activitycaseReponseIds = prodactivityCategoryMultiplelist.Where(p => activityCaseManufacturingProcessList.Contains(p.ProductActivityCaseId.Value) && p.CategoryActionId == s.ProdActivityCategoryChildId).Select(s => s.ProductActivityCaseId).ToList();
                        var actionmultipleIds = prodactivityActionMultiplelist.Where(p => activityCaseManufacturingProcessList.Contains(p.ProductActivityCaseId.Value) && activitycaseReponseIds.Contains(p.ProductActivityCaseId) && p.ActionId == s.ProdActivityActionChildD).Select(s => s.ProductActivityCaseId).ToList();
                        if (actionmultipleIds != null && actionmultipleIds.Count > 0)
                        {
                            var responseId = templateTestCaseCheckListResponse.Where(r => actionmultipleIds.Contains(r.ProductActivityCaseId)).Select(r => r.ProductActivityCaseResponsId).ToList();
                            var productActivityCaseResponsDutyIds = templateTestCaseCheckListResponseDuty.Where(r => responseId.Contains(r.ProductActivityCaseResponsId.Value)).Select(t => t.ProductActivityCaseResponsDutyId).ToList();
                            if (productActivityCaseResponsDutyIds != null && productActivityCaseResponsDutyIds.Count > 0)
                            {
                                var empIds = templateTestCaseCheckListResponseResponsible.Where(r => productActivityCaseResponsDutyIds.Contains(r.ProductActivityCaseResponsDutyId.Value)).Select(s => s.EmployeeId.Value).ToList();

                                if (empIds != null && empIds.Count > 0)
                                {

                                    responsibilityUsers.AddRange(empIds);

                                }
                                var selectPermissiondata = productActivityPermissionList.Where(p => p.UserID == userId && productActivityCaseResponsDutyIds.Contains(p.ProductActivityCaseResponsDutyId.Value)).ToList();
                                // ProductActivityPermissions

                                if (selectPermissiondata != null && selectPermissiondata.Count > 0)
                                {
                                    selectPermissiondata.ForEach(d =>
                                    {
                                        ProductActivityPermissionModel ProductActivityPermissionData = new ProductActivityPermissionModel();


                                        ProductActivityPermissionData.IsChecker = d.IsChecker;
                                        ProductActivityPermissionData.IsCheckOut = d.IsCheckOut;
                                        ProductActivityPermissionData.IsUpdateStatus = d.IsUpdateStatus;
                                        ProductActivityPermissionData.ProductActivityPermissionId = d.ProductActivityPermissionId;
                                        ProductActivityPermissionData.ProductActivityCaseId = d.ProductActivityCaseId;
                                        ProductActivityPermissionData.IsViewFile = d.IsViewFile;
                                        ProductActivityPermissionData.IsViewHistory = d.IsViewHistory;
                                        ProductActivityPermissionData.IsCopyLink = d.IsCopyLink;
                                        ProductActivityPermissionData.IsMail = d.IsMail;
                                        ProductActivityPermissionData.IsActivityInfo = d.IsActivityInfo;
                                        ProductActivityPermissionData.IsNonCompliance = d.IsNonCompliance;
                                        ProductActivityPermissionData.IsSupportDocuments = d.IsSupportDocuments;
                                        ProductActivityPermissionData.UserID = d.UserID;
                                        ProductActivityPermissionData.ProductActivityCaseResponsDutyId = d.ProductActivityCaseResponsDutyId;
                                        ProductActivityPermissions.Add(ProductActivityPermissionData);
                                    });
                                }
                            }
                        }
                        else
                        {
                            responsibilityUsers.Add(userId.Value);
                        }


                        ProductActivityAppModel productActivityApp = new ProductActivityAppModel();
                        productActivityApp.ProductActivityPermissionData = new ProductActivityPermissionModel();
                        productActivityApp.Type = s.Type;
                        productActivityApp.ResponsibilityUsers = responsibilityUsers;
                        productActivityApp.ProductActivityCaseId = s.ProductActivityCaseId;
                        productActivityApp.SupportDocCount = s.SupportDocCount;
                        productActivityApp.ProductionActivityAppLineId = s.ProductionActivityAppLineId;
                        productActivityApp.ProductionActivityAppId = s.ProductionActivityAppId;
                        productActivityApp.Comment = s.Comment;
                        productActivityApp.LineComment = s.LineComment;
                        productActivityApp.ActivityStatusId = s.ActivityStatusId;
                        productActivityApp.ActivityStatus = s.ActivityStatusId > 0 && applicationMasterDetail != null && applicationMasterDetail.Count() > 0 ? applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == s.ActivityStatusId)?.Value : string.Empty;
                        productActivityApp.ManufacturingProcessId = s.ManufacturingProcessId;
                        productActivityApp.ProdActivityResultId = s.ProdActivityResultId;
                        productActivityApp.ProdActivityResult = s.ProdActivityResultId > 0 && applicationMasterDetail != null && applicationMasterDetail.Count() > 0 ? applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == s.ProdActivityResultId)?.Value : string.Empty;
                        productActivityApp.ManufacturingProcess = s.ManufacturingProcess;
                        productActivityApp.CompanyId = s.CompanyId;
                        productActivityApp.CompanyName = s.CompanyName;
                        productActivityApp.ProdActivityActionId = s.ProdActivityActionId;
                        productActivityApp.ProdActivityAction = s.ProdActivityAction;
                        productActivityApp.ActionDropdown = s.ActionDropdown;
                        productActivityApp.ProdActivityCategoryId = s.ProdActivityCategoryId;
                        productActivityApp.ProdActivityCategory = s.ProdActivityCategory;
                        productActivityApp.IsTemplateUpload = s.IsTemplateUpload;
                        productActivityApp.IsTemplateUploadFlag = s.IsTemplateUploadFlag;
                        productActivityApp.ProdOrderNo = s.ProdOrderNo;
                        productActivityApp.StatusCodeID = s.StatusCodeID;
                        productActivityApp.ModifiedByUserID = s.ModifiedByUserID;
                        productActivityApp.ModifiedDate = s.ModifiedDate;
                        productActivityApp.SessionId = s.SessionId;
                        productActivityApp.LineSessionId = s.LineSessionId;
                        productActivityApp.AddedByUserID = s.AddedByUserID;
                        productActivityApp.AddedDate = s.AddedDate;
                        productActivityApp.AddedByUser = appUser != null && appUser.Count() > 0 ? appUser.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName : string.Empty;
                        productActivityApp.ModifiedByUser = appUser != null && appUser.Count() > 0 ? appUser.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName : string.Empty;
                        productActivityApp.StatusCode = codeMasters != null && codeMasters.Count() > 0 ? codeMasters.FirstOrDefault(f => f.CodeId == s.StatusCodeID)?.CodeValue : string.Empty;
                        productActivityApp.ProductActivityCaseLineId = s.ProductActivityCaseLineId;
                        productActivityApp.NameOfTemplate = s.NameOfTemplate;
                        productActivityApp.Link = s.Link;
                        productActivityApp.LocationToSaveId = s.LocationToSaveId;
                        productActivityApp.QaCheck = s.QaCheck == true ? true : false;
                        productActivityApp.ActivityProfileNo = s.ProfileNo;
                        productActivityApp.ProfileId = s.ProfileId;
                        if (s.IsOthersOptions == true)
                        {
                            productActivityApp.ProdOrderNoDesc = "Other";
                        }
                        else
                        {
                            productActivityApp.ItemNo = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.ItemNo) : string.Empty;
                            productActivityApp.Description = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.Description) : string.Empty;
                            productActivityApp.Description1 = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.Description1) : string.Empty;
                            productActivityApp.BatchNo = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.BatchNo) : string.Empty;
                            productActivityApp.RePlanRefNo = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.RePlanRefNo) : string.Empty;
                            productActivityApp.ProdOrderNoDesc = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.ProdOrderNoDesc) : string.Empty;
                        }
                        productActivityApp.NavprodOrderLineId = s.NavprodOrderLineId;
                        productActivityApp.IsOthersOptions = s.IsOthersOptions;
                        productActivityApp.ManufacturingProcessChildId = s.ManufacturingProcessChildId;
                        productActivityApp.ProdActivityActionChildD = s.ProdActivityActionChildD;
                        productActivityApp.ProdActivityCategoryChildId = s.ProdActivityCategoryChildId;
                        productActivityApp.ManufacturingProcessChild = s.ManufacturingProcessChildId > 0 && applicationMasterChild != null && applicationMasterChild.Count() > 0 ? applicationMasterChild.FirstOrDefault(f => f.ApplicationMasterChildId == s.ManufacturingProcessChildId)?.Value : string.Empty;
                        productActivityApp.ProdActivityActionChild = s.ProdActivityActionChildD > 0 && applicationMasterChild != null && applicationMasterChild.Count() > 0 ? applicationMasterChild.FirstOrDefault(f => f.ApplicationMasterChildId == s.ProdActivityActionChildD)?.Value : string.Empty;
                        productActivityApp.ProdActivityCategoryChild = s.ProdActivityCategoryChildId > 0 && applicationMasterChild != null && applicationMasterChild.Count() > 0 ? applicationMasterChild.FirstOrDefault(f => f.ApplicationMasterChildId == s.ProdActivityCategoryChildId)?.Value : string.Empty;
                        productActivityApp.TopicId = s.TopicId;
                        productActivityApp.LocationId = s.LocationId;
                        productActivityApp.LocationName = s.LocationName + "|" + s.ProdOrderNo;
                        productActivityApp.ProductionActivityAppLineQaCheckerModels = productionActivityAppLineQaChecker != null ? productionActivityAppLineQaChecker.Where(z => z.ProductionActivityAppLineId == s.ProductionActivityAppLineId).ToList() : new List<ProductionActivityAppLineQaCheckerModel>();
                        productActivityApp.DocumentPermissionData = new DocumentPermissionModel();
                        productActivityApp.ActivityMasterIds = activityMasterMultiple != null && activityMasterMultiple.Count > 0 ? activityMasterMultiple.Where(a => a.ProductionActivityAppLineId == s.ProductionActivityAppLineId).Select(z => z.AcitivityMasterID).ToList() : new List<long?>();
                        var masterList = activityMasterMultiple != null && activityMasterMultiple.Count > 0 ? activityMasterMultiple.Where(a => a.ProductionActivityAppLineId == s.ProductionActivityAppLineId).Select(z => z.AcitivityMasterName).ToList() : new List<string?>();
                        productActivityApp.ActivityMaster = string.Join(",", masterList);
                        var emailcreated = activityEmailTopicList.Where(a => a.ActivityMasterId == s.ProductionActivityAppLineId && a.EmailTopicSessionId != null)?.FirstOrDefault();
                        if (emailcreated != null)
                        {
                            productActivityApp.IsEmailCreated = true;
                        }
                        if (documents != null && s.LineSessionId != null)
                        {
                            var counts = documents.FirstOrDefault(w => w.SessionId == s.LineSessionId);
                            if (counts != null)
                            {
                                productActivityApp.DocumentId = counts.DocumentId;
                                productActivityApp.FileProfileTypeId = counts.FilterProfileTypeId;
                                productActivityApp.DocumentID = counts.DocumentId;
                                productActivityApp.DocumentParentId = counts.DocumentParentId;
                                productActivityApp.FileName = counts.FileName;
                                productActivityApp.ProfileNo = counts.ProfileNo;
                                productActivityApp.FilePath = counts.FilePath;
                                productActivityApp.UniqueSessionId = counts.UniqueSessionId;
                                productActivityApp.IsNewPath = counts.IsNewPath == true ? true : false;
                                productActivityApp.ContentType = counts.ContentType;
                                productActivityApp.IsLocked = counts.IsLocked;
                                productActivityApp.LockedByUserId = counts.LockedByUserId;
                                productActivityApp.ModifiedDate = counts.UploadDate;
                                productActivityApp.ModifiedByUser = appUser != null && appUser.Count() > 0 && counts.AddedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.AddedByUserId)?.UserName : "";
                                productActivityApp.LockedByUser = appUser != null && appUser.Count() > 0 && counts.LockedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.LockedByUserId)?.UserName : "";
                                productActivityApp.LocationToSaveId = counts.FilterProfileTypeId;
                            }
                        }
                        if (ProductActivityPermissions != null && ProductActivityPermissions.Count > 0)
                        {
                            ProductActivityPermissions.ForEach(d =>
                            {

                                if (d.ProductActivityPermissionId > 0)
                                {
                                    productActivityApp.IsActionPermission = true;
                                    if (userId == d.UserID)
                                    {
                                        productActivityApp.ProductActivityPermissionData.IsChecker = d.IsChecker;
                                        productActivityApp.ProductActivityPermissionData.IsUpdateStatus = d.IsUpdateStatus;
                                        productActivityApp.ProductActivityPermissionData.IsCheckOut = d.IsCheckOut;
                                        productActivityApp.ProductActivityPermissionData.ProductActivityPermissionId = d.ProductActivityPermissionId;
                                        productActivityApp.ProductActivityPermissionData.ProductActivityCaseId = d.ProductActivityCaseId;
                                        productActivityApp.ProductActivityPermissionData.IsViewFile = d.IsViewFile;
                                        productActivityApp.ProductActivityPermissionData.IsViewHistory = d.IsViewHistory;
                                        productActivityApp.ProductActivityPermissionData.IsCopyLink = d.IsCopyLink;
                                        productActivityApp.ProductActivityPermissionData.IsMail = d.IsMail;
                                        productActivityApp.ProductActivityPermissionData.IsActivityInfo = d.IsActivityInfo;
                                        productActivityApp.ProductActivityPermissionData.IsNonCompliance = d.IsNonCompliance;
                                        productActivityApp.ProductActivityPermissionData.IsSupportDocuments = d.IsSupportDocuments;
                                        productActivityApp.ProductActivityPermissionData.UserID = d.UserID;
                                        productActivityApp.ProductActivityPermissionData.ProductActivityCaseResponsDutyId = d.ProductActivityCaseResponsDutyId;
                                    }
                                    else
                                    {
                                        productActivityApp.IsActionPermission = false;
                                    }


                                }
                                else
                                {
                                    productActivityApp.IsActionPermission = false;
                                }
                            });
                        }
                        else
                        {
                            productActivityApp.IsActionPermission = true;
                        }
                        if (productActivityApp.ResponsibilityUsers != null && productActivityApp.ResponsibilityUsers.Count > 0)
                        {
                            if (productActivityApp.ResponsibilityUsers.Contains(userId.Value))
                            {
                                productActivityAppModels.Add(productActivityApp);
                            }
                        }
                        else
                        {
                            productActivityAppModels.Add(productActivityApp);
                        }
                        //}
                    });
                }
                if (value.ActivityMasterId > 0)
                {
                    productActivityAppModels = productActivityAppModels.Where(w => w.ActivityMasterIds.Contains(value.ActivityMasterId)).ToList();
                }
                return productActivityAppModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<IReadOnlyList<ProductionActivityApp>> GetAlllocAsync(long? location)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("LoctionId", location);
                var query = @"select * from ProductionActivityApp  where LocationID =@LoctionId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityApp>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductActivityAppModel> UpdateproductActivityAppLineCommentField(ProductActivityAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Comment", value.LineComment, DbType.String);
                        parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                        parameters.Add("ProductionActivityAppLineId", value.ProductionActivityAppLineId);
                        var query = "Update ProductionActivityAppLine SET Comment=@Comment WHERE ProductionActivityAppLineId=@ProductionActivityAppLineId";
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
        public async Task<ProductActivityAppModel> DeleteproductActivityAppLine(ProductActivityAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", value.LineSessionId, DbType.Guid);
                        parameters.Add("ProductionActivityAppLineId", value.ProductionActivityAppLineId);
                        var query = "Delete from  ActivityMasterMultiple WHERE ProductionActivityAppLineId=@ProductionActivityAppLineId;";
                        query += "Delete from  ProductionActivityAppLine WHERE ProductionActivityAppLineId=@ProductionActivityAppLineId;";
                        query += "Update Documents Set Islatest=0  Where SessionId=@SessionId;";
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
        public async Task<DocumentsModel> DeleteSupportingDocuments(DocumentsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", value.SessionID, DbType.Guid);
                        var query = string.Empty;
                        query += "Delete from  ProductionActivityAppLineDoc WHERE SessionId=@SessionId;";
                        query += "Update Documents Set Islatest=0  Where SessionId=@SessionId;";
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
        public async Task<ProductionActivityNonComplianceUserModel> DeleteProductionActivityNonCompliance(ProductionActivityNonComplianceUserModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductionActivityNonComplianceUserId", value.ProductionActivityNonComplianceUserId);
                        var query = string.Empty;
                        query += "Delete from  ProductionActivityNonComplianceUser WHERE ProductionActivityNonComplianceUserId=@ProductionActivityNonComplianceUserId;";
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
        public async Task<ProductionActivityNonComplianceModel> GetProductionActivityNonComplianceAsync(string type, long? id, string actionType)
        {
            ProductionActivityNonComplianceModel productActivityAppModels = new ProductionActivityNonComplianceModel();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("id", id);
                parameters.Add("ActionType", actionType, DbType.String);
                parameters.Add("Type", type, DbType.String);
                var query = "select  * from ProductionActivityNonCompliance where Type=@Type AND  ActionType=@ActionType\n\r";
                if (type == "Production Activity")
                {
                    query += "AND ProductionActivityAppLineId=@id;";
                }
                if (type == "Ipir")
                {
                    query += "AND IpirReportId=@id;";
                }
                if (type == "Production Routine")
                {
                    query += "AND ProductionActivityRoutineAppLineId=@id;";
                }
                if (type == "Production Planning")
                {
                    query += "AND ProductionActivityPlanningAppLineId=@id;";
                }
                query += "Select * from ProductionActivityNonComplianceUser;";
                query += "select UserName,UserId,SessionId from ApplicationUser;";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query, parameters);
                    var productActivityAppModel = result.Read<ProductionActivityNonComplianceModel>().ToList();
                    var productionActivityNonComplianceUsers = result.Read<ProductionActivityNonComplianceUserModel>().ToList();
                    var userNames = result.Read<ApplicationUser>().ToList();
                    if (productActivityAppModel.Count() > 0)
                    {
                        productActivityAppModels = productActivityAppModel.FirstOrDefault();

                        var productionActivityNonComplianceUser = productionActivityNonComplianceUsers.Where(w => w.ProductionActivityNonComplianceId == productActivityAppModels.ProductionActivityNonComplianceId).ToList();
                        if (productionActivityNonComplianceUser != null && productionActivityNonComplianceUser.Count() > 0)
                        {
                            List<ProductionActivityNonComplianceUserModel> ProductionActivityNonComplianceUserModels = new List<ProductionActivityNonComplianceUserModel>();
                            productionActivityNonComplianceUser.ToList().ForEach(a =>
                            {
                                ProductionActivityNonComplianceUserModel productionActivityNonComplianceUserModel = new ProductionActivityNonComplianceUserModel();
                                productionActivityNonComplianceUserModel.ProductionActivityNonComplianceId = a.ProductionActivityNonComplianceId;
                                productionActivityNonComplianceUserModel.ProductionActivityNonComplianceUserId = a.ProductionActivityNonComplianceUserId;
                                productionActivityNonComplianceUserModel.UserId = a.UserId;
                                productionActivityNonComplianceUserModel.Notes = productActivityAppModels?.Notes;
                                productionActivityNonComplianceUserModel.UserName = userNames != null ? userNames.FirstOrDefault(f => f.UserID == a.UserId)?.UserName : string.Empty;
                                ProductionActivityNonComplianceUserModels.Add(productionActivityNonComplianceUserModel);

                            });
                            productActivityAppModels.ProductionActivityNonComplianceUserModels = ProductionActivityNonComplianceUserModels;
                        }
                    }
                    return productActivityAppModels;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductionActivityNonComplianceModel> InsertProductionActivityNonCompliance(ProductionActivityNonComplianceModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Type", value.Type, DbType.String);
                        parameters.Add("ActionType", value.ActionType, DbType.String);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ProductionActivityAppLineId", value.ProductionActivityAppLineId);
                        parameters.Add("IpirReportId", value.IpirReportId);
                        parameters.Add("ProductionActivityRoutineAppLineId", value.ProductionActivityRoutineAppLineId);
                        parameters.Add("ProductionActivityPlanningAppLineId", value.ProductionActivityPlanningAppLineId);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("ProductionActivityNonComplianceId", value.ProductionActivityNonComplianceId);
                        parameters.Add("Notes", value.Notes, DbType.String);
                        if (value.ProductionActivityNonComplianceId > 0)
                        {
                            if (!string.IsNullOrEmpty(value.Notes))
                            {
                                var query = "Update ProductionActivityNonCompliance Set Notes=@Notes,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID  Where ProductionActivityNonComplianceId=@ProductionActivityNonComplianceId;";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            }
                        }
                        else
                        {
                            var query = @"INSERT INTO ProductionActivityNonCompliance(Type,ActionType,AddedByUserID,AddedDate,StatusCodeID,ModifiedByUserID,ModifiedDate,ProductionActivityAppLineId,IpirReportId,ProductionActivityRoutineAppLineId,ProductionActivityPlanningAppLineId,Notes) 
				                       OUTPUT INSERTED.ProductionActivityNonComplianceId 
				                       VALUES (@Type,@ActionType,@AddedByUserID,@AddedDate,@StatusCodeID,@ModifiedByUserID,@ModifiedDate,@ProductionActivityAppLineId,@IpirReportId,@ProductionActivityRoutineAppLineId,@ProductionActivityPlanningAppLineId,@Notes)";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);

                            value.ProductionActivityNonComplianceId = insertedId;
                        }
                        if (value.ProductionActivityNonComplianceId > 0)
                        {
                            var Deletequery = "DELETE  FROM ProductionActivityNonComplianceUser WHERE ProductionActivityNonComplianceId = " + value.ProductionActivityNonComplianceId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        if (value.UserIDs != null)
                        {
                            var listData = value.UserIDs.ToList();
                            if (listData.Count > 0)
                            {
                                var querys = string.Empty;
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [ProductionActivityNonComplianceUser](UserId,ProductionActivityNonComplianceId) " +
                                                        "VALUES ( " + s + "," + value.ProductionActivityNonComplianceId + ");\r\n";
                                });
                                if (!string.IsNullOrEmpty(querys))
                                {
                                    await connection.ExecuteAsync(querys, null);
                                }
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
        public async Task<ProductActivityAppModel> UpdateActivityStatus(ProductActivityAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductionActivityAppLineId", value.ProductionActivityAppLineId);
                        parameters.Add("ActivityStatusId", value.ActivityStatusId);
                        var query = string.Empty;
                        query += "Update ProductionActivityAppLine Set ActivityStatusId=@ActivityStatusId  Where ProductionActivityAppLineId=@ProductionActivityAppLineId;";
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
        public async Task<ProductActivityAppModel> UpdateActivityMaster(ProductActivityAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (value.ProductionActivityAppLineId > 0)
                        {
                            var Deletequery = "DELETE  FROM ActivityMasterMultiple WHERE ProductionActivityAppLineId = " + value.ProductionActivityAppLineId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        if (value.ActivityMasterIds != null)
                        {
                            var listData = value.ActivityMasterIds.ToList();
                            if (listData.Count > 0)
                            {
                                var querys = string.Empty;
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [ActivityMasterMultiple](AcitivityMasterId,ProductionActivityAppLineId) " +
                                                        "VALUES ( " + s + "," + value.ProductionActivityAppLineId + ");\r\n";
                                });
                                if (!string.IsNullOrEmpty(querys))
                                {
                                    await connection.ExecuteAsync(querys, null);
                                }
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
        public async Task<ActivityEmailTopics> GetActivityEmailTopicsOneItem(long? ActivityMasterId, string? ActivityType)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ActivityMasterId", ActivityMasterId);
                parameters.Add("ActivityType", ActivityType, DbType.String);
                var query = "select * from ActivityEmailTopics where ActivityMasterId=@ActivityMasterId AND ActivityType=@ActivityType";
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
        public async Task<IReadOnlyList<view_ActivityEmailSubjects>> GetProductActivityEmailActivitySubjects(long? ActivityMasterId, string? ActivityType, long? UserId)
        {
            try
            {
                List<view_ActivityEmailSubjects> activityEmailSubjects = new List<view_ActivityEmailSubjects>();
                var result = await GetActivityEmailTopicsOneItem(ActivityMasterId, ActivityType);
                if (result != null && result.EmailTopicSessionId != null)
                {
                    var query = "select ET.ID as TopicId,EC.ID, EC.Name as TopicName,EC.AddedDate,EC.AddedByUserID, E.FirstName, a.UserName as AddedByUser, ET.DueDate as DueDate, E.LastName from EmailTopics ET INNER JOIN EmailConversations EC ON EC.TopicID = ET.ID  inner join ApplicationUser a on a.UserID = EC.AddedByUserID INNER JOIN EmailConversationParticipant ECP ON ECP.ConversationId = EC.ID AND ECP.UserId = " + UserId + "CROSS APPLY(SELECT DISTINCT ReplyId = CASE WHEN ECC.ReplyId >0 THEN ECC.ReplyId ELSE ECC.ID END FROM EmailConversations ECC WHERE ECC.TopicID=ET.ID AND (ECC.OnBehalf = " + UserId + "OR EXISTS(SELECT * FROM EmailConversationAssignTo TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = " + UserId + " or TP.AddedByUserID = " + UserId + ")) OR EXISTS(SELECT * FROM EmailConversationAssignCC TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = " + UserId + " or TP.AddedByUserID = " + UserId + ")) OR EXISTS(SELECT * FROM EmailConversationParticipant TP WHERE ECC.ID = TP.ConversationId AND (TP.UserId = " + UserId + " or TP.AddedByUserID = " + UserId + "))))K LEFT JOIN Employee E ON EC.LastUpdateUserID = E.UserId where ET.SessionId = '" + result.EmailTopicSessionId + "' AND  ET.OnDraft = 0 AND EC.ID=K.ReplyId";

                    var records = new List<view_ActivityEmailSubjects>();

                    using (var connection = CreateConnection())
                    {
                        records = (await connection.QueryAsync<view_ActivityEmailSubjects>(query)).ToList();
                    }
                    if (records.Count > 0)
                    {
                        records.ForEach(r =>
                        {
                            view_ActivityEmailSubjects view_ActivityEmailSubjects = new view_ActivityEmailSubjects();
                            view_ActivityEmailSubjects.TopicId = r.TopicId;
                            view_ActivityEmailSubjects.AddedByUserID = r.AddedByUserID;
                            view_ActivityEmailSubjects.AddedDate = r.AddedDate;
                            view_ActivityEmailSubjects.FirstName = r.FirstName;
                            view_ActivityEmailSubjects.LastName = r.LastName;
                            view_ActivityEmailSubjects.DueDate = r.DueDate;
                            view_ActivityEmailSubjects.TopicName = r.TopicName;
                            view_ActivityEmailSubjects.AddedByUser = r.AddedByUser;
                            activityEmailSubjects.Add(view_ActivityEmailSubjects);
                        });
                    }
                }
                return activityEmailSubjects;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        
    }
}
