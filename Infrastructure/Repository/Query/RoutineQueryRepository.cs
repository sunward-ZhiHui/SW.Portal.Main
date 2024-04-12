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
    public class RoutineQueryRepository : QueryRepository<ProductionActivityRoutineAppLine>, IRoutineQueryRepository
    {
        public RoutineQueryRepository(IConfiguration configuration)
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
        public async Task<MultipleProductioRoutineAppLineItemLists> GetMultipleRoutineQueryAsync(List<long?> ProductionActivityAppLineIds, List<Guid?> SessionIds, List<long?> navprodOrderLineIds, List<int?> statusCodeIds, List<long?> masterChildIds, List<long?> masterDetaildChildIds, List<long?> manufacturingProcessChildIds)
        {
            MultipleProductioRoutineAppLineItemLists multipleProductioAppLineItemLists = new MultipleProductioRoutineAppLineItemLists();
            try
            {
                var query = "select ProductActivityCaseResponsId,ProductActivityCaseId from ProductActivityCaseRespons;";
                query += "select ProductActivityCaseResponsDutyId, ProductActivityCaseResponsId from ProductActivityCaseResponsDuty;";
                query += "select EmployeeId, ProductActivityCaseResponsDutyId from ProductActivityCaseResponsResponsible;";
                query += "select t1.RoutineInfoMultipleId,t1.RoutineInfoId,t1.ProductionActivityRoutineAppLineId,t2.Value as AcitivityMasterName from RoutineInfoMultiple t1 JOIN ApplicationMasterDetail t2 ON t1.RoutineInfoId=t2.ApplicationMasterDetailID;";
                ProductionActivityAppLineIds = ProductionActivityAppLineIds != null && ProductionActivityAppLineIds.Count > 0 ? ProductionActivityAppLineIds : new List<long?>() { -1 };
                query += "select * from ProductionActivityRoutineAppLineQaChecker where ProductionActivityRoutineAppLineId in(" + string.Join(',', ProductionActivityAppLineIds) + ");";
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
                query += "select ActivityEmailTopicID,ActivityType,EmailTopicSessionId,ActivityMasterId,SessionId from ActivityEmailTopics where documentsessionid is not null AND ActivityType='RoutineActivity' AND ActivityMasterId in(" + string.Join(',', ProductionActivityAppLineIds) + ");";
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
                    multipleProductioAppLineItemLists.ActivityMasterMultiple = result.Read<RoutineInfoMultiple>().ToList();
                    multipleProductioAppLineItemLists.ProductionActivityAppLineQaCheckerModel = result.Read<ProductionActivityRoutineAppLineQaCheckerModel>().ToList();
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
        public async Task<IReadOnlyList<ProductionActivityRoutineAppModel>> GetAllProductionActivityRoutineAsync(ProductionActivityRoutineAppModel value)
        {
            List<ProductionActivityRoutineAppModel> productActivityAppModels = new List<ProductionActivityRoutineAppModel>();
            try
            {
                var userId = value.AddedByUserID;
                var parameters = new DynamicParameters();
                parameters.Add("AddedByUserID", value.AddedByUserID);
                parameters.Add("CompanyID", value.CompanyId);
                parameters.Add("ProdOrderNo", value.ProdOrderNo, DbType.String);
                parameters.Add("LocationID", value.LocationId);
                parameters.Add("ProdActivityResultId", value.ProdActivityResultId);
                parameters.Add("ManufacturingProcessChildId", value.ManufacturingProcessChildId);
                parameters.Add("ProdActivityCategoryChildId", value.ProdActivityCategoryChildId);
                parameters.Add("ProdActivityActionChildD", value.ProdActivityActionChildD);
                parameters.Add("RoutineStatusId", value.RoutineStatusId);
                parameters.Add("VisaMasterId", value.VisaMasterId);
                parameters.Add("NavprodOrderLineId", value.NavprodOrderLineId);
                parameters.Add("StartDate", value.StartDate, DbType.DateTime);
                parameters.Add("EndDate", value.EndDate, DbType.DateTime);
                parameters.Add("TimeSheetAction", value.TimeSheetAction);
                parameters.Add("ItemName", value.ItemName);
                parameters.Add("LotNo", value.LotNo);
                var query = "";

                if(value.TimeSheetAction == true)
                {
                    query = @"select t1.ProductionActivityRoutineAppLineID,t1.ProductionActivityRoutineAppID,t1.ActionDropdown,t1.ProdActivityActionID,t1.ProdActivityCategoryID,t1.ManufacturingProcessID,t1.IsTemplateUpload,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID as LineSessionId,t1.ProductActivityCaseLineID,t1.NavprodOrderLineID,t1.Comment as LineComment,t1.QaCheck,t1.IsOthersOptions,t1.ProdActivityResultID,t1.ManufacturingProcessChildID,t1.ProdActivityCategoryChildID,t1.ProdActivityActionChildD,t1.TopicID,t1.QaCheckUserID,t1.QaCheckDate,t1.ProductActivityCaseID,t1.VisaMasterID,t1.RoutineStatusID,t1.CommentImage,t1.CommentImageType,t1.ProfileID,t1.ProfileNo,t1.IsCheckNoIssue,t1.CheckedByID,t1.CheckedDate,t1.CheckedRemark,t1.IsCheckReferSupportDocument,CASE WHEN  t1.ProfileNo IS NULL THEN '' ELSE  t1.ProfileNo END AS ProfileNo,t1.ProfileId
                    ,t2.CompanyID,t10.PlantCode as CompanyName,t2.ProdOrderNo,t2.Comment,t2.SessionId,t2.LocationID,t14.Description as LocationName,t2.BatchNo,
                    'Production Routine' as Type,
                    (case when t1.IsTemplateUpload=1 then 'Yes' ELSE 'No' END) as IsTemplateUploadFlag,
                    (select COUNT(tt1.ProductionActivityAppLineDocId) from ProductionActivityAppLineDoc tt1 WHERE tt1.Type = 'Production Routine' AND tt1.ProductionActivityAppLineID=t1.ProductionActivityRoutineAppLineID) as SupportDocCount,t12.NameOfTemplate,t12.Link,t12.LocationToSaveId
                    from ProductionActivityRoutineAppLine t1 
                    JOIN ProductionActivityRoutineApp t2 ON t1.ProductionActivityRoutineAppID=t2.ProductionActivityRoutineAppID  AND (TimeSheetAction = @TimeSheetAction OR (TimeSheetAction = 1 AND @TimeSheetAction IS NULL) OR (TimeSheetAction IS NULL AND @TimeSheetAction = 0)) 
                    
                  
                    JOIN Plant as t10 ON t10.PlantID = t2.CompanyID 
                    LEFT JOIN ICTMaster t14 ON t14.ictMasterId=t2.LocationId
                    LEFT JOIN ProductActivityCaseLine as t12 ON t12.ProductActivityCaseLineId = t1.ProductActivityCaseLineId
                    WHERE t1.ProductionActivityRoutineAppLineID>0";
                }
                else
                {
                    query = @"select t1.ProductionActivityRoutineAppLineID,t1.ProductionActivityRoutineAppID,t1.ActionDropdown,t1.ProdActivityActionID,t1.ProdActivityCategoryID,t1.ManufacturingProcessID,t1.IsTemplateUpload,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID as LineSessionId,t1.ProductActivityCaseLineID,t1.NavprodOrderLineID,t1.Comment as LineComment,t1.QaCheck,t1.IsOthersOptions,t1.ProdActivityResultID,t1.ManufacturingProcessChildID,t1.ProdActivityCategoryChildID,t1.ProdActivityActionChildD,t1.TopicID,t1.QaCheckUserID,t1.QaCheckDate,t1.ProductActivityCaseID,t1.VisaMasterID,t1.RoutineStatusID,t1.CommentImage,t1.CommentImageType,t1.ProfileID,t1.ProfileNo,t1.IsCheckNoIssue,t1.CheckedByID,t1.CheckedDate,t1.CheckedRemark,t1.IsCheckReferSupportDocument,CASE WHEN  t1.ProfileNo IS NULL THEN '' ELSE  t1.ProfileNo END AS ProfileNo,t1.ProfileId
                    ,t2.CompanyID,t10.PlantCode as CompanyName,t2.ProdOrderNo,t2.Comment,t2.SessionId,t2.LocationID,t14.Description as LocationName,t2.BatchNo,
                    'Production Routine' as Type,
                    (case when t1.IsTemplateUpload=1 then 'Yes' ELSE 'No' END) as IsTemplateUploadFlag,
                    (select COUNT(tt1.ProductionActivityAppLineDocId) from ProductionActivityAppLineDoc tt1 WHERE tt1.Type = 'Production Routine' AND tt1.ProductionActivityAppLineID=t1.ProductionActivityRoutineAppLineID) as SupportDocCount,t12.NameOfTemplate,t12.Link,t12.LocationToSaveId
                    from ProductionActivityRoutineAppLine t1 
                    JOIN ProductionActivityRoutineApp t2 ON t1.ProductionActivityRoutineAppID=t2.ProductionActivityRoutineAppID  AND (TimeSheetAction = @TimeSheetAction OR (TimeSheetAction = 1 AND @TimeSheetAction IS NULL) OR (TimeSheetAction IS NULL AND @TimeSheetAction = 0))
                    JOIN Plant as t10 ON t10.PlantID = t2.CompanyID 
                    LEFT JOIN ICTMaster t14 ON t14.ictMasterId=t2.LocationId
                    LEFT JOIN ProductActivityCaseLine as t12 ON t12.ProductActivityCaseLineId = t1.ProductActivityCaseLineId
                    WHERE t1.ProductionActivityRoutineAppLineID>0";
                }
                
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
                if (value.TimeSheetAction == true)
                {
                    if (value.ItemName == null || value.ItemName == "")
                    {
                        query += "\n\rAND (t2.ItemName IS NULL or t2.ItemName = '')";
                    }
                    else
                    {
                        query += "\n\rAND t2.ItemName=@ItemName";
                    }                    
                    
                    if (value.LotNo == null || value.LotNo == "")
                    {
                        query += "\n\rAND (t2.LotNo IS NULL OR t2.LotNo = '')";
                    }
                    else
                    {
                        query += "\n\rAND t2.LotNo=@LotNo";
                    }

                    if (value.LocationId > 0)
                    {
                        query += "\n\rAND t2.LocationID=@LocationID";
                    }
                    else
                    {                        
                        query += "\n\rAND (t2.LocationID IS NULL or t2.LocationID = 0 or t2.LocationID = '')";
                    }

                }
                else
                {
                    if (value.LocationId > 0)
                    {
                        query += "\n\rAND t2.LocationID=@LocationID";
                    }
                }
               
                if (value.ProdActivityResultId > 0)
                {
                    query += "\n\rAND t1.ProdActivityResultID=@ProdActivityResultId";
                }
                if (value.RoutineStatusId > 0)
                {
                    query += "\n\rAND t1.RoutineStatusId=@RoutineStatusId";
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
                    query += "\n\r AND CAST(t1.AddedDate AS Date)<='" + to + "'\r\n";
                }
                if (value.GetTypes == "User")
                {
                    query += "\n\rAND t1.AddedByUserID=@AddedByUserID";
                }
                var employeeAll = await GetAllUserWithoutStatussAsync();
                var productActivityApps = new List<ProductionActivityRoutineAppModel>();
                using (var connection = CreateConnection())
                {
                    productActivityApps = (await connection.QueryAsync<ProductionActivityRoutineAppModel>(query, parameters)).ToList();
                }
                if (productActivityApps.Count > 0)
                {
                    var masterChildIds = new List<long?>();
                    var masterDetaildChildIds = new List<long?>();
                    masterDetaildChildIds.AddRange(productActivityApps.ToList().Where(w => w.RoutineStatusId > 0).Select(s => s.RoutineStatusId).Distinct().ToList());
                    masterDetaildChildIds.AddRange(productActivityApps.ToList().Where(w => w.ProdActivityResultId > 0).Select(s => s.ProdActivityResultId).Distinct().ToList());
                    masterChildIds.AddRange(productActivityApps.ToList().Where(w => w.ManufacturingProcessChildId > 0).Select(s => s.ManufacturingProcessChildId).Distinct().ToList());
                    masterChildIds.AddRange(productActivityApps.ToList().Where(w => w.ProdActivityCategoryChildId > 0).Select(s => s.ProdActivityCategoryChildId).Distinct().ToList());
                    masterChildIds.AddRange(productActivityApps.ToList().Where(w => w.ProdActivityActionChildD > 0).Select(s => s.ProdActivityActionChildD).Distinct().ToList());
                    var manufacturingProcessChildIds = productActivityApps.ToList().Where(w => w.ManufacturingProcessChildId > 0).Select(s => s.ManufacturingProcessChildId).Distinct().ToList();
                    var statusCodeIds = productActivityApps.ToList().Where(w => w.StatusCodeID > 0).Select(s => s.StatusCodeID).Distinct().ToList();
                    var navprodOrderLineIds = productActivityApps.ToList().Where(w => w.NavprodOrderLineId > 0).Select(s => s.NavprodOrderLineId).Distinct().ToList();
                    var productionActivityAppLineIds = productActivityApps.ToList().Select(s => s.ProductionActivityRoutineAppLineId).Distinct().ToList();
                    var sessionIds = productActivityApps.ToList().Where(w => w.LineSessionId != null).Select(s => s.LineSessionId).ToList();
                    var addedIds = productActivityApps.ToList().Select(s => s.AddedByUserID).Distinct().ToList();
                    addedIds.Add(userId);
                    var employee = employeeAll != null && employeeAll.Count() > 0 ? employeeAll.Where(w => addedIds.Contains(w.UserID)).ToList() : null;
                    var loginUser = employee != null && employee.Count() > 0 ? employee.FirstOrDefault(w => w.UserID == userId)?.DepartmentID : null;

                    var templateTestCaseCheckList = await GetMultipleRoutineQueryAsync(productionActivityAppLineIds, sessionIds, navprodOrderLineIds, statusCodeIds, masterChildIds, masterDetaildChildIds, manufacturingProcessChildIds);
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

                        var currentActivityResponseIds = templateTestCaseCheckListResponseDuty.Where(r => r.ProductActivityCaseResponsId == s.ProductionActivityRoutineAppLineId).Select(r => r.ProductActivityCaseResponsDutyId).ToList();

                        var activityCaseManufacturingProcessList = activityCaseList.Where(a => a.ManufacturingProcessChildId == s.ManufacturingProcessChildId).Select(s => s.ProductActivityCaseId).ToList();
                        var activitycaseReponseIds = prodactivityCategoryMultiplelist.Where(p => activityCaseManufacturingProcessList.Contains(p.ProductActivityCaseId.Value) && p.CategoryActionId == s.ProdActivityCategoryChildId).Select(s => s.ProductActivityCaseId).ToList();
                        var actionmultipleIds = prodactivityActionMultiplelist.Where(p => activityCaseManufacturingProcessList.Contains(p.ProductActivityCaseId.Value) && activitycaseReponseIds.Contains(p.ProductActivityCaseId) && p.ActionId == s.ProdActivityActionChildD).Select(s => s.ProductActivityCaseId).ToList();
                        if (actionmultipleIds != null && actionmultipleIds.Count > 0)
                        {
                            var responseId = templateTestCaseCheckListResponse.Where(r => actionmultipleIds.Contains(r.ProductActivityCaseId)).Select(r => r.ProductActivityCaseResponsId).ToList();
                            var productActivityCaseResponsDutyIds = templateTestCaseCheckListResponseDuty.Where(r => responseId.Contains(r.ProductActivityCaseResponsId.Value)).Select(t => t.ProductActivityCaseResponsDutyId).ToList();
                            if (productActivityCaseResponsDutyIds != null && productActivityCaseResponsDutyIds.Count > 0)
                            {
                                if (s.AddedByUserID == userId)
                                {
                                    responsibilityUsers.Add(userId.Value);
                                }
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


                        ProductionActivityRoutineAppModel productActivityApp = new ProductionActivityRoutineAppModel();
                        productActivityApp.ProductActivityPermissionData = new ProductActivityPermissionModel();
                        productActivityApp.Type = s.Type;
                        productActivityApp.ResponsibilityUsers = responsibilityUsers;
                        productActivityApp.ProductActivityCaseId = s.ProductActivityCaseId;
                        productActivityApp.SupportDocCount = s.SupportDocCount;
                        productActivityApp.ProductionActivityRoutineAppLineId = s.ProductionActivityRoutineAppLineId;
                        productActivityApp.ProductionActivityRoutineAppId = s.ProductionActivityRoutineAppId;
                        productActivityApp.Comment = s.Comment;
                        productActivityApp.LineComment = s.LineComment;
                        productActivityApp.RoutineStatusId = s.RoutineStatusId;
                        productActivityApp.RoutineStatus = s.RoutineStatusId > 0 && applicationMasterDetail != null && applicationMasterDetail.Count() > 0 ? applicationMasterDetail.FirstOrDefault(f => f.ApplicationMasterDetailId == s.RoutineStatusId)?.Value : string.Empty;
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
                        productActivityApp.CheckedById = s.CheckedById;
                        productActivityApp.IsCheckNoIssue = s.IsCheckNoIssue == true ? true : false;
                        productActivityApp.IsCheckReferSupportDocument = s.IsCheckReferSupportDocument == true ? true : false;
                        productActivityApp.CheckedRemark = s.CheckedRemark;
                        productActivityApp.CheckedByUser = appUser != null && appUser.Count() > 0 ? appUser.FirstOrDefault(f => f.UserID == s.CheckedById)?.UserName : string.Empty;
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
                            productActivityApp.ProdOrderNoDesc = s.ProdOrderNo;
                            productActivityApp.BatchNo = s.BatchNo;
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
                        productActivityApp.LocationName = s.LocationName + "|" + s.BatchNo + "|" + s.ProdOrderNo;
                        productActivityApp.ProductionActivityRoutineAppLineQaCheckerModels = productionActivityAppLineQaChecker != null ? productionActivityAppLineQaChecker.Where(z => z.ProductionActivityRoutineAppLineId == s.ProductionActivityRoutineAppLineId).ToList() : new List<ProductionActivityRoutineAppLineQaCheckerModel>();
                        productActivityApp.DocumentPermissionData = new DocumentPermissionModel();
                        productActivityApp.RoutineInfoIds = activityMasterMultiple != null && activityMasterMultiple.Count > 0 ? activityMasterMultiple.Where(a => a.ProductionActivityRoutineAppLineId == s.ProductionActivityRoutineAppLineId).Select(z => z.RoutineInfoId).ToList() : new List<long?>();
                        var masterList = activityMasterMultiple != null && activityMasterMultiple.Count > 0 ? activityMasterMultiple.Where(a => a.ProductionActivityRoutineAppLineId == s.ProductionActivityRoutineAppLineId).Select(z => z.AcitivityMasterName).ToList() : new List<string?>();
                        productActivityApp.RoutineInfoStatus = string.Join(",", masterList);
                        var emailcreated = activityEmailTopicList.Where(a => a.ActivityMasterId == s.ProductionActivityRoutineAppLineId)?.FirstOrDefault();
                        if (emailcreated != null)
                        {
                            productActivityApp.IsPartialEmailCreated = true;
                            productActivityApp.EmailActivitySessionId = emailcreated.SessionId;
                            if (emailcreated.EmailTopicSessionId != null)
                            {
                                productActivityApp.EmailSessionId = emailcreated.EmailTopicSessionId;
                                productActivityApp.IsEmailCreated = true;
                            }
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
                if (value.VisaMasterId > 0)
                {
                    productActivityAppModels = productActivityAppModels.Where(w => w.RoutineInfoIds.Contains(value.VisaMasterId)).ToList();
                }
                return productActivityAppModels;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }

        }
        public async Task<ProductionActivityRoutineAppModel> DeleteproductActivityRoutineAppLine(ProductionActivityRoutineAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", value.LineSessionId);
                        parameters.Add("ProductionActivityRoutineAppLineId", value.ProductionActivityRoutineAppLineId);
                        var query = "Delete from  RoutineInfoMultiple WHERE ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId;";
                        query += "Delete from  ProductionActivityRoutineAppLine WHERE ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId;";
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
        public async Task<ProductionActivityRoutineAppModel> UpdateproductActivityRoutineAppLineCommentField(ProductionActivityRoutineAppModel value)
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
                        parameters.Add("ProductionActivityRoutineAppLineId", value.ProductionActivityRoutineAppLineId);
                        var query = "Update ProductionActivityRoutineAppLine SET Comment=@Comment WHERE ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId";
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

        public async Task<ProductionActivityRoutineAppModel> UpdateActivityRoutineStatus(ProductionActivityRoutineAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductionActivityRoutineAppLineId", value.ProductionActivityRoutineAppLineId);
                        parameters.Add("RoutineStatusId", value.RoutineStatusId);
                        var query = string.Empty;
                        query += "Update ProductionActivityRoutineAppLine Set RoutineStatusId=@RoutineStatusId  Where ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId;";
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
        public async Task<ProductionActivityRoutineAppModel> GetProductActivityRoutineAppLineOneItem(long? ProductionActivityAppLineID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductionActivityRoutineAppLineId", ProductionActivityAppLineID);
                var query = "select t1.ProductionActivityRoutineAppLineId,SessionID,(select COUNT(*) from Documents t2 where t2.SessionID=t1.SessionID AND (t2.IsDelete IS NULL OR t2.IsDelete=0))as IsDocuments\r\nfrom ProductionActivityRoutineAppLine t1 where t1.ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId;";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<ProductionActivityRoutineAppModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductionActivityRoutineAppModel> UpdateActivityRoutineMaster(ProductionActivityRoutineAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        if (value.ProductionActivityRoutineAppLineId > 0)
                        {
                            var Deletequery = "DELETE  FROM RoutineInfoMultiple WHERE ProductionActivityRoutineAppLineId = " + value.ProductionActivityRoutineAppLineId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        if (value.RoutineInfoIds != null)
                        {
                            var listData = value.RoutineInfoIds.ToList();
                            if (listData.Count > 0)
                            {
                                var querys = string.Empty;
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [RoutineInfoMultiple](RoutineInfoId,ProductionActivityRoutineAppLineId) " +
                                                        "VALUES ( " + s + "," + value.ProductionActivityRoutineAppLineId + ");\r\n";
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


        public async Task<IReadOnlyList<ProductionActivityRoutineAppLine>> GetAllAsync()
        {
            try
            {
                var query = "select  AMC.Value as Process,AMD.Value as Result,AMC1.Value as Category,AMC2.Value AS Action from ProductionActivityRoutineAppLine as PAAL inner Join ApplicationMasterChild as AMC ON AMC.ApplicationMasterChildID = PAAL.ManufacturingProcessChildID \r\ninner Join ApplicationMasterChild as AMC1 ON AMC1.ApplicationMasterChildID = PAAL.ProdActivityCategoryChildID inner Join ApplicationMasterChild as AMC2 ON AMC2.ApplicationMasterChildID = PAAL.ProdActivityActionChildD inner join ApplicationMasterDetail as AMD ON AMD.ApplicationMasterDetailID = PAAL.ProdActivityResultID";


                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityRoutineAppLine>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductionActivityRoutineAppModel> UpdateRoutineChecker(ProductionActivityRoutineAppModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IsCheckNoIssue", value.IsCheckNoIssue);
                        parameters.Add("IsCheckReferSupportDocument", value.IsCheckReferSupportDocument);
                        parameters.Add("CheckedRemark", value.CheckedRemark, DbType.String);
                        parameters.Add("CheckedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("CheckedById", value.CheckedById);
                        parameters.Add("ProductionActivityRoutineAppLineId", value.ProductionActivityRoutineAppLineId);
                        var query = "Update ProductionActivityRoutineAppLine SET CheckedDate=@CheckedDate,@CheckedById=@CheckedById,IsCheckNoIssue=@IsCheckNoIssue,IsCheckReferSupportDocument=@IsCheckReferSupportDocument,CheckedRemark=@CheckedRemark WHERE ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId";
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


        public async Task<IReadOnlyList<ProductionActivityRoutineCheckedDetailsModel>> GetProductionActivityRoutineCheckedDetails(long? value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductionActivityRoutineAppLineId", value);
                var query = "select t1.*,t2.Value as RoutineResultName,t4.userName as checkedByUserName,t3.Value as RoutineStatusName from ProductionActivityRoutineCheckedDetails t1" +
                    " LEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.RoutineResultID LEFT JOIN ApplicationUser t4 ON t4.userId=t1.checkedByID " +
                    " LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t1.RoutineStatusID  where t1.ProductionActivityRoutineAppLineId =@ProductionActivityRoutineAppLineId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityRoutineCheckedDetailsModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductionActivityRoutineCheckedDetailsModel> DeleteProductionActivityRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductionActivityRoutineCheckedDetailsId", value.ProductionActivityRoutineCheckedDetailsId);
                        var query = string.Empty;
                        query += "Delete from  ProductionActivityRoutineCheckedDetails WHERE ProductionActivityRoutineCheckedDetailsId=@ProductionActivityRoutineCheckedDetailsId;";
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
        public async Task<ProductionActivityRoutineCheckedDetailsModel> InsertProductionActivityRoutineCheckedDetails(ProductionActivityRoutineCheckedDetailsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProductionActivityRoutineCheckedDetailsId", value.ProductionActivityRoutineCheckedDetailsId);
                        parameters.Add("ProductionActivityRoutineAppLineId", value.ProductionActivityRoutineAppLineId);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("ActivityInfoId", value.ActivityInfoId);
                        parameters.Add("RoutineStatusId", value.RoutineStatusId);
                        parameters.Add("RoutineResultId", value.RoutineResultId);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("ProductionActivityRoutineAppId", value.ProductionActivityRoutineAppId);
                        parameters.Add("IsCheckNoIssue", value.IsCheckNoIssue);
                        parameters.Add("CheckedById", value.CheckedById);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("CommentImageType", value.CommentImageType);
                        parameters.Add("IsCheckReferSupportDocument", value.IsCheckReferSupportDocument);
                        parameters.Add("CheckedComment", value.CheckedComment, DbType.String);
                        parameters.Add("CheckedDate", value.CheckedDate, DbType.DateTime);
                        string? hex = null;
                        if (value.CommentImage != null && !string.IsNullOrEmpty(value.CommentImages))
                        {
                            var image = Convert.FromBase64String(value.CommentImages);
                            hex = BitConverter.ToString(image);
                            hex = hex.Replace("-", "");
                            hex = "0x" + hex;
                        }
                        if (value.ProductionActivityRoutineCheckedDetailsId > 0)
                        {

                            var query = "Update ProductionActivityRoutineCheckedDetails Set RoutineStatusId=@RoutineStatusId,RoutineResultId=@RoutineResultId,SessionId=@SessionId,ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID,IsCheckNoIssue=@IsCheckNoIssue,CheckedById=@CheckedById,CheckedComment=@CheckedComment,\n\r";
                            if (!string.IsNullOrEmpty(hex))
                            {
                                query += "CommentImage=(CONVERT(VARBINARY(MAX), '" + hex + "',1)),\n\r";
                            }
                            query += "CommentImageType=@CommentImageType,IsCheckReferSupportDocument=@IsCheckReferSupportDocument,CheckedDate=@CheckedDate  Where ProductionActivityRoutineCheckedDetailsId=@ProductionActivityRoutineCheckedDetailsId;";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO ProductionActivityRoutineCheckedDetails(CheckedDate,RoutineStatusId,RoutineResultId,SessionId,ProductionActivityRoutineAppId,ActivityInfoId,AddedByUserID,AddedDate,StatusCodeID,ModifiedByUserID,ModifiedDate,ProductionActivityRoutineAppLineId,IsCheckNoIssue,CheckedById,CheckedComment,CommentImageType,IsCheckReferSupportDocument\n\r";
                            if (!string.IsNullOrEmpty(hex))
                            {
                                query += ",CommentImage \n\r";
                            }
                            query += ")\n\r";
                            query += "OUTPUT INSERTED.ProductionActivityRoutineCheckedDetailsId  VALUES (@CheckedDate,@RoutineStatusId,@RoutineResultId,@SessionId,@ProductionActivityRoutineAppId,@ActivityInfoId,@AddedByUserID,@AddedDate,@StatusCodeID,@ModifiedByUserID,@ModifiedDate,@ProductionActivityRoutineAppLineId,@IsCheckNoIssue,@CheckedById,@CheckedComment,@CommentImageType,@IsCheckReferSupportDocument\n\r";
                            if (!string.IsNullOrEmpty(hex))
                            {
                                query += ",(CONVERT(VARBINARY(MAX), '" + hex + "',1))\n\r";
                            }
                            query += ");";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                            var querys = string.Empty;
                            querys += "Update ProductionActivityRoutineAppLine Set RoutineStatusId=@RoutineStatusId,ProdActivityResultId=@RoutineResultId  Where ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId;";
                            await connection.QuerySingleOrDefaultAsync<long>(querys, parameters);
                            value.ProductionActivityRoutineCheckedDetailsId = insertedId;
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

        public async Task<IReadOnlyList<ProductionActivityRoutineEmailModel>> GetProductionActivityRoutineEmailList(long? ProductionActivityRoutineAppLineID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductionActivityRoutineAppLineID", ProductionActivityRoutineAppLineID);
                var query = @"select AE.SessionId as ActivityRoutineEmailSessionId,AE.EmailTopicSessionId,PR.ProductionActivityRoutineAppLineID From ProductionActivityRoutineAppLine PR
                               left join  ActivityEmailTopics AE on AE.ActivityMasterId = PR.ProductionActivityRoutineAppLineID Where AE.ActivityType =  'RoutineActivity' and PR.ProductionActivityRoutineAppLineID =@ProductionActivityRoutineAppLineID
                              ";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityRoutineEmailModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
