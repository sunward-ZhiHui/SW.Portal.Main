using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NAV;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
                var query = "select  EmployeeID, UserID,FirstName,LastName,NickName,DepartmentName,PlantID,DepartmentID from view_GetEmployee where  (Status!='Resign' or Status is null)";

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
        public async Task<IReadOnlyList<ProductionActivityAppLineQaCheckerModel>> GetProductionActivityAppLineQaCheckerAsync(List<long?> addedIds)
        {
            try
            {
                addedIds = addedIds != null && addedIds.Count > 0 ? addedIds : new List<long?>() { -1 };
                var query = "select * from ProductionActivityAppLineQaChecker where ProductionActivityAppLineId in(" + string.Join(',', addedIds) + ");";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductionActivityAppLineQaCheckerModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<Documents>> GetAllDocumentsAsync(List<Guid?> SessionIds)
        {
            try
            {
                SessionIds = SessionIds != null && SessionIds.Count > 0 ? SessionIds : new List<Guid?>() { Guid.NewGuid() };
                var query = DocumentQueryString() + " where  SessionId in(" + string.Join(",", SessionIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ") AND IsLatest=1 AND (IsDelete is null or IsDelete=0);";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ApplicationUser>> GetApplicationUserAsync()
        {
            try
            {
                var query = "select UserName,UserId,SessionId from ApplicationUser";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ApplicationUser>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<MultipleProductioAppLineItemLists> GetMultipleQueryAsync()
        {
            MultipleProductioAppLineItemLists multipleProductioAppLineItemLists = new MultipleProductioAppLineItemLists();
            try
            {
                var query = "select ProductActivityCaseResponsId,ProductActivityCaseId from ProductActivityCaseRespons;";
                query += "select ProductActivityCaseResponsDutyId, ProductActivityCaseResponsId from ProductActivityCaseResponsDuty;";
                query += "select EmployeeId, ProductActivityCaseResponsDutyId from ProductActivityCaseResponsResponsible;";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    multipleProductioAppLineItemLists.ProductActivityCaseRespons = result.Read<ProductActivityCaseRespons>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCaseResponsDuty = result.Read<ProductActivityCaseResponsDuty>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityCaseResponsResponsible = result.Read<ProductActivityCaseResponsResponsible>().ToList();
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
        public async Task<IReadOnlyList<ProductActivityAppModel>> GetAllAsync(long? CompanyID, string? prodorderNo, long? userId, long? locationID)
        {
            List<ProductActivityAppModel> productActivityAppModels = new List<ProductActivityAppModel>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyID);
                parameters.Add("ProdOrderNo", prodorderNo);
                parameters.Add("LocationID", locationID);
                var query = @"select 
                    t1.ProductionActivityAppLineID,t1.ProductionActivityAppID,t1.ActionDropdown,t1.ProdActivityActionID,t1.ProdActivityCategoryID,t1.ManufacturingProcessID,t1.IsTemplateUpload,t1.StatusCodeID,t1.AddedByUserID,t1.AddedDate,t1.ModifiedByUserID,t1.ModifiedDate,t1.SessionID as LineSessionId,t1.ProductActivityCaseLineID,t1.NavprodOrderLineID,t1.Comment as LineComment,t1.QaCheck,t1.IsOthersOptions,t1.ProdActivityResultID,t1.ManufacturingProcessChildID,t1.ProdActivityCategoryChildID,t1.ProdActivityActionChildD,t1.TopicID,t1.QaCheckUserID,t1.QaCheckDate,t1.LocationID,t1.ProductActivityCaseID,t1.ActivityMasterID,t1.ActivityStatusID,t3.UserName as AddedByUser,t4.UserName as ModifiedByUser,t5.CodeValue as StatusCode,t6.Value as ProdActivityResult,t7.Value as ManufacturingProcessChild,t8.Value as ProdActivityCategoryChild,t9.Value AS ProdActivityActionChild,t2.CompanyID,t10.PlantCode as CompanyName,t2.ProdOrderNo,t2.Comment,t2.SessionId,t2.LocationID,
                    t11.ItemNo,t11.Description,t11.Description1,t11.BatchNo,t11.RePlanRefNo,CONCAT(t11.ProdOrderNo,'|',t11.Description,(case when ISNULL(NULLIF(t11.Description1, ''), null) is NULL then  t11.Description1 ELSE  CONCAT(' | ',Description1) END)) as ProdOrderNoDesc,'Production Activity' as Type,
                    (case when t1.IsTemplateUpload=1 then 'Yes' ELSE 'No' END) as IsTemplateUploadFlag,
                    (select COUNT(tt1.ProductionActivityAppLineDocId) from ProductionActivityAppLineDoc tt1 WHERE tt1.Type = 'Production Activity' AND tt1.ProductionActivityAppLineID=t1.ProductionActivityAppLineID) as SupportDocCount,t12.NameOfTemplate,t12.Link,t12.LocationToSaveId
                    from ProductionActivityAppLine t1 
                    JOIN ProductionActivityApp t2 ON t1.ProductionActivityAppId=t2.ProductionActivityAppId
                    JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID
                    LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedByUserID
                    LEFT JOIN CodeMaster t5 ON t5.CodeID=t1.StatusCodeID
                    LEFT JOIN ApplicationMasterDetail t6 ON t6.ApplicationMasterDetailID=t1.ProdActivityResultID
                    LEFT JOIN ApplicationMasterChild as t7 ON t7.ApplicationMasterChildID = t1.ManufacturingProcessChildID 
                    LEFT JOIN ApplicationMasterChild as t8 ON t8.ApplicationMasterChildID = t1.ProdActivityCategoryChildID 
                    LEFT JOIN ApplicationMasterChild as t9 ON t9.ApplicationMasterChildID = t1.ProdActivityActionChildD
                    JOIN Plant as t10 ON t10.PlantID = t2.CompanyID 
                    JOIN NavprodOrderLine as t11 ON t11.NavprodOrderLineId = t1.NavprodOrderLineId 
                    LEFT JOIN ProductActivityCaseLine as t12 ON t12.ProductActivityCaseLineId = t1.ProductActivityCaseLineId
                    WHERE t2.CompanyId=@CompanyID 
                    AND t1.NavprodOrderLineId is Not Null 
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
                        var productionActivityAppLineIds = productActivityApps.ToList().Select(s => s.ProductionActivityAppLineId).Distinct().ToList();
                        var addedIds = productActivityApps.ToList().Select(s => s.AddedByUserID).Distinct().ToList();
                        addedIds.Add(userId);
                        var employee = employeeAll!=null && employeeAll.Count()>0?employeeAll.Where(w => addedIds.Contains(w.UserID)).ToList():null;
                        var loginUser = employee != null && employee.Count() > 0 ? employee.FirstOrDefault(w => w.UserID == userId)?.DepartmentID : null;
                        var sessionIds = productActivityApps.ToList().Where(w => w.LineSessionId != null).Select(s => s.LineSessionId).ToList();
                        var documents = await GetAllDocumentsAsync(sessionIds);
                        var docIds = documents.Select(s => s.DocumentId).ToList();
                        var userIds = documents != null && documents.Count > 0 ? documents.Where(x => x.LockedByUserId > 0).Select(s => s.LockedByUserId).Distinct().ToList() : new List<long?>();
                        var filterProfileTypeIds = documents != null && documents.Count > 0 ? documents.Where(x => x.FilterProfileTypeId > 0).Select(s => s.FilterProfileTypeId).Distinct().ToList() : new List<long?>();
                        var appUser = await GetApplicationUserAsync();
                        var templateTestCaseCheckList = await GetMultipleQueryAsync();
                        var productionActivityAppLineQaChecker = await GetProductionActivityAppLineQaCheckerAsync(productionActivityAppLineIds);
                        var templateTestCaseCheckListResponse = templateTestCaseCheckList.ProductActivityCaseRespons.ToList();
                        var templateTestCaseCheckListResponseDuty = templateTestCaseCheckList.ProductActivityCaseResponsDuty.ToList();
                        var templateTestCaseCheckListResponseResponsible = templateTestCaseCheckList.ProductActivityCaseResponsResponsible.ToList();
                        var templateTestCaseCheckListIds = productActivityApps.Where(w => w.ProductActivityCaseId != null).Select(s => s.ProductActivityCaseId).ToList();
                        var docRoles = await GetDocumentUserRoleAsync(filterProfileTypeIds);
                        var docPermission = await GetDocumentPermissionByRoll();
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
                                productActivityApp.AddedByUser = s.AddedByUser;
                                productActivityApp.ModifiedByUser = s.ModifiedByUser;
                                productActivityApp.StatusCode = s.StatusCode;
                                productActivityApp.ProductActivityCaseLineId = s.ProductActivityCaseLineId;
                                productActivityApp.NameOfTemplate = s.NameOfTemplate;
                                productActivityApp.Link = s.Link;
                                productActivityApp.LocationToSaveId = s.LocationToSaveId;
                                productActivityApp.QaCheck = s.QaCheck == true ? true : false;
                                productActivityApp.ItemNo = s.ItemNo;
                                productActivityApp.Description = s.Description;
                                productActivityApp.Description1 = s.Description1;
                                productActivityApp.BatchNo = s.BatchNo;
                                productActivityApp.RePlanRefNo = s.RePlanRefNo;
                                productActivityApp.ProdOrderNoDesc = s.ProdOrderNoDesc;
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
                                if (documents != null && s.LineSessionId != null)
                                {
                                    var counts = documents.FirstOrDefault(w => w.SessionId == s.LineSessionId);
                                    if (counts != null)
                                    {
                                        productActivityApp.DocumentId = counts.DocumentId;
                                        productActivityApp.DocumentID = counts.DocumentId;
                                        productActivityApp.DocumentParentId = counts.DocumentParentId;
                                        productActivityApp.FileName = counts.FileName;
                                        productActivityApp.FilePath = counts.FilePath;
                                        productActivityApp.ContentType = counts.ContentType;
                                        productActivityApp.IsLocked = counts.IsLocked;
                                        productActivityApp.LockedByUserId = counts.LockedByUserId;
                                        productActivityApp.ModifiedDate = counts.UploadDate;
                                        productActivityApp.ModifiedByUser = appUser != null && appUser.Count() > 0 && counts.AddedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.AddedByUserId)?.UserName : "";
                                        productActivityApp.LockedByUser = appUser != null && appUser.Count() > 0 && counts.LockedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.LockedByUserId)?.UserName : "";
                                        productActivityApp.LocationToSaveId = counts.FilterProfileTypeId;
                                        long? roleId = 0;
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
                                        }
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

        public async Task<IReadOnlyList<ProductionActivityApp>> GetAllprodAsync(string? NAVProdOrder)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProdOrderNo", NAVProdOrder);
                var query = @"select * from ProductionActivityApp  where ProdOrderNo =@ProdOrderNo";

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

        public async Task<long> Insert(ProductionActivityAppLine PPALinelist)
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
                            parameters.Add("Comment", PPALinelist.Comment);
                            parameters.Add("ActionDropdown", PPALinelist.ActionDropdown);
                            parameters.Add("ManufacturingProcessID", PPALinelist.ManufacturingProcessID);
                            parameters.Add("ProdActivityCategoryID", PPALinelist.ProdActivityCategoryID);
                            parameters.Add("NavprodOrderLineID", PPALinelist.NavprodOrderLineID);
                            // parameters.Add("Description", PPAlist.Description);
                            // parameters.Add("TopicID", PPALinelist.TopicID);
                            //parameters.Add("ICTMasterID", PPAlist.ICTMasterID);
                            //parameters.Add("SiteName", PPAlist.SiteName);
                            //parameters.Add("ZoneName", PPAlist.ZoneName);
                            //parameters.Add("Companyid", PPALinelist.com);
                            // parameters.Add("DeropdownName", PPAlist.DeropdownName);
                            //parameters.Add("NavprodOrderLineId", PPAlist.NavprodOrderLineId);
                            //parameters.Add("BatchNo", PPAlist.BatchNo);
                            //parameters.Add("prodOrderNo", PPALinelist.prod);

                            parameters.Add("SessionId", PPALinelist.SessionId);
                            parameters.Add("AddedByUserID", PPALinelist.AddedByUserID);
                            parameters.Add("AddedDate", PPALinelist.AddedDate);
                            parameters.Add("LocationID", PPALinelist.LocationID);
                            parameters.Add("StatusCodeID", PPALinelist.StatusCodeID);

                            var query = "INSERT INTO ProductionActivityAppLine(Comment,NavprodOrderLineID,ProdActivityCategoryID,ManufacturingProcessID,ActionDropdown,SessionId,AddedByUserID,AddedDate,StatusCodeID,LocationID) VALUES (@ProdActivityCategoryID,@ManufacturingProcessID,@Comment,@ActionDropdown,@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@LocationID,@NavprodOrderLineID)";

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
    }
}
