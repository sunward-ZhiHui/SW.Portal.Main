using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Query
{
    public interface IFileprofileQueryRepository : IQueryRepository<Fileprofiletype>
    {
        Task<IReadOnlyList<Fileprofiletype>> GetFileprofiletypeAsync();
        Task<long?> DeleteFileProfileType(long? fileProfileTypeId);
        Task<FileProfileTypeModel> GetFileProfileTypeBySession(Guid? SessionId);
        Task<DocumentTypeModel> GetAllSelectedFileAsync(DocumentSearchModel documentSearchModel);
        Task<DocumentTypeModel> GetFileProfileTypeDocumentByHistory(SearchModel searchModel);
        Task<DocumentsModel> GetFileProfileTypeDocumentDelete(DocumentsModel documentsModel);
        Task<DocumentsModel> GetFileProfileTypeCheckOut(DocumentsModel documentsModel);
        Task<IReadOnlyList<DocumentsModel>> GetAllFileProfileDocumentAsync();
        Task<IReadOnlyList<DocumentLinkModel>> GetDocumentLinkByDocumentId(long? id);
        Task<DocumentLink> InsertDocumentLink(DocumentLink documentLink);
        Task<DocumentLinkModel> DeleteDocumentLink(DocumentLinkModel documentLink);
        Task<IReadOnlyList<DocumentLinkModel>> GetParentDocumentsByLinkDocumentId(long? id);
        Task<DocumentsModel> UpdateDescriptionField(DocumentsModel documentsModel);
        Task<DocumentsModel> UpdateExpiryDateField(DocumentsModel documentsModel);
        Task<DocumentsModel> UpdateDocumentRename(DocumentsModel value);
        Task<IReadOnlyList<UserGroup>> GetUserGroups();
        Task<List<UserGroup>> GetAllUserGroups();        
        Task<IReadOnlyList<DocumentRole>> GetDocumentRole();
        Task<DocumentProfileNoSeriesModel> GetDocumentProfileNoSeriesById(long? Id);
        Task<Fileprofiletype> GetByFileprofiletypeIdAsync(long? Id);        
        Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetDocumentProfiles();
        Task<FileProfileTypeModel> InsertOrUpdateFileProfileType(FileProfileTypeModel fileProfileTypeModel);
        Task<DocumentPermissionModel> GetDocumentPermissionByRoleID(long? Id);
        Task<DocumentUserRoleModel> InsertFileProfileTypeAccess(DocumentUserRoleModel documentUserRole);
        Task<IReadOnlyList<DocumentUserRoleModel>> GetDocumentUserRoleList(long? Id);
        Task<DocumentUserRoleModel> DeleteDocumentUserRole(DocumentUserRoleModel value);
        Task<IReadOnlyList<FileProfileSetupFormModel>> GetFileProfileSetupFormList(long? Id);
        Task<IReadOnlyList<DocumentNoSeriesModel>> GetReserveProfileNumberSeries(long? Id, long? ProfileId);
        Task<DocumentTypeModel> GetAllDocumentDeleteAsync();
        Task<DocumentsModel> ReStoreFileProfileTypeAndDocument(DocumentsModel documentsModel);
        Task<IReadOnlyList<DocumentsModel>> GetFileContetTypes();
        Task<DocumentPermissionModel> GetDocumentUserRoleByUserIDAsync(long? fileProfileTypeId, long? userId);
        Task<DocumentUserRoleModel> UpdateDocumentUserRole(DocumentUserRoleModel documentUserRoleModel);
        Task<IReadOnlyList<DocumentRole>> GetDocumentRoleList();
        Task<DocumentRole> InsertOrUpdateDocumentRole(DocumentRole documentRole);
        Task<DocumentRole> DeleteDocumentRole(DocumentRole documentRole);
        FileProfileTypeModel GeFileProfileTypeNameCheckValidation(string? value, long id);
        DocumentRole GetDocumentRoleNameCheckValidation(string? value, long id);
        Task<DocumentPermission> GetDocumentPermissionData(long? id);
        Task<DocumentPermission> InsertOrUpdateDocumentPermission(DocumentPermission documentPermission);
        Task<long?> DeleteDocumentPermissions(long? Id);
        Task<IReadOnlyList<DocumentDmsShare>> GetDocumentDMSShareList(Guid? docSessionID);
        Task<DocumentDmsShare> InsertOrUpdateDocumentDmsShare(DocumentDmsShare documentDmsShare);
        Task<DocumentDmsShare> DeleteDocumentDmsShare(DocumentDmsShare value);
        Task<FileProfileTypeModel> UpdateProfileTypeInfo(FileProfileTypeModel value);
        Task<long?> MoveToFileProfileTypeUpdateInfo(List<DocumentsModel> value, long? FileprofileTypeId);
        Task<IReadOnlyList<Documents>> GetAllSelectedFilesAsync(DocumentSearchModel documentSearchModel);
        Task<IReadOnlyList<DocumentsModel>> GetUploadedButNoProfileNo(DocumentsUploadModel documentsUploadModel);
        Task<IReadOnlyList<DocumentsModel>> GetNoProfileNo(long? UserId,DateTime? StartDate);
        Task<DocumentsUploadModel> GetDocumentDeleteForNoProfileNo(DocumentsUploadModel documentsModel);
    }
}
