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
    public class GetAllfileprofiletypeDrodownQuery : PagedRequest, IRequest<List<FileProfileTypeModel>>
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
        public DocumentSearchModel DocumentSearchModel { get; private set; }
        public GetAllSelectedfileprofiletypeQuery(DocumentSearchModel documentSearchModel)
        {
            this.DocumentSearchModel = documentSearchModel;
        }
    }
    public class GetAllSelectedfileprofiletypesQuery : PagedRequest, IRequest<List<Documents>>
    {
        public DocumentSearchModel DocumentSearchModel { get; private set; }
        public GetAllSelectedfileprofiletypesQuery(DocumentSearchModel documentSearchModel)
        {
            this.DocumentSearchModel = documentSearchModel;
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

    public class GetGroupByUserids : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long Id { get; private set; }

        public GetGroupByUserids(long Id)
        {
            this.Id = Id;            
        }
    }
    
    public class GetAllUserGroups : PagedRequest, IRequest<List<UserGroup>>
    {

    }    
    public class GetDocumentRoles : PagedRequest, IRequest<List<DocumentRole>>
    {

    }
    public class GetDocumentRoleList : PagedRequest, IRequest<List<DocumentRole>>
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
    public class GetByFileprofiletypeId : PagedRequest, IRequest<Fileprofiletype>
    {
        public long? Id { get; private set; }
        public GetByFileprofiletypeId(long? id)
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
    public class GetFileContetTypes : PagedRequest, IRequest<List<DocumentsModel>>
    {
    }

    public class InsertCreateDocument : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public InsertCreateDocument(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class GetByIdDocument : PagedRequest, IRequest<Documents>
    {
        public long? DocumentId { get; set; }
        public GetByIdDocument(long? docid)
        {
            this.DocumentId = docid;
        }
    }
    public class GetByUniqueDocument : PagedRequest, IRequest<List<Documents>>
    {
        public string DocumentId { get; set; }
        public GetByUniqueDocument(string docid)
        {
            this.DocumentId = docid;
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
    public class UpdateEmailDocumentBySession : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public UpdateEmailDocumentBySession(DocumentsUploadModel documentsUploadModel)
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
    public class GetDocumentUserRoleByUserID : PagedRequest, IRequest<DocumentPermissionModel>
    {
        public long? Id { get; private set; }
        public long? UserId { get; private set; }
        public GetDocumentUserRoleByUserID(long? id, long? userId)
        {
            this.Id = id;
            this.UserId = userId;
        }
    }
    public class UpdateDocumentUserRole : PagedRequest, IRequest<DocumentUserRoleModel>
    {
        public DocumentUserRoleModel DocumentUserRoleModel { get; private set; }
        public UpdateDocumentUserRole(DocumentUserRoleModel documentUserRoleModel)
        {
            this.DocumentUserRoleModel = documentUserRoleModel;
        }
    }

    public class InsertOrUpdateDocumentRole : DocumentRole, IRequest<DocumentRole>
    {

    }
    public class InsertOrUpdateDocumentPermission : DocumentPermission, IRequest<DocumentPermission>
    {
        public DocumentPermission DocumentPermission { get; private set; }
        public InsertOrUpdateDocumentPermission(DocumentPermission documentPermission)
        {
            this.DocumentPermission = documentPermission;
        }
    }
    public class DeleteDocumentRole : DocumentRole, IRequest<DocumentRole>
    {
        public DocumentRole DocumentRole { get; private set; }
        public DeleteDocumentRole(DocumentRole documentRole)
        {
            this.DocumentRole = documentRole;
        }
    }
    public class GetDocumentPermissionData : DocumentRole, IRequest<DocumentPermission>
    {
        public long? Id { get; private set; }
        public GetDocumentPermissionData(long? id)
        {
            this.Id = id;
        }
    }
    public class DeleteDocumentPermissions : PagedRequest, IRequest<long?>
    {
        public long? Id { get; private set; }
        public DeleteDocumentPermissions(long? id)
        {
            this.Id = id;
        }
    }

    public class InsertOrUpdateDocumentDmsShare : DocumentDmsShare, IRequest<DocumentDmsShare>
    {
    }
    public class GetDocumentDMSShareList : PagedRequest, IRequest<List<DocumentDmsShare>>
    {
        public Guid? SessionId { get; private set; }
        public GetDocumentDMSShareList(Guid? sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class DeleteDocumentDmsShare : DocumentDmsShare, IRequest<DocumentDmsShare>
    {
        public DocumentDmsShare DocumentDmsShare { get; private set; }
        public DeleteDocumentDmsShare(DocumentDmsShare documentDmsShare)
        {
            this.DocumentDmsShare = documentDmsShare;
        }
    }
    public class UpdateProfileTypeInfo : DocumentDmsShare, IRequest<FileProfileTypeModel>
    {
        public FileProfileTypeModel FileProfileTypeModel { get; private set; }
        public UpdateProfileTypeInfo(FileProfileTypeModel fileProfileTypeModel)
        {
            this.FileProfileTypeModel = fileProfileTypeModel;
        }
    }
    public class MoveToFileProfileTypeUpdateInfo : DocumentsModel, IRequest<long?>
    {
        public List<DocumentsModel> DocumentsModel { get; private set; }
        public long? FileprofileTypeId { get; private set; }
        public MoveToFileProfileTypeUpdateInfo(List<DocumentsModel> documentsModel, long? fileprofileTypeId)
        {
            this.DocumentsModel = documentsModel;
            this.FileprofileTypeId = fileprofileTypeId;
        }
    }
    public class UpdateDocumentNoDocumentBySession : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public UpdateDocumentNoDocumentBySession(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class GetUploadedButNoProfileNo : PagedRequest, IRequest<List<DocumentsModel>>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public GetUploadedButNoProfileNo(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class GetNoProfileNo : PagedRequest, IRequest<List<DocumentsModel>>
    {
        public long? UserId { get; private set; }
        public DateTime? StartDate { get; private set; }
        public GetNoProfileNo(long? userId, DateTime? startDate)
        {
            this.UserId = userId;
            this.StartDate = startDate;
        }
    }
    public class UpdateDocumentNoDocumentByNoProfile : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public UpdateDocumentNoDocumentByNoProfile(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class GetDocumentDeleteForNoProfileNo : PagedRequest, IRequest<DocumentsUploadModel>
    {
        public DocumentsUploadModel DocumentsUploadModel { get; private set; }
        public GetDocumentDeleteForNoProfileNo(DocumentsUploadModel documentsUploadModel)
        {
            this.DocumentsUploadModel = documentsUploadModel;
        }
    }
    public class GetFileProfileTypeList : PagedRequest, IRequest<DocumentsModel>
    {
        public long FileProfileType { get; set; }
        public GetFileProfileTypeList(long FileProfileType)
        {
            this.FileProfileType = FileProfileType;
        }
    }
}
