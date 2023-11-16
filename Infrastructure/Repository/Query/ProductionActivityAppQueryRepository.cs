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
        public ProductionActivityAppQueryRepository(IConfiguration configuration)
            : base(configuration)
        {

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
        public async Task<IReadOnlyList<DocumentsModel>> GetSupportingDocumentsAsync(long? ProductionActivityAppLineId)
        {
            List<DocumentsModel> supportingDocuments = new List<DocumentsModel>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProductionActivityAppLineId", ProductionActivityAppLineId);
                var appUsers = await GetApplicationUserAsync();
                var query = string.Empty;

                query = @"select t1.* from Documents t1 where t1.IsLatest=1 AND (t1.IsDelete=0 or IsDelete is null) AND SessionID in(select  SessionID from ProductionActivityAppLineDoc where ProductionActivityAppLineID=@ProductionActivityAppLineId)";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DocumentsModel>(query, parameters)).ToList();
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
                    return supportingDocuments;
                }
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
                using (var connection = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyID", CompanyId);
                    var results = await connection.QueryMultipleAsync("select ProdOrderNo,BatchNo from ProductionSimulation where companyid=@CompanyID;" +
                        "select * from NavprodOrderLine where companyid=@CompanyID AND ProdOrderNo!='' AND  RePlanRefNo is not null;" +
                        "select No,BatchNos,PackSize from Navitems where companyid=@CompanyID;", parameters);
                    var productionsimulationlist = results.Read<ProductionSimulation>().ToList();
                    var navprodOrderLineList = results.Read<NavprodOrderLine>().ToList();
                    var productActivityApps = navprodOrderLineList.Where(w => w.ProdOrderNo != null && w.ProdOrderNo != "" && w.CompanyId == CompanyId && w.RePlanRefNo != null).Select(s => new { s.RePlanRefNo, s.BatchNo, s.CompanyId }).Distinct().ToList();
                    var navItesmList = results.Read<Navitems>().ToList();
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
                        if (PPAlist.ProductionActivityAppLineId > 0)
                        {
                            var appquery = " UPDATE ProductionActivityAppLine SET ProductionActivityAppID = @Appid,ProdActivityResultID =@ProdActivityResultID,ManufacturingProcessChildID =@ManufacturingProcessChildID,ProdActivityCategoryChildID=@ProdActivityCategoryChildID," +
                            "ProdActivityActionChildD=@ProdActivityActionChildD,Comment=@PAApplineComment,NavprodOrderLineId=@AppLineNavprodOrderLineID,SessionId=@applineSessionId,StatusCodeID=@applineStatusCodeID,ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,ActivityStatusId=@ActivityStatusId,IsOthersOptions=@IsOthersOptions,LocationID=@LocationID,QaCheck=@QaCheck WHERE ProductionActivityAppLineId = @ProductionActivityAppLineId";

                            await connection.ExecuteAsync(appquery, parameters);
                        }
                        else
                        {
                            var applinequery = "INSERT INTO ProductionActivityAppLine(ProductionActivityAppID,ProdActivityResultID,ManufacturingProcessChildID,ProdActivityCategoryChildID,ProdActivityActionChildD,Comment,NavprodOrderLineId,AddedByUserID,AddedDate,SessionId,StatusCodeID,ModifiedByUserID,ModifiedDate,ActivityStatusId,IsOthersOptions,LocationID,QaCheck) " +
                                " OUTPUT INSERTED.ProductionActivityAppLineId " +
                                "VALUES (@Appid,@ProdActivityResultID,@ManufacturingProcessChildID,@ProdActivityCategoryChildID,@ProdActivityActionChildD,@PAApplineComment,@AppLineNavprodOrderLineID,@applineAddedByUserID,@applineAddedDate,@applineSessionId,@applineStatusCodeID,@applineAddedByUserID,@applineAddedDate,@ActivityStatusId,@IsOthersOptions,@LocationID,@QaCheck)";

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

    }
}





