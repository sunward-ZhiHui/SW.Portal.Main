using Azure.Core;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repository.Query
{
    public class ProductionActivityAppQueryRepository : QueryRepository<ProductionActivityApp>, IProductionActivityAppQueryRepository
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public ProductionActivityAppQueryRepository(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }

        public async Task<IReadOnlyList<ProductionActivityApp>> GetAllAsync(long? CompanyId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyId);
                var query = " select i.ICTMasterID,s.Name as SiteName , z.Name as ZoneName, i.Name,i.Companyid, " +
                    "CONCAT(i.Name ,+'|'+i.Description+'|', s.Name, Z.Name ) as DeropdownName,  " +
                    "i.Name,i.Description,i.siteid,i.locationid,i.zoneid,i.areaid from ICTMaster i " +
                    "left join ICTMaster z on z.ICTMasterID = i.ParentICTID left join ICTMaster s on s.ICTMasterID = z.ParentICTID where i.companyid=@CompanyID and i.MasterType = 572 ";

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
        public async Task<ProductionActivityApp> GetAllOneLocationAsync(string? locationName)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("locationName", locationName, DbType.String);
                var query = " select i.ICTMasterID,s.Name as SiteName , z.Name as ZoneName, i.Name,i.Companyid, " +
                    "CONCAT(i.Name ,+'|'+i.Description+'|', s.Name, Z.Name ) as DeropdownName,  " +
                    "i.Name,i.Description,i.siteid,i.locationid,i.zoneid,i.areaid from ICTMaster i left join ICTMaster z on z.ICTMasterID = i.ParentICTID " +
                    "left join ICTMaster s on s.ICTMasterID = z.ParentICTID where  i.MasterType = 572 AND i.name=@locationName;";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ProductionActivityApp>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavprodOrderLineModel>> GetAllNavprodOrderLineAsync(long? CompanyId, string? Replanrefno)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyId);
                parameters.Add("Replanrefno", Replanrefno, DbType.String);
                var query = "select t1.*,\r\nt3.BaseUnitofMeasure as Uom,\r\nt3.InternalRef,\r\nt3.PackQty,\r\nt3.PackQty as OutputQty,\r\n" +
                    "(case when ISNULL(NULLIF(t1.Description1, ''), null) is NULL then  t1.Description ELSE  CONCAT(t1.Description,' | ',Description1) END) as Name \r\n" +
                    "from NavprodOrderLine t1\r\nLEFT JOIN (SELECT t2.BaseUnitofMeasure,t2.InternalRef,t2.PackQty,t2.no FROM NAVItems t2 WHERE t2.CompanyId=@CompanyId) t3 ON t1.ItemNo=t3.no\r\n" +
                    "where replanrefno=@Replanrefno AND t1.CompanyID=@CompanyId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<NavprodOrderLineModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
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
        public async Task<IReadOnlyList<ApplicationUser>> GetApplicationUserAsync()
        {
            try
            {
                var query = "select UserName,UserId from ApplicationUser";

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
        private async Task<IReadOnlyList<Documents>> GetProductionActivityAppLineDoc(long? ProductionActivityAppLineId, string? Type)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                if (Type == "Production Activity")
                {
                    parameters.Add("ProductionActivityAppLineId", ProductionActivityAppLineId);
                    query = "select t1.SessionID,t1.DocumentID from documents t1 JOIN ProductionActivityAppLineDoc t2 ON t1.DocumentID=t2.DocumentID where t2.ProductionActivityAppLineID=@ProductionActivityAppLineId";

                }
                if (Type == "Production Routine")
                {
                    parameters.Add("ProductionActivityRoutineAppLineId", ProductionActivityAppLineId);
                    query = "select t1.SessionID,t1.DocumentID from documents t1 JOIN ProductionActivityRoutineAppLineDoc t2 ON t1.DocumentID=t2.DocumentID where t2.ProductionActivityRoutineAppLineId=@ProductionActivityRoutineAppLineId";

                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentsModel>> GetSupportingDocumentsAsync(long? ProductionActivityAppLineId, string? Type)
        {
            List<DocumentsModel> supportingDocuments = new List<DocumentsModel>();
            try
            {
                var appUsers = await GetApplicationUserAsync();
                var query = string.Empty;
                var docs = await GetProductionActivityAppLineDoc(ProductionActivityAppLineId, Type);
                if (docs != null && docs.Count > 0)
                {
                    var SessionIds = docs.Where(w => w.SessionId != null).Select(s => s.SessionId).ToList();
                    if (SessionIds.Count > 0)
                    {
                        query = @"" + DocumentQueryString() + " where IsLatest=1 AND (IsDelete=0 or IsDelete is null) AND SessionID in(" + string.Join(",", SessionIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ")";
                        using (var connection = CreateConnection())
                        {
                            var result = (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                            if (result.Count > 0)
                            {
                                result.ForEach(s =>
                                {
                                    var lastIndex = s.FileName != null ? s.FileName.LastIndexOf(".") : 0;
                                    lastIndex = lastIndex > 0 ? lastIndex : 0;
                                    var name = s.FileName != null ? s.FileName?.Substring(lastIndex) : "";
                                    var fileName = s.FileName?.Split(name);
                                    s.Extension = s.FileName != null ? s.FileName?.Split(".").Last() : "";
                                    s.AddedByUser = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                                    s.FileName = s.FileName != null ? (s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + name : s.FileName) : s.FileName;
                                    s.OriginalFileName = s.FileName;
                                    s.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                                    supportingDocuments.Add(s);
                                });
                            }
                        }
                    }
                }
                return supportingDocuments;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<ProductActivityCaseLineModel>> GetProductActivityCaseLineTemplateItemsAsync(long? ManufacturingProcessId, long? CategoryActionId, long? prodActivityActionChildD)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ManufacturingProcessId", ManufacturingProcessId);
                parameters.Add("CategoryActionId", CategoryActionId);
                parameters.Add("ActionId", prodActivityActionChildD);
                var query = string.Empty;
                if (prodActivityActionChildD > 0)
                {
                    query = @"declare @var1 bigint;
                            select  @var1 = ProductActivityCaseID from ProductActivityCaseActionMultiple where ActionId=@ActionId
                            select t1.*,t2.Name as TemplateProfileName,CASE WHEN t1.IsAutoNumbering = 1  THEN 'Yes' ELSE 'No' END AS AutoNumbering from ProductActivityCaseLine t1
                            LEFT JOIN DocumentProfileNoSeries t2 ON t2.ProfileId=t1.TemplateProfileId LEFT JOIN ProductActivityCase t3 ON t3.ProductActivityCaseID=t1.ProductActivityCaseID
                            WHERE t3.ManufacturingProcessChildID=@ManufacturingProcessId AND t1.ProductActivityCaseLineID IN(select ProductActivityCaseLineID from ProductActivityCaseLine where ProductActivityCaseId= @var1)";
                }
                else
                {
                    query = "declare @var1 bigint;\r\nselect  @var1 = ProductActivityCaseID from ProductActivityCaseCategoryMultiple where CategoryActionId=@CategoryActionId\r\n" +
                       "select t1.*,\r\nt2.Name as TemplateProfileName,\r\nCASE WHEN t1.IsAutoNumbering = 1  THEN 'Yes' ELSE 'No' END AS AutoNumbering\r\nfrom ProductActivityCaseLine t1\r\n" +
                       "LEFT JOIN DocumentProfileNoSeries t2 ON t2.ProfileId=t1.TemplateProfileId\r\nLEFT JOIN ProductActivityCase t3 ON t3.ProductActivityCaseID=t1.ProductActivityCaseID\r\n" +
                       "WHERE t3.ManufacturingProcessChildID=@ManufacturingProcessId AND t1.ProductActivityCaseLineID IN(select ProductActivityCaseLineID from ProductActivityCaseLine where ProductActivityCaseId= @var1)";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ProductActivityCaseLineModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<NavprodOrderLineModel>> GetAllAsyncPO(long? CompanyId)
        {
            List<NavprodOrderLineModel> productActivityAppModels = new List<NavprodOrderLineModel>();
            try
            {
                var productionsimulationlist = new List<ProductionSimulation>();
                var navprodOrderLineList = new List<NavprodOrderLine>();
                var navItesmList = new List<Navitems>();
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyID", CompanyId);
                    var results = await connection.QueryMultipleAsync("select ProdOrderNo,BatchNo from ProductionSimulation where companyid=@CompanyID;" +
                        "select * from NavprodOrderLine where companyid=@CompanyID AND ProdOrderNo!='' AND  RePlanRefNo is not null;" +
                        "select No,BatchNos,PackSize from Navitems where companyid=@CompanyID;", parameters);
                    productionsimulationlist = results.Read<ProductionSimulation>().ToList();
                    navprodOrderLineList = results.Read<NavprodOrderLine>().ToList();
                    navItesmList = results.Read<Navitems>().ToList();
                }
                if (navprodOrderLineList != null && navprodOrderLineList.Count > 0)
                {
                    var productActivityApps = navprodOrderLineList.Where(w => w.ProdOrderNo != null && w.ProdOrderNo != "" && w.CompanyId == CompanyId && w.RePlanRefNo != null).Select(s => new { s.RePlanRefNo, s.BatchNo, s.CompanyId }).Distinct().ToList();

                    if (productActivityApps != null && productActivityApps.Count > 0)
                    {
                        long i = 1;
                        var navprodOrderLine = navprodOrderLineList.Select(s => new { s.RePlanRefNo, s.CompanyId, s.ItemNo, s.Description }).ToList();
                        productActivityApps.ForEach(s =>
                        {
                            var navItems = navprodOrderLine.Where(f => f.RePlanRefNo == s.RePlanRefNo && f.CompanyId == CompanyId).Select(x => x.ItemNo).ToList();
                            var description = "";
                            var batchNo = "";
                            batchNo = productionsimulationlist.Where(p => p.ProdOrderNo == s.RePlanRefNo).Select(x => x.BatchNo).FirstOrDefault();
                            description = navprodOrderLine.FirstOrDefault(f => f.RePlanRefNo == s.RePlanRefNo && f.CompanyId == CompanyId)?.Description;
                            var batchNos = navItesmList.Where(w => navItems.Contains(w.No) && w.BatchNos != null).Select(b => b.BatchNos).Distinct().ToList();
                            var packSize = navItesmList.Where(w => navItems.Contains(w.No) && w.PackSize != null).Select(b => b.PackSize).Distinct().ToList();
                            NavprodOrderLineModel productActivityApp = new NavprodOrderLineModel();
                            productActivityApp.NavprodOrderLineId = i;
                            productActivityApp.ProdOrderNo = s.RePlanRefNo;
                            productActivityApp.Description = description;
                            productActivityApp.BatchNo = s.BatchNo;
                            productActivityApp.Name = s.RePlanRefNo + " || " + batchNo + (description == "" ? "" : ("||" + description));
                            productActivityApp.CompanyId = s.CompanyId;
                            productActivityApp.BatchNos = navItems != null ? string.Join("||", batchNos) : "";
                            productActivityApp.BatchSize = navItems != null ? string.Join("||", packSize) : "";
                            i++;
                            productActivityAppModels.Add(productActivityApp);
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
        public async Task<ProductionActivityApp> GetAllListAsync(long? CompanyId, string? Replanrefno, long? locationId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyId);
                parameters.Add("ProdOrderNo", Replanrefno);
                parameters.Add("locationId", locationId);
                var query = "SELECT * FROM ProductionActivityApp WHERE CompanyId=@CompanyId AND ProdOrderNo=@ProdOrderNo";
                if (locationId > 0)
                {
                    query += " AND locationId=@locationId";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ProductionActivityApp>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<ProductActivityCase> GetProductActivityCaseAsync(long? ManufacturingProcessChildId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ManufacturingProcessChildId", ManufacturingProcessChildId);
                var query = "SELECT ProfileId,ManufacturingProcessChildId,ProductActivityCaseId FROM ProductActivityCase WHERE ManufacturingProcessChildId=@ManufacturingProcessChildId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ProductActivityCase>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> Insert(ProductActivityAppModel PPAlist)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyID", PPAlist.CompanyId);
                        parameters.Add("ProdOrderNo", PPAlist.ProdOrderNo, DbType.String);
                        parameters.Add("SessionId", PPAlist.SessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", PPAlist.AddedByUserID);
                        parameters.Add("ModifiedByUserID", PPAlist.ModifiedByUserID);
                        parameters.Add("ModifiedDate", PPAlist.ModifiedDate);
                        parameters.Add("AddedDate", PPAlist.AddedDate, DbType.DateTime);
                        parameters.Add("LocationID", PPAlist.LocationId);
                        parameters.Add("StatusCodeID", PPAlist.StatusCodeID);
                        var lists = await GetAllListAsync(PPAlist.CompanyId, PPAlist.ProdOrderNo, PPAlist.LocationId);
                        if (lists != null)
                        {
                            PPAlist.ProductionActivityAppId = lists.ProductionActivityAppID;
                        }
                        else
                        {
                            var query = @"INSERT INTO ProductionActivityApp(SessionId,AddedByUserID,AddedDate,StatusCodeID,LocationID,CompanyID,ProdOrderNo,ModifiedByUserID,ModifiedDate) 
				                       OUTPUT INSERTED.ProductionActivityAppID 
				                       VALUES (@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@LocationID,@CompanyID,@ProdOrderNo,@AddedByUserID,@AddedDate)";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);

                            PPAlist.ProductionActivityAppId = insertedId;
                        }
                        parameters.Add("ProductionActivityAppLineId", PPAlist.ProductionActivityAppLineId);
                        parameters.Add("Appid", PPAlist.ProductionActivityAppId);
                        parameters.Add("IsOthersOptions", PPAlist.IsOthersOptions == true ? true : false);
                        parameters.Add("ProdActivityResultID", PPAlist.ProdActivityResultId);
                        parameters.Add("ActivityStatusId", PPAlist.ActivityStatusId);
                        parameters.Add("ManufacturingProcessChildID", PPAlist.ManufacturingProcessChildId);
                        parameters.Add("ProdActivityCategoryChildID", PPAlist.ProdActivityCategoryChildId);
                        parameters.Add("ProdActivityActionChildD", PPAlist.ProdActivityActionChildD);
                        parameters.Add("PAApplineComment", PPAlist.LineComment, DbType.String);
                        parameters.Add("AppLineNavprodOrderLineID", PPAlist.NavprodOrderLineId);
                        parameters.Add("applineAddedByUserID", PPAlist.AddedByUserID);
                        parameters.Add("applineAddedDate", PPAlist.AddedDate, DbType.DateTime);
                        parameters.Add("applineSessionId", PPAlist.LineSessionId, DbType.Guid);
                        parameters.Add("applineStatusCodeID", PPAlist.StatusCodeID);
                        parameters.Add("QaCheck", PPAlist.QaCheck == true ? true : false);
                        var ProfileNo = string.Empty;
                        long? ProfileId = null;
                        if (PPAlist.ProfileId > 0)
                        {
                           
                        }
                        else
                        {
                            if (PPAlist.ManufacturingProcessChildId > 0)
                            {
                                var profileData = await GetProductActivityCaseAsync(PPAlist.ManufacturingProcessChildId);
                                
                                if (profileData != null && profileData.ProfileId > 0)
                                {
                                    ProfileId = profileData.ProfileId;
                                     ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = profileData.ProfileId, AddedByUserID = PPAlist.AddedByUserID, StatusCodeID = 710, Title = "Production Activity" });
                                }
                               
                            }
                        }
                        parameters.Add("ProfileId", ProfileId);
                        parameters.Add("ProfileNo", ProfileNo, DbType.String);
                        if (PPAlist.ProductionActivityAppLineId > 0)
                        {
                            var appquery = " UPDATE ProductionActivityAppLine SET ProductionActivityAppID = @Appid,ProdActivityResultID =@ProdActivityResultID,ManufacturingProcessChildID =@ManufacturingProcessChildID,ProdActivityCategoryChildID=@ProdActivityCategoryChildID," +
                            "ProdActivityActionChildD=@ProdActivityActionChildD,Comment=@PAApplineComment,NavprodOrderLineId=@AppLineNavprodOrderLineID,SessionId=@applineSessionId,StatusCodeID=@applineStatusCodeID,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,ActivityStatusId=@ActivityStatusId,IsOthersOptions=@IsOthersOptions,LocationID=@LocationID,QaCheck=@QaCheck WHERE ProductionActivityAppLineId = @ProductionActivityAppLineId";

                            await connection.ExecuteAsync(appquery, parameters);
                        }
                        else
                        {

                            var applinequery = "INSERT INTO ProductionActivityAppLine(ProductionActivityAppID,ProdActivityResultID,ManufacturingProcessChildID,ProdActivityCategoryChildID,ProdActivityActionChildD,Comment,NavprodOrderLineId,AddedByUserID,AddedDate,SessionId,StatusCodeID,ModifiedByUserID,ModifiedDate,ActivityStatusId,IsOthersOptions,LocationID,QaCheck,ProfileNo,ProfileId) " +
                                " OUTPUT INSERTED.ProductionActivityAppLineId " +
                                "VALUES (@Appid,@ProdActivityResultID,@ManufacturingProcessChildID,@ProdActivityCategoryChildID,@ProdActivityActionChildD,@PAApplineComment,@AppLineNavprodOrderLineID,@applineAddedByUserID,@applineAddedDate,@applineSessionId,@applineStatusCodeID,@applineAddedByUserID,@applineAddedDate,@ActivityStatusId,@IsOthersOptions,@LocationID,@QaCheck,@ProfileNo,@ProfileId)";

                            PPAlist.ProductionActivityAppLineId = await connection.ExecuteScalarAsync<long>(applinequery, parameters);
                        }
                        if (PPAlist.ProductionActivityAppLineId > 0)
                        {
                            var Deletequery = "DELETE  FROM ActivityMasterMultiple WHERE ProductionActivityAppLineId = " + PPAlist.ProductionActivityAppLineId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        if (PPAlist.ActivityMasterIds != null)
                        {
                            var listData = PPAlist.ActivityMasterIds.ToList();
                            if (listData.Count > 0)
                            {
                                var querys = string.Empty;
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [ActivityMasterMultiple](AcitivityMasterId,ProductionActivityAppLineId) " +
                                                        "VALUES ( " + s + "," + PPAlist.ProductionActivityAppLineId + ");\r\n";
                                });
                                if (!string.IsNullOrEmpty(querys))
                                {
                                    await connection.ExecuteAsync(querys, null);
                                }
                            }
                        }
                        return PPAlist.ProductionActivityAppLineId.Value;
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
        public async Task<ActivityEmailTopicsModel> InserProductionActivityEmail(ActivityEmailTopicsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ActivityMasterId", value.ActivityMasterId);
                        parameters.Add("CategoryActionId", value.CategoryActionId);
                        parameters.Add("ActionId", value.ActionId);
                        parameters.Add("ActivityType", value.ActivityType, DbType.String);
                        parameters.Add("SubjectName", value.SubjectName);
                        parameters.Add("FromId", value.FromId);
                        parameters.Add("ToIds", value.ToId != null && value.ToId.Count() > 0 ? string.Join(',', value.ToId) : "");
                        parameters.Add("CcIds", value.CcId != null && value.CcId.Count() > 0 ? string.Join(',', value.CcId) : "");
                        parameters.Add("ManufacturingProcessId", value.ManufacturingProcessId);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("StatusCodeId", value.StatusCodeId);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("DocumentSessionId", value.DocumentSessionId, DbType.Guid);
                        parameters.Add("Comment", value.Comment, DbType.String);
                        parameters.Add("BackURL", value.BackURL, DbType.String);
                        parameters.Add("IsDraft", value.IsDraft);
                        parameters.Add("ModifiedByUserId", value.ModifiedByUserId);
                        parameters.Add("ModifiedDate", value.ModifiedDate, DbType.DateTime);
                        var query = @"INSERT INTO ActivityEmailTopics(ActivityMasterId,CategoryActionId,ActionId,ActivityType,SubjectName,FromId,ToIds,CcIds,ManufacturingProcessId,AddedByUserID,AddedDate,StatusCodeId,SessionId,DocumentSessionId,Comment,IsDraft,BackURL,ModifiedByUserId,ModifiedDate) 
				                       OUTPUT INSERTED.ActivityEmailTopicID 
				                       VALUES (@ActivityMasterId,@CategoryActionId,@ActionId,@ActivityType,@SubjectName,@FromId,@ToIds,@CcIds,@ManufacturingProcessId,@AddedByUserID,@AddedDate,@StatusCodeId,@SessionId,@DocumentSessionId,@Comment,@IsDraft,@BackURL,@ModifiedByUserId,@ModifiedDate)";
                        value.ActivityEmailTopicID = await connection.ExecuteScalarAsync<long>(query, parameters);


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
        public async Task<ProductionActivityRoutineAppModel> GetAllRoutineListAsync(long? CompanyId, string? Replanrefno, long? locationId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyId);
                parameters.Add("ProdOrderNo", Replanrefno);
                parameters.Add("locationId", locationId);
                var query = "SELECT * FROM ProductionActivityRoutineApp WHERE CompanyId=@CompanyId AND ProdOrderNo=@ProdOrderNo";
                if (locationId > 0)
                {
                    query += " AND locationId=@locationId";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<ProductionActivityRoutineAppModel>(query, parameters));
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<long> InsertProductionRoutine(ProductionActivityRoutineAppModel PPAlist)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("CompanyID", PPAlist.CompanyId);
                        parameters.Add("ProdOrderNo", PPAlist.ProdOrderNo, DbType.String);
                        parameters.Add("SessionId", PPAlist.SessionId, DbType.Guid);
                        parameters.Add("AddedByUserID", PPAlist.AddedByUserID);
                        parameters.Add("ModifiedByUserID", PPAlist.ModifiedByUserID);
                        parameters.Add("ModifiedDate", PPAlist.ModifiedDate);
                        parameters.Add("AddedDate", PPAlist.AddedDate, DbType.DateTime);
                        parameters.Add("LocationID", PPAlist.LocationId);
                        parameters.Add("StatusCodeID", PPAlist.StatusCodeID);
                        var lists = await GetAllRoutineListAsync(PPAlist.CompanyId, PPAlist.ProdOrderNo, PPAlist.LocationId);
                        if (lists != null)
                        {
                            PPAlist.ProductionActivityRoutineAppId = lists.ProductionActivityRoutineAppId;
                        }
                        else
                        {
                            var query = @"INSERT INTO ProductionActivityRoutineApp(SessionId,AddedByUserID,AddedDate,StatusCodeID,LocationID,CompanyID,ProdOrderNo,ModifiedByUserID,ModifiedDate) 
				                       OUTPUT INSERTED.ProductionActivityRoutineAppId 
				                       VALUES (@SessionId,@AddedByUserID,@AddedDate,@StatusCodeID,@LocationID,@CompanyID,@ProdOrderNo,@AddedByUserID,@AddedDate)";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);

                            PPAlist.ProductionActivityRoutineAppId = insertedId;
                        }
                        parameters.Add("ProductionActivityRoutineAppLineId", PPAlist.ProductionActivityRoutineAppLineId);
                        parameters.Add("Appid", PPAlist.ProductionActivityRoutineAppId);
                        parameters.Add("IsOthersOptions", PPAlist.IsOthersOptions == true ? true : false);
                        parameters.Add("ProdActivityResultID", PPAlist.ProdActivityResultId);
                        parameters.Add("RoutineStatusId", PPAlist.RoutineStatusId);
                        parameters.Add("ManufacturingProcessChildID", PPAlist.ManufacturingProcessChildId);
                        parameters.Add("ProdActivityCategoryChildID", PPAlist.ProdActivityCategoryChildId);
                        parameters.Add("ProdActivityActionChildD", PPAlist.ProdActivityActionChildD);
                        parameters.Add("PAApplineComment", PPAlist.LineComment, DbType.String);
                        parameters.Add("AppLineNavprodOrderLineID", PPAlist.NavprodOrderLineId);
                        parameters.Add("applineAddedByUserID", PPAlist.AddedByUserID);
                        parameters.Add("applineAddedDate", PPAlist.AddedDate, DbType.DateTime);
                        parameters.Add("applineSessionId", PPAlist.LineSessionId, DbType.Guid);
                        parameters.Add("applineStatusCodeID", PPAlist.StatusCodeID);
                        parameters.Add("QaCheck", PPAlist.QaCheck == true ? true : false);
                        parameters.Add("IsCheckReferSupportDocument", PPAlist.IsCheckReferSupportDocument == true ? true : false);
                        parameters.Add("IsCheckNoIssue", PPAlist.IsCheckNoIssue == true ? true : false);
                        var ProfileNo = string.Empty;
                        long? ProfileId = null;
                        if (PPAlist.ProfileId > 0)
                        {

                        }
                        else
                        {
                            if (PPAlist.ManufacturingProcessChildId > 0)
                            {
                                var profileData = await GetProductActivityCaseAsync(PPAlist.ManufacturingProcessChildId);
                                
                                if (profileData != null && profileData.ProfileId > 0)
                                {
                                    ProfileId=profileData.ProfileId;
                                    ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = profileData.ProfileId, AddedByUserID = PPAlist.AddedByUserID, StatusCodeID = 710, Title = "Production Activity" });
                                }
                                
                            }
                        }
                        parameters.Add("ProfileId", ProfileId);
                        parameters.Add("ProfileNo", ProfileNo, DbType.String);
                        if (PPAlist.ProductionActivityRoutineAppLineId > 0)
                        {
                            var appquery = " UPDATE ProductionActivityRoutineAppLine SET ProductionActivityRoutineAppId = @Appid,ProdActivityResultID =@ProdActivityResultID,ManufacturingProcessChildID =@ManufacturingProcessChildID,ProdActivityCategoryChildID=@ProdActivityCategoryChildID," +
                            "ProdActivityActionChildD=@ProdActivityActionChildD,Comment=@PAApplineComment,NavprodOrderLineId=@AppLineNavprodOrderLineID,SessionId=@applineSessionId,StatusCodeID=@applineStatusCodeID,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,RoutineStatusId=@RoutineStatusId,IsOthersOptions=@IsOthersOptions,LocationID=@LocationID,QaCheck=@QaCheck WHERE ProductionActivityRoutineAppLineId = @ProductionActivityRoutineAppLineId";

                            await connection.ExecuteAsync(appquery, parameters);
                        }
                        else
                        {

                            var applinequery = "INSERT INTO ProductionActivityRoutineAppLine(IsCheckNoIssue,IsCheckReferSupportDocument,ProductionActivityRoutineAppId,ProdActivityResultID,ManufacturingProcessChildID,ProdActivityCategoryChildID,ProdActivityActionChildD,Comment,NavprodOrderLineId,AddedByUserID,AddedDate,SessionId,StatusCodeID,ModifiedByUserID,ModifiedDate,RoutineStatusId,IsOthersOptions,LocationID,QaCheck,ProfileNo,ProfileId) " +
                                " OUTPUT INSERTED.ProductionActivityRoutineAppLineId " +
                                "VALUES (@IsCheckNoIssue,@IsCheckReferSupportDocument,@Appid,@ProdActivityResultID,@ManufacturingProcessChildID,@ProdActivityCategoryChildID,@ProdActivityActionChildD,@PAApplineComment,@AppLineNavprodOrderLineID,@applineAddedByUserID,@applineAddedDate,@applineSessionId,@applineStatusCodeID,@applineAddedByUserID,@applineAddedDate,@RoutineStatusId,@IsOthersOptions,@LocationID,@QaCheck,@ProfileNo,@ProfileId)";

                            PPAlist.ProductionActivityRoutineAppLineId = await connection.ExecuteScalarAsync<long>(applinequery, parameters);
                        }
                        if (PPAlist.ProductionActivityRoutineAppLineId > 0)
                        {
                            var Deletequery = "DELETE  FROM RoutineInfoMultiple WHERE ProductionActivityRoutineAppLineId = " + PPAlist.ProductionActivityRoutineAppLineId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        if (PPAlist.RoutineInfoIds != null)
                        {
                            var listData = PPAlist.RoutineInfoIds.ToList();
                            if (listData.Count > 0)
                            {
                                var querys = string.Empty;
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [RoutineInfoMultiple](RoutineInfoId,ProductionActivityRoutineAppLineId) " +
                                                        "VALUES ( " + s + "," + PPAlist.ProductionActivityRoutineAppLineId + ");\r\n";
                                });
                                if (!string.IsNullOrEmpty(querys))
                                {
                                    await connection.ExecuteAsync(querys, null);
                                }
                            }
                        }
                        return PPAlist.ProductionActivityRoutineAppLineId.Value;
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





