using Core.Entities;
using Core.Repositories.Query;
using Infrastructure.Repository.Query.Base;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Views;
using Core.EntityModels;
using IdentityModel.Client;
using NAV;
using Application.Queries;
using Infrastructure.Data;
using Newtonsoft.Json.Linq;
using Google.Cloud.Firestore.V1;
using static iText.IO.Image.Jpeg2000ImageData;

namespace Infrastructure.Repository.Query
{
    public class IpirAppQueryRepostitory : DbConnector, IIpirAppQueryRepostitory
    {
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeriesSeviceQueryRepository;
        public IpirAppQueryRepostitory(IConfiguration configuration, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeriesSeviceQueryRepository)
            : base(configuration)
        {
            _generateDocumentNoSeriesSeviceQueryRepository = generateDocumentNoSeriesSeviceQueryRepository;
        }
        public async Task<IReadOnlyList<IpirApp>> GetAllByAsync()
        {
            List<IpirApp> IpirApps = new List<IpirApp>();
            try
            {
                var query = "select t1.*,t2.PlantCode as CompanyCode,t2.Description as CompanyName,t3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,t6.Name as LocationName,   \r\nt7.ItemNo,t7.Description,t7.Description1,t7.RePlanRefNo,t7.BatchNo,t8.Name as ProfileName,t10.UserName as ReportingPersonalName,t11.UserName as DetectedByName,   (SELECT COUNT(*) from Documents t9 Where t9.SessionID=t1.SessionID AND t9.IsLatest=1) as IsDocuments   from IpirApp t1   \r\nJOIN Plant t2 ON t1.CompanyID=t2.PlantID   JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID   \r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID   \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID   \r\nLEFT JOIN ICTMaster t6 ON t6.ICTMasterID=t1.LocationID   \r\nLEFT JOIN NAVProdOrderLine t7 ON t7.NAVProdOrderLineId=t1.LocationID   \r\nJOIN DocumentProfileNoSeries t8 ON t8.ProfileID=t1.ProfileID  \r\nLEFT JOIN ApplicationUser t10 ON t10.UserID=t1.ReportingPersonal \r\nLEFT JOIN ApplicationUser t11 ON t11.UserID=t1.DetectedBy ";

                var result = new List<IpirApp>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<IpirApp>(query)).ToList();
                }
                if (result != null && result.Count > 0)
                {

                    var IpirAppIds = result.ToList().Select(s => s.IpirAppId).ToList();
                    var sessionIds = result.ToList().Where(w => w.SessionID != null).Select(s => s.SessionID).ToList();
                    var resultData = await GetMultipleQueryAsync(sessionIds, IpirAppIds);
                    var documents = resultData.Documents.ToList();
                    var appUser = resultData.ApplicationUser.ToList();
                    var ipirAppIssueDeps = resultData.IpirAppIssueDep.ToList();


                    result.ForEach(s =>
                    {
                        s.ActivityIssueRelates = ipirAppIssueDeps != null && ipirAppIssueDeps.Count > 0 ? ipirAppIssueDeps.Where(a => a.IpirAppID == s.IpirAppId && a.Type == "Issue").ToList() : new List<IpirAppIssueDep>();
                        s.ActivityIssueRelateIds = ipirAppIssueDeps != null && ipirAppIssueDeps.Count > 0 ? ipirAppIssueDeps.Where(a => a.IpirAppID == s.IpirAppId && a.Type == "Issue").Select(z => z.ActivityInfoIssueId).ToList() : new List<long?>();
                        s.DepartmentIds = ipirAppIssueDeps != null && ipirAppIssueDeps.Count > 0 ? ipirAppIssueDeps.Where(a => a.IpirAppID == s.IpirAppId && a.Type == "Department").Select(z => z.DepartmentID).ToList() : new List<long?>();

                        if (documents != null && s.SessionID != null)
                        {
                            var counts = documents.FirstOrDefault(w => w.SessionId == s.SessionID);
                            if (counts != null)
                            {

                                s.DocumentId = counts.DocumentId;
                                s.FileProfileTypeId = counts.FilterProfileTypeId;
                                s.DocumentID = counts.DocumentId;
                                s.DocumentParentId = counts.DocumentParentId;
                                s.FileName = counts.FileName;
                                s.DocProfileNo = counts.ProfileNo;
                                s.FilePath = counts.FilePath;
                                s.UniqueSessionId = counts.UniqueSessionId;
                                s.IsNewPath = counts.IsNewPath == true ? true : false;
                                s.ContentType = counts.ContentType;
                                s.IsLocked = counts.IsLocked;
                                s.LockedByUserId = counts.LockedByUserId;
                                s.ModifiedDate = counts.UploadDate;
                                s.ModifiedByUser = appUser != null && appUser.Count() > 0 && counts.AddedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.AddedByUserId)?.UserName : "";
                                s.LockedByUser = appUser != null && appUser.Count() > 0 && counts.LockedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.LockedByUserId)?.UserName : "";

                            }
                        }
                        IpirApps.Add(s);
                    });
                }
                return IpirApps;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<IPIRReportingInformation>> GetAllIPIRmobileByAsync(long IpirappId)
        {
            List<IPIRReportingInformation> IpirApps = new List<IPIRReportingInformation>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IpirappId", IpirappId);
                var query = @"select *,t2.value as IssueRelatedName From IPIRReportingInformation t1 
                            LEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.IssueRelatedTo where T1.IpirAppID = @IpirappId";
                var result = new List<IPIRReportingInformation>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<IPIRReportingInformation>(query, parameters)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    var informationIDs = result.ToList().Select(s => s.ReportinginformationID).ToList();
                    var IpirAppIds = result.ToList().Select(s => s.IpirAppID).ToList();
                    var sessionIds = result.ToList().Where(w => w.SessionId != null).Select(s => s.SessionId).ToList();
                    var resultData = await GetMultipleQueryAsync(sessionIds, IpirAppIds);
                    var documents = resultData.Documents.ToList();
                    var appUser = resultData.ApplicationUser.ToList();

                    var Data = await GetIssueRelatedAssignToQueryAsync(informationIDs);
                    var issueReportingAssignTo = Data.IpirreportingAssignTo.ToList();
                    result.ForEach(s =>
                    {

                        s.AssignToIds = issueReportingAssignTo != null && issueReportingAssignTo.Count > 0 ? issueReportingAssignTo.Where(a => a.ReportinginformationID == s.ReportinginformationID).Select(z => z.AssignToId).ToList() : new List<long>();
                        if (documents != null && s.SessionId != null)
                        {
                            var counts = documents.FirstOrDefault(w => w.SessionId == s.SessionId);
                            if (counts != null)
                            {

                                s.DocumentId = counts.DocumentId;
                                s.FileProfileTypeId = counts.FilterProfileTypeId;
                                s.DocumentID = counts.DocumentId;
                                s.DocumentParentId = counts.DocumentParentId;
                                s.FileName = counts.FileName;
                                s.ProfileNo = counts.ProfileNo;
                                s.FilePath = counts.FilePath;
                                s.UniqueSessionId = counts.UniqueSessionId;
                                s.IsNewPath = counts.IsNewPath == true ? true : false;
                                s.ContentType = counts.ContentType;
                                s.IsLocked = counts.IsLocked;
                                s.LockedByUserId = counts.LockedByUserId;
                                s.ModifiedDate = counts.UploadDate;
                                s.ModifiedByUser = appUser != null && appUser.Count() > 0 && counts.AddedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.AddedByUserId)?.UserName : "";
                                s.LockedByUser = appUser != null && appUser.Count() > 0 && counts.LockedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.LockedByUserId)?.UserName : "";

                            }
                        }
                        IpirApps.Add(s);
                    });
                }
                return IpirApps;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IpirAppIssueDep> GetIpirAppIssueDepByDynamicForm(long? IpirAppIssueDepId)
        {
            IpirAppIssueDep MultipleIpirAppItemLists = new IpirAppIssueDep();
            try
            {
                var query = string.Empty;
                var parameters = new DynamicParameters();
                parameters.Add("IpirAppIssueDepId", IpirAppIssueDepId);
                query += "select t1.*,t2.Value as issueRelateName,t3.SessionID as DynamicFormDataSessionID,t4.SessionID as DynamicFormSessionID  from IpirAppIssueDep t1 \r\n" +
                    "LEFT JOIN ApplicationMasterDetail t2 ON t1.ActivityInfoIssueID=t2.ApplicationMasterDetailID\r\n" +
                    "LEFT JOIN DynamicFormData t3 ON t3.DynamicFormDataID=t1.DynamicFormDataID\r\n" +
                    "LEFT JOIN DynamicForm t4 ON t4.ID=t3.DynamicFormID where t1.IpirAppIssueDepId=@IpirAppIssueDepId;";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<IpirAppIssueDep>(query, parameters)).ToList();
                    if (result != null && result.Count > 0)
                    {
                        MultipleIpirAppItemLists = result.FirstOrDefault();
                    }
                }
                return MultipleIpirAppItemLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<MultipleIpirAppItemLists> GetMultipleQueryAsync(List<Guid?> SessionIds, List<long> IpirAppIds)
        {
            MultipleIpirAppItemLists MultipleIpirAppItemLists = new MultipleIpirAppItemLists();
            try
            {
                var query = string.Empty;
                SessionIds = SessionIds != null && SessionIds.Count > 0 ? SessionIds : new List<Guid?>() { Guid.NewGuid() };
                IpirAppIds = IpirAppIds != null && IpirAppIds.Count > 0 ? IpirAppIds : new List<long>() { -1 };
                query += DocumentQueryString() + " where  SessionId in(" + string.Join(",", SessionIds.Select(x => string.Format("'{0}'", x.ToString().Replace("'", "''")))) + ") AND IsLatest=1 AND (IsDelete is null or IsDelete=0);";
                query += "select UserName,UserId,SessionId from ApplicationUser;";
                query += "select t1.*,t2.Value as issueRelateName,t3.SessionID as DynamicFormDataSessionID,t4.SessionID as DynamicFormSessionID  from IpirAppIssueDep t1 \r\nLEFT JOIN ApplicationMasterDetail t2 ON t1.ActivityInfoIssueID=t2.ApplicationMasterDetailID\r\nLEFT JOIN DynamicFormData t3 ON t3.DynamicFormDataID=t1.DynamicFormDataID\r\nLEFT JOIN DynamicForm t4 ON t4.ID=t3.DynamicFormID where t1.IpirAppId in(" + string.Join(',', IpirAppIds) + ");";

                using (var connection = CreateConnection())
                {

                    var result = await connection.QueryMultipleAsync(query);
                    MultipleIpirAppItemLists.Documents = result.Read<Documents>().ToList();
                    MultipleIpirAppItemLists.ApplicationUser = result.Read<ApplicationUser>().ToList();
                    MultipleIpirAppItemLists.IpirAppIssueDep = result.Read<IpirAppIssueDep>().ToList();

                }
                return MultipleIpirAppItemLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<MultipleIpirAppItemLists> GetIssueRelatedAssignToQueryAsync(List<long> ReportingInformationID)
        {
            MultipleIpirAppItemLists MultipleIpirAppItemLists = new MultipleIpirAppItemLists();
            try
            {
                var query = string.Empty;

                ReportingInformationID = ReportingInformationID != null && ReportingInformationID.Count > 0 ? ReportingInformationID : new List<long>() { -1 };


                query += "select * from IssueReportAssignTo where ReportinginformationID in(" + string.Join(',', ReportingInformationID) + ");";
                using (var connection = CreateConnection())
                {

                    var result = await connection.QueryMultipleAsync(query);

                    MultipleIpirAppItemLists.IpirreportingAssignTo = result.Read<IssueReportAssignTo>().ToList();
                }
                return MultipleIpirAppItemLists;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IpirApp> GetAllByOneAsync(Guid? SessionId)
        {
            IpirApp IpirApps = new IpirApp();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select t1.*,t2.PlantCode as CompanyCode,t2.Description as CompanyName,t3.CodeValue as StatusCode,t4.UserName as AddedBy,t5.UserName as ModifiedBy,t6.Name as LocationName,   \r\nt7.ItemNo,t7.Description,t7.Description1,t7.RePlanRefNo,t7.BatchNo,t8.Name as ProfileName,t10.UserName as ReportingPersonalName,t11.UserName as DetectedByName,   (SELECT COUNT(*) from Documents t9 Where t9.SessionID=t1.SessionID AND t9.IsLatest=1 AND t9.IsTemp=0) as IsDocuments   from IpirApp t1   \r\nJOIN Plant t2 ON t1.CompanyID=t2.PlantID   JOIN CodeMaster t3 ON t3.CodeID=t1.StatusCodeID   \r\nJOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID   \r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID   \r\nLEFT JOIN ICTMaster t6 ON t6.ICTMasterID=t1.LocationID   \r\nLEFT JOIN NAVProdOrderLine t7 ON t7.NAVProdOrderLineId=t1.LocationID   \r\nJOIN DocumentProfileNoSeries t8 ON t8.ProfileID=t1.ProfileID  \r\nLEFT JOIN ApplicationUser t10 ON t10.UserID=t1.ReportingPersonal \r\nLEFT JOIN ApplicationUser t11 ON t11.UserID=t1.DetectedBy t1.SessionId=@SessionId";
                var result = new List<IpirApp>();
                using (var connection = CreateConnection())
                {
                    result = (await connection.QueryAsync<IpirApp>(query, parameters)).ToList();
                }
                if (result != null && result.Count > 0)
                {
                    var IpirAppIds = result.ToList().Select(s => s.IpirAppId).ToList();
                    var sessionIds = result.ToList().Where(w => w.SessionID != null).Select(s => s.SessionID).ToList();
                    var resultData = await GetMultipleQueryAsync(sessionIds, IpirAppIds);
                    var documents = resultData.Documents.ToList();
                    var appUser = resultData.ApplicationUser.ToList();
                    var ipirAppIssueDeps = resultData.IpirAppIssueDep.ToList();
                    result.ForEach(s =>
                    {
                        s.ActivityIssueRelates = ipirAppIssueDeps != null && ipirAppIssueDeps.Count > 0 ? ipirAppIssueDeps.Where(a => a.IpirAppID == s.IpirAppId && a.Type == "Issue").ToList() : new List<IpirAppIssueDep>();
                        s.ActivityIssueRelateIds = ipirAppIssueDeps != null && ipirAppIssueDeps.Count > 0 ? ipirAppIssueDeps.Where(a => a.IpirAppID == s.IpirAppId && a.Type == "Issue").Select(z => z.ActivityInfoIssueId).ToList() : new List<long?>();
                        s.DepartmentIds = ipirAppIssueDeps != null && ipirAppIssueDeps.Count > 0 ? ipirAppIssueDeps.Where(a => a.IpirAppID == s.IpirAppId && a.Type == "Department").Select(z => z.DepartmentID).ToList() : new List<long?>();
                        if (documents != null && s.SessionID != null)
                        {
                            var counts = documents.FirstOrDefault(w => w.SessionId == s.SessionID);
                            if (counts != null)
                            {
                                s.DocumentId = counts.DocumentId;
                                s.FileProfileTypeId = counts.FilterProfileTypeId;
                                s.DocumentID = counts.DocumentId;
                                s.DocumentParentId = counts.DocumentParentId;
                                s.FileName = counts.FileName;
                                s.ProfileNo = counts.ProfileNo;
                                s.FilePath = counts.FilePath;
                                s.UniqueSessionId = counts.UniqueSessionId;
                                s.IsNewPath = counts.IsNewPath == true ? true : false;
                                s.ContentType = counts.ContentType;
                                s.IsLocked = counts.IsLocked;
                                s.LockedByUserId = counts.LockedByUserId;
                                s.ModifiedDate = counts.UploadDate;
                                s.ModifiedByUser = appUser != null && appUser.Count() > 0 && counts.AddedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.AddedByUserId)?.UserName : "";
                                s.LockedByUser = appUser != null && appUser.Count() > 0 && counts.LockedByUserId != null ? appUser.FirstOrDefault(f => f.UserID == counts.LockedByUserId)?.UserName : "";
                            }
                        }
                        IpirApps = s;
                    });
                }
                return IpirApps;
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
        public async Task<IpirApp> DeleteIpirApp(IpirApp value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IpirAppId", value.IpirAppId);
                        var query = string.Empty;
                        query += "Delete from  IpirAppIssueDep WHERE IpirAppId=@IpirAppId;";
                        query += "Delete from  IpirAppSupportDoc WHERE IpirAppId=@IpirAppId;";
                        query += "Delete from  IpirAppCheckedDetails WHERE IpirAppId=@IpirAppId;";
                        query += "Delete from  IpirApp WHERE IpirAppId=@IpirAppId;";
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
        public async Task<IpirApp> InsertOrUpdateIpirApp(IpirApp value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IpirAppId", value.IpirAppId);
                        parameters.Add("CompanyID", value.CompanyID);
                        parameters.Add("ProfileId", value.ProfileId);
                        parameters.Add("LocationID", value.LocationID);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("NavprodOrderLineID", value.NavprodOrderLineID);
                        parameters.Add("ReportingPersonal", value.ReportingPersonal);
                        parameters.Add("DetectedBy", value.DetectedBy);
                        parameters.Add("SessionID", value.SessionID, DbType.Guid);
                        parameters.Add("RefNo", value.RefNo);
                        parameters.Add("ProdOrderNo", value.ProdOrderNo, DbType.String);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("FixedAssetNo", value.FixedAssetNo, DbType.String);
                        parameters.Add("Comment", value.Comment, DbType.String);
                        parameters.Add("MachineName", value.MachineName, DbType.String);
                        parameters.Add("ActivityStatusId", value.ActivityStatusId);
                        if (value.IpirAppId > 0)
                        {

                            var query = "Update IpirApp Set ActivityStatusId=@ActivityStatusId,MachineName=@MachineName,DetectedBy=@DetectedBy,SessionID=@SessionID,CompanyID=@CompanyID,ProfileId=@ProfileId,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID,StatusCodeID=@StatusCodeID,LocationID=@LocationID,NavprodOrderLineID=@NavprodOrderLineID,ReportingPersonal=@ReportingPersonal,RefNo=@RefNo," +
                            "ProdOrderNo=@ProdOrderNo,FixedAssetNo=@FixedAssetNo,Comment=@Comment  Where IpirAppId=@IpirAppId;";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        else
                        {
                            var ProfileNo = string.Empty;
                            if (value.ProfileId > 0)
                            {
                                ProfileNo = await _generateDocumentNoSeriesSeviceQueryRepository.GenerateDocumentProfileAutoNumber(new DocumentNoSeriesModel { ProfileID = value.ProfileId, AddedByUserID = value.AddedByUserID, StatusCodeID = 710, Title = "Ipir App" });
                                value.ProfileNo = ProfileNo;
                            }
                            parameters.Add("ProfileNo", ProfileNo, DbType.String);
                            value.SessionID = Guid.NewGuid();
                            parameters.Add("SessionID", value.SessionID, DbType.Guid);
                            var query = @"INSERT INTO IpirApp(ActivityStatusId,MachineName,DetectedBy,SessionID,CompanyID,ProfileId,AddedByUserID,AddedDate,StatusCodeID,ModifiedByUserID,ModifiedDate,LocationID,NavprodOrderLineID,ReportingPersonal,RefNo,ProdOrderNo,FixedAssetNo,Comment,ProfileNo) 
				                       OUTPUT INSERTED.IpirAppId ,INSERTED.SessionID
				                       VALUES (@ActivityStatusId,@MachineName,@DetectedBy,@SessionID,@CompanyID,@ProfileId,@AddedByUserID,@AddedDate,@StatusCodeID,@ModifiedByUserID,@ModifiedDate,@LocationID,@NavprodOrderLineID,@ReportingPersonal,@RefNo,@ProdOrderNo,@FixedAssetNo,@Comment,@ProfileNo)";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                            value.IpirAppId = insertedId;
                        }
                        if (value.IpirAppId > 0)
                        {
                            var Deletequery = "DELETE  FROM IpirAppIssueDep WHERE IpirAppId = " + value.IpirAppId + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        var querys = string.Empty;
                        if (value.ActivityIssueRelateIds != null)
                        {
                            var listData = value.ActivityIssueRelateIds.ToList();
                            if (listData.Count > 0)
                            {
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [IpirAppIssueDep](ActivityInfoIssueID,IpirAppId,Type) VALUES ( " + s + "," + value.IpirAppId + ",'Issue');\r\n";
                                });

                            }
                        }
                        if (value.DepartmentIds != null)
                        {
                            var listData = value.DepartmentIds.ToList();
                            if (listData.Count > 0)
                            {
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO [IpirAppIssueDep](DepartmentID,IpirAppId,Type) VALUES ( " + s + "," + value.IpirAppId + ",'Department');\r\n";
                                });

                            }
                        }
                        if (!string.IsNullOrEmpty(querys))
                        {
                            await connection.ExecuteAsync(querys, null);
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
        public async Task<IReadOnlyList<IpirAppCheckedDetailsModel>> GetIpirAppDetails(long? value)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("IpirAppId", value);
                var query = "select t1.*,t2.Value as ActivityResultName,t4.userName as checkedByUserName,t3.Value as ActivityStatusName from IpirAppCheckedDetails t1" +
                    " LEFT JOIN ApplicationMasterDetail t2 ON t2.ApplicationMasterDetailID=t1.ActivityResultID LEFT JOIN ApplicationUser t4 ON t4.userId=t1.checkedByID " +
                    " LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t1.ActivityStatusID  where t1.IpirAppId =@IpirAppId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<IpirAppCheckedDetailsModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IpirAppCheckedDetailsModel> DeleteIpirAppCheckedDetails(IpirAppCheckedDetailsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IpirAppCheckedDetailsId", value.IpirAppCheckedDetailsId);
                        var query = string.Empty;
                        query += "Delete from  IpirAppCheckedDetails WHERE IpirAppCheckedDetailsId=@IpirAppCheckedDetailsId;";
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
        public async Task<IpirAppCheckedDetailsModel> InsertIpirAppCheckedDetails(IpirAppCheckedDetailsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IpirAppId", value.IpirAppId);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("ActivityInfoId", value.ActivityInfoId);
                        parameters.Add("ActivityStatusId", value.ActivityStatusId);
                        parameters.Add("ActivityResultId", value.ActivityResultId);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("IpirAppCheckedDetailsId", value.IpirAppCheckedDetailsId);
                        parameters.Add("IsCheckNoIssue", value.IsCheckNoIssue);
                        parameters.Add("CheckedById", value.CheckedById);
                        parameters.Add("StatusCodeID", value.StatusCodeID);
                        parameters.Add("CommentImageType", value.CommentImageType);
                        parameters.Add("IsCheckReferSupportDocument", value.IsCheckReferSupportDocument);
                        parameters.Add("CheckedComment", value.CheckedComment, DbType.String);
                        parameters.Add("CheckedDate", value.CheckedDate, DbType.DateTime);
                        string hex = string.Empty;
                        if (value.CommentImage != null && !string.IsNullOrEmpty(value.CommentImages))
                        {
                            var image = Convert.FromBase64String(value.CommentImages);
                            hex = BitConverter.ToString(image);
                            hex = hex.Replace("-", "");
                            hex = "0x" + hex;
                        }

                        if (value.IpirAppCheckedDetailsId > 0)
                        {

                            var query = "Update IpirAppCheckedDetails Set ActivityStatusId=@ActivityStatusId,ActivityResultId=@ActivityResultId,SessionId=@SessionId,IpirAppId=@IpirAppId,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID,IsCheckNoIssue=@IsCheckNoIssue,CheckedById=@CheckedById,CheckedComment=@CheckedComment,CommentImageType=@CommentImageType,IsCheckReferSupportDocument=@IsCheckReferSupportDocument,CheckedDate=@CheckedDate\n\r";
                            if (!string.IsNullOrEmpty(hex))
                            {
                                query += ",CommentImage=(CONVERT(VARBINARY(MAX), '" + hex + "',1))\n\r";
                            }
                            query += "Where IpirAppCheckedDetailsId=@IpirAppCheckedDetailsId;";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        else
                        {

                            var query = "INSERT INTO IpirAppCheckedDetails(CheckedDate,ActivityStatusId,ActivityResultId,SessionId,IpirAppId,ActivityInfoId,AddedByUserID,AddedDate,StatusCodeID,ModifiedByUserID,ModifiedDate,IsCheckNoIssue,CheckedById,CheckedComment,CommentImageType,IsCheckReferSupportDocument\n\r";
                            if (!string.IsNullOrEmpty(hex))
                            {
                                query += ",CommentImage \n\r";
                            }
                            query += ")\n\r";
                            query += "OUTPUT INSERTED.IpirAppCheckedDetailsId  VALUES (@CheckedDate,@ActivityStatusId,@ActivityResultId,@SessionId,@IpirAppId,@ActivityInfoId,@AddedByUserID,@AddedDate,@StatusCodeID,@ModifiedByUserID,@ModifiedDate,@IsCheckNoIssue,@CheckedById,@CheckedComment,@CommentImageType,@IsCheckReferSupportDocument\n\r";
                            if (!string.IsNullOrEmpty(hex))
                            {
                                query += ",(CONVERT(VARBINARY(MAX), '" + hex + "',1))\n\r";
                            }
                            query += ");";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                            var querys = string.Empty;
                            //querys += "Update ProductionActivityAppLine Set ActivityStatusId=@ActivityStatusId,ProdActivityResultId=@ActivityResultId  Where ProductionActivityAppLineId=@ProductionActivityAppLineId;";
                            // await connection.QuerySingleOrDefaultAsync<long>(querys, parameters);
                            value.IpirAppCheckedDetailsId = insertedId;
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

        public async Task<IPIRReportingInformation> InsertOrUpdateIpirReportingInformation(IPIRReportingInformation value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IpirAppID", value.IpirAppID);
                        parameters.Add("ReportinginformationID", value.ReportinginformationID);
                        parameters.Add("AssignToIds", value.AssignToIds);
                        parameters.Add("IssueRelatedTo", value.IssueRelatedTo);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedByUserID", value.ModifiedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        parameters.Add("AddedDate", value.AddedDate, DbType.DateTime);
                        parameters.Add("IssueDescription", value.IssueDescription);
                        parameters.Add("ReportBy", value.ReportBy);
                        parameters.Add("SessionId", value.SessionId);
                        if (value.ReportinginformationID > 0)
                        {

                            var query = @"Update IPIRReportingInformation Set IpirAppID=@IpirAppID,IssueRelatedTo=@IssueRelatedTo,SessionID=@SessionId,ModifiedDate=@ModifiedDate,ModifiedByUserID=@ModifiedByUserID,IssueDescription=@IssueDescription,ReportBy=@ReportBy 
                           Where ReportinginformationID=@ReportinginformationID;";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        else
                        {

                            value.SessionId = Guid.NewGuid();
                            parameters.Add("SessionId", value.SessionId);
                            var query = @"INSERT INTO IPIRReportingInformation(IpirAppID,IssueRelatedTo,AddedByUserID,SessionID,AddedDate,IssueDescription,ReportBy)
                                        OUTPUT INSERTED.ReportinginformationID ,INSERTED.SessionID
				                       VALUES (@IpirAppID,@IssueRelatedTo,@AddedByUserID,@SessionId,@AddedDate,@IssueDescription,@ReportBy)";
                            var insertedId = await connection.ExecuteScalarAsync<long>(query, parameters);
                            value.ReportinginformationID = insertedId;
                        }
                        if (value.ReportinginformationID > 0)
                        {
                            var Deletequery = "Delete From IssueReportAssignTo where ReportinginformationID =  " + value.ReportinginformationID + ";";
                            await connection.ExecuteAsync(Deletequery);
                        }
                        var querys = string.Empty;
                        if (value.AssignToIds != null)
                        {
                            var listData = value.AssignToIds.ToList();
                            if (listData.Count > 0)
                            {
                                listData.ForEach(s =>
                                {
                                    querys += "INSERT INTO IssueReportAssignTo(IPIRId,AssignToId,ReportinginformationID) VALUES ( " + value.IpirAppID + "," + s + "," + value.ReportinginformationID + " );";
                                });

                            }
                        }

                        if (!string.IsNullOrEmpty(querys))
                        {
                            await connection.ExecuteAsync(querys, null);
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

        public async Task<IPIRReportingInformation> DeleteIpirReportingInformation(IPIRReportingInformation value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ReportinginformationID", value.ReportinginformationID);
                        var query = string.Empty;

                        query += "Delete from  IPIRReportingInformation WHERE ReportinginformationID=@ReportinginformationID;";
                        query += "Delete from  IssueReportAssignTo WHERE ReportinginformationID=@ReportinginformationID;";
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
        public async Task<IpirAppIssueDep> UpdateDynamicFormDataIssueDetails(Guid? SessionId, long? ActivityInfoIssueId, long? dynamicFormDataId)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("SessionId", SessionId, DbType.Guid);
                        parameters.Add("ActivityInfoIssueId", ActivityInfoIssueId);
                        var query = string.Empty;
                        query += "select t1.* from IpirAppIssueDep t1\r\nJOIN IpirApp t2 ON t2.IpirAppID=t1.IpirAppID\r\n" +
                            "where t1.ActivityInfoIssueID=@ActivityInfoIssueId AND t2.SessionID=@SessionId";
                        var result = await connection.QuerySingleOrDefaultAsync<IpirAppIssueDep>(query, parameters);
                        if (result != null && result.IpirAppIssueDepId > 0)
                        {
                            if (result.DynamicFormDataId > 0)
                            {

                            }
                            else
                            {
                                result.DynamicFormDataId = dynamicFormDataId;
                                parameters.Add("IpirAppIssueDepId", result.IpirAppIssueDepId);
                                parameters.Add("DynamicFormDataId", dynamicFormDataId);
                                var querys = "Update IpirAppIssueDep SET DynamicFormDataId=@DynamicFormDataId WHERE IpirAppIssueDepId = @IpirAppIssueDepId";
                                var rowsAffected = await connection.ExecuteAsync(querys, parameters);
                            }
                        }
                        return result ?? new IpirAppIssueDep();
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
