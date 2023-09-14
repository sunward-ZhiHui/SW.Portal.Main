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
        Task<IReadOnlyList<Fileprofiletype>> GetAllAsync(long FileProfileTypeID);
        Task<long?> DeleteFileProfileType(long? fileProfileTypeId);
        Task<FileProfileTypeModel> GetFileProfileTypeBySession(Guid? SessionId);
        Task<DocumentTypeModel> GetAllSelectedFileAsync(long? selectedFileProfileTypeID);
        Task<DocumentTypeModel> GetFileProfileTypeDocumentByHistory(SearchModel searchModel);
        Task<DocumentsModel> GetFileProfileTypeDelete(DocumentsModel documentsModel);
        Task<DocumentsModel> GetFileProfileTypeCheckOut(DocumentsModel documentsModel);
        Task<IReadOnlyList<DocumentsModel>> GetAllFileProfileDocumentAsync();
        Task<IReadOnlyList<DocumentLinkModel>> GetDocumentLinkByDocumentId(long? id);
        Task<DocumentLink> InsertDocumentLink(DocumentLink documentLink);
        Task<DocumentLinkModel> DeleteDocumentLink(DocumentLinkModel documentLink);
        Task<IReadOnlyList<DocumentLinkModel>> GetParentDocumentsByLinkDocumentId(long? id);
        Task<DocumentsModel> UpdateDescriptionField(DocumentsModel documentsModel);
        Task<DocumentsModel> UpdateExpiryDateField(DocumentsModel documentsModel);
        Task<DocumentPermissionModel> GetAllSelectedFilePermissionAsync(long? DocumentId,long? selectedFileProfileTypeID);
        Task<DocumentsModel> UpdateDocumentRename(DocumentsModel value);
        Task<IReadOnlyList<UserGroup>> GetUserGroups();
        Task<IReadOnlyList<DocumentRole>> GetDocumentRole();
        Task<DocumentProfileNoSeriesModel> GetDocumentProfileNoSeriesById(long? Id);
        Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetDocumentProfiles();
        Task<FileProfileTypeModel> InsertOrUpdateFileProfileType(FileProfileTypeModel fileProfileTypeModel);
        Task<DocumentPermissionModel> GetDocumentPermissionByRoleID(long? Id);
        Task<DocumentUserRoleModel> InsertFileProfileTypeAccess(DocumentUserRoleModel documentUserRole);
        Task<IReadOnlyList<DocumentUserRoleModel>> GetDocumentUserRoleList(long? Id);
        Task<DocumentUserRoleModel> DeleteDocumentUserRole(DocumentUserRoleModel value);
        Task<IReadOnlyList<FileProfileSetupFormModel>> GetFileProfileSetupFormList(long? Id);
        Task<IReadOnlyList<DocumentNoSeriesModel>> GetReserveProfileNumberSeries(long? Id,long? ProfileId);

    }
}
