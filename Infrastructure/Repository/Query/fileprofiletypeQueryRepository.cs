using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using Dapper;
using Infrastructure.Repository.Query.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
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


        public async Task<DocumentTypeModel> GetAllSelectedFileAsync(long selectedFileProfileTypeID)
        {
            DocumentTypeModel DocumentTypeModel = new DocumentTypeModel();
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            try
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
                parameters.Add("FileProfileTypeId", selectedFileProfileTypeID);
                parameters.Add("documentsIds", string.Join(',', linkfileProfileTypeDocumentids));
                var query = DocumentQueryString() + " where FilterProfileTypeId=@FileProfileTypeId " +
                    "AND IsLatest=1 " +
                    "AND (ArchiveStatusId != 2562 OR ArchiveStatusId  IS NULL) " +
                    "OR (DocumentID in(@documentsIds) AND IsLatest=1) " +
                    "order by DocumentId desc";

                using (var connection = CreateConnection())
                {
                    var documents = (await connection.QueryAsync<DocumentsModel>(query, parameters)).ToList();
                    if (documents != null && documents.Count > 0)
                    {
                        var docIds = documents.Select(a => a.DocumentID).ToList();
                        var notes = await GeNotesAsync(docIds);
                        var taskMasternotes = await GetTaskMasterAsync(docIds);
                        var setAccess = roleItemsList;
                        documents.ForEach(s =>
                         {
                             var documentcount = documents?.Where(w => w.DocumentParentId == s.DocumentParentId).Count();
                             var name = s.FileName != null ? s.FileName?.Substring(s.FileName.LastIndexOf(".")) : "";
                             var fileName = s.FileName?.Split(name);
                             DocumentsModel documentsModels = new DocumentsModel();
                             var setAccessFlag = roleItemsList.Where(a => a.UserId == userData.UserID && a.DocumentId == s.DocumentID).Count();
                             documentsModels.NotesCount = notes.Where(a => a.DocumentId == s.DocumentID).Count();
                             documentsModels.NotesColor = "";
                             var taskNotesCount = taskMasternotes.Where(a => a.SourceId == s.DocumentID).Count();
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
                             documentsModels.SetAccessFlag = setAccessFlag > 0 ? true : false;
                             documentsModels.SessionId = s.SessionId;
                             documentsModels.DocumentID = s.DocumentID;
                             documentsModels.FileName = s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + name : s.FileName;
                             documentsModels.ContentType = s.ContentType;
                             documentsModels.FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024));
                             documentsModels.UploadDate = s.UploadDate;
                             documentsModels.SessionID = s.SessionId;
                             documentsModels.FilterProfileTypeId = s.FilterProfileTypeId;
                             documentsModels.FileProfileTypeName = fileProfileType.FirstOrDefault(p => p.FileProfileTypeId == s.FilterProfileTypeId)?.Name;
                             documentsModels.DocumentParentId = s.DocumentParentId;
                             documentsModels.TableName = s.TableName;
                             documentsModels.IsMobileUpload = s.IsMobileUpload;
                             documentsModels.Type = "Document";
                             documentsModels.ExpiryDate = s.ExpiryDate;
                             documentsModels.FileIndex = s.FileIndex;
                             documentsModels.TotalDocument = documentcount == 1 ? 1 : (documentcount + 1);
                             documentsModels.UploadedByUserId = s.AddedByUserID;
                             documentsModels.ModifiedByUserID = s.ModifiedByUserID;
                             documentsModels.AddedDate = s.ModifiedDate == null ? s.UploadDate : s.ModifiedDate;
                             documentsModels.AddedByUser = s.ModifiedByUserID == null ? appUsers.FirstOrDefault(f => f.UserID == s.AddedByUserID)?.UserName : appUsers.FirstOrDefault(f => f.UserID == s.ModifiedByUserID)?.UserName;
                             documentsModels.IsLocked = s.IsLocked;
                             documentsModels.LockedByUserId = s.LockedByUserId;
                             documentsModels.LockedDate = s.LockedDate;
                             documentsModels.AddedByUserID = s.AddedByUserID;
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
                                 var roleDocItem = setAccess.FirstOrDefault(u => u.DocumentId == s.DocumentID);
                                 if (roleDocItem != null)
                                 {
                                     var roleItem = setAccess.FirstOrDefault(u => u.UserId == userData.UserID && u.DocumentId == s.DocumentID);
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
                             var description = linkfileProfileTypes?.Where(f => f.FileProfileTypeId == selectedFileProfileTypeID && f.TransactionSessionId == s.SessionId && f.DocumentId == s.DocumentID).FirstOrDefault()?.Description;
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
                         });
                    }
                    DocumentTypeModel.DocumentsData = documentsModel.OrderByDescending(a => a.DocumentID).ToList();
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
                    return DocumentTypeModel;
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
        public async Task<IReadOnlyList<Fileprofiletype>> GetFileprofiletypeAsync()
        {
            try
            {
                var query = "select FileProfileTypeId,Name,IsDocumentAccess,IsEnableCreateTask,IsExpiryDate,IsHidden,AddedByUserId from FileProfileType where IsHidden is null or IsHidden=0";

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
        public async Task<IReadOnlyList<LinkFileProfileTypeDocument>> GetLinkFileProfileTypeDocumentAsync(long fileProfileTypeId)
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
        public async Task<IReadOnlyList<CloseDocumentPermission>> GetCloseDocumentPermissionAsync(long fileProfileTypeId)
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
        public async Task<IReadOnlyList<DocumentUserRole>> GetDocumentUserRoleAsync(long fileProfileTypeId)
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
                    "FilePath " +
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
                        var fileName = documetsparent.FileName.Split('.');
                        documentsModel.DocumentID = documetsparent.DocumentID;
                        documentsModel.FileName = documetsparent.FileIndex > 0 ? fileName[0] + "_V0" + documetsparent.FileIndex + "." + fileName[1] : documetsparent.FileName;
                        documentsModel.ContentType = documetsparent.ContentType;
                        documentsModel.FileSize = (long)Math.Round(Convert.ToDouble(documetsparent.FileSize / 1024));
                        documentsModel.UploadDate = documetsparent.UploadDate;
                        documentsModel.SessionID = documetsparent.SessionId;
                        documentsModel.FilterProfileTypeId = documetsparent.FilterProfileTypeId;
                        documentsModel.FileProfileTypeName = fileProfileType?.FirstOrDefault(f => f.FileProfileTypeId == documetsparent.FilterProfileTypeId)?.Name;
                        documentsModel.DocumentParentId = documetsparent.DocumentParentId;
                        documentsModel.TableName = documetsparent.TableName;
                        documentsModel.Type = "Document";
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
                                    FileName = s.FileIndex > 0 ? fileName[0] + "_V0" + s.FileIndex + "." + fileName[1] : s.FileName,
                                    ContentType = s.ContentType,
                                    FileSize = (long)Math.Round(Convert.ToDouble(s.FileSize / 1024)),
                                    UploadDate = s.UploadDate,
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
    }
}
