using Core.AttributeDynamicData;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Internal;
using IdentityModel.Client;
using Infrastructure.Repository.Query.Base;
using Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using NAV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.DB.DataStoreLongrunnersWatch;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Repository.Query
{
    public class FileprofiletypeQueryRepository : QueryRepository<Fileprofiletype>, IFileprofileQueryRepository
    {
        private readonly ILocalStorageService<ApplicationUser> _localStorageService;

        private IJSRuntime _jsRuntime;
        public FileprofiletypeQueryRepository(IConfiguration configuration, ILocalStorageService<ApplicationUser> localStorageService, IJSRuntime jsRuntime) : base(configuration)
        {
            _localStorageService = localStorageService;
            _jsRuntime = jsRuntime;

        }
        public async Task<IReadOnlyList<DocumentsModel>> GetAllFileProfileDocumentAsync()
        {
            try
            {
                /* var query = "select ROW_NUMBER() OVER(ORDER BY name) AS UniqueNo,*,Name as Filename,\r\n" +
                     "FileProfileTypeID as DocumentID,\r\n" +
                     "AddedBy as AddedByUser,\r\n" +
                     "ModifiedBy as ModifiedByUser,\r\n" +
                     "ModifiedDate,\r\n" +
                     "AddedDate,\r\n" +
                     "Profile as ProfileNo,\r\n" +
                     "CONCAT('>',Name) as BreadcumName,\r\n" +
                     //"CASE WHEN ModifiedByUserID >0 THEN ModifiedBy ELSE AddedBy END AS AddedByUser,\r\n" +
                     //"CASE WHEN ModifiedByUserID >0 THEN ModifiedDate ELSE AddedDate END AS AddedDate,\r\n" +
                     "CONCAT((select count(FileProfileTypeID) as counts from FileProfileType tt where tt.parentId=t2.FileProfileTypeID),' ','items') as FileSizes,\r\n" +
                     "CONCAT((Select COUNT(DocumentID) as DocCount from Documents where FilterProfileTypeId=t2.FileProfileTypeID\r\n" +
                     "AND IsLatest=1  \r\n" +
                     "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) \r\n" +
                     "OR (DocumentID in(select DocumentID from LinkFileProfileTypeDocument where FileProfileTypeID=t2.FileProfileTypeID ) AND IsLatest=1)),' ','files') as FileCounts\r\n" +
                     "from view_FileProfileTypeDocument t2";*/
                var query = "select \r\n Name,\r\nCASE WHEN  IsExpiryDate IS NULL THEN 0 ELSE  IsExpiryDate END AS IsExpiryDate,\r\nCASE WHEN  IsDuplicateUpload IS NULL THEN 0 ELSE  IsDuplicateUpload END AS IsDuplicateUpload,\r\n IsExpiryDate as IsExpiryDates,\r\n ProfileID,\r\n FileProfileTypeID,\r\n StatusCodeID,\r\n AddedByUserID,\r\n AddedDate,\r\n Description,\r\n ModifiedByUserID,\r\n ModifiedDate,\r\n ParentID,\r\n SessionID,\r\n IsDelete,\r\n DeleteByUserID,\r\n DeleteByDate,\r\n DynamicFormID,\r\nROW_NUMBER() OVER(ORDER BY name) AS UniqueNo,Name as Filename, \r\nFileProfileTypeID as DocumentID, \r\nModifiedDate, \r\nAddedDate, \r\nCONCAT('>',Name) as BreadcumName, \r\nCASE WHEN ModifiedByUserID >0 THEN ModifiedDate ELSE AddedDate END AS AddedDate\r\nfrom FileProfileType where (IsDelete = 0 or IsDelete is null)";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<FileProfileTypeModel> GetFileProfileTypeBySession(Guid? SessionId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SessionId", SessionId, DbType.Guid);
                var query = "select  t1.*,\r\n(select  COUNT(t2.FileProfileSessionID) from DynamicFormData t2 where t2.FileProfileSessionID=t1.SessionID) as IsdynamicFormExits\r\nfrom FileProfileType t1 where t1.sessionId=@SessionId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<FileProfileTypeModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentsModel>> GetAllFileProfileDocumentIdAsync(long? selectedFileProfileTypeID)
        {
            try
            {
                var query = "select  *,SessionId as FileProfileTypeSessionId,ROW_NUMBER() OVER(ORDER BY name) AS UniqueNo,Name as Filename,\r\nFileProfileTypeID as DocumentID,\r\nProfile as ProfileNo,\r\n--CASE WHEN ModifiedByUserID >0 THEN ModifiedBy ELSE AddedBy END AS AddedByUser,\r\n--CASE WHEN ModifiedByUserID >0 THEN ModifiedDate ELSE AddedDate END AS AddedDate,\r\nCONCAT((select count(*) as counts from FileProfileType tt where tt.parentId=t2.FileProfileTypeID AND (tt.isDelete is null or tt.isdelete=0)),' ','items') as FileSizes,\r\nCONCAT((Select COUNT(*) as DocCount from Documents where FilterProfileTypeId=t2.FileProfileTypeID AND IsLatest=1 AND  (isDelete is null or isDelete=0) AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) OR (DocumentID in(select DocumentID from LinkFileProfileTypeDocument where FileProfileTypeID=t2.FileProfileTypeID ) AND IsLatest=1)),' ','files') as FileCounts\r\nfrom view_FileProfileTypeDocument t2 ";
                if (selectedFileProfileTypeID == null)
                {
                    query += "\r\nWhere parentid is null AND IsDelete is null or IsDelete=0";
                }
                else
                {
                    query += "\r\nwhere parentid=" + selectedFileProfileTypeID + " AND IsDelete is null or IsDelete = 0";
                }
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public static string FormatSize(Int64 bytes)
        {
            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
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
        public async Task<DocumentPermissionModel> GetDocumentPermissionByRoleID(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DocumentRoleId", Id);
                var query = "select  * from DocumentPermission where DocumentRoleId=@DocumentRoleId order by DocumentPermissionID desc";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DocumentPermissionModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<UserGroup>> GetUserGroups()
        {
            try
            {
                var query = "select  * from UserGroup where IsTMS=1 AND StatusCodeID=1";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserGroup>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<UserGroup>> GetAllUserGroups()
        {
            try
            {
                var query = "select  * from UserGroup where StatusCodeID=1";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserGroup>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetDocumentProfiles()
        {
            try
            {
                var query = "select  * from DocumentProfileNoSeries";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentProfileNoSeriesModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentProfileNoSeriesModel> GetDocumentProfileNoSeriesById(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ProfileId", Id);
                var query = "select  * from DocumentProfileNoSeries where ProfileId=@ProfileId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DocumentProfileNoSeriesModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentRole>> GetDocumentRole()
        {
            try
            {
                var query = "select  * from DocumentRole where StatusCodeID=1";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentRole>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentRole>> GetDocumentRoleList()
        {
            try
            {
                var query = "select  t1.*,t2.UserName as AddedBy,t2.UserName as ModifiedBy,t4.CodeValue as StatusCode from DocumentRole t1 \r\nLEFT JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t3 ON t3.UserID=t1.ModifiedByUserID\r\nLEFT JOIN CodeMaster t4 ON  t4.CodeID=t1.StatusCodeID\r\nwhere t1.StatusCodeID=1";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentRole>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<Fileprofiletype>> GetFileprofiletypeAsync()
        {
            try
            {
                var query = "select FileProfileTypeId,Name,IsDocumentAccess,IsEnableCreateTask,IsExpiryDate,IsHidden,AddedByUserId,profileId,sessionId,DynamicFormId,IsDuplicateUpload from FileProfileType";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Fileprofiletype>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<Fileprofiletype> GetByFileprofiletypeIdAsync(long? FileProfileTypeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeID", FileProfileTypeId);
                var query = "select  * from FileProfileType where FileProfileTypeID=@FileProfileTypeID";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<Fileprofiletype>(query, parameters);
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
        public async Task<IReadOnlyList<LinkFileProfileTypeDocument>> GetLinkFileProfileTypeDocumentAsync(List<long?> fileProfileTypeId)
        {
            try
            {
                fileProfileTypeId = fileProfileTypeId != null && fileProfileTypeId.Count > 0 ? fileProfileTypeId : new List<long?>() { -1 };
                var query = "select  LinkFileProfileTypeDocumentId,TransactionSessionId,DocumentId,FileProfileTypeId from LinkFileProfileTypeDocument where FileProfileTypeId in(" + string.Join(',', fileProfileTypeId) + ")";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<LinkFileProfileTypeDocument>(query)).ToList();
                }
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
                //var query = "select  * from DocumentUserRole where FileProfileTypeId in(" + string.Join(',', fileProfileTypeId) + ")";
                var query = "select  t1.* from DocumentUserRole t1\r\n" +
                    "JOIN Employee t2 ON t2.UserID=t1.UserID\r\n" +
                    "LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t2.AcceptanceStatus\r\n" +
                    "Where  t1.FileProfileTypeID in(" + string.Join(',', fileProfileTypeId) + ") AND (t3.Value is null or t3.Value!='Resign')";
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
        public async Task<int> GetDocumentUserRoleByUserIDExitsAsync(long? fileProfileTypeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeID", fileProfileTypeId);
                var query = "select  t1.* from DocumentUserRole t1\r\n" +
                    "Where  t1.FileProfileTypeID=@FileProfileTypeID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<int>(query, parameters)).Count();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentPermissionModel> GetDocumentUserRoleByUserIDAsync(long? fileProfileTypeId, long? userId)
        {
            try
            {
                var Exits = await GetDocumentUserRoleByUserIDExitsAsync(fileProfileTypeId);
                if (Exits > 0)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("FileProfileTypeID", fileProfileTypeId);
                    parameters.Add("UserID", userId);
                    var query = "select  t4.* from DocumentUserRole t1\r\n" +
                        "JOIN Employee t2 ON t2.UserID=t1.UserID\r\n" +
                        "JOIN DocumentPermission t4 ON t4.DocumentRoleID=t1.RoleID\r\n" +
                        "LEFT JOIN ApplicationMasterDetail t3 ON t3.ApplicationMasterDetailID=t2.AcceptanceStatus\r\n" +
                        "Where  t1.FileProfileTypeID=@FileProfileTypeID AND t1.UserID=@UserID AND (t3.Value is null or t3.Value!='Resign')";
                    using (var connection = CreateConnection())
                    {
                        var result = await connection.QueryFirstOrDefaultAsync<DocumentPermissionModel>(query, parameters);
                        // if (result != null)
                        // {
                        return result;
                        /*}
                        else
                        {
                            DocumentPermissionModel documentPermissionModel = new DocumentPermissionModel();
                            documentPermissionModel.DocumentPermissionID = -1;
                            documentPermissionModel.IsPermissionExits = true;
                            return documentPermissionModel;
                        }*/
                    }
                }
                else
                {
                    DocumentPermissionModel documentPermissionModel = new DocumentPermissionModel();
                    documentPermissionModel.DocumentPermissionID = -1;
                    documentPermissionModel.IsPermissionExits = true;
                    return documentPermissionModel;
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
        private async Task<DocumentsModel> GetParentDocuments(SearchModel searchModel, IReadOnlyList<ApplicationUser> appUsers, IReadOnlyList<Fileprofiletype> fileProfileType)
        {
            try
            {
                DocumentsModel documentsModel = new DocumentsModel();
                var parameters = new DynamicParameters();
                parameters.Add("FilterProfileTypeId", searchModel.MasterTypeID);
                parameters.Add("DocumentId", searchModel.ParentID);
                parameters.Add("SessionID", searchModel.SessionID, DbType.Guid);
                var query = DocumentQueryString() + " where FilterProfileTypeId=@FilterProfileTypeId AND SessionID=@SessionID AND DocumentId=@DocumentId AND IsLatest=0 AND IsDelete is null or IsDelete=0";

                using (var connection = CreateConnection())
                {
                    var documetsparent = await connection.QueryFirstOrDefaultAsync<DocumentsModel>(query, parameters);
                    if (documetsparent != null)
                    {
                        if (documetsparent.FileName != null)
                        {
                            var fileName = documetsparent.FileName.Split('.');

                            documentsModel.FileName = documetsparent.FileName != null && documetsparent.FileIndex > 0 ? fileName[0] + "_V0" + documetsparent.FileIndex + "." + fileName[1] : documetsparent.FileName;
                        }
                        documentsModel.DocumentID = documetsparent.DocumentID;
                        documentsModel.ContentType = documetsparent.ContentType;
                        documentsModel.FileSize = (long)Math.Round(Convert.ToDouble(documetsparent.FileSize / 1024));
                        documentsModel.UploadDate = documetsparent.UploadDate;
                        documentsModel.SessionID = documetsparent.SessionId;
                        documentsModel.FilterProfileTypeId = documetsparent.FilterProfileTypeId;
                        documentsModel.FileProfileTypeName = fileProfileType?.FirstOrDefault(f => f.FileProfileTypeId == documetsparent.FilterProfileTypeId)?.Name;
                        documentsModel.DocumentParentId = documetsparent.DocumentParentId;
                        documentsModel.TableName = documetsparent.TableName;
                        documentsModel.Type = "Document";
                        documentsModel.IsNewPath = documetsparent.IsNewPath;
                        documentsModel.AddedDate = documetsparent.AddedDate;
                        documentsModel.UniqueSessionId = documetsparent.UniqueSessionId;
                        documentsModel.AddedByUser = appUsers?.FirstOrDefault(f => f.UserID == documetsparent.AddedByUserID)?.UserName;
                        documentsModel.IsCompressed = documetsparent.IsCompressed;
                        documentsModel.FileIndex = documetsparent.FileIndex;
                        documentsModel.ProfileNo = documetsparent.ProfileNo;
                        documentsModel.Description = documetsparent.Description;
                        documentsModel.FilePath = documetsparent.FilePath;

                    }
                    return documentsModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        private async Task<IReadOnlyList<DocumentsModel>> GetHistoryDocuments(SearchModel searchModel, IReadOnlyList<ApplicationUser> appUsers, IReadOnlyList<Fileprofiletype> fileProfileType)
        {
            try
            {
                List<DocumentsModel> documentsModel = new List<DocumentsModel>();
                var parameters = new DynamicParameters();
                parameters.Add("FilterProfileTypeId", searchModel.MasterTypeID);
                parameters.Add("DocumentParentId", searchModel.ParentID);
                parameters.Add("SessionID", searchModel.SessionID, DbType.Guid);
                var query = DocumentQueryString() + " where FilterProfileTypeId=@FilterProfileTypeId AND SessionID=@SessionID AND DocumentParentId=@DocumentParentId AND IsLatest=0 AND IsDelete is null or IsDelete=0";

                using (var connection = CreateConnection())
                {
                    var documentlist = (await connection.QueryAsync<DocumentsModel>(query, parameters)).ToList();
                    if (searchModel.ParentID != null)
                    {
                        if (documentlist != null && documentlist.Count > 0)
                        {
                            documentlist.ForEach(s =>
                            {
                                var fileName = s.FileName.Split('.');
                                DocumentsModel documentsModels = new DocumentsModel
                                {
                                    DocumentID = s.DocumentID,
                                    FileName = s.FileName != null && s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + "." + fileName[1] : s.FileName,
                                    ContentType = s.ContentType,
                                    FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024)),
                                    UploadDate = s.UploadDate,
                                    IsNewPath = s.IsNewPath,
                                    SessionID = s.SessionId,
                                    FilterProfileTypeId = s.FilterProfileTypeId,
                                    FileProfileTypeName = fileProfileType.FirstOrDefault(f => f.FileProfileTypeId == s.FilterProfileTypeId)?.Name,
                                    DocumentParentId = s.DocumentParentId,
                                    TableName = s.TableName,
                                    UniqueSessionId = s.UniqueSessionId,
                                    Type = "Document",
                                    AddedDate = s.AddedDate,
                                    AddedByUser = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName,
                                    IsCompressed = s.IsCompressed,
                                    ProfileNo = s.ProfileNo,
                                    Description = s.Description,
                                    FilePath = s.FilePath
                                };
                                documentsModel.Add(documentsModels);
                            });
                        }
                    }
                    return documentsModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentTypeModel> GetFileProfileTypeDocumentByHistory(SearchModel searchModel)
        {
            DocumentTypeModel DocumentTypeModel = new DocumentTypeModel();
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
            {
                //var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                var appUsers = await GetApplicationUserAsync();
                var fileProfileType = await GetFileprofiletypeAsync();
                List<long?> fileProfileIds = new List<long?>();
                if (searchModel.MasterTypeID > 0)
                {
                    fileProfileIds.Add(searchModel.MasterTypeID);
                }
                var roleItemsList = await GetDocumentUserRoleAsync(fileProfileIds);
                var docParent = await GetParentDocuments(searchModel, appUsers, fileProfileType);
                var documentPermission = await GetDocumentPermissionByRoll();
                if (docParent != null && docParent.DocumentID > 0)
                {
                    documentsModel.Add(docParent);
                }
                var historyData = await GetHistoryDocuments(searchModel, appUsers, fileProfileType);
                if (historyData != null)
                {
                    documentsModel.AddRange(historyData);
                }
                DocumentTypeModel documentTypeModel = new DocumentTypeModel();
                documentTypeModel.DocumentsData = documentsModel.OrderByDescending(a => a.DocumentID).ToList();
                var roleid = roleItemsList.FirstOrDefault()?.RoleId;
                if (roleid != null)
                {
                    var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)roleid).FirstOrDefault();
                    if (permissionData != null)
                    {
                        documentTypeModel.DocumentPermissionData = permissionData;
                    }
                }
                else
                {
                    documentTypeModel.DocumentPermissionData = new DocumentPermissionModel { IsUpdateDocument = true, IsRead = true, IsShare = true, IsEdit = true, IsMove = true };
                }
                return documentTypeModel;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<MultipleFileProfileItemLists> GetMultipleFileProfileTypeQueryAsync(List<Guid?> SessionIds, List<long?> documentIds, List<long?> userIds)
        {
            MultipleFileProfileItemLists multipleProductioAppLineItemLists = new MultipleFileProfileItemLists();
            try
            {
                var lists = string.Join(',', SessionIds.Select(i => $"'{i}'"));
                var query = "select  DocumentDmsShareId,DocumentId,isDeleted,DocSessionId from DocumentDmsShare where (isDeleted is null or isDeleted=0) AND DocSessionId in(" + lists + ");";
                query += "select  DynamicFormDataId,DynamicFormId,SessionId from DynamicFormData where SessionId in(" + lists + ");";
                query += "select  ActivityEmailTopicID,DocumentSessionId from ActivityEmailTopics where DocumentSessionId in(" + lists + ");";
                userIds = userIds != null && userIds.Count > 0 ? userIds : new List<long?>() { -1 };
                query += "select UserName,UserId from ApplicationUser where userId in(" + string.Join(',', userIds.Distinct()) + ");";
                documentIds = documentIds != null && documentIds.Count > 0 ? documentIds : new List<long?>() { -1 };
                query += "select FileProfileTypeId,Name,IsDocumentAccess,IsEnableCreateTask,IsExpiryDate,IsHidden,AddedByUserId,profileId,sessionId,DynamicFormId,(select t1.SessionID from DynamicForm t1 where t1.ID=DynamicFormId) as DynamicFormSessionID from FileProfileType where FileProfileTypeId in(" + string.Join(',', documentIds) + ");";
                query += "select ProductionActivityAppLineID,ProfileNo,SessionID,'Activity' as Type from ProductionActivityAppLine where SessionID in(" + lists + ")\r\nunion All\r\nselect ProductionActivityRoutineAppLineID as ProductionActivityAppLineID,ProfileNo, SessionID,'Rountine' as Type from ProductionActivityRoutineAppLine where SessionID in(" + lists + ")";
                query += "select  t1.*,t2.SessionID as DynamicFormDataSessionID,t3.DynamicFormID,t4.SessionID as  DynamicFormSessionID from DynamicFormDataUpload t1 JOIN DynamicFormData t2 ON t1.DynamicFormDataID=t2.DynamicFormDataID LEFT JOIN DynamicFormData t3 ON t3.DynamicFormDataID=t2.DynamicFormDataID LEFT JOIN DynamicForm t4 ON t4.ID=t3.DynamicFormID where t1.SessionId in(" + lists + ");";
                using (var connection = CreateConnection())
                {
                    var result = await connection.QueryMultipleAsync(query);
                    multipleProductioAppLineItemLists.DocumentDmsShare = result.Read<DocumentDmsShare>().ToList();
                    multipleProductioAppLineItemLists.DynamicFormData = result.Read<DynamicFormData>().ToList();
                    multipleProductioAppLineItemLists.ActivityEmailTopics = result.Read<ActivityEmailTopics>().ToList();
                    multipleProductioAppLineItemLists.ApplicationUser = result.Read<ApplicationUser>().ToList();
                    multipleProductioAppLineItemLists.Fileprofiletype = result.Read<Fileprofiletype>().ToList();
                    multipleProductioAppLineItemLists.ProductActivityAppModel = result.Read<ProductActivityAppModel>().ToList();
                    multipleProductioAppLineItemLists.DynamicFormDataUpload = result.Read<DynamicFormDataUpload>().ToList();
                    return multipleProductioAppLineItemLists;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentTypeModel> GetAllSelectedFileAsync(DocumentSearchModel documentSearchModel)
        {
            DocumentTypeModel DocumentTypeModel = new DocumentTypeModel();
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
            {
                long? selectedFileProfileTypeID = documentSearchModel.FileProfileTypeId;
                var counts = 1;
                if (documentSearchModel.Type == null && documentSearchModel.AttachSessionId == null)
                {
                    var docs = await GetAllFileProfileDocumentIdAsync(selectedFileProfileTypeID);
                    counts = docs != null ? (docs.Count + 1) : 1;
                    DocumentTypeModel.DocumentsData.AddRange(docs);
                }
                if (documentSearchModel.FileProfileTypeIds == null)
                {
                    documentSearchModel.FileProfileTypeIds = new List<long?>();
                    if (selectedFileProfileTypeID > 0)
                    {
                        documentSearchModel.FileProfileTypeIds.Add(selectedFileProfileTypeID);
                    }
                }

                if (documentSearchModel.FileProfileTypeIds != null && documentSearchModel.FileProfileTypeIds.Count > 0 || documentSearchModel.AttachSessionId != null)
                {
                    var query = string.Empty;
                    var parameters = new DynamicParameters();
                    var linkfileProfileTypes = new List<LinkFileProfileTypeDocument>();
                    if (documentSearchModel.AttachSessionId == null)
                    {
                        linkfileProfileTypes = (await GetLinkFileProfileTypeDocumentAsync(documentSearchModel.FileProfileTypeIds)).ToList();
                        List<long?> linkfileProfileTypeDocumentids = new List<long?>();
                        linkfileProfileTypeDocumentids = linkfileProfileTypes != null && linkfileProfileTypes.Count > 0 ? linkfileProfileTypes.Select(s => s.DocumentId).Distinct().ToList() : new List<long?>() { -1 };
                        var fileProfileTypeId = documentSearchModel.FileProfileTypeIds != null && documentSearchModel.FileProfileTypeIds.Count > 0 ? documentSearchModel.FileProfileTypeIds : new List<long?>() { -1 };
                        var filterQuery = string.Empty;
                        if (!string.IsNullOrEmpty(documentSearchModel.FileName))
                        {
                            filterQuery += "AND FileName like '%" + documentSearchModel.FileName + "%'\r\n";
                        }
                        if (!string.IsNullOrEmpty(documentSearchModel.Extension))
                        {
                            filterQuery += "AND FileName like '%" + documentSearchModel.Extension + "%'\r\n";
                        }
                        if (!string.IsNullOrEmpty(documentSearchModel.ProfileNo))
                        {
                            filterQuery += "AND ProfileNo like '%" + documentSearchModel.ProfileNo + "%'\r\n";
                        }
                        if (documentSearchModel.FromDate != null)
                        {
                            var from = documentSearchModel.FromDate.Value.ToString("yyyy-MM-dd");
                            filterQuery += "AND CAST(uploadDate AS Date) >='" + from + "'\r\n";
                        }
                        if (documentSearchModel.ToDate != null)
                        {
                            var to = documentSearchModel.ToDate.Value.ToString("yyyy-MM-dd");
                            filterQuery += "AND CAST(uploadDate AS Date)<='" + to + "'\r\n";
                        }

                        query = DocumentQueryString() + " where" +
                            " FilterProfileTypeId in(" + string.Join(",", fileProfileTypeId.Distinct()) + ") " + filterQuery +
                            "AND IsLatest=1 AND (IsDelete is null or IsDelete=0) And SessionID is Not null\r\n" +
                            "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) " +
                            "OR (DocumentID in(" + string.Join(",", linkfileProfileTypeDocumentids) + ") AND IsLatest=1) " +
                            "order by DocumentId desc";
                    }
                    else
                    {
                        parameters.Add("SessionID", documentSearchModel.AttachSessionId, DbType.Guid);
                        query = DocumentQueryString() + " where\r\n" +
                            " IsLatest=1 AND (IsDelete is null or IsDelete=0) And SessionID is Not null\r\n" +
                            "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL)\r\n" +
                            "AND SessionID=@SessionID" +
                            "\r\norder by DocumentId desc";
                    }
                    var documents = new List<Documents>();
                    using (var connection = CreateConnection())
                    {
                        documents = (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                    }
                    if (documents != null && documents.Count > 0)
                    {
                        List<long?> userIds = new List<long?>();
                        userIds.AddRange(documents.Select(a => a.AddedByUserId).ToList());
                        userIds.AddRange(documents.Where(a => a.ModifiedByUserId > 0).Select(a => a.ModifiedByUserId).ToList());
                        userIds.AddRange(documents.Where(a => a.LockedByUserId > 0).Select(a => a.LockedByUserId).ToList());
                        var sessionIds = documents.Where(a => a.SessionId != null).Select(a => a.SessionId).ToList();
                        var filterProfileTypeIds = documents.Where(w => w.FilterProfileTypeId > 0).Select(a => a.FilterProfileTypeId).ToList();
                        var multipleData = await GetMultipleFileProfileTypeQueryAsync(sessionIds, filterProfileTypeIds, userIds);
                        var docShares = multipleData.DocumentDmsShare;
                        var dynamicFormData = multipleData.DynamicFormDataUpload;
                        var emailTopics = multipleData.ActivityEmailTopics;
                        var appUsers = multipleData.ApplicationUser;
                        var fileProfileType = multipleData.Fileprofiletype;
                        var productActivityApp = multipleData.ProductActivityAppModel;
                        var docIds = documents.Select(a => a.DocumentId).ToList();
                        documents.ForEach(s =>
                        {
                            var fileprfiles = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId);
                            var documentcount = documents?.Where(w => w.DocumentParentId == s.DocumentParentId).Count();
                            var lastIndex = s.FileName != null ? s.FileName.LastIndexOf(".") : 0;
                            lastIndex = lastIndex > 0 ? lastIndex : 0;
                            var name = s.FileName != null ? s.FileName?.Substring(lastIndex) : "";
                            var fileName = s.FileName?.Split(name);
                            DocumentsModel documentsModels = new DocumentsModel();
                            documentsModels.UniqueNo = counts;
                            documentsModels.SharesCount = 0;
                            documentsModels.DynamicFormId = fileprfiles?.DynamicFormId;
                            documentsModels.DynamicFormSessionId = fileprfiles?.DynamicFormSessionId;
                            var sharesCountCount = docShares.Where(a => a.DocSessionId == s.SessionId).Count();
                            if (sharesCountCount > 0)
                            {
                                documentsModels.SharesCount = sharesCountCount;
                            }
                            if (dynamicFormData != null)
                            {
                                var isDynamicFromData = dynamicFormData.Where(a => a.SessionId == s.SessionId).FirstOrDefault();
                                if (isDynamicFromData != null)
                                {
                                    documentsModels.IsDynamicFromData = isDynamicFromData != null ? true : false;
                                    documentsModels.DynamicFormDataSessionId = isDynamicFromData != null ? isDynamicFromData.DynamicFormDataSessionId : null;
                                    documentsModels.DynamicFormId = isDynamicFromData != null ? isDynamicFromData.DynamicFormId : null;
                                    documentsModels.DynamicFormSessionId = isDynamicFromData != null ? isDynamicFromData.DynamicFormSessionId : null;
                                }
                            }
                            if (emailTopics != null)
                            {
                                var isemailTopics = emailTopics.Where(a => a.DocumentSessionId == s.SessionId).Count();
                                documentsModels.IsEmailTopics = isemailTopics > 0 ? true : false;
                            }
                            documentsModels.Extension = s.FileName != null ? s.FileName?.Split(".").Last() : "";
                            documentsModels.SessionId = s.SessionId;
                            documentsModels.DocumentID = s.DocumentId;
                            documentsModels.FileName = s.FileName != null ? (s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + name : s.FileName) : s.FileName;
                            documentsModels.OriginalFileName = s.FileName;
                            documentsModels.ContentType = s.ContentType;
                            documentsModels.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                            documentsModels.FileSizes = s.FileSize > 0 ? FormatSize((long)s.FileSize) : "";
                            documentsModels.UploadDate = s.UploadDate;
                            documentsModels.UniqueSessionId = s.UniqueSessionId;
                            documentsModels.SessionID = s.SessionId;
                            documentsModels.FilterProfileTypeId = s.FilterProfileTypeId;
                            documentsModels.FileProfileTypeName = fileprfiles?.Name;
                            documentsModels.FileProfileTypeSessionId = fileprfiles?.SessionId;
                            documentsModels.ProfileID = fileprfiles?.ProfileId;
                          
                            documentsModels.DocumentParentId = s.DocumentParentId;
                            documentsModels.TableName = s.TableName;
                            documentsModels.IsMobileUpload = s.IsMobileUpload;
                            documentsModels.Type = "Document";
                            documentsModels.ExpiryDate = s.ExpiryDate;
                            documentsModels.FileIndex = s.FileIndex;
                            documentsModels.TotalDocument = documentcount == 1 ? 1 : (documentcount + 1);
                            documentsModels.UploadedByUserId = s.AddedByUserId;
                            documentsModels.ModifiedByUserID = s.ModifiedByUserId;
                            documentsModels.AddedDate = s.UploadDate;
                            documentsModels.ModifiedDate = s.ModifiedDate;
                            documentsModels.SourceFrom = s.SourceFrom;
                            documentsModels.AddedByUser = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserId)?.UserName;
                            documentsModels.AddedBy = documentsModels.AddedByUser;
                            documentsModels.ModifiedByUser = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserId)?.UserName;
                            documentsModels.ModifiedBy = documentsModels.ModifiedByUser;
                            documentsModels.IsLocked = s.IsLocked;
                            documentsModels.LockedByUserId = s.LockedByUserId;
                            documentsModels.LockedDate = s.LockedDate;
                            documentsModels.IsNewPath = s.IsNewPath;
                            documentsModels.AddedByUserID = s.AddedByUserId;
                            documentsModels.IsCompressed = s.IsCompressed;
                            documentsModels.LockedByUser = appUsers.FirstOrDefault(f => f.UserID == s.LockedByUserId)?.UserName;
                            documentsModels.isDocumentAccess = fileprfiles?.IsDocumentAccess;
                            documentsModels.IsEnableCreateTask = fileprfiles?.IsEnableCreateTask;
                            documentsModels.CloseDocumentId = s.CloseDocumentId;
                            documentsModels.CssClass = s.CloseDocumentId != null && s.CloseDocumentId == 2561 ? "blue-grey lighten - 3" : "transparent";
                            documentsModels.ProfileNo = s.ProfileNo;
                            documentsModels.FilePath = s.FilePath;
                            documentsModels.FileProfileTypeAddedByUserId = fileprfiles?.AddedByUserId;
                            documentsModels.IsExpiryDate = fileprfiles?.IsExpiryDate;
                            var description = linkfileProfileTypes?.Where(f => f.FileProfileTypeId == selectedFileProfileTypeID && f.TransactionSessionId == s.SessionId && f.DocumentId == s.DocumentId).FirstOrDefault()?.Description;
                            var productActivity = productActivityApp.Where(a => a.SessionId == s.SessionId).FirstOrDefault();
                            if (productActivity != null)
                            {
                                documentsModels.ActivityType = productActivity?.Type;
                                documentsModels.ActivityProfileNo = productActivity?.ProfileNo;
                            }
                            if (description != null)
                            {
                                documentsModels.Description = description;
                            }
                            else
                            {
                                documentsModels.Description = s.Description;
                            }
                            documentsModel.Add(documentsModels);
                            counts++;
                        });
                    }
                    DocumentTypeModel.DocumentsData.AddRange(documentsModel.OrderByDescending(a => a.DocumentID).ToList());

                }
                return DocumentTypeModel;

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<Documents>> GetAllSelectedFilesAsync(DocumentSearchModel documentSearchModel)
        {
            List<Documents> documents = new List<Documents>();
            try
            {

                var query = string.Empty;
                var parameters = new DynamicParameters();
                var linkfileProfileTypes = new List<LinkFileProfileTypeDocument>();

                linkfileProfileTypes = (await GetLinkFileProfileTypeDocumentAsync(documentSearchModel.FileProfileTypeIds)).ToList();
                List<long?> linkfileProfileTypeDocumentids = new List<long?>();
                linkfileProfileTypeDocumentids = linkfileProfileTypes != null && linkfileProfileTypes.Count > 0 ? linkfileProfileTypes.Select(s => s.DocumentId).Distinct().ToList() : new List<long?>() { -1 };
                var fileProfileTypeId = documentSearchModel.FileProfileTypeIds != null && documentSearchModel.FileProfileTypeIds.Count > 0 ? documentSearchModel.FileProfileTypeIds : new List<long?>() { -1 };
                var filterQuery = string.Empty;
                if (!string.IsNullOrEmpty(documentSearchModel.FileName))
                {
                    filterQuery += "AND FileName like '%" + documentSearchModel.FileName + "%'\r\n";
                }
                if (!string.IsNullOrEmpty(documentSearchModel.Extension))
                {
                    filterQuery += "AND FileName like '%" + documentSearchModel.Extension + "%'\r\n";
                }
                if (!string.IsNullOrEmpty(documentSearchModel.ProfileNo))
                {
                    filterQuery += "AND ProfileNo like '%" + documentSearchModel.ProfileNo + "%'\r\n";
                }
                if (documentSearchModel.FromDate != null)
                {
                    var from = documentSearchModel.FromDate.Value.ToString("yyyy-MM-dd");
                    filterQuery += "AND CAST(uploadDate AS Date) >='" + from + "'\r\n";
                }
                if (documentSearchModel.ToDate != null)
                {
                    var to = documentSearchModel.ToDate.Value.ToString("yyyy-MM-dd");
                    filterQuery += "AND CAST(uploadDate AS Date)<='" + to + "'\r\n";
                }

                query = DocumentQueryString() + " where" +
                    " FilterProfileTypeId in(" + string.Join(",", fileProfileTypeId.Distinct()) + ") " + filterQuery +
                    "AND IsLatest=1 AND (IsDelete is null or IsDelete=0) And SessionID is Not null\r\n" +
                    "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) " +
                    "OR (DocumentID in(" + string.Join(",", linkfileProfileTypeDocumentids) + ") AND IsLatest=1) " +
                    "order by DocumentId desc";

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
        public async Task<IReadOnlyList<DocumentLinkModel>> GetDocumentLinkByDocumentId(long? id)
        {
            List<DocumentLinkModel> DocumentLinkModels = new List<DocumentLinkModel>();
            try
            {
                var fileProfileList = await GetFileprofiletypeAsync();
                var parameters = new DynamicParameters();
                parameters.Add("DocumentID", id, DbType.Int64);
                var query = "select\r\nt1.DocumentLinkId,\r\nt1.DocumentID,\r\nt1.FileProfieTypeID,\r\n" +
                    "t1.LinkDocumentId,\r\nt1.DocumentPath,\r\nt1.FolderID,\r\n" +
                    "t1.StatusCodeID,\r\nt1.AddedByUserID,\r\nt1.ModifiedByUserID,\r\n" +
                    "t1.ModifiedDate,\r\nt1.AddedDate,\r\nt2.SessionID,\r\nt2.FileSize, \r\n" +
                    "t2.FileName as Title,\r\nt2.ContentType, \r\nt2.FileName as LinkDocumentName,\r\nt3.UniqueSessionId as UniqueSessionId,\r\n" +
                    "t2.FilterProfileTypeID as PathFileProfieTypeId,\r\nt2.FolderID as LinkFolderID,\r\n" +
                    "t2.FilePath,\r\nt3.TableName as Type,\r\nt3.FileName as DocumentName,\r\nt4.UserName as AddedByUser,\r\nt3.IsNewPath as IsNewPath,\r\n" +

                    "t5.UserName as ModifiedByUser,\r\nt6.CodeValue as StatusCode\r\nfrom DocumentLink t1 \r\n" +
                    "JOIN Documents t2 ON t1.LinkDocumentId=t2.DocumentID\r\nJOIN Documents t3 ON t1.DocumentID=t3.DocumentID \r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\n" +
                    "LEFT JOIN CodeMaster t6 ON t6.CodeID=t1.StatusCodeID where t1.DocumentID=@DocumentID AND (t2.IsDelete=0 or t2.IsDelete Is Null) AND (t3.IsDelete=0 or t3.IsDelete Is Null) ";
                using (var connection = CreateConnection())
                {
                    var DocumentLink = (await connection.QueryAsync<DocumentLinkModel>(query, parameters)).ToList();
                    if (DocumentLink != null && DocumentLink.Count > 0)
                    {
                        DocumentLink.ForEach(s =>
                        {

                            DocumentLinkModel DocumentLinkModel = new DocumentLinkModel();
                            DocumentLinkModel.DocumentLinkId = s.DocumentLinkId;
                            DocumentLinkModel.DocumentID = s.LinkDocumentId;
                            DocumentLinkModel.LinkDocumentId = s.LinkDocumentId;
                            DocumentLinkModel.ModifiedByUser = s.ModifiedByUser;
                            DocumentLinkModel.StatusCode = s.StatusCode;
                            DocumentLinkModel.AddedByUserID = s.AddedByUserID;
                            DocumentLinkModel.ModifiedByUserID = s.ModifiedByUserID;
                            DocumentLinkModel.StatusCodeID = s.StatusCodeID;
                            DocumentLinkModel.AddedDate = s.AddedDate;
                            DocumentLinkModel.ModifiedDate = s.ModifiedDate;
                            DocumentLinkModel.DocumentId = s.DocumentId;
                            DocumentLinkModel.AddedByUser = s.AddedByUser;
                            DocumentLinkModel.FileProfieTypeId = s.FileProfieTypeId;
                            DocumentLinkModel.FolderId = s.FolderId;
                            DocumentLinkModel.DocumentPath = s.DocumentPath;
                            DocumentLinkModel.SessionId = s.SessionId;
                            var filesize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                            DocumentLinkModel.FileSize = filesize;
                            DocumentLinkModel.Title = s.Title;
                            DocumentLinkModel.Type = s.Type;
                            DocumentLinkModel.IsNewPath = s.IsNewPath;
                            DocumentLinkModel.UniqueSessionId = s.UniqueSessionId;
                            DocumentLinkModel.ContentType = s.ContentType;
                            DocumentLinkModel.DocumentName = s.DocumentName;
                            DocumentLinkModel.LinkDocumentName = s.LinkDocumentName;
                            var FileProfileTypeId = s.PathFileProfieTypeId;
                            DocumentLinkModel.PathFileProfieTypeId = FileProfileTypeId;
                            DocumentLinkModel.PathFileProfieTypeId = FileProfileTypeId;
                            DocumentLinkModel.FilePath = s.FilePath;
                            DocumentLinkModels.Add(DocumentLinkModel);
                        });

                    }
                }
                return DocumentLinkModels.OrderByDescending(d => d.AddedDate).ToList();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentLinkModel>> GetParentDocumentsByLinkDocumentId(long? id)
        {
            List<DocumentLinkModel> DocumentLinkModels = new List<DocumentLinkModel>();
            try
            {
                var fileProfileList = await GetFileprofiletypeAsync();
                var parameters = new DynamicParameters();
                parameters.Add("LinkDocumentId", id, DbType.Int64);
                var query = "select\r\nt1.DocumentLinkId,\r\n" +
                    "t1.DocumentID,\r\nt1.FileProfieTypeID,\r\n" +
                    "t1.LinkDocumentId,\r\nt1.DocumentPath,\r\n" +
                    "t1.FolderID,\r\nt1.StatusCodeID,\r\nt1.AddedByUserID,\r\n" +
                    "t1.ModifiedByUserID,\r\nt1.ModifiedDate,\r\nt1.AddedDate,\r\n" +
                    "t3.SessionID,\r\n" +
                    "t2.IsNewPath,\r\n" +
                    "t2.UniqueSessionId,\r\n" +
                    "t3.FileSize, \r\n" +
                    "t3.FileName as Title,\r\n" +
                    "t3.ContentType, \r\nt3.FileName as LinkDocumentName,\r\n" +
                    "t3.FilterProfileTypeID as PathFileProfieTypeId,\r\n" +
                    "t3.FolderID as LinkFolderID,\r\n" +
                    "t3.FilePath,\r\n" +
                    "t2.TableName as Type,\r\n" +
                    "t2.FileName as DocumentName,\r\n" +
                    "t4.UserName as AddedByUser,\r\nt5.UserName as ModifiedByUser,\r\n" +
                    "t6.CodeValue as StatusCode\r\nfrom DocumentLink t1 \r\n" +
                    "JOIN Documents t2 ON " +
                    "t1.LinkDocumentId=t2.DocumentID\r\n" +
                    "JOIN Documents t3 ON t1.DocumentID=t3.DocumentID \r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\n" +
                    "LEFT JOIN CodeMaster t6 ON t6.CodeID=t1.StatusCodeID where t1.LinkDocumentId=@LinkDocumentId AND (t2.IsDelete=0 or t2.IsDelete Is Null) AND (t3.IsDelete=0 or t3.IsDelete Is Null)";
                using (var connection = CreateConnection())
                {
                    var DocumentLink = (await connection.QueryAsync<DocumentLinkModel>(query, parameters)).ToList();
                    if (DocumentLink != null && DocumentLink.Count > 0)
                    {
                        DocumentLink.ForEach(s =>
                        {

                            DocumentLinkModel DocumentLinkModel = new DocumentLinkModel();
                            DocumentLinkModel.DocumentLinkId = s.DocumentLinkId;
                            DocumentLinkModel.DocumentID = s.DocumentID;
                            DocumentLinkModel.LinkDocumentId = s.LinkDocumentId;
                            DocumentLinkModel.ModifiedByUser = s.ModifiedByUser;
                            DocumentLinkModel.StatusCode = s.StatusCode;
                            DocumentLinkModel.AddedByUserID = s.AddedByUserID;
                            DocumentLinkModel.ModifiedByUserID = s.ModifiedByUserID;
                            DocumentLinkModel.StatusCodeID = s.StatusCodeID;
                            DocumentLinkModel.AddedDate = s.AddedDate;
                            DocumentLinkModel.ModifiedDate = s.ModifiedDate;
                            DocumentLinkModel.DocumentId = s.DocumentId;
                            DocumentLinkModel.AddedByUser = s.AddedByUser;
                            DocumentLinkModel.FileProfieTypeId = s.FileProfieTypeId;
                            DocumentLinkModel.FolderId = s.FolderId;
                            DocumentLinkModel.DocumentPath = s.DocumentPath;
                            DocumentLinkModel.SessionId = s.SessionId;
                            var filesize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                            DocumentLinkModel.FileSize = filesize;
                            DocumentLinkModel.Title = s.Title;
                            DocumentLinkModel.Type = s.Type;
                            DocumentLinkModel.ContentType = s.ContentType;
                            DocumentLinkModel.IsNewPath = s.IsNewPath;
                            DocumentLinkModel.UniqueSessionId = s.UniqueSessionId;
                            DocumentLinkModel.DocumentName = s.DocumentName;
                            DocumentLinkModel.LinkDocumentName = s.LinkDocumentName;
                            var FileProfileTypeId = s.PathFileProfieTypeId;
                            DocumentLinkModel.PathFileProfieTypeId = FileProfileTypeId;
                            DocumentLinkModel.PathFileProfieTypeId = FileProfileTypeId;
                            DocumentLinkModel.FilePath = s.FilePath;
                            DocumentLinkModels.Add(DocumentLinkModel);
                        });

                    }
                }
                return DocumentLinkModels.OrderByDescending(d => d.AddedDate).ToList();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentLink> CheckDocumentLinkExits(DocumentLink documentLink)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DocumentId", documentLink.DocumentId);
                parameters.Add("LinkDocumentId", documentLink.LinkDocumentId);
                var query = "select  * from DocumentLink where DocumentId=@DocumentId AND LinkDocumentId=@LinkDocumentId";

                using (var connection = CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<DocumentLink>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DocumentUserRole>> GetDocumentUserRoleByDocEmptyAsync(List<long?> fileProfileTypeId)
        {
            try
            {

                fileProfileTypeId = fileProfileTypeId != null && fileProfileTypeId.Count > 0 ? fileProfileTypeId : new List<long?>() { -1 };
                var query = "select  * from DocumentUserRole where DocumentID is null AND FileProfileTypeId in(" + string.Join(',', fileProfileTypeId) + ")";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentUserRole>(query, null)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<List<ViewEmployee>> GetGroupByUsers(long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                var query = @"select concat(emp.NickName,' | ',emp.FirstName,',',emp.LastName) as Name, emp.EmployeeID, emp.UserID, emp.SageID,emp.PlantID, emp.LevelID, emp.LanguageID, emp.CityID, emp.RegionID, emp.ReportID, emp.FirstName, emp.NickName, emp.LastName, emp.Gender, emp.JobTitle, emp.Email, u.LoginID, u.LoginPassword,u.UserCode,emp.TypeOfEmployeement, emp.Signature, emp.ImageUrl, emp.DateOfEmployeement,emp.LastWorkingDate, emp.Extension, emp.SpeedDial, emp.SkypeAddress, emp.Mobile,emp.IsActive, emp.SectionID,  emp.DivisionID,emp.DesignationId, emp.DepartmentId, emp.SubSectionId, emp.SubSectionTid, u.StatusCodeID,  emp.AddedByUserId, emp.ModifiedByUserId,emp.AddedDate, emp.ModifiedDate, emp.AcceptanceStatus, emp.ExpectedJoiningDate, emp.AcceptanceStatusDate, emp.HeadCount, a.UserName as AddedByUser, mo.UserName as ModifiedByUser, ag.Value as Status, u.SessionID, u.InvalidAttempts, 
                u.Locked,d.Name as DesignationName,p.PlantCode as CompanyName from UserGroupUser UGU
                INNER JOIN Employee emp on emp.UserID = UGU.UserID
                LEFT JOIN ApplicationUser u ON u.userId = emp.userId
                LEFT JOIN ApplicationUser a ON a.UserId = emp.AddedByUserID 
                LEFT JOIN ApplicationUser mo ON mo.UserId = emp.ModifiedByUserID 
                LEFT JOIN ApplicationMasterDetail ag ON ag.ApplicationMasterDetailID = emp.AcceptanceStatus 
                LEFT JOIN plant p ON p.plantId = emp.plantId 
                LEFT JOIN Designation d ON d.DesignationID = emp.DesignationID
                where (ag.Value!='Resign' or ag.Value is null) and UGU.UserGroupID = @id";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<ViewEmployee>(query, parameters)).ToList();
                    // Pass parameters to the QueryAsync method
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<UserGroupUser>> GetUserGroupUser()
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
        public async Task<IReadOnlyList<DocumentsModel>> GetUploadedButNoProfileNo(DocumentsUploadModel documentsUploadModel)
        {
            List<DocumentsModel> result = new List<DocumentsModel>();
            try
            {
                var appUsers = await GetApplicationUserAsync();
                var query = DocumentQueryString();
                var listss = documentsUploadModel.FailedDocumentsUploadModels.Select(s => s.FilePath).ToList();
                var lists = string.Join(',', listss.Select(i => $"'{i}'"));
                query += "\n\rwhere FilePath in(" + lists + ") AND FilterProfileTypeID IS NULL AND SourceFrom='FileProfile' AND IsLatest=1 AND AddedByUserID=" + documentsUploadModel.UserId + " AND (IsDelete is null or IsDelete=0) And SessionID is Not null order by DocumentId desc";


                var data = new List<DocumentsModel>();
                using (var connection = CreateConnection())
                {
                    data = (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                }
                if (data != null && data.Count() > 0)
                {
                    data.ForEach(s =>
                    {
                        s.AddedByUser = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.ModifiedByUser = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                        s.FileSizes = s.FileSize > 0 ? FormatSize((long)s.FileSize) : "";
                        s.FilePath = s.FilePath;
                        result.Add(s);
                    });
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentsModel>> GetNoProfileNo(long? UserId, DateTime? StartDate)
        {
            List<DocumentsModel> result = new List<DocumentsModel>();
            try
            {
                var appUsers = await GetApplicationUserAsync();
                var query = DocumentQueryString();
                if (StartDate != null)
                {
                    var to = StartDate.Value.ToString("yyyy-MM-dd");
                    query += "\n\rwhere  FilterProfileTypeID IS NULL AND CAST(uploadDate AS Date)='" + to + "' AND SourceFrom='FileProfile' AND IsNewPath=1 AND IsLatest=1 AND AddedByUserID=" + UserId + " AND (IsDelete is null or IsDelete=0) And SessionID is Not null order by DocumentId desc";
                }
                else
                {
                    query += "\n\rwhere  FilterProfileTypeID IS NULL AND SourceFrom='FileProfile' AND IsNewPath=1 AND IsLatest=1 AND AddedByUserID=" + UserId + " AND (IsDelete is null or IsDelete=0) And SessionID is Not null order by DocumentId desc";
                }

                var data = new List<DocumentsModel>();
                using (var connection = CreateConnection())
                {
                    data = (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                }
                if (data != null && data.Count() > 0)
                {
                    data.ForEach(s =>
                    {
                        s.AddedByUser = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName;
                        s.ModifiedByUser = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                        s.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                        s.FileSizes = s.FileSize > 0 ? FormatSize((long)s.FileSize) : "";
                        result.Add(s);
                    });
                }
                return result;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<LeveMasterUsersModel>> GetLeveMasterUsers(IEnumerable<long> SelectLevelMasterIDs)
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

        public async Task<IReadOnlyList<DocumentUserRoleModel>> GetDocumentUserRoleList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", Id);
                var query = "select t1.*,t2.DocumentRoleName,t2.DocumentRoleDescription,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.Name as FileProfileType,\r\nt5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\nt8.Name as DesignationName,\r\nCONCAT(t6.FirstName,' | ',t6.LastName) as FullName\r\n" +
                    "from DocumentUserRole t1\r\n" +
                    "JOIN DocumentRole t2 ON t1.RoleID=t2.DocumentRoleID\r\n" +
                    "LEFT JOIN UserGroup t3 ON t1.UserGroupID=t3.UserGroupID\r\n" +
                    "LEFT JOIN FileProfileType t4 ON t4.FileProfileTypeID=t1.FileProfileTypeID\r\n" +
                    "LEFT JOIN LevelMaster t5 ON t1.LevelID=t5.LevelID\r\n" +
                    "JOIN Employee t6 ON t1.UserID=t6.UserID\r\n" +
                    "LEFT JOIN Department t7 ON t6.DepartmentID=t7.DepartmentID\r\n" +
                    "LEFT JOIN Designation t8 ON t8.DesignationID=t6.DesignationID\r\n\r\n WHERE t1.FileProfileTypeID=@FileProfileTypeId";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentUserRoleModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<FileProfileSetUpDropDown>> GetFileProfileSetUpDropDownAsync()
        {
            try
            {
                var query = "select * from FileProfileSetUpDropDown";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FileProfileSetUpDropDown>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<FileProfileSetupFormModel>> GetFileProfileSetupFormList(long? Id)
        {
            try
            {
                var dropDownvalus = await GetFileProfileSetUpDropDownAsync();
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", Id);
                var query = "select t1.*,t2.Name as FileProfileTypeName,\r\nt3.CodeValue as ControlType,\r\nt4.CodeValue as StatusCode,\r\nt5.UserName as AddedByUser,\r\nt6.UserName as ModifiedByUser\r\nfrom FileProfileSetupForm t1\r\nJOIN FileProfileType t2  ON t1.FileProfileTypeId=t2.FileProfileTypeId\r\n" +
                    "JOIN CodeMaster t3 ON t3.CodeID=t1.ControlTypeID\r\n" +
                    "JOIN CodeMaster t4 ON t4.CodeID=t1.StatusCodeID\r\n" +
                    "JOIN ApplicationUser t5 ON t5.UserID=t1.AddedByUserID\r\n" +
                    "LEFT JOIN ApplicationUser t6 ON t6.UserID=t1.ModifiedByUserID\r\nWHERE t1.FileProfileTypeID=@FileProfileTypeId";
                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<FileProfileSetupFormModel>(query, parameters)).ToList();
                    if (result != null && result.Count > 0)
                    {
                        result.ForEach(s =>
                        {
                            s.DropDownValues = dropDownvalus.Where(d => d.FileProfileSetupFormId == s.FileProfileSetupFormId).Select(d => d.DisplayValue).ToList();
                            s.PropertyName = s.ControlType.ToLower() + '_' + s.FileProfileSetupFormId;
                            s.DropDownItems = dropDownvalus.Where(w => w.FileProfileSetupFormId == s.FileProfileSetupFormId).Select(tn => new DropDownNameItems { Value = tn.DisplayValue, Text = tn.DisplayValue }).ToList();
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
        public async Task<IReadOnlyList<DocumentNoSeriesModel>> GetReserveProfileNumberSeries(long? Id, long? ProfileId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeID", Id);
                parameters.Add("ProfileId", ProfileId);
                var query = "SELECT tt1.* From(select t1.*,\r\n" +
                    "t2.UserName as AddedByUser,\r\n" +
                    "t3.UserName as RequestorName,\r\n" +
                    "t4.Name as ProfileName,\r\n" +
                    "t5.Name as FileProfileTypeName,\r\n" +
                    "t6.UserName as ModifiedByUser\r\nfrom DocumentNoSeries t1\r\n" +
                    "JOIN ApplicationUser t2 ON t2.UserID=t1.AddedByUserID\r\n" +
                    "JOIN ApplicationUser t3 ON t3.UserID=t1.RequestorId\r\n" +
                    "JOIN DocumentProfileNoSeries t4 ON t4.ProfileID=t1.ProfileID\r\n" +
                    "LEFT JOIN FileProfileType t5 ON t5.FileProfileTypeID=t1.FileProfileTypeID\r\n" +
                    "LEFT JOIN ApplicationUser t6 ON t6.UserID=t1.ModifiedByUserID\r\n" +
                    "WHERE  (t1.IsUpload=0 OR t1.FileProfileTypeID=@FileProfileTypeID) AND t1.ProfileId=@ProfileId)tt1 \r\n" +
                    "WHERE  tt1.IsUpload!=1 AND tt1.FileProfileTypeID=@FileProfileTypeID OR tt1.FileProfileTypeID is NULL\r\n";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentNoSeriesModel>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentsModel>> GetAllFileProfileDeleteAsync(ApplicationUser userData)
        {
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
            {
                var query = "select  *,ROW_NUMBER() OVER(ORDER BY name) AS UniqueNo,Name as Filename,\r\n" +
                    "FileProfileTypeID as DocumentID,\r\n" +
                    "Profile as ProfileNo,\r\n" +
                    "CASE WHEN ModifiedByUserID >0 THEN ModifiedBy ELSE AddedBy END AS AddedByUser,\r\n" +
                    "CASE WHEN ModifiedByUserID >0 THEN ModifiedDate ELSE AddedDate END AS AddedDate,\r\n" +
                    "CONCAT((select count(*) as counts from FileProfileType tt where tt.parentId=t2.FileProfileTypeID),' ','items') as FileSizes,\r\n" +
                    "CONCAT((Select COUNT(*) as DocCount from Documents where FilterProfileTypeId=t2.FileProfileTypeID\r\nAND IsLatest=1  \r\nAND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) \r\nOR (DocumentID in(select DocumentID from LinkFileProfileTypeDocument where FileProfileTypeID=t2.FileProfileTypeID ) AND IsLatest=1)),' ','files') as FileCounts\r\n" +
                    "from view_FileProfileTypeDocument t2 WHERE IsDelete = 1";

                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                    if (result != null && result.Count > 0)
                    {
                        var documentPermission = await GetDocumentPermissionByRoll();
                        var FileProfileTypeIds = result.Where(w => w.FilterProfileTypeId > 0).Select(s => s.FileProfileTypeId).Distinct().ToList();
                        var roleItemsList = await GetDocumentUserRoleAsync(FileProfileTypeIds);
                        result.ForEach(s =>
                        {
                            var isPerAccess = 0;
                            var exits = roleItemsList.Where(f => f.FileProfileTypeId == s.FilterProfileTypeId).ToList();
                            if (exits.Count > 0)
                            {
                                var userExits = exits.FirstOrDefault(f => f.UserId == userData.UserID);
                                if (userExits != null)
                                {
                                    var per = documentPermission.FirstOrDefault(d => d.DocumentRoleID == userExits.RoleId);
                                    if (per != null && per.IsDelete == true)
                                    {
                                        isPerAccess = 1;
                                    }
                                }
                            }
                            else
                            {
                                isPerAccess = 1;
                            }
                            if (isPerAccess == 1 || s.AddedByUserID == userData.UserID)
                            {
                                documentsModel.Add(s);
                            }
                        });
                    }
                    return documentsModel;
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentTypeModel> GetAllDocumentDeleteAsync()
        {
            var userData = await _localStorageService.GetItem<ApplicationUser>("user");
            DocumentTypeModel DocumentTypeModel = new DocumentTypeModel();
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
            {
                var docs = await GetAllFileProfileDeleteAsync(userData);
                var counts = docs != null ? (docs.Count + 1) : 1;
                DocumentTypeModel.DocumentsData.AddRange(docs);

                var appUsers = await GetApplicationUserAsync();
                var fileProfileType = await GetFileprofiletypeAsync();
                var parameters = new DynamicParameters();
                var query = DocumentQueryString() + " where IsDelete=1 order by DocumentId desc";

                using (var connection = CreateConnection())
                {
                    var documents = (await connection.QueryAsync<Documents>(query)).ToList();
                    if (documents != null && documents.Count > 0)
                    {
                        var documentPermission = await GetDocumentPermissionByRoll();
                        var FileProfileTypeIds = documents.Where(w=>w.FilterProfileTypeId>0).Select(s => s.FilterProfileTypeId).Distinct().ToList();
                        var roleItemsList = await GetDocumentUserRoleAsync(FileProfileTypeIds);
                        documents.ForEach(s =>
                        {
                            var isPerAccess = 0;
                            var exits = roleItemsList.Where(f => f.FileProfileTypeId == s.FilterProfileTypeId).ToList();
                            if (exits.Count > 0)
                            {
                                var userExits = exits.FirstOrDefault(f => f.UserId == userData.UserID);
                                if (userExits != null)
                                {
                                    var per = documentPermission.FirstOrDefault(d => d.DocumentRoleID == userExits.RoleId);
                                    if (per != null && per.IsDelete == true)
                                    {
                                        isPerAccess = 1;
                                    }
                                }
                            }
                            else
                            {
                                isPerAccess = 1;
                            }
                            if (isPerAccess == 1 || s.AddedByUserId == userData.UserID)
                            {
                                var documentcount = documents?.Where(w => w.DocumentParentId == s.DocumentParentId).Count();
                                var lastIndex = s.FileName != null ? s.FileName.LastIndexOf(".") : 0;
                                lastIndex = lastIndex > 0 ? lastIndex : 0;
                                var name = s.FileName != null ? s.FileName?.Substring(lastIndex) : "";
                                var fileName = s.FileName?.Split(name);
                                DocumentsModel documentsModels = new DocumentsModel();
                                documentsModels.UniqueNo = counts;
                                documentsModels.Extension = s.FileName != null ? s.FileName?.Split(".").Last() : "";
                                documentsModels.SessionId = s.SessionId;
                                documentsModels.DocumentID = s.DocumentId;
                                documentsModels.FileName = s.FileName != null ? (s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + name : s.FileName) : s.FileName;
                                documentsModels.ContentType = s.ContentType;
                                documentsModels.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                                documentsModels.FileSizes = s.FileSize > 0 ? FormatSize((long)s.FileSize) : "";
                                documentsModels.UploadDate = s.UploadDate;
                                documentsModels.SessionID = s.SessionId;
                                documentsModels.FileProfileTypeId = s.FilterProfileTypeId;
                                documentsModels.FilterProfileTypeId = s.FilterProfileTypeId;
                                documentsModels.FileProfileTypeName = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.Name;
                                documentsModels.ProfileID = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.ProfileId;
                                documentsModels.DocumentParentId = s.DocumentParentId;
                                documentsModels.TableName = s.TableName;
                                documentsModels.IsMobileUpload = s.IsMobileUpload;
                                documentsModels.Type = "Document";
                                documentsModels.ExpiryDate = s.ExpiryDate;
                                documentsModels.FileIndex = s.FileIndex;
                                documentsModels.TotalDocument = documentcount == 1 ? 1 : (documentcount + 1);
                                documentsModels.UploadedByUserId = s.AddedByUserId;
                                documentsModels.ModifiedByUserID = s.ModifiedByUserId;
                                documentsModels.AddedDate = s.UploadDate;
                                documentsModels.ModifiedDate = s.ModifiedDate;
                                documentsModels.AddedByUser = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserId)?.UserName;
                                documentsModels.AddedBy = appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserId)?.UserName;
                                documentsModels.ModifiedByUser = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserId)?.UserName;
                                documentsModels.ModifiedBy = appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserId)?.UserName;
                                documentsModels.IsLocked = s.IsLocked;
                                documentsModels.LockedByUserId = s.LockedByUserId;
                                documentsModels.LockedDate = s.LockedDate;
                                documentsModels.IsNewPath = s.IsNewPath;
                                documentsModels.AddedByUserID = s.AddedByUserId;
                                documentsModels.IsCompressed = s.IsCompressed;
                                documentsModels.LockedByUser = appUsers.FirstOrDefault(f => f.UserID == s.LockedByUserId)?.UserName;
                                documentsModels.isDocumentAccess = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.IsDocumentAccess;
                                documentsModels.IsEnableCreateTask = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.IsEnableCreateTask;
                                documentsModels.CloseDocumentId = s.CloseDocumentId;
                                documentsModels.CssClass = s.CloseDocumentId != null && s.CloseDocumentId == 2561 ? "blue-grey lighten - 3" : "transparent";
                                documentsModels.ProfileNo = s.ProfileNo;
                                documentsModels.FilePath = s.FilePath;
                                documentsModels.DeleteByDate = s.DeleteByDate;
                                documentsModels.DeleteByUserID = s.DeleteByUserID;
                                documentsModels.DeleteByUser = appUsers.FirstOrDefault(f => f.UserID == s.DeleteByUserID)?.UserName;
                                documentsModels.FileProfileTypeAddedByUserId = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.AddedByUserId;

                                documentsModel.Add(documentsModels);

                                counts++;
                            }
                        });
                    }
                    DocumentTypeModel.DocumentsData.AddRange(documentsModel.OrderByDescending(a => a.DocumentID).ToList());
                }
                return DocumentTypeModel;

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<IReadOnlyList<DocumentsModel>> GetFileContetTypes()
        {
            try
            {
                var query = "Select tt1.* from \r\n(select \r\nCONCAT('.',right(FilePath, charindex('.', reverse(FilePath) + '.') - 1)) as Extension \r\n" +
                    "from Documents where  filterprofiletypeid>0 AND islatest=1 AND (isDelete is null or isdelete=0) AND filepath is not null) tt1 group by Extension";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentsModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public DocumentRole GetDocumentRoleNameCheckValidation(string? value, long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DocumentRoleName", value);
                if (id > 0)
                {
                    parameters.Add("DocumentRoleId", id);

                    query = "SELECT * FROM DocumentRole Where DocumentRoleId!=@DocumentRoleId AND DocumentRoleName = @DocumentRoleName";
                }
                else
                {
                    query = "SELECT * FROM DocumentRole Where DocumentRoleName = @DocumentRoleName";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<DocumentRole>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public FileProfileTypeModel GeFileProfileTypeNameCheckValidation(string? value, long id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("Name", value);
                if (id > 0)
                {
                    parameters.Add("FileProfileTypeId", id);

                    query = "SELECT * FROM FileProfileType Where FileProfileTypeId!=@FileProfileTypeId AND Name = @Name";
                }
                else
                {
                    query = "SELECT * FROM FileProfileType Where Name = @Name";
                }
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<FileProfileTypeModel>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<DocumentPermission> GetDocumentPermissionData(long? id)
        {
            try
            {
                var parameters = new DynamicParameters();
                var query = string.Empty;
                parameters.Add("DocumentRoleId", id);

                query = "SELECT * FROM DocumentPermission Where DocumentRoleId=@DocumentRoleId";
                using (var connection = CreateConnection())
                {
                    return await connection.QuerySingleOrDefaultAsync<DocumentPermission>(query, parameters);
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        public async Task<IReadOnlyList<DocumentDmsShare>> GetDocumentDMSShareList(Guid? docSessionID)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DocSessionID", docSessionID);
                var query = "select t1.*,t2.FileName,\r\nt3.UserName as AddedBy,t4.UserName as ModifiedBy,t5.CodeValue as StatusCode\r\nfrom DocumentDmsShare t1 \r\n" +
                    "JOIN Documents t2 ON t2.DocumentID=t1.DocumentID\r\n" +
                    "LEFT JOIN ApplicationUser t3 ON t3.UserID=t1.AddedByUserID\r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.ModifiedByUserID\r\n" +
                    "LEFT JOIN CodeMaster t5 ON  t5.CodeID=t1.StatusCodeID where (t1.IsDeleted=0 OR t1.IsDeleted is Null) AND t1.DocSessionID=@docSessionID";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentDmsShare>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }


        public async Task<long?> DeleteFileProfileType(long? fileProfileTypeId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                        var parameters = new DynamicParameters();
                        parameters.Add("FileProfileTypeID", fileProfileTypeId);
                        parameters.Add("IsDelete", 1);
                        parameters.Add("DeleteByUserID", userData.UserID);
                        parameters.Add("DeleteByDate", DateTime.Now, DbType.DateTime);
                        var query = "Update FileProfileType SET IsDelete=@IsDelete,DeleteByDate=@DeleteByDate,DeleteByUserID=@DeleteByUserID WHERE FileProfileTypeID= @FileProfileTypeID";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return fileProfileTypeId;
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
        public async Task<DocumentsModel> GetFileProfileTypeDocumentDelete(DocumentsModel documentsModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentID", documentsModel.DocumentID);
                        parameters.Add("IsDelete", 1);
                        parameters.Add("DeleteByUserID", userData.UserID);
                        parameters.Add("DeleteByDate", DateTime.Now, DbType.DateTime);
                        var Addquerys = "UPDATE Documents SET IsDelete = @IsDelete,DeleteByUserID=@DeleteByUserID,DeleteByDate=@DeleteByDate WHERE  DocumentID = @DocumentID";
                        await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);


                        return documentsModel;
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
        public async Task<DocumentsModel> GetFileProfileTypeCheckOut(DocumentsModel documentsModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentID", documentsModel.DocumentID);
                        parameters.Add("LockedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("LockedByUserId", userData.UserID);
                        parameters.Add("IsLocked", 1, (DbType?)SqlDbType.Bit);
                        var Addquerys = "UPDATE Documents SET IsLocked = @IsLocked,LockedDate=@LockedDate,LockedByUserId=@LockedByUserId WHERE  DocumentID = @DocumentID";
                        await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);


                        return documentsModel;
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
        public async Task<DocumentLinkModel> DeleteDocumentLink(DocumentLinkModel documentLink)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentLinkId", documentLink.DocumentLinkId);

                        var query = "Delete from DocumentLink where DocumentLinkId=@DocumentLinkId";

                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);


                        return documentLink;
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
        public async Task<DocumentLink> InsertDocumentLink(DocumentLink documentLink)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var checkLink = await CheckDocumentLinkExits(documentLink);
                        if (checkLink == null)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentId", documentLink.DocumentId);
                            parameters.Add("AddedByUserId", documentLink.AddedByUserId);
                            parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("StatusCodeId", documentLink.StatusCodeId);
                            parameters.Add("LinkDocumentId", documentLink.LinkDocumentId);
                            parameters.Add("FolderId", documentLink.FolderId);
                            parameters.Add("FileProfieTypeId", documentLink.FileProfieTypeId);
                            parameters.Add("DocumentPath", documentLink.DocumentPath);
                            var query = "INSERT INTO [DocumentLink](DocumentId,AddedByUserId,AddedDate,StatusCodeId,LinkDocumentId,FolderId,FileProfieTypeId,DocumentPath) OUTPUT INSERTED.DocumentLinkId VALUES " +
                                "(@DocumentId,@AddedByUserId,@AddedDate,@StatusCodeId,@LinkDocumentId,@FolderId,@FileProfieTypeId,@DocumentPath)";

                            documentLink.DocumentLinkId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        return documentLink;
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
        public async Task<DocumentsModel> UpdateExpiryDateField(DocumentsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentId", value.DocumentID);
                        parameters.Add("FilterProfileTypeId", value.FilterProfileTypeId);
                        parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                        parameters.Add("ExpiryDate", value.ExpiryDate, DbType.DateTime);
                        var query = "Update Documents SET ExpiryDate=@ExpiryDate,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                            "DocumentId= @DocumentId AND FilterProfileTypeId=@FilterProfileTypeId";
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
        public async Task<DocumentsModel> UpdateDocumentRename(DocumentsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentId", value.DocumentID);
                        parameters.Add("FileName", value.FileName, DbType.String);
                        parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                        parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                        var query = "Update Documents SET FileName=@FileName,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                            "DocumentId= @DocumentId";
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
        public async Task<DocumentsModel> UpdateDescriptionField(DocumentsModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        if (value.Type == "Document")
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentId", value.DocumentID);
                            parameters.Add("FilterProfileTypeId", value.FilterProfileTypeId);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                            parameters.Add("Description", value.Description);

                            var query = "Update Documents SET Description=@Description,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                "DocumentId= @DocumentId AND FilterProfileTypeId=@FilterProfileTypeId";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                            var parametersLink = new DynamicParameters();
                            parametersLink.Add("DocumentId", value.DocumentID);
                            parametersLink.Add("Description", value.Description);
                            parametersLink.Add("TransactionSessionId", value.SessionId);
                            var querys = "Update LinkFileProfileTypeDocument SET Description=@Description WHERE " +
                                "DocumentId= @DocumentId AND TransactionSessionId=@TransactionSessionId";
                            await connection.QuerySingleOrDefaultAsync<long>(querys, parametersLink);
                        }
                        else
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("FileProfileTypeId", value.DocumentID);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                            parameters.Add("Description", value.Description);
                            var query = "Update Fileprofiletype SET Description=@Description,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                "FileProfileTypeId=@FileProfileTypeId";
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
        public async Task<FileProfileTypeModel> InsertOrUpdateFileProfileType(FileProfileTypeModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        value.SessionId ??= Guid.NewGuid();
                        var parameters = new DynamicParameters();

                        parameters.Add("Name", value.Name);
                        parameters.Add("ProfileId", value.ProfileId);
                        parameters.Add("ParentId", value.ParentId);
                        parameters.Add("StatusCodeID", value.StatusCodeID == null ? 1 : value.StatusCodeID);
                        parameters.Add("AddedDate", value.AddedDate);
                        parameters.Add("AddedByUserID", value.AddedByUserID);
                        parameters.Add("ModifiedDate", value.ModifiedDate);
                        parameters.Add("ModifiedByUserId", value.ModifiedByUserID);
                        parameters.Add("Description", value.Description);
                        parameters.Add("IsExpiryDate", value.IsExpiryDate);
                        parameters.Add("IsAllowMobileUpload", value.IsAllowMobileUpload);
                        parameters.Add("IsDocumentAccess", value.IsDocumentAccess);
                        parameters.Add("ShelfLifeDuration", value.ShelfLifeDuration);
                        parameters.Add("ShelfLifeDurationId", value.ShelfLifeDurationId);
                        parameters.Add("Hints", value.Hints);
                        parameters.Add("IsEnableCreateTask", value.IsEnableCreateTask);
                        parameters.Add("IsCreateByYear", value.IsCreateByYear);
                        parameters.Add("IsCreateByMonth", value.IsCreateByMonth);
                        parameters.Add("IsHidden", value.IsHidden);
                        parameters.Add("ProfileInfo", value.ProfileInfo);
                        parameters.Add("IsTemplateCaseNo", value.IsTemplateCaseNo);
                        parameters.Add("TemplateTestCaseId", value.TemplateTestCaseId);
                        parameters.Add("SessionId", value.SessionId, DbType.Guid);
                        parameters.Add("DynamicFormId", value.DynamicFormId);
                        parameters.Add("IsDuplicateUpload", value.IsDuplicateUpload);
                        if (value.FileProfileTypeId <= 0)
                        {

                            var query = "INSERT INTO [FileProfileType](Name,ProfileId,ParentId,StatusCodeID,AddedDate,AddedByUserID,ModifiedDate,ModifiedByUserId,Description,IsExpiryDate," +
                                "IsAllowMobileUpload,IsDocumentAccess,ShelfLifeDuration,ShelfLifeDurationId,Hints,IsEnableCreateTask,IsCreateByYear,IsCreateByMonth," +
                                "IsHidden,ProfileInfo,IsTemplateCaseNo,TemplateTestCaseId,SessionId,DynamicFormId,IsDuplicateUpload) " +
                                "OUTPUT INSERTED.FileProfileTypeId VALUES " +
                               "(@Name,@ProfileId,@ParentId,@StatusCodeID,@AddedDate,@AddedByUserID,@ModifiedDate,@ModifiedByUserId,@Description,@IsExpiryDate," +
                               "@IsAllowMobileUpload,@IsDocumentAccess,@ShelfLifeDuration,@ShelfLifeDurationId,@Hints,@IsEnableCreateTask,@IsCreateByYear,@IsCreateByMonth," +
                               "@IsHidden,@ProfileInfo,@IsTemplateCaseNo,@TemplateTestCaseId,@SessionId,@DynamicFormId,@IsDuplicateUpload)";
                            value.FileProfileTypeId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);

                        }
                        else
                        {
                            parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                            var query = "Update Fileprofiletype SET Name=@Name,ProfileId=@ProfileId,ParentId=@ParentId,StatusCodeID=@StatusCodeID,AddedDate=@AddedDate,IsDuplicateUpload=@IsDuplicateUpload," +
                                "AddedByUserID=@AddedByUserID,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId,Description=@Description,IsExpiryDate=@IsExpiryDate,IsAllowMobileUpload=@IsAllowMobileUpload,IsDocumentAccess=@IsDocumentAccess," +
                                "ShelfLifeDuration=@ShelfLifeDuration,ShelfLifeDurationId=@ShelfLifeDurationId,Hints=@Hints,IsEnableCreateTask=@IsEnableCreateTask,IsCreateByYear=@IsCreateByYear," +
                                "IsCreateByMonth=@IsCreateByMonth,IsHidden=@IsHidden,ProfileInfo=@ProfileInfo,IsTemplateCaseNo=@IsTemplateCaseNo,TemplateTestCaseId=@TemplateTestCaseId,SessionId=@SessionId,DynamicFormId=@DynamicFormId " +
                                "WHERE FileProfileTypeId=@FileProfileTypeId";
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
        public async Task<DocumentUserRoleModel> InsertFileProfileTypeAccess(DocumentUserRoleModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    ///value.FileProfileTypeIds = new List<long?>();
                    value.FileProfileTypeIds.Add(value.FileProfileTypeId);
                    //if(value.SelectFileProfileTypeId>0)
                    //{
                    //   value.FileProfileTypeIds.Add(value.SelectFileProfileTypeId);
                    // }
                    value.FileProfileTypeIds = value.FileProfileTypeIds.Distinct().ToList();
                    var userExitsRoles = await GetDocumentUserRoleByDocEmptyAsync(value.FileProfileTypeIds);
                    var userGroupUsers = await GetUserGroupUser();
                    var LevelUsers = await GetLeveMasterUsers(value.SelectLevelMasterIDs);

                    try
                    {
                        var query = string.Empty;
                        value.FileProfileTypeIds = value.FileProfileTypeIds.Distinct().ToList();
                        if (value.FileProfileTypeIds.Count() > 0)
                        {
                            value.FileProfileTypeIds.ForEach(f =>
                            {
                                if (value.Type == "User")
                                {
                                    if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                                    {
                                        foreach (var item in value.SelectUserIDs)
                                        {
                                            var counts = userExitsRoles.Where(w => w.UserId == item && w.FileProfileTypeId == f).Count();
                                            if (counts == 0)
                                            {
                                                query += "INSERT INTO [DocumentUserRole](FileProfileTypeId,UserId,RoleId) OUTPUT INSERTED.DocumentUserRoleId " +
                                                    "VALUES (" + f + "," + item + "," + value.RoleID + ");";
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
                                                var counts = userExitsRoles.Where(w => w.UserId == s.UserId && w.FileProfileTypeId == f).Count();
                                                if (counts == 0)
                                                {
                                                    query += "INSERT INTO [DocumentUserRole](FileProfileTypeId,UserId,RoleId,UserGroupId) OUTPUT INSERTED.DocumentUserRoleId " +
                                                    "VALUES (" + f + "," + s.UserId + "," + value.UserGroupRoleID + "," + s.UserGroupId + ");";
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
                                            var counts = userExitsRoles.Where(w => w.UserId == s.UserId && w.FileProfileTypeId == f).Count();
                                            if (counts == 0)
                                            {
                                                query += "INSERT INTO [DocumentUserRole](FileProfileTypeId,UserId,RoleId,LevelId) OUTPUT INSERTED.DocumentUserRoleId " +
                                                "VALUES (" + f + "," + s.UserId + "," + value.LevelRoleID + "," + s.LevelId + ");";
                                            }
                                        });
                                    }
                                }
                            });
                            if (!string.IsNullOrEmpty(query))
                            {
                                await connection.QuerySingleOrDefaultAsync<long>(query, null);
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
        public async Task<DocumentUserRoleModel> DeleteDocumentUserRole(DocumentUserRoleModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentUserRoleID", value.DocumentUserRoleID);
                        var query = "DELETE FROM DocumentUserRole WHERE " +
                            "DocumentUserRoleID= @DocumentUserRoleID";
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
        public async Task<DocumentsModel> ReStoreFileProfileTypeAndDocument(DocumentsModel documentsModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentID", documentsModel.DocumentID);
                        parameters.Add("FileProfileTypeId", documentsModel.DocumentID);
                        parameters.Add("IsDelete", null);
                        parameters.Add("DeleteByUserID", null);
                        parameters.Add("DeleteByDate", null);
                        parameters.Add("RestoreByUserID", userData.UserID);
                        parameters.Add("RestoreByDate", DateTime.Now, DbType.DateTime);
                        if (documentsModel.Type == "Document")
                        {
                            var Addquerys = "UPDATE Documents SET IsDelete = @IsDelete,DeleteByUserID=@DeleteByUserID,DeleteByDate=@DeleteByDate,RestoreByUserID=@RestoreByUserID,RestoreByDate=@RestoreByDate WHERE  DocumentID = @DocumentID";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);
                        }
                        else
                        {
                            var Addquerys = "UPDATE FileProfileType SET IsDelete = @IsDelete,DeleteByUserID=@DeleteByUserID,DeleteByDate=@DeleteByDate,RestoreByUserID=@RestoreByUserID,RestoreByDate=@RestoreByDate WHERE  FileProfileTypeId = @FileProfileTypeId";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);
                        }

                        return documentsModel;
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
        public async Task<DocumentUserRoleModel> UpdateDocumentUserRole(DocumentUserRoleModel documentUserRoleModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentUserRoleID", documentUserRoleModel.DocumentUserRoleID);
                        parameters.Add("RoleID", documentUserRoleModel.RoleID);
                        var Addquerys = "UPDATE DocumentUserRole SET RoleID = @RoleID WHERE  DocumentUserRoleID = @DocumentUserRoleID";
                        await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);

                        return documentUserRoleModel;
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
        public async Task<DocumentRole> InsertOrUpdateDocumentRole(DocumentRole documentRole)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentRoleId", documentRole.DocumentRoleId);
                        parameters.Add("DocumentRoleName", documentRole.DocumentRoleName, DbType.String);
                        parameters.Add("DocumentRoleDescription", documentRole.DocumentRoleDescription, DbType.String);
                        parameters.Add("AddedByUserID", documentRole.AddedByUserId);
                        parameters.Add("ModifiedByUserID", documentRole.ModifiedByUserId);
                        parameters.Add("AddedDate", documentRole.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", documentRole.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", documentRole.StatusCodeId);
                        if (documentRole.DocumentRoleId > 0)
                        {
                            var query = " UPDATE DocumentRole SET DocumentRoleName = @DocumentRoleName,DocumentRoleDescription =@DocumentRoleDescription,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID\n\r" +
                                "WHERE DocumentRoleId = @DocumentRoleId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO DocumentRole(DocumentRoleName,DocumentRoleDescription,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID)  " +
                                "OUTPUT INSERTED.DocumentRoleId VALUES " +
                                "(@DocumentRoleName,@DocumentRoleDescription,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID)";

                            documentRole.DocumentRoleId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }

                        return documentRole;
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
        public async Task<DocumentRole> DeleteDocumentRole(DocumentRole value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentRoleId", value.DocumentRoleId);
                        var query = "DELETE FROM DocumentRole WHERE " +
                            "DocumentRoleId= @DocumentRoleId";
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
        public async Task<DocumentPermission> InsertOrUpdateDocumentPermission(DocumentPermission documentPermission)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentPermissionId", documentPermission.DocumentPermissionId);
                        parameters.Add("DocumentRoleId", documentPermission.DocumentRoleId);
                        parameters.Add("AddedByUserID", documentPermission.AddedByUserId);
                        parameters.Add("ModifiedByUserID", documentPermission.ModifiedByUserId);
                        parameters.Add("AddedDate", documentPermission.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", documentPermission.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", documentPermission.StatusCodeId);

                        parameters.Add("IsRead", documentPermission.IsRead);
                        parameters.Add("IsCreateFolder", documentPermission.IsCreateFolder);
                        parameters.Add("IsCreateDocument", documentPermission.IsCreateDocument);
                        parameters.Add("IsSetAlert", documentPermission.IsSetAlert);
                        parameters.Add("IsEditIndex", documentPermission.IsEditIndex);
                        parameters.Add("IsRename", documentPermission.IsRename);
                        parameters.Add("IsUpdateDocument", documentPermission.IsUpdateDocument);
                        parameters.Add("IsCopy", documentPermission.IsCopy);
                        parameters.Add("IsMove", documentPermission.IsMove);
                        parameters.Add("IsDelete", documentPermission.IsDelete);
                        parameters.Add("IsRelationship", documentPermission.IsRelationship);
                        parameters.Add("IsListVersion", documentPermission.IsListVersion);
                        parameters.Add("IsInvitation", documentPermission.IsInvitation);
                        parameters.Add("IsSendEmail", documentPermission.IsSendEmail);
                        parameters.Add("IsDiscussion", documentPermission.IsDiscussion);
                        parameters.Add("IsAccessControl", documentPermission.IsAccessControl);
                        parameters.Add("IsAuditTrail", documentPermission.IsAuditTrail);
                        parameters.Add("IsRequired", documentPermission.IsRequired);
                        parameters.Add("IsFileDelete", documentPermission.IsFileDelete);
                        parameters.Add("IsEdit", documentPermission.IsEdit);
                        parameters.Add("IsGrantAdminPermission", documentPermission.IsGrantAdminPermission);
                        parameters.Add("IsDocumentAccess", documentPermission.IsDocumentAccess);
                        parameters.Add("IsCreateTask", documentPermission.IsCreateTask);
                        parameters.Add("IsEnableProfileTypeInfo", documentPermission.IsEnableProfileTypeInfo);
                        parameters.Add("IsShare", documentPermission.IsShare);
                        parameters.Add("IsCloseDocument", documentPermission.IsCloseDocument);
                        parameters.Add("IsEditFolder", documentPermission.IsEditFolder);
                        parameters.Add("IsDeleteFolder", documentPermission.IsDeleteFolder);
                        parameters.Add("IsReserveProfileNumber", documentPermission.IsReserveProfileNumber);

                        if (documentPermission.DocumentPermissionId > 0)
                        {
                            var query = " UPDATE DocumentPermission SET DocumentRoleId=@DocumentRoleId,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,\n\r" +
                                "IsRead=@IsRead,IsCreateFolder=@IsCreateFolder,IsCreateDocument=@IsCreateDocument,\r\n" +
                                "IsSetAlert=@IsSetAlert,IsEditIndex=@IsEditIndex,\r\n" +
                                "IsRename=@IsRename,IsUpdateDocument=@IsUpdateDocument,IsCopy=@IsCopy,IsMove=@IsMove,IsDelete=@IsDelete,IsRelationship=@IsRelationship,\r\n" +
                                "IsListVersion=@IsListVersion,IsInvitation=@IsInvitation,IsSendEmail=@IsSendEmail,IsDiscussion=@IsDiscussion,IsAccessControl=@IsAccessControl,\r\n" +
                                "IsAuditTrail=@IsAuditTrail,IsRequired=@IsRequired,IsFileDelete=@IsFileDelete,\r\n" +
                                "IsEdit=@IsEdit,IsGrantAdminPermission=@IsGrantAdminPermission,IsDocumentAccess=@IsDocumentAccess,IsCreateTask=@IsCreateTask,\r\n" +
                                "IsEnableProfileTypeInfo=@IsEnableProfileTypeInfo,IsShare=@IsShare,\r\n" +
                                "IsCloseDocument=@IsCloseDocument,IsEditFolder=@IsEditFolder,\r\n" +
                                "IsDeleteFolder=@IsDeleteFolder,IsReserveProfileNumber=@IsReserveProfileNumber\r\n" +
                                "WHERE DocumentPermissionId = @DocumentPermissionId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO DocumentPermission(DocumentRoleId,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsRead,\r\n" +
                                "IsCreateFolder,\r\nIsCreateDocument,\r\nIsSetAlert,\r\n IsEditIndex,\r\n" +
                                "IsRename,\r\n IsUpdateDocument,\r\n" + "IsCopy,\r\nIsMove,\r\n" +
                                "IsDelete,\r\nIsRelationship,\r\n IsListVersion,\r\n IsInvitation,\r\nIsSendEmail,\r\nIsDiscussion,\r\nIsAccessControl,\r\n IsAuditTrail,\r\n" +
                                "IsRequired,\r\nIsFileDelete,\r\nIsEdit,\r\n" +
                                " IsGrantAdminPermission,\r\nIsDocumentAccess,\r\nIsCreateTask,\r\nIsEnableProfileTypeInfo,\r\nIsShare,\r\nIsCloseDocument,\r\nIsEditFolder,\r\n" +
                                "IsDeleteFolder,\r\nIsReserveProfileNumber)\r\n" +
                                "OUTPUT INSERTED.DocumentPermissionId VALUES\r\n" +
                                "(@DocumentRoleId,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsRead,\r\n@IsCreateFolder,\r\n@IsCreateDocument,\r\n" +
                                "@IsSetAlert,\r\n@IsEditIndex,\r\n@IsRename,\r\n@IsUpdateDocument,\r\n@IsCopy,\r\n@IsMove,\r\n@IsDelete,\r\n@IsRelationship,\r\n" +
                                "@IsListVersion,\r\n@IsInvitation,\r\n@IsSendEmail,\r\n@IsDiscussion,\r\n@IsAccessControl,\r\n@IsAuditTrail,\r\n@IsRequired,\r\n" +
                                "@IsFileDelete,\r\n@IsEdit,\r\n@IsGrantAdminPermission,\r\n@IsDocumentAccess,\r\n@IsCreateTask,\r\n" +
                                "@IsEnableProfileTypeInfo,\r\n@IsShare,\r\n@IsCloseDocument,\r\n@IsEditFolder,\r\n@IsDeleteFolder,@IsReserveProfileNumber)";

                            documentPermission.DocumentPermissionId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }

                        return documentPermission;
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
        public async Task<long?> DeleteDocumentPermissions(long? Id)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentRoleId", Id);
                        var query = "DELETE FROM DocumentPermission WHERE " +
                            "DocumentRoleId= @DocumentRoleId";
                        await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        return Id;
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
        public async Task<DocumentDmsShare> InsertOrUpdateDocumentDmsShare(DocumentDmsShare documentDmsShare)
        {
            try
            {
                using (var connection = CreateConnection())
                {



                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentDmsShareId", documentDmsShare.DocumentDmsShareId);
                        parameters.Add("DocSessionId", documentDmsShare.DocSessionId, DbType.Guid);
                        parameters.Add("SessionId", documentDmsShare.SessionId, DbType.Guid);
                        parameters.Add("DocumentId", documentDmsShare.DocumentId);
                        parameters.Add("IsExpiry", documentDmsShare.IsExpiry);
                        parameters.Add("ExpiryDate", documentDmsShare.ExpiryDate, DbType.DateTime);
                        parameters.Add("AddedByUserID", documentDmsShare.AddedByUserID);
                        parameters.Add("ModifiedByUserID", documentDmsShare.ModifiedByUserID);
                        parameters.Add("AddedDate", documentDmsShare.AddedDate, DbType.DateTime);
                        parameters.Add("ModifiedDate", documentDmsShare.ModifiedDate, DbType.DateTime);
                        parameters.Add("StatusCodeID", documentDmsShare.StatusCodeID);
                        parameters.Add("IsDeleted", documentDmsShare.IsDeleted);
                        parameters.Add("Description", documentDmsShare.Description, DbType.String);
                        if (documentDmsShare.DocumentDmsShareId > 0)
                        {
                            var query = " UPDATE DocumentDmsShare SET DocSessionId = @DocSessionId,SessionId =@SessionId,DocumentId=@DocumentId,IsExpiry=@IsExpiry,ExpiryDate=@ExpiryDate,\n\r" +
                                "ModifiedByUserID=@ModifiedByUserID,ModifiedDate=@ModifiedDate,StatusCodeID=@StatusCodeID,IsDeleted=@IsDeleted,Description=@Description\n\r" +
                                "WHERE DocumentDmsShareId = @DocumentDmsShareId";
                            await connection.ExecuteAsync(query, parameters);

                        }
                        else
                        {
                            var query = "INSERT INTO DocumentDmsShare(DocSessionId,SessionId,DocumentId,IsExpiry,ExpiryDate,AddedByUserID,ModifiedByUserID,AddedDate,ModifiedDate,StatusCodeID,IsDeleted,Description)  " +
                                "OUTPUT INSERTED.DocumentDmsShareId VALUES " +
                                "(@DocSessionId,@SessionId,@DocumentId,@IsExpiry,@ExpiryDate,@AddedByUserID,@ModifiedByUserID,@AddedDate,@ModifiedDate,@StatusCodeID,@IsDeleted,@Description)";

                            documentDmsShare.DocumentDmsShareId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }

                        return documentDmsShare;
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
        public async Task<DocumentDmsShare> DeleteDocumentDmsShare(DocumentDmsShare value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("DocumentDmsShareId", value.DocumentDmsShareId);
                        parameters.Add("IsDeleted", 1);
                        var query = "Update DocumentDmsShare SET IsDeleted=@IsDeleted WHERE DocumentDmsShareId= @DocumentDmsShareId";
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
        public async Task<FileProfileTypeModel> UpdateProfileTypeInfo(FileProfileTypeModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("ProfileTypeInfo", value.ProfileTypeInfo);
                        parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                        var query = "Update FileProfileType SET ProfileTypeInfo=@ProfileTypeInfo WHERE FileProfileTypeId= @FileProfileTypeId";
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
        public async Task<long?> MoveToFileProfileTypeUpdateInfo(List<DocumentsModel> value, long? FileprofileTypeId)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var query = string.Empty;
                        var parameters = new DynamicParameters();
                        parameters.Add("FilterProfileTypeId", FileprofileTypeId);
                        if (FileprofileTypeId > 0 && value != null && value.Count() > 0)
                        {
                            var docs = value.Where(s => s.Type == "Document").ToList();
                            if (docs.Count() > 0)
                            {
                                docs.ForEach(s =>
                                {
                                    if (s.SessionID != null && s.FilterProfileTypeId > 0 && s.Type == "Document")
                                    {
                                        query += "Update Documents SET FilterProfileTypeId=@FilterProfileTypeId WHERE FilterProfileTypeId=" + s.FilterProfileTypeId + " AND  SessionID='" + s.SessionID + "';";
                                    }
                                });
                            }
                            var folders = value.Where(s => s.Type == null && s.FilterProfileTypeId == null).ToList();
                            if (folders.Count() > 0)
                            {
                                folders.ForEach(a =>
                                {
                                    query += "Update FileProfileType SET ParentId=@FilterProfileTypeId WHERE FileProfileTypeId=" + a.FileProfileTypeId + ";";
                                });
                            }
                        }
                        if (!string.IsNullOrEmpty(query))
                        {
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters);
                        }
                        return FileprofileTypeId;
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
        public async Task<DocumentsUploadModel> GetDocumentDeleteForNoProfileNo(DocumentsUploadModel documentsModel)
        {
            try
            {
                using (var connection = CreateConnection())
                {

                    try
                    {
                        var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                        var parameters = new DynamicParameters();
                        parameters.Add("IsDelete", 1);
                        parameters.Add("DeleteByUserID", userData.UserID);
                        parameters.Add("DeleteByDate", DateTime.Now, DbType.DateTime);
                        var Addquerys = "UPDATE Documents SET IsDelete = @IsDelete,DeleteByUserID=@DeleteByUserID,DeleteByDate=@DeleteByDate WHERE  DocumentID in(" + string.Join(',', documentsModel.DocumentIds.Select(s => s.DocumentID).ToList()) + ")";
                        await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters);
                        return documentsModel;
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
       
        public async Task<DocumentsModel>GetAllFileProfileDocumentSessionId(long FileProfileTypeID)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("FileProfileTypeID", FileProfileTypeID);
                var query = "select SessionId,Name,Description,ProfileID,FileProfileTypeID From FileProfileType where FileProfileTypeID = @FileProfileTypeID";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryFirstOrDefaultAsync<DocumentsModel>(query, parameters));

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);

            }
        }

        public async Task<IReadOnlyList<FileProfileTypeModel>> GetAllFileProfileDropdownAsync()
        {
            try
            {
                var query = "Select * From FileProfileType where ParentID Is Null";
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<FileProfileTypeModel>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
    }
}
