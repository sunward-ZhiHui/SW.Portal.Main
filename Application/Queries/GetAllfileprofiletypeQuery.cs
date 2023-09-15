using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllfileprofiletypeQuery : PagedRequest, IRequest<List<Fileprofiletype>>
    {
        public long FileProfileTypeID { get; private set; }
        public GetAllfileprofiletypeQuery(long FileProfileTypeID)
        {
            this.FileProfileTypeID = FileProfileTypeID;
        }
    }
    public class GetAllfileprofiletypeListQuery : PagedRequest, IRequest<List<DocumentsModel>>
    {

    }
    public class DeleteFileProfileType : PagedRequest, IRequest<long?>
    {
        public long? FileProfileTypeID { get; private set; }
        public DeleteFileProfileType(long? fileProfileTypeID)
        {
            this.FileProfileTypeID = fileProfileTypeID;
        }
    }
    
    public class GetAllSelectedfileprofiletypeQuery : PagedRequest, IRequest<DocumentTypeModel>
    {
        public long? selectedFileProfileTypeID { get; private set; }
        public GetAllSelectedfileprofiletypeQuery(long? selectedFileProfileTypeID)
        {
            this.selectedFileProfileTypeID = selectedFileProfileTypeID;
        }
    }

    public class GetFileProfileTypeDocumentByHistory : PagedRequest, IRequest<DocumentTypeModel>
    {
        public SearchModel SearchModel { get; private set; }
        public GetFileProfileTypeDocumentByHistory(SearchModel searchModel)
        {
            this.SearchModel = searchModel;
        }
    }
    public class GetFileProfileTypeDocumentDelete : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetFileProfileTypeDocumentDelete(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class GetFileProfileTypeCheckOut : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetFileProfileTypeCheckOut(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class GetFileProfileTypeLinkDocument : PagedRequest, IRequest<List<DocumentLinkModel>>
    {
        public long? Id { get; private set; }
        public GetFileProfileTypeLinkDocument(long? id)
        {
            this.Id = id;
        }
    }
    public class GetParentDocumentsByLinkDocument : PagedRequest, IRequest<List<DocumentLinkModel>>
    {
        public long? Id { get; private set; }
        public GetParentDocumentsByLinkDocument(long? id)
        {
            this.Id = id;
        }
    }
    public class InsertDocumentLink : PagedRequest, IRequest<DocumentLink>
    {
        public DocumentLink DocumentLink { get; private set; }
        public InsertDocumentLink(DocumentLink documentLink)
        {
            this.DocumentLink = documentLink;
        }
    }
    public class DeleteDocumentLink : PagedRequest, IRequest<DocumentLinkModel>
    {
        public DocumentLinkModel DocumentLink { get; private set; }
        public DeleteDocumentLink(DocumentLinkModel documentLink)
        {
            this.DocumentLink = documentLink;
        }
    }
    public class GetFileDownload : PagedRequest, IRequest<DocumentsModel>
    {
        public long? DocumentId { get; private set; }
        public GetFileDownload(long? documentId)
        {
            this.DocumentId = documentId;
        }
    }
    public class GetUpdateDescriptionField : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetUpdateDescriptionField(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class GetUpdateExpiryDateField : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetUpdateExpiryDateField(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class GetUpdateDocumentRename : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public GetUpdateDocumentRename(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    public class GetSelectedFilePermission : PagedRequest, IRequest<DocumentPermissionModel>
    {
        public long? DocumentId { get; private set; }
        public long? FileProfileTypeID { get; private set; }
        public GetSelectedFilePermission(long? documentId, long? fileProfileTypeID)
        {
            this.DocumentId = documentId;
            this.FileProfileTypeID = fileProfileTypeID;
        }
    }
    public class GetUserGroups : PagedRequest, IRequest<List<UserGroup>>
    {

    }
    public class GetDocumentRoles : PagedRequest, IRequest<List<DocumentRole>>
    {

    }
    public class GetDocumentProfiles : PagedRequest, IRequest<List<DocumentProfileNoSeriesModel>>
    {

    }
    public class InsertOrUpdateFileProfileType : PagedRequest, IRequest<FileProfileTypeModel>
    {
        public FileProfileTypeModel FileProfileTypeModel { get; private set; }
        public InsertOrUpdateFileProfileType(FileProfileTypeModel fileProfileTypeModel)
        {
            this.FileProfileTypeModel = fileProfileTypeModel;
        }
    }
    public class GetFileProfileTypeBySession : PagedRequest, IRequest<FileProfileTypeModel>
    {
        public Guid? SessionId { get; private set; }
        public GetFileProfileTypeBySession(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetDocumentPermissionByRoleID : PagedRequest, IRequest<DocumentPermissionModel>
    {
        public long? Id { get; private set; }
        public GetDocumentPermissionByRoleID(long? id)
        {
            this.Id = id;
        }
    }
    public class GetDocumentProfileNoSeriesById : PagedRequest, IRequest<DocumentProfileNoSeriesModel>
    {
        public long? Id { get; private set; }
        public GetDocumentProfileNoSeriesById(long? id)
        {
            this.Id = id;
        }
    }
    
    public class InsertFileProfileTypeAccess : PagedRequest, IRequest<DocumentUserRoleModel>
    {
        public DocumentUserRoleModel DocumentUserRole { get; private set; }
        public InsertFileProfileTypeAccess(DocumentUserRoleModel documentUserRole)
        {
            this.DocumentUserRole = documentUserRole;
        }
    }
    public class GetDocumentUserRoleList : PagedRequest, IRequest<List<DocumentUserRoleModel>>
    {
        public long? Id { get; private set; }
        public GetDocumentUserRoleList(long? id)
        {
            this.Id = id;
        }
    }
    public class DeleteDocumentUserRole : PagedRequest, IRequest<DocumentUserRoleModel>
    {
        public DocumentUserRoleModel DocumentUserRole { get; private set; }
        public DeleteDocumentUserRole(DocumentUserRoleModel documentUserRole)
        {
            this.DocumentUserRole = documentUserRole;
        }
    }
    public class GetFileProfileSetupFormList : PagedRequest, IRequest<List<FileProfileSetupFormModel>>
    {
        public long? Id { get; private set; }
        public GetFileProfileSetupFormList(long? id)
        {
            this.Id = id;
        }
    }
    public class GetReserveProfileNumberSeries : PagedRequest, IRequest<List<DocumentNoSeriesModel>>
    {
        public long? Id { get; private set; }
        public long? ProfileId { get; private set; }
        public GetReserveProfileNumberSeries(long? id, long? profileId)
        {
            this.Id = id;
            this.ProfileId = profileId;
        }
    }
    
    public class InsertCreateDocument : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public InsertCreateDocument(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class UpdateCreateDocumentBySession : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public UpdateCreateDocumentBySession(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class InsertOrUpdateReserveProfileNumberSeries : PagedRequest, IRequest<DocumentNoSeriesModel>
    {
        public DocumentNoSeriesModel DocumentNoSeriesModel { get; private set; }
        public InsertOrUpdateReserveProfileNumberSeries(DocumentNoSeriesModel documentNoSeriesModel)
        {
            this.DocumentNoSeriesModel = documentNoSeriesModel;
        }
    }
    public class UpdateCreateDocumentBySessionReserveSeries : PagedRequest, IRequest<DocumentNoSeriesModel>
    {
        public DocumentNoSeriesModel DocumentNoSeriesModel { get; private set; }
        public UpdateCreateDocumentBySessionReserveSeries(DocumentNoSeriesModel documentNoSeriesModel)
        {
            this.DocumentNoSeriesModel = documentNoSeriesModel;
        }
    }
    public class UpdateReserveNumberDescriptionField : PagedRequest, IRequest<DocumentNoSeriesModel>
    {
        public DocumentNoSeriesModel DocumentNoSeriesModel { get; private set; }
        public UpdateReserveNumberDescriptionField(DocumentNoSeriesModel documentNoSeriesModel)
        {
            this.DocumentNoSeriesModel = documentNoSeriesModel;
        }
    }
    public class GetAllDocumentDelete : PagedRequest, IRequest<DocumentTypeModel>
    {
    }
    public class ReStoreFileProfileTypeAndDocument : PagedRequest, IRequest<DocumentsModel>
    {
        public DocumentsModel DocumentsModel { get; private set; }
        public ReStoreFileProfileTypeAndDocument(DocumentsModel documentsModel)
        {
            this.DocumentsModel = documentsModel;
        }
    }
    

}
