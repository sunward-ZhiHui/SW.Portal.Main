using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using DevExpress.Internal;
using Infrastructure.Repository.Query.Base;
using Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Xpo.DB.DataStoreLongrunnersWatch;

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
                var query = "select ROW_NUMBER() OVER(ORDER BY name) AS UniqueNo,*,Name as Filename,\r\n" +
                    "FileProfileTypeID as DocumentID,\r\n" +
                    "AddedBy as AddedByUser,\r\n" +
                    "ModifiedBy as ModifiedByUser,\r\n" +
                    "ModifiedDate,\r\n" +
                    "AddedDate,\r\n" +
                    "Profile as ProfileNo,\r\n" +
                    //"CASE WHEN ModifiedByUserID >0 THEN ModifiedBy ELSE AddedBy END AS AddedByUser,\r\n" +
                    //"CASE WHEN ModifiedByUserID >0 THEN ModifiedDate ELSE AddedDate END AS AddedDate,\r\n" +
                    "CONCAT((select count(*) as counts from FileProfileType tt where tt.parentId=t2.FileProfileTypeID),' ','items') as FileSizes,\r\n" +
                    "CONCAT((Select COUNT(*) as DocCount from Documents where FilterProfileTypeId=t2.FileProfileTypeID\r\n" +
                    "AND IsLatest=1  \r\n" +
                    "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) \r\n" +
                    "OR (DocumentID in(select DocumentID from LinkFileProfileTypeDocument where FileProfileTypeID=t2.FileProfileTypeID ) AND IsLatest=1)),' ','files') as FileCounts\r\n" +
                    "from view_FileProfileTypeDocument t2";
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
                var query = "select  * from FileProfileType where sessionId=@SessionId";

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
        public async Task<IReadOnlyList<Fileprofiletype>> GetAllAsync(long fileProfileTypeID)
        {
            List<Fileprofiletype> fileprofiletype = new List<Fileprofiletype>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", fileProfileTypeID);

                var query = @"WITH RecursiveHierarchy AS(
                                SELECT
                                    FileProfileTypeID,
                                    ParentID,
                                    Name,
                                    FileProfileTypeID AS RootID

                                FROM
                                    FileProfileType
                                WHERE
                                    ParentID IS NULL

                                UNION ALL

                                SELECT
                                    t.FileProfileTypeID,
                                    t.ParentID,
                                    t.Name,
                                    rh.RootID

                                FROM
                                    FileProfileType t
                                JOIN
                                    RecursiveHierarchy rh ON t.ParentID = rh.FileProfileTypeID
                            )
                            SELECT
                                FileProfileTypeID,
                                ParentID,
                                Name,
                                RootID


                            FROM
                                RecursiveHierarchy
                            WHERE
                                RootID = @FileProfileTypeId";


                using (var connection = CreateConnection())
                {
                    var result = (await connection.QueryAsync<Fileprofiletype>(query, parameters)).ToList();
                    result.ForEach(s =>
                    {
                        if (!s.ParentId.HasValue)
                        {
                            Fileprofiletype applicationChildDataResponse = new Fileprofiletype
                            {
                                FileProfileTypeId = s.FileProfileTypeId,
                                ProfileId = s.ProfileId,
                                ParentId = s.ParentId,
                                Name = s.Name,
                                Label = s.Name,
                            };
                            fileprofiletype.Add(applicationChildDataResponse);
                        }
                        else
                        {
                            var applicationChild = fileprofiletype.FirstOrDefault(a => a.FileProfileTypeId == s.ParentId);
                            if (applicationChild != null)
                            {
                                applicationChild.Children.Add(new Fileprofiletype
                                {
                                    FileProfileTypeId = s.FileProfileTypeId,
                                    ProfileId = s.ProfileId,
                                    ParentId = s.ParentId,
                                    Name = s.Name,
                                    Label = s.Name,
                                });
                            }
                            else
                            {
                                fileprofiletype.ToList().ForEach(applicationChildModel =>
                                {
                                    AddChildLevelData(applicationChildModel, s);
                                });
                            }
                        }
                    });
                }


                return fileprofiletype;
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }

        private void AddChildLevelData(Fileprofiletype applicationChildModel, Fileprofiletype childData)
        {
            applicationChildModel.Children.ToList().ForEach(parent =>
            {
                if (parent.FileProfileTypeId == childData.ParentId)
                {
                    parent.Children.Add(new Fileprofiletype
                    {
                        FileProfileTypeId = childData.FileProfileTypeId,
                        ProfileId = childData.ProfileId,
                        ParentId = childData.ParentId,
                        Name = childData.Name,
                        Label = childData.Name,
                    });
                }
                else
                {
                    AddChildLevelData(parent, childData);
                }
            });
        }
        public async Task<IReadOnlyList<DocumentsModel>> GetAllFileProfileDocumentIdAsync(long? selectedFileProfileTypeID)
        {
            try
            {
                var query = "select  *,ROW_NUMBER() OVER(ORDER BY name) AS UniqueNo,Name as Filename,\r\nFileProfileTypeID as DocumentID,\r\nProfile as ProfileNo,\r\nCASE WHEN ModifiedByUserID >0 THEN ModifiedBy ELSE AddedBy END AS AddedByUser,\r\nCASE WHEN ModifiedByUserID >0 THEN ModifiedDate ELSE AddedDate END AS AddedDate,\r\nCONCAT((select count(*) as counts from FileProfileType tt where tt.parentId=t2.FileProfileTypeID),' ','items') as FileSizes,\r\nCONCAT((Select COUNT(*) as DocCount from Documents where FilterProfileTypeId=t2.FileProfileTypeID\r\nAND IsLatest=1  \r\nAND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) \r\nOR (DocumentID in(select DocumentID from LinkFileProfileTypeDocument where FileProfileTypeID=t2.FileProfileTypeID ) AND IsLatest=1)),' ','files') as FileCounts\r\nfrom view_FileProfileTypeDocument t2";
                if (selectedFileProfileTypeID == null)
                {
                    query += "\r\nwhere parentid is null";
                }
                else
                {
                    query += "\r\nwhere parentid=" + selectedFileProfileTypeID;
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
        public async Task<IReadOnlyList<Fileprofiletype>> GetFileprofiletypeAsync()
        {
            try
            {
                var query = "select FileProfileTypeId,Name,IsDocumentAccess,IsEnableCreateTask,IsExpiryDate,IsHidden,AddedByUserId,profileId from FileProfileType";

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
        public async Task<IReadOnlyList<LinkFileProfileTypeDocument>> GetLinkFileProfileTypeDocumentAsync(long? fileProfileTypeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", fileProfileTypeId);

                var query = "select  * from LinkFileProfileTypeDocument where FileProfileTypeId=@FileProfileTypeId";

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
        public async Task<IReadOnlyList<CloseDocumentPermission>> GetCloseDocumentPermissionAsync(long? fileProfileTypeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", fileProfileTypeId);

                var query = "select  * from CloseDocumentPermission where FileProfileTypeId=@FileProfileTypeId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<CloseDocumentPermission>(query, parameters)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<DocumentUserRole>> GetDocumentUserRoleAsync(long? fileProfileTypeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", fileProfileTypeId);
                var query = "select  * from DocumentUserRole where FileProfileTypeId=@FileProfileTypeId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentUserRole>(query, parameters)).ToList();
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
                    "IsNewPath " +
                    "from Documents ";
            return query;
        }
        public async Task<IReadOnlyList<Notes>> GeNotesAsync(List<long> documentIds)
        {
            try
            {
                documentIds = documentIds != null && documentIds.Count > 0 ? documentIds : new List<long>() { -1 };

                var query = "select  NotesId,DocumentId from Notes where DocumentId in(" + string.Join(',', documentIds) + ")";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<Notes>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<IReadOnlyList<TaskMaster>> GetTaskMasterAsync(List<long> documentIds)
        {
            try
            {
                documentIds = documentIds != null && documentIds.Count > 0 ? documentIds : new List<long>() { -1 };
                var query = "select  TaskId,SourceId from TaskMaster where SourceId in(" + string.Join(',', documentIds) + ")";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<TaskMaster>(query)).ToList();
                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
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
                var query = DocumentQueryString() + " where FilterProfileTypeId=@FilterProfileTypeId AND SessionID=@SessionID AND DocumentId=@DocumentId AND IsLatest=0";

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
                var query = DocumentQueryString() + " where FilterProfileTypeId=@FilterProfileTypeId AND SessionID=@SessionID AND DocumentParentId=@DocumentParentId AND IsLatest=0";

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
                var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                var appUsers = await GetApplicationUserAsync();
                var fileProfileType = await GetFileprofiletypeAsync();
                var roleItemsList = await GetDocumentUserRoleAsync(searchModel.MasterTypeID.Value);
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
        public async Task<DocumentsModel> GetFileProfileTypeDelete(DocumentsModel documentsModel)
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
                            parameters.Add("DocumentID", documentsModel.DocumentID);
                            parameters.Add("IsLatest", 1, (DbType?)SqlDbType.Bit);
                            var Addquerys = "UPDATE Documents SET IsLatest = @IsLatest WHERE  DocumentID = @DocumentID";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters, transaction);

                            transaction.Commit();

                            return documentsModel;
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
        public async Task<DocumentsModel> GetFileProfileTypeCheckOut(DocumentsModel documentsModel)
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
                            var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentID", documentsModel.DocumentID);
                            parameters.Add("LockedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("LockedByUserId", userData.UserID);
                            parameters.Add("IsLocked", 1, (DbType?)SqlDbType.Bit);
                            var Addquerys = "UPDATE Documents SET IsLocked = @IsLocked,LockedDate=@LockedDate,LockedByUserId=@LockedByUserId WHERE  DocumentID = @DocumentID";
                            await connection.QuerySingleOrDefaultAsync<long>(Addquerys, parameters, transaction);

                            transaction.Commit();

                            return documentsModel;
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
        public async Task<DocumentTypeModel> GetAllSelectedFileAsync(long? selectedFileProfileTypeID)
        {
            DocumentTypeModel DocumentTypeModel = new DocumentTypeModel();
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
            {
                var docs = await GetAllFileProfileDocumentIdAsync(selectedFileProfileTypeID);
                var counts = docs != null ? (docs.Count + 1) : 1;
                DocumentTypeModel.DocumentsData.AddRange(docs);
                if (selectedFileProfileTypeID > 0)
                {
                    var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                    var appUsers = await GetApplicationUserAsync();
                    var fileProfileType = await GetFileprofiletypeAsync();
                    var linkfileProfileTypes = await GetLinkFileProfileTypeDocumentAsync(selectedFileProfileTypeID);
                    var roleItemsList = await GetDocumentUserRoleAsync(selectedFileProfileTypeID);
                    var closedocumentPermission = await GetCloseDocumentPermissionAsync(selectedFileProfileTypeID);
                    var linkfileProfileTypeDocumentids = linkfileProfileTypes != null && linkfileProfileTypes.Count > 0 ? linkfileProfileTypes.Select(s => s.DocumentId).Distinct().ToList() : new List<long?>() { -1 };
                    var parameters = new DynamicParameters();
                    var documentPermission = await GetDocumentPermissionByRoll();
                    parameters.Add("FileProfileTypeId", selectedFileProfileTypeID, DbType.Int64);
                    var query = DocumentQueryString() + " where FilterProfileTypeId=@FileProfileTypeId " +
                        "AND IsLatest=1 " +
                        "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) " +
                        "OR (DocumentID in(" + string.Join(",", linkfileProfileTypeDocumentids) + ") AND IsLatest=1) " +
                        "order by DocumentId desc";

                    using (var connection = CreateConnection())
                    {
                        var documents = (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                        if (documents != null && documents.Count > 0)
                        {
                            var docIds = documents.Select(a => a.DocumentId).ToList();
                            var notes = await GeNotesAsync(docIds);
                            var taskMasternotes = await GetTaskMasterAsync(docIds);
                            var setAccess = roleItemsList;
                            documents.ForEach(s =>
                            {
                                var documentcount = documents?.Where(w => w.DocumentParentId == s.DocumentParentId).Count();
                                var lastIndex = s.FileName != null ? s.FileName.LastIndexOf(".") : 0;
                                lastIndex = lastIndex > 0 ? lastIndex : 0;
                                var name = s.FileName != null ? s.FileName?.Substring(lastIndex) : "";
                                var fileName = s.FileName?.Split(name);
                                DocumentsModel documentsModels = new DocumentsModel();
                                documentsModels.UniqueNo = counts;
                                var setAccessFlag = roleItemsList.Where(a => a.UserId == userData.UserID && a.DocumentId == s.DocumentId).Count();
                                documentsModels.NotesCount = notes.Where(a => a.DocumentId == s.DocumentId).Count();
                                documentsModels.NotesColor = "";
                                var taskNotesCount = taskMasternotes.Where(a => a.SourceId == s.DocumentId).Count();
                                if (taskNotesCount > 0)
                                {
                                    documentsModels.NotesColor = "green";
                                }
                                else
                                {
                                    if (documentsModels.NotesCount > 0)
                                    {
                                        documentsModels.NotesColor = "blue";
                                    }
                                }
                                documentsModels.Extension = s.FileName != null ? s.FileName?.Split(".").Last() : "";
                                documentsModels.SetAccessFlag = setAccessFlag > 0 ? true : false;
                                documentsModels.SessionId = s.SessionId;
                                documentsModels.DocumentID = s.DocumentId;
                                documentsModels.FileName = s.FileName != null ? (s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + name : s.FileName) : s.FileName;
                                documentsModels.ContentType = s.ContentType;
                                documentsModels.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                                documentsModels.FileSizes = s.FileSize > 0 ? FormatSize((long)s.FileSize) : "";
                                documentsModels.UploadDate = s.UploadDate;
                                documentsModels.SessionID = s.SessionId;
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
                                //documentsModels.AddedByUser = s.ModifiedByUserId == null ? appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserId)?.UserName : appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserId)?.UserName;
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
                                documentsModels.FileProfileTypeAddedByUserId = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.AddedByUserId;
                                if (documentsModels.isDocumentAccess == true && (documentsModels.IsEnableCreateTask == false || documentsModels.IsEnableCreateTask == null))
                                {
                                    documentsModels.ItemsAllFlag = true;
                                }
                                if (documentsModels.isDocumentAccess != true && (documentsModels.IsEnableCreateTask == false || documentsModels.IsEnableCreateTask == null))
                                {
                                    documentsModels.ItemsFlag = true;
                                }
                                if (documentsModels.isDocumentAccess != true && documentsModels.IsEnableCreateTask == true)
                                {
                                    documentsModels.ItemsWithCreateTask = true;
                                }
                                if (documentsModels.isDocumentAccess == true && documentsModels.IsEnableCreateTask == true)
                                {
                                    documentsModels.ItemsAllWithCreateTask = true;
                                }
                                if (setAccess.Count > 0)
                                {
                                    var roleDocItem = setAccess.FirstOrDefault(u => u.DocumentId == s.DocumentId);
                                    if (roleDocItem != null)
                                    {
                                        var roleItem = setAccess.FirstOrDefault(u => u.UserId == userData.UserID && u.DocumentId == s.DocumentId);
                                        if (roleItem != null)
                                        {
                                            var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)roleItem.RoleId).FirstOrDefault();
                                            documentsModels.DocumentPermissionData = permissionData;
                                        }
                                        else
                                        {
                                            documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = true, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };
                                        }
                                    }
                                    else
                                    {
                                        var filprofilepermission = setAccess.FirstOrDefault(u => u.FileProfileTypeId == s.FilterProfileTypeId && u.DocumentId == null && u.UserId == userData.UserID);
                                        if (filprofilepermission != null)
                                        {
                                            var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)filprofilepermission.RoleId).FirstOrDefault();
                                            documentsModels.DocumentPermissionData = permissionData;
                                        }
                                        else
                                        {
                                            documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = true, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = true, IsCopy = true, IsCreateFolder = true, IsEdit = true, IsMove = true, IsShare = true, IsFileDelete = true };
                                        }
                                    }
                                }
                                else
                                {
                                    documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = false };
                                }
                                documentsModels.IsExpiryDate = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.IsExpiryDate;
                                var description = linkfileProfileTypes?.Where(f => f.FileProfileTypeId == selectedFileProfileTypeID && f.TransactionSessionId == s.SessionId && f.DocumentId == s.DocumentId).FirstOrDefault()?.Description;
                                if (description != null)
                                {
                                    documentsModels.Description = description;
                                }
                                else
                                {
                                    documentsModels.Description = s.Description;
                                }
                                if (documentsModels.DocumentPermissionData != null)
                                {
                                    if (documentsModels.DocumentPermissionData.IsRead == true)
                                    {
                                        documentsModel.Add(documentsModels);
                                    }
                                    else if (documentsModels.DocumentPermissionData.IsRead == false)
                                    {

                                    }
                                }
                                else
                                {
                                    documentsModel.Add(documentsModels);
                                }
                                counts++;
                            });
                        }
                        DocumentTypeModel.DocumentsData.AddRange(documentsModel.OrderByDescending(a => a.DocumentID).ToList());
                        var roleItems = roleItemsList.Where(w => w.FileProfileTypeId == selectedFileProfileTypeID).ToList();
                        if (roleItems.Count > 0)
                        {
                            var roleItem = roleItems.FirstOrDefault(u => u.UserId == userData.UserID);
                            if (roleItem != null)
                            {
                                DocumentTypeModel.DocumentPermissionData = documentPermission.Where(z => z.DocumentRoleID == (int)roleItem.RoleId).FirstOrDefault();
                                if (closedocumentPermission.Count > 0)
                                {
                                    var userpermission = closedocumentPermission.FirstOrDefault(f => f.UserId == userData.UserID);
                                    if (userpermission != null)
                                    {
                                        DocumentTypeModel.DocumentPermissionData.IsCloseDocument = true;
                                    }
                                    else
                                    {
                                        DocumentTypeModel.DocumentPermissionData.IsCloseDocument = false;
                                    }
                                }
                                else
                                {
                                    DocumentTypeModel.DocumentPermissionData.IsCloseDocument = true;
                                }
                            }
                            else
                            {
                                if (DocumentTypeModel.DocumentsData.Count > 0)
                                {
                                    DocumentTypeModel.DocumentsData.ForEach(p =>
                                    {
                                        p.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false };
                                        if (closedocumentPermission.Count > 0)
                                        {
                                            var userpermission = closedocumentPermission.FirstOrDefault(f => f.UserId == userData.UserID);
                                            if (userpermission != null)
                                            {
                                                p.DocumentPermissionData.IsCloseDocument = true;
                                            }
                                            else
                                            {
                                                p.DocumentPermissionData.IsCloseDocument = false;
                                            }
                                        }
                                        else
                                        {
                                            p.DocumentPermissionData.IsCloseDocument = true;
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {

                            DocumentTypeModel.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = false };
                            if (closedocumentPermission.Count > 0)
                            {
                                var userpermission = closedocumentPermission.FirstOrDefault(f => f.UserId == userData.UserID);
                                if (userpermission != null)
                                {
                                    DocumentTypeModel.DocumentPermissionData.IsCloseDocument = true;
                                }
                                else
                                {
                                    DocumentTypeModel.DocumentPermissionData.IsCloseDocument = false;
                                }
                            }
                            else
                            {
                                DocumentTypeModel.DocumentPermissionData.IsCloseDocument = true;
                            }
                        }
                        DocumentTypeModel.OpenDocument = DocumentTypeModel.DocumentsData.Where(d => d.CloseDocumentId == null || d.CloseDocumentId < 0).ToList().Count();
                        if (DocumentTypeModel != null)
                        {
                            DocumentTypeModel.IsExpiryDate = DocumentTypeModel.DocumentsData.FirstOrDefault()?.IsExpiryDate;
                            DocumentTypeModel.TotalDocument = DocumentTypeModel.DocumentsData.ToList().Count();
                            DocumentTypeModel.OpenDocument = DocumentTypeModel.DocumentsData.Where(d => d.CloseDocumentId == null || d.CloseDocumentId < 0).ToList().Count();
                        }
                    }
                }
                return DocumentTypeModel;

            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message, exp);
            }
        }
        public async Task<DocumentPermissionModel> GetAllSelectedFilePermissionAsync(long? DocumentId, long? selectedFileProfileTypeID)
        {
            DocumentTypeModel DocumentTypeModel = new DocumentTypeModel();
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
            {
                var docs = await GetAllFileProfileDocumentIdAsync(selectedFileProfileTypeID);
                var counts = docs != null ? (docs.Count + 1) : 1;
                DocumentTypeModel.DocumentsData.AddRange(docs);
                if (selectedFileProfileTypeID > 0)
                {
                    var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                    var appUsers = await GetApplicationUserAsync();
                    var fileProfileType = await GetFileprofiletypeAsync();
                    var roleItemsList = await GetDocumentUserRoleAsync(selectedFileProfileTypeID);
                    var closedocumentPermission = await GetCloseDocumentPermissionAsync(selectedFileProfileTypeID);
                    var parameters = new DynamicParameters();
                    var documentPermission = await GetDocumentPermissionByRoll();
                    parameters.Add("DocumentID", DocumentId, DbType.Int64);
                    var query = DocumentQueryString() + " where DocumentID=@DocumentID ";

                    using (var connection = CreateConnection())
                    {
                        var documents = (await connection.QueryAsync<Documents>(query, parameters)).ToList();
                        if (documents != null && documents.Count > 0)
                        {
                            var setAccess = roleItemsList;
                            documents.ForEach(s =>
                            {
                                DocumentsModel documentsModels = new DocumentsModel();
                                documentsModels.UniqueNo = counts;
                                if (setAccess.Count > 0)
                                {
                                    var roleDocItem = setAccess.FirstOrDefault(u => u.DocumentId == s.DocumentId);
                                    if (roleDocItem != null)
                                    {
                                        var roleItem = setAccess.FirstOrDefault(u => u.UserId == userData.UserID && u.DocumentId == s.DocumentId);
                                        if (roleItem != null)
                                        {
                                            var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)roleItem.RoleId).FirstOrDefault();
                                            documentsModels.DocumentPermissionData = permissionData;
                                        }
                                        else
                                        {
                                            documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = true, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };
                                        }
                                    }
                                    else
                                    {
                                        var filprofilepermission = setAccess.FirstOrDefault(u => u.FileProfileTypeId == s.FilterProfileTypeId && u.DocumentId == null && u.UserId == userData.UserID);
                                        if (filprofilepermission != null)
                                        {
                                            var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)filprofilepermission.RoleId).FirstOrDefault();
                                            documentsModels.DocumentPermissionData = permissionData;
                                        }
                                        else
                                        {
                                            documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = true, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = true, IsCopy = true, IsCreateFolder = true, IsEdit = true, IsMove = true, IsShare = true, IsFileDelete = true };
                                        }
                                    }
                                }
                                else
                                {
                                    documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = false };
                                }

                                if (documentsModels.DocumentPermissionData != null)
                                {
                                    if (documentsModels.DocumentPermissionData.IsRead == true)
                                    {
                                        documentsModel.Add(documentsModels);
                                    }
                                    else if (documentsModels.DocumentPermissionData.IsRead == false)
                                    {

                                    }
                                }
                                else
                                {
                                    documentsModel.Add(documentsModels);
                                }
                                counts++;
                            });
                        }
                        DocumentTypeModel.DocumentsData.AddRange(documentsModel.OrderByDescending(a => a.DocumentID).ToList());
                        var roleItems = roleItemsList.Where(w => w.FileProfileTypeId == selectedFileProfileTypeID).ToList();
                        if (roleItems.Count > 0)
                        {
                            var roleItem = roleItems.FirstOrDefault(u => u.UserId == userData.UserID);
                            if (roleItem != null)
                            {
                            }
                            else
                            {
                                if (DocumentTypeModel.DocumentsData.Count > 0)
                                {
                                    DocumentTypeModel.DocumentsData.ForEach(p =>
                                    {
                                        p.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false };
                                        if (closedocumentPermission.Count > 0)
                                        {
                                            var userpermission = closedocumentPermission.FirstOrDefault(f => f.UserId == userData.UserID);
                                            if (userpermission != null)
                                            {
                                                p.DocumentPermissionData.IsCloseDocument = true;
                                            }
                                            else
                                            {
                                                p.DocumentPermissionData.IsCloseDocument = false;
                                            }
                                        }
                                        else
                                        {
                                            p.DocumentPermissionData.IsCloseDocument = true;
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
                return DocumentTypeModel.DocumentPermissionData;

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
                    "t2.FileName as Title,\r\nt2.ContentType, \r\nt2.FileName as LinkDocumentName,\r\n" +
                    "t2.FilterProfileTypeID as PathFileProfieTypeId,\r\nt2.FolderID as LinkFolderID,\r\n" +
                    "t2.FilePath,\r\nt3.TableName as Type,\r\nt3.FileName as DocumentName,\r\nt4.UserName as AddedByUser,\r\nt3.IsNewPath as IsNewPath,\r\n" +

                    "t5.UserName as ModifiedByUser,\r\nt6.CodeValue as StatusCode\r\nfrom DocumentLink t1 \r\n" +
                    "JOIN Documents t2 ON t1.LinkDocumentId=t2.DocumentID\r\nJOIN Documents t3 ON t1.DocumentID=t3.DocumentID \r\n" +
                    "LEFT JOIN ApplicationUser t4 ON t4.UserID=t1.AddedByUserID\r\nLEFT JOIN ApplicationUser t5 ON t5.UserID=t1.ModifiedByUserID\r\n" +
                    "LEFT JOIN CodeMaster t6 ON t6.CodeID=t1.StatusCodeID where t1.DocumentID=@DocumentID";
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
                    "LEFT JOIN CodeMaster t6 ON t6.CodeID=t1.StatusCodeID where t1.LinkDocumentId=@LinkDocumentId";
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
        public async Task<DocumentLinkModel> DeleteDocumentLink(DocumentLinkModel documentLink)
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
                            parameters.Add("DocumentLinkId", documentLink.DocumentLinkId);

                            var query = "Delete from DocumentLink where DocumentLinkId=@DocumentLinkId";

                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            transaction.Commit();

                            return documentLink;
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
        public async Task<DocumentLink> InsertDocumentLink(DocumentLink documentLink)
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
                            var checkLink = await CheckDocumentLinkExits(documentLink);
                            if (checkLink == null)
                            {
                                var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                                var parameters = new DynamicParameters();
                                parameters.Add("DocumentId", documentLink.DocumentId);
                                parameters.Add("AddedByUserId", userData.UserID);
                                parameters.Add("AddedDate", DateTime.Now, DbType.DateTime);
                                parameters.Add("StatusCodeId", documentLink.StatusCodeId);
                                parameters.Add("LinkDocumentId", documentLink.LinkDocumentId);
                                parameters.Add("FolderId", documentLink.FolderId);
                                parameters.Add("FileProfieTypeId", documentLink.FileProfieTypeId);
                                parameters.Add("DocumentPath", documentLink.DocumentPath);
                                var query = "INSERT INTO [DocumentLink](DocumentId,AddedByUserId,AddedDate,StatusCodeId,LinkDocumentId,FolderId,FileProfieTypeId,DocumentPath) OUTPUT INSERTED.DocumentLinkId VALUES " +
                                    "(@DocumentId,@AddedByUserId,@AddedDate,@StatusCodeId,@LinkDocumentId,@FolderId,@FileProfieTypeId,@DocumentPath)";

                                documentLink.DocumentLinkId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                                transaction.Commit();
                            }
                            return documentLink;
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
        public async Task<DocumentsModel> UpdateExpiryDateField(DocumentsModel value)
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
                            var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentId", value.DocumentID);
                            parameters.Add("FilterProfileTypeId", value.FilterProfileTypeId);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", userData.UserID);
                            parameters.Add("ExpiryDate", value.ExpiryDate, DbType.DateTime);
                            var query = "Update Documents SET ExpiryDate=@ExpiryDate,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                "DocumentId= @DocumentId AND FilterProfileTypeId=@FilterProfileTypeId";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
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
        public async Task<DocumentsModel> UpdateDocumentRename(DocumentsModel value)
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
                            var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                            var parameters = new DynamicParameters();
                            parameters.Add("DocumentId", value.DocumentID);
                            parameters.Add("FileName", value.FileName, DbType.String);
                            parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                            parameters.Add("ModifiedByUserId", userData.UserID);
                            var query = "Update Documents SET FileName=@FileName,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                "DocumentId= @DocumentId";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
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
        public async Task<DocumentsModel> UpdateDescriptionField(DocumentsModel value)
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
                            var userData = await _localStorageService.GetItem<ApplicationUser>("user");
                            if (value.Type == "Document")
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("DocumentId", value.DocumentID);
                                parameters.Add("FilterProfileTypeId", value.FilterProfileTypeId);
                                parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                                parameters.Add("ModifiedByUserId", userData.UserID);
                                parameters.Add("Description", value.Description);

                                var query = "Update Documents SET Description=@Description,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                    "DocumentId= @DocumentId AND FilterProfileTypeId=@FilterProfileTypeId";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                                var parametersLink = new DynamicParameters();
                                parametersLink.Add("DocumentId", value.DocumentID);
                                parametersLink.Add("Description", value.Description);
                                parametersLink.Add("TransactionSessionId", value.SessionId);
                                var querys = "Update LinkFileProfileTypeDocument SET Description=@Description WHERE " +
                                    "DocumentId= @DocumentId AND TransactionSessionId=@TransactionSessionId";
                                await connection.QuerySingleOrDefaultAsync<long>(querys, parametersLink, transaction);
                            }
                            else
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("FileProfileTypeId", value.DocumentID);
                                parameters.Add("ModifiedDate", DateTime.Now, DbType.DateTime);
                                parameters.Add("ModifiedByUserId", userData.UserID);
                                parameters.Add("Description", value.Description);
                                var query = "Update Fileprofiletype SET Description=@Description,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId WHERE " +
                                    "FileProfileTypeId=@FileProfileTypeId";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            }
                            transaction.Commit();
                            return value;
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
        public async Task<FileProfileTypeModel> InsertOrUpdateFileProfileType(FileProfileTypeModel value)
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
                            if (value.FileProfileTypeId <= 0)
                            {

                                var query = "INSERT INTO [FileProfileType](Name,ProfileId,ParentId,StatusCodeID,AddedDate,AddedByUserID,ModifiedDate,ModifiedByUserId,Description,IsExpiryDate," +
                                    "IsAllowMobileUpload,IsDocumentAccess,ShelfLifeDuration,ShelfLifeDurationId,Hints,IsEnableCreateTask,IsCreateByYear,IsCreateByMonth," +
                                    "IsHidden,ProfileInfo,IsTemplateCaseNo,TemplateTestCaseId,SessionId) " +
                                    "OUTPUT INSERTED.FileProfileTypeId VALUES " +
                                   "(@Name,@ProfileId,@ParentId,@StatusCodeID,@AddedDate,@AddedByUserID,@ModifiedDate,@ModifiedByUserId,@Description,@IsExpiryDate," +
                                   "@IsAllowMobileUpload,@IsDocumentAccess,@ShelfLifeDuration,@ShelfLifeDurationId,@Hints,@IsEnableCreateTask,@IsCreateByYear,@IsCreateByMonth," +
                                   "@IsHidden,@ProfileInfo,@IsTemplateCaseNo,@TemplateTestCaseId,@SessionId)";
                                value.FileProfileTypeId = await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);

                            }
                            else
                            {
                                parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                                var query = "Update Fileprofiletype SET Name=@Name,ProfileId=@ProfileId,ParentId=@ParentId,StatusCodeID=@StatusCodeID,AddedDate=@AddedDate," +
                                    "AddedByUserID=@AddedByUserID,ModifiedDate=@ModifiedDate,ModifiedByUserId=@ModifiedByUserId,Description=@Description,IsExpiryDate=@IsExpiryDate,IsAllowMobileUpload=@IsAllowMobileUpload,IsDocumentAccess=@IsDocumentAccess," +
                                    "ShelfLifeDuration=@ShelfLifeDuration,ShelfLifeDurationId=@ShelfLifeDurationId,Hints=@Hints,IsEnableCreateTask=@IsEnableCreateTask,IsCreateByYear=@IsCreateByYear," +
                                    "IsCreateByMonth=@IsCreateByMonth,IsHidden=@IsHidden,ProfileInfo=@ProfileInfo,IsTemplateCaseNo=@IsTemplateCaseNo,TemplateTestCaseId=@TemplateTestCaseId,SessionId=@SessionId " +
                                    "WHERE FileProfileTypeId=@FileProfileTypeId";
                                await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            }
                            transaction.Commit();
                            return value;
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
        public async Task<IReadOnlyList<DocumentUserRole>> GetDocumentUserRoleByDocEmptyAsync(long? fileProfileTypeId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", fileProfileTypeId);
                var query = "select  * from DocumentUserRole where DocumentID is null AND FileProfileTypeId=@FileProfileTypeId";

                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<DocumentUserRole>(query, parameters)).ToList();
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
        public async Task<DocumentUserRoleModel> InsertFileProfileTypeAccess(DocumentUserRoleModel value)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    var userExitsRoles = await GetDocumentUserRoleByDocEmptyAsync(value.FileProfileTypeId);
                    var userGroupUsers = await GetUserGroupUser();
                    var LevelUsers = await GetLeveMasterUsers(value.SelectLevelMasterIDs);
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            if (value.Type == "User")
                            {
                                if (value.SelectUserIDs != null && value.SelectUserIDs.Count() > 0)
                                {
                                    foreach (var item in value.SelectUserIDs)
                                    {
                                        var counts = userExitsRoles.Where(w => w.UserId == item).Count();
                                        if (counts == 0)
                                        {
                                            var parameters = new DynamicParameters();
                                            parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                                            parameters.Add("UserId", item);
                                            parameters.Add("RoleId", value.RoleID);
                                            var query = "INSERT INTO [DocumentUserRole](FileProfileTypeId,UserId,RoleId) OUTPUT INSERTED.DocumentUserRoleId " +
                                                "VALUES (@FileProfileTypeId,@UserId,@RoleId)";
                                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
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
                                            var counts = userExitsRoles.Where(w => w.UserId == s.UserId).Count();
                                            if (counts == 0)
                                            {
                                                var parameters = new DynamicParameters();
                                                parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                                                parameters.Add("UserId", s.UserId);
                                                parameters.Add("UserGroupId", s.UserGroupId);
                                                parameters.Add("RoleId", value.UserGroupRoleID);
                                                var query = "INSERT INTO [DocumentUserRole](FileProfileTypeId,UserId,RoleId,UserGroupId) OUTPUT INSERTED.DocumentUserRoleId " +
                                                    "VALUES (@FileProfileTypeId,@UserId,@RoleId,@UserGroupId)";
                                                connection.QueryFirstOrDefault<long>(query, parameters, transaction);
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
                                        var counts = userExitsRoles.Where(w => w.UserId == s.UserId).Count();
                                        if (counts == 0)
                                        {
                                            var parameters = new DynamicParameters();
                                            parameters.Add("FileProfileTypeId", value.FileProfileTypeId);
                                            parameters.Add("UserId", s.UserId);
                                            parameters.Add("LevelId", s.LevelId);
                                            parameters.Add("RoleId", value.LevelRoleID);
                                            var query = "INSERT INTO [DocumentUserRole](FileProfileTypeId,UserId,RoleId,LevelId) OUTPUT INSERTED.DocumentUserRoleId " +
                                                "VALUES (@FileProfileTypeId,@UserId,@RoleId,@LevelId)";
                                            connection.QueryFirstOrDefault<long>(query, parameters, transaction);
                                        }
                                    });
                                }
                            }
                            transaction.Commit();
                            return value;
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
        public async Task<IReadOnlyList<DocumentUserRoleModel>> GetDocumentUserRoleList(long? Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FileProfileTypeId", Id);
                var query = "select t1.*,t2.DocumentRoleName,t2.DocumentRoleDescription,\r\nt3.Name as UserGroup,\r\nt3.Description as UserGroupDescription,\r\nt4.Name as FileProfileType,\r\nt5.Name as LevelName,\r\nt6.NickName,\r\nt6.FirstName,\r\nt6.LastName,\r\nt7.Name as DepartmentName,\r\nt8.Name as DesignationName,\r\nCONCAT(case when t6.NickName is NULL\r\n then  t6.FirstName\r\n ELSE\r\n  t6.NickName END,' | ',t6.LastName) as FullName\r\n" +
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
        public async Task<DocumentUserRoleModel> DeleteDocumentUserRole(DocumentUserRoleModel value)
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
                            parameters.Add("DocumentUserRoleID", value.DocumentUserRoleID);
                            var query = "DELETE FROM DocumentUserRole WHERE " +
                                "DocumentUserRoleID= @DocumentUserRoleID";
                            await connection.QuerySingleOrDefaultAsync<long>(query, parameters, transaction);
                            transaction.Commit();
                            return value;
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

    }
}
