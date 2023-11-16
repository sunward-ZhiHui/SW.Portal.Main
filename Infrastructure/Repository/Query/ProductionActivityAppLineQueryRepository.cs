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
        public async Task<MultipleProductioAppLineItemLists> GetMultipleQueryAsync(List<long?> ProductionActivityAppLineIds, List<Guid?> SessionIds, List<long?> navprodOrderLineIds)
        {
            MultipleProductioAppLineItemLists multipleProductioAppLineItemLists = new MultipleProductioAppLineItemLists();
            try
            {
                var query = "select ProductActivityCaseResponsId,ProductActivityCaseId from ProductActivityCaseRespons;";
                query += "select ProductActivityCaseResponsDutyId, ProductActivityCaseResponsId from ProductActivityCaseResponsDuty;";
                query += "select EmployeeId, ProductActivityCaseResponsDutyId from ProductActivityCaseResponsResponsible;";
                query += "select * from ActivityMasterMultiple;";
                ProductionActivityAppLineIds = ProductionActivityAppLineIds != null && ProductionActivityAppLineIds.Count > 0 ? ProductionActivityAppLineIds : new List<long?>() { -1 };
                query += "select * from ProductionActivityAppLineQaChecker where ProductionActivityAppLineId in(" + string.Join(',', ProductionActivityAppLineIds) + ");";
                SessionIds = SessionIds != null && SessionIds.Count > 0 ? SessionIds : new List<Guid?>() { Guid.NewGuid() };
                query += DocumentQueryString() + " where  SessionId in(" + string.Join(",", SessionIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ") AND IsLatest=1 AND (IsDelete is null or IsDelete=0);";
                query += "select UserName,UserId,SessionId from ApplicationUser;";
                navprodOrderLineIds = navprodOrderLineIds != null && navprodOrderLineIds.Count > 0 ? navprodOrderLineIds : new List<long?>() { -1 };
                query += "select t11.NAVProdOrderLineId,t11.ItemNo,t11.Description,t11.Description1,t11.BatchNo,t11.RePlanRefNo,CONCAT(t11.ProdOrderNo,'|',t11.Description,(case when ISNULL(NULLIF(t11.Description1, ''), null) is NULL then  t11.Description1 ELSE  CONCAT(' | ',Description1) END)) as ProdOrderNoDesc from NavprodOrderLine t11 where t11.NAVProdOrderLineId in(" + string.Join(',', navprodOrderLineIds) + ");";
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
                }
                return multipleProductioAppLineItemLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentUserRole>> GetDocumentUserRoleAsync(List<long?> fileProfileTypeId)
        {
            try
            {
                fileProfileTypeId = fileProfileTypeId != null && fileProfileTypeId.Count > 0 ? fileProfileTypeId : new List<long?>() { -1 };
                var query = "select  * from DocumentUserRole where FileProfileTypeId in(" + string.Join(',', fileProfileTypeId) + ")";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentUserRole>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentPermissionModel>> GetDocumentPermissionByRoll()
        {
            try
            {
                var query = "select  * from DocumentPermission";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentPermissionModel>(query)).ToList();
                }
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
        public async Task<IReadOnlyList<ProductActivityAppModel>> GetAllAsync(long? CompanyID, string? prodorderNo, long? userId, long? locationID)
        {
            List<ProductActivityAppModel> productActivityAppModels = new List<ProductActivityAppModel>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyID);
                parameters.Add("ProdOrderNo", prodorderNo, DbType.String);
                parameters.Add("LocationID", locationID);
                var query = @"select 
                    t1.ProductionActivityAppLineID,t1.ProductionActivityAppID,t1.ActionDropdown,t1.ProdActivityActionID,t1.ProdActivityCategoryID,t1.ManufacturingProcessID,t1.IsTemplateUpload,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID as LineSessionId,t1.ProductActivityCaseLineID,t1.NavprodOrderLineID,t1.Comment as LineComment,t1.QaCheck,t1.IsOthersOptions,t1.ProdActivityResultID,t1.ManufacturingProcessChildID,t1.ProdActivityCategoryChildID,t1.ProdActivityActionChildD,t1.TopicID,t1.QaCheckUserID,t1.QaCheckDate,t1.LocationID,t1.ProductActivityCaseID,t1.ActivityMasterID,t1.ActivityStatusID,t5.CodeValue as StatusCode,t6.Value as ProdActivityResult,t7.Value as ManufacturingProcessChild,t8.Value as ProdActivityCategoryChild,t9.Value AS ProdActivityActionChild,t2.CompanyID,t10.PlantCode as CompanyName,t2.ProdOrderNo,t2.Comment,t2.SessionId,t2.LocationID,
                    'Production Activity' as Type,
                    (case when t1.IsTemplateUpload=1 then 'Yes' ELSE 'No' END) as IsTemplateUploadFlag,
                    (select COUNT(tt1.ProductionActivityAppLineDocId) from ProductionActivityAppLineDoc tt1 WHERE tt1.Type = 'Production Activity' AND tt1.ProductionActivityAppLineID=t1.ProductionActivityAppLineID) as SupportDocCount,t12.NameOfTemplate,t12.Link,t12.LocationToSaveId
                    from ProductionActivityAppLine t1 
                    JOIN ProductionActivityApp t2 ON t1.ProductionActivityAppId=t2.ProductionActivityAppId
                    LEFT JOIN CodeMaster t5 ON t5.CodeID=t1.StatusCodeID
                    LEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.ProdActivityResultID
                    LEFT JOIN ApplicationMasterChild as t7 ON t7.ApplicationMasterChildID = t1.ManufacturingProcessChildID 
                    LEFT JOIN ApplicationMasterChild as t8 ON t8.ApplicationMasterChildID = t1.ProdActivityCategoryChildID 
                    LEFT JOIN ApplicationMasterChild as t9 ON t9.ApplicationMasterChildID = t1.ProdActivityActionChildD
                    JOIN Plant as t10 ON t10.PlantID = t2.CompanyID 
                    LEFT JOIN ProductActivityCaseLine as t12 ON t12.ProductActivityCaseLineId = t1.ProductActivityCaseLineId
                    WHERE t2.CompanyId=@CompanyID 
                    AND t2.ProdOrderNo=@ProdOrderNo ";
                if (locationID > 0)
                {
                    query += " AND t2.LocationID=@LocationID";
                }
                var employeeAll = await GetAllUserWithoutStatussAsync();
                using (var connection = CreateConnection())
                {
                    var productActivityApps = (await connection.QueryAsync<ProductActivityAppModel>(query, parameters)).ToList();
                    if (productActivityApps.Count > 0)
                    {
                        var navprodOrderLineIds = productActivityApps.ToList().Where(w => w.NavprodOrderLineId > 0).Select(s => s.NavprodOrderLineId).Distinct().ToList();
                        var productionActivityAppLineIds = productActivityApps.ToList().Select(s => s.ProductionActivityAppLineId).Distinct().ToList();
                        var sessionIds = productActivityApps.ToList().Where(w => w.LineSessionId != null).Select(s => s.LineSessionId).ToList();
                        var addedIds = productActivityApps.ToList().Select(s => s.AddedByUserID).Distinct().ToList();
                        addedIds.Add(userId);
                        var employee = employeeAll != null && employeeAll.Count() > 0 ? employeeAll.Where(w => addedIds.Contains(w.UserID)).ToList() : null;
                        var loginUser = employee != null && employee.Count() > 0 ? employee.FirstOrDefault(w => w.UserID == userId)?.DepartmentID : null;

                        var templateTestCaseCheckList = await GetMultipleQueryAsync(productionActivityAppLineIds, sessionIds, navprodOrderLineIds);
                        var productionActivityAppLineQaChecker = templateTestCaseCheckList.ProductionActivityAppLineQaCheckerModel.ToList();
                        var templateTestCaseCheckListResponse = templateTestCaseCheckList.ProductActivityCaseRespons.ToList();
                        var templateTestCaseCheckListResponseDuty = templateTestCaseCheckList.ProductActivityCaseResponsDuty.ToList();
                        var templateTestCaseCheckListResponseResponsible = templateTestCaseCheckList.ProductActivityCaseResponsResponsible.ToList();


                        var documents = templateTestCaseCheckList.Documents.ToList();
                        var docIds = documents.Select(s => s.DocumentId).ToList();
                        var userIds = documents != null && documents.Count > 0 ? documents.Where(x => x.LockedByUserId > 0).Select(s => s.LockedByUserId).Distinct().ToList() : new List<long?>();
                        var filterProfileTypeIds = documents != null && documents.Count > 0 ? documents.Where(x => x.FilterProfileTypeId > 0).Select(s => s.FilterProfileTypeId).Distinct().ToList() : new List<long?>();
                        var appUser = templateTestCaseCheckList.ApplicationUser.ToList();

                        var navprodOrderLines = templateTestCaseCheckList.NavprodOrderLine.ToList();
                        var activityMasterMultiple = templateTestCaseCheckList.ActivityMasterMultiple.ToList();
                        var templateTestCaseCheckListIds = productActivityApps.Where(w => w.ProductActivityCaseId != null).Select(s => s.ProductActivityCaseId).ToList();
                        //  var docRoles = await GetDocumentUserRoleAsync(filterProfileTypeIds);
                        // var docPermission = await GetDocumentPermissionByRoll();
                        productActivityApps.ForEach(s =>
                        {
                            var checkSameDept = false;

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
                            {
                                List<long> responsibilityUsers = new List<long>();
                                if (s.ProductActivityCaseId > 0)
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
                                }

                                ProductActivityAppModel productActivityApp = new ProductActivityAppModel();
                                productActivityApp.Type = s.Type;
                                productActivityApp.ResponsibilityUsers = responsibilityUsers;
                                productActivityApp.ProductActivityCaseId = s.ProductActivityCaseId;
                                productActivityApp.SupportDocCount = s.SupportDocCount;
                                productActivityApp.ProductionActivityAppLineId = s.ProductionActivityAppLineId;
                                productActivityApp.ProductionActivityAppId = s.ProductionActivityAppId;
                                productActivityApp.Comment = s.Comment;
                                productActivityApp.LineComment = s.LineComment;
                                productActivityApp.ActivityStatusId = s.ActivityStatusId;
                                productActivityApp.ManufacturingProcessId = s.ManufacturingProcessId;
                                productActivityApp.ProdActivityResultId = s.ProdActivityResultId;
                                productActivityApp.ProdActivityResult = s.ProdActivityResult;
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
                                productActivityApp.StatusCode = s.StatusCode;
                                productActivityApp.ProductActivityCaseLineId = s.ProductActivityCaseLineId;
                                productActivityApp.NameOfTemplate = s.NameOfTemplate;
                                productActivityApp.Link = s.Link;
                                productActivityApp.LocationToSaveId = s.LocationToSaveId;
                                productActivityApp.QaCheck = s.QaCheck == true ? true : false;
                                productActivityApp.ItemNo = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.ItemNo) : string.Empty;
                                productActivityApp.Description = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.Description) : string.Empty;
                                productActivityApp.Description1 = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.Description1) : string.Empty;
                                productActivityApp.BatchNo = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.BatchNo) : string.Empty;
                                productActivityApp.RePlanRefNo = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.RePlanRefNo) : string.Empty;
                                productActivityApp.ProdOrderNoDesc = s.NavprodOrderLineId > 0 && navprodOrderLines != null && navprodOrderLines.Count() > 0 ? (navprodOrderLines.FirstOrDefault(f => f.NavprodOrderLineId == s.NavprodOrderLineId)?.ProdOrderNoDesc) : string.Empty;
                                productActivityApp.NavprodOrderLineId = s.NavprodOrderLineId;
                                productActivityApp.IsOthersOptions = s.IsOthersOptions;
                                productActivityApp.ManufacturingProcessChildId = s.ManufacturingProcessChildId;
                                productActivityApp.ProdActivityActionChildD = s.ProdActivityActionChildD;
                                productActivityApp.ProdActivityCategoryChildId = s.ProdActivityCategoryChildId;
                                productActivityApp.ManufacturingProcessChild = s.ManufacturingProcessChild;
                                productActivityApp.ProdActivityActionChild = s.ProdActivityActionChild;
                                productActivityApp.ProdActivityCategoryChild = s.ProdActivityCategoryChild;
                                productActivityApp.TopicId = s.TopicId;
                                productActivityApp.LocationId = s.LocationId;
                                productActivityApp.ProductionActivityAppLineQaCheckerModels = productionActivityAppLineQaChecker != null ? productionActivityAppLineQaChecker.Where(z => z.ProductionActivityAppLineId == s.ProductionActivityAppLineId).ToList() : new List<ProductionActivityAppLineQaCheckerModel>();
                                productActivityApp.DocumentPermissionData = new DocumentPermissionModel();
                                productActivityApp.ActivityMasterIds = activityMasterMultiple != null && activityMasterMultiple.Count > 0 ? activityMasterMultiple.Where(a => a.ProductionActivityAppLineId == s.ProductionActivityAppLineId).Select(z => z.AcitivityMasterID).ToList() : new List<long?>();
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
                                        /* long? roleId = 0;
                                         roleId = docRoles != null && docRoles.Count() > 0 ? docRoles.FirstOrDefault(f => f.FileProfileTypeId == counts.FilterProfileTypeId && f.UserId == userId)?.RoleId : null;
                                         if (roleId != null)
                                         {
                                             var documentPermission = docPermission != null && docPermission.Count() > 0 ? docPermission.FirstOrDefault(r => r.DocumentRoleID == roleId) : null;
                                             if (documentPermission != null)
                                             {
                                                 productActivityApp.DocumentPermissionData.IsRead = documentPermission.IsRead;
                                                 productActivityApp.DocumentPermissionData.IsCreateFolder = documentPermission.IsCreateFolder;
                                                 productActivityApp.DocumentPermissionData.IsCreateDocument = documentPermission.IsCreateDocument.GetValueOrDefault(false);
                                                 productActivityApp.DocumentPermissionData.IsSetAlert = documentPermission.IsSetAlert;
                                                 productActivityApp.DocumentPermissionData.IsEditIndex = documentPermission.IsEditIndex;
                                                 productActivityApp.DocumentPermissionData.IsRename = documentPermission.IsRename;
                                                 productActivityApp.DocumentPermissionData.IsUpdateDocument = documentPermission.IsUpdateDocument;
                                                 productActivityApp.DocumentPermissionData.IsCopy = documentPermission.IsCopy;
                                                 productActivityApp.DocumentPermissionData.IsMove = documentPermission.IsMove;
                                                 productActivityApp.DocumentPermissionData.IsDelete = documentPermission.IsDelete;
                                                 productActivityApp.DocumentPermissionData.IsRelationship = documentPermission.IsRelationship;
                                                 productActivityApp.DocumentPermissionData.IsListVersion = documentPermission.IsListVersion;
                                                 productActivityApp.DocumentPermissionData.IsInvitation = documentPermission.IsInvitation;
                                                 productActivityApp.DocumentPermissionData.IsSendEmail = documentPermission.IsSendEmail;
                                                 productActivityApp.DocumentPermissionData.IsDiscussion = documentPermission.IsDiscussion;
                                                 productActivityApp.DocumentPermissionData.IsAccessControl = documentPermission.IsAccessControl;
                                                 productActivityApp.DocumentPermissionData.IsAuditTrail = documentPermission.IsAuditTrail;
                                                 productActivityApp.DocumentPermissionData.IsRequired = documentPermission.IsRequired;
                                                 productActivityApp.DocumentPermissionData.IsEdit = documentPermission.IsEdit;
                                                 productActivityApp.DocumentPermissionData.IsFileDelete = documentPermission.IsFileDelete;
                                                 productActivityApp.DocumentPermissionData.IsGrantAdminPermission = documentPermission.IsGrantAdminPermission;
                                                 productActivityApp.DocumentPermissionData.IsDocumentAccess = documentPermission.IsDocumentAccess;
                                                 productActivityApp.DocumentPermissionData.IsCreateTask = documentPermission.IsCreateTask;
                                                 productActivityApp.DocumentPermissionData.IsEnableProfileTypeInfo = documentPermission.IsEnableProfileTypeInfo;
                                                 productActivityApp.DocumentPermissionData.DocumentPermissionID = documentPermission.DocumentPermissionID;
                                             }
                                             else
                                             {
                                                 productActivityApp.DocumentPermissionData.IsCreateDocument = false;
                                                 productActivityApp.DocumentPermissionData.IsDocumentAccess = false;
                                                 productActivityApp.DocumentPermissionData.IsRead = false;
                                                 productActivityApp.DocumentPermissionData.IsDelete = false;
                                                 productActivityApp.DocumentPermissionData.IsUpdateDocument = false;
                                             }
                                         }
                                         else
                                         {
                                             productActivityApp.DocumentPermissionData.IsCreateDocument = false;
                                             productActivityApp.DocumentPermissionData.IsDocumentAccess = false;
                                             productActivityApp.DocumentPermissionData.IsRead = true;
                                             productActivityApp.DocumentPermissionData.IsDelete = true;
                                             productActivityApp.DocumentPermissionData.IsUpdateDocument = true;
                                         }*/
                                    }
                                    productActivityAppModels.Add(productActivityApp);
                                }
                            }
                        });
                    }
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
                    var result = await connection.QueryMultipleAsync(query,parameters);
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
    }
}
