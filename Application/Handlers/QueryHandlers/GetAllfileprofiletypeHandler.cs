using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllfileprofiletypeHandler : IRequestHandler<GetAllfileprofiletypeQuery, List<Fileprofiletype>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetAllfileprofiletypeHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<Fileprofiletype>> Handle(GetAllfileprofiletypeQuery request, CancellationToken cancellationToken)
        {
            return (List<Fileprofiletype>)await _fileprofileQueryRepository.GetAllAsync(request.FileProfileTypeID);
        }

    }
    public class DeleteFileProfileTypeHandler : IRequestHandler<DeleteFileProfileType, long?>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public DeleteFileProfileTypeHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<long?> Handle(DeleteFileProfileType request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.DeleteFileProfileType(request.FileProfileTypeID);
        }

    }

    public class GetAllfileprofiletypeListQueryHandler : IRequestHandler<GetAllfileprofiletypeListQuery, List<DocumentsModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetAllfileprofiletypeListQueryHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentsModel>> Handle(GetAllfileprofiletypeListQuery request, CancellationToken cancellationToken)
        {
            return (List<DocumentsModel>)await _fileprofileQueryRepository.GetAllFileProfileDocumentAsync();
        }

    }
    public class GetAllselectedfileprofiletypeHandler : IRequestHandler<GetAllSelectedfileprofiletypeQuery, DocumentTypeModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetAllselectedfileprofiletypeHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentTypeModel> Handle(GetAllSelectedfileprofiletypeQuery request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetAllSelectedFileAsync(request.selectedFileProfileTypeID);
        }

    }
    public class GetFileProfileTypeDocumentByHistoryHandler : IRequestHandler<GetFileProfileTypeDocumentByHistory, DocumentTypeModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetFileProfileTypeDocumentByHistoryHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentTypeModel> Handle(GetFileProfileTypeDocumentByHistory request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetFileProfileTypeDocumentByHistory(request.SearchModel);
        }

    }
    public class GetFileProfileTypeLinkDocumentHandler : IRequestHandler<GetFileProfileTypeLinkDocument, List<DocumentLinkModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetFileProfileTypeLinkDocumentHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentLinkModel>> Handle(GetFileProfileTypeLinkDocument request, CancellationToken cancellationToken)
        {
            return (List<DocumentLinkModel>)await _fileprofileQueryRepository.GetDocumentLinkByDocumentId(request.Id);
        }

    }
    public class GetParentDocumentsByLinkDocumentHandler : IRequestHandler<GetParentDocumentsByLinkDocument, List<DocumentLinkModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetParentDocumentsByLinkDocumentHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentLinkModel>> Handle(GetParentDocumentsByLinkDocument request, CancellationToken cancellationToken)
        {
            return (List<DocumentLinkModel>)await _fileprofileQueryRepository.GetParentDocumentsByLinkDocumentId(request.Id);
        }

    }

    public class GetFileProfileTypeDeleteHandler : IRequestHandler<GetFileProfileTypeDelete, DocumentsModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetFileProfileTypeDeleteHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentsModel> Handle(GetFileProfileTypeDelete request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetFileProfileTypeDelete(request.DocumentsModel);
        }

    }
    public class GetFileProfileTypeCheckOutHandler : IRequestHandler<GetFileProfileTypeCheckOut, DocumentsModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetFileProfileTypeCheckOutHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentsModel> Handle(GetFileProfileTypeCheckOut request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetFileProfileTypeCheckOut(request.DocumentsModel);
        }

    }
    public class GetUpdateDescriptionFieldHandler : IRequestHandler<GetUpdateDescriptionField, DocumentsModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetUpdateDescriptionFieldHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentsModel> Handle(GetUpdateDescriptionField request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.UpdateDescriptionField(request.DocumentsModel);
        }

    }
    public class GetUpdateExpiryDateFieldHandler : IRequestHandler<GetUpdateExpiryDateField, DocumentsModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetUpdateExpiryDateFieldHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentsModel> Handle(GetUpdateExpiryDateField request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.UpdateExpiryDateField(request.DocumentsModel);
        }

    }
    public class GetUpdateDocumentRenameHandler : IRequestHandler<GetUpdateDocumentRename, DocumentsModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetUpdateDocumentRenameHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentsModel> Handle(GetUpdateDocumentRename request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.UpdateDocumentRename(request.DocumentsModel);
        }

    }
    public class GetSelectedFilePermissionHandler : IRequestHandler<GetSelectedFilePermission, DocumentPermissionModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetSelectedFilePermissionHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentPermissionModel> Handle(GetSelectedFilePermission request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetAllSelectedFilePermissionAsync(request.DocumentId, request.FileProfileTypeID);
        }

    }
    public class GetFileDownloadHandler : IRequestHandler<GetFileDownload, DocumentsModel>
    {
        private readonly IConfiguration _configuration;
        private readonly IDocumentsQueryRepository _documentsQueryRepository;
        public GetFileDownloadHandler(IConfiguration configuration, IDocumentsQueryRepository documentsQueryRepository)
        {
            _configuration = configuration;
            _documentsQueryRepository = documentsQueryRepository;
        }
        public Task<DocumentsModel> Handle(GetFileDownload request, CancellationToken cancellationToken)
        {
            var response = new DocumentsModel
            {
                DocumentID = request.DocumentId.Value,

            };
            var documents = _documentsQueryRepository.GetByOneAsync(request.DocumentId);
            if (documents != null && documents.FilePath != null)
            {
                response.FileName = documents.FileName;
                if (documents.IsNewPath == true)
                {
                    var filePath = "AppUpload/" + documents.FilePath;
                    if (File.Exists(filePath))
                    {
                        response.ImageData = File.ReadAllBytes(filePath);
                    }
                    else
                    {
                        response.StatusCodeID = 0;
                    }
                }
                else
                {
                    var url = _configuration["DocumentsUrl:ServerUrl"] + documents.FilePath;
                    var webClient = new WebClient();
                    {
                        response.ImageData = webClient.DownloadData(new Uri(url));
                    }

                }
            }
            return Task.FromResult(response);
        }

    }
    public class GetInsertDocumentLinkHandler : IRequestHandler<InsertDocumentLink, DocumentLink>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetInsertDocumentLinkHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentLink> Handle(InsertDocumentLink request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.InsertDocumentLink(request.DocumentLink);
        }

    }
    public class DeleteDocumentLinkHandler : IRequestHandler<DeleteDocumentLink, DocumentLinkModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public DeleteDocumentLinkHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentLinkModel> Handle(DeleteDocumentLink request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.DeleteDocumentLink(request.DocumentLink);
        }

    }
    public class GetUserGroupsHandler : IRequestHandler<GetUserGroups, List<UserGroup>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetUserGroupsHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<UserGroup>> Handle(GetUserGroups request, CancellationToken cancellationToken)
        {
            return (List<UserGroup>)await _fileprofileQueryRepository.GetUserGroups();
        }

    }
    public class GetDocumentRolesHandler : IRequestHandler<GetDocumentRoles, List<DocumentRole>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetDocumentRolesHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentRole>> Handle(GetDocumentRoles request, CancellationToken cancellationToken)
        {
            return (List<DocumentRole>)await _fileprofileQueryRepository.GetDocumentRole();
        }

    }
    public class GetDocumentProfilesHandler : IRequestHandler<GetDocumentProfiles, List<DocumentProfileNoSeriesModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetDocumentProfilesHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentProfileNoSeriesModel>> Handle(GetDocumentProfiles request, CancellationToken cancellationToken)
        {
            return (List<DocumentProfileNoSeriesModel>)await _fileprofileQueryRepository.GetDocumentProfiles();
        }

    }
    public class GetDocumentProfileNoSeriesByIdHandler : IRequestHandler<GetDocumentProfileNoSeriesById, DocumentProfileNoSeriesModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetDocumentProfileNoSeriesByIdHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentProfileNoSeriesModel> Handle(GetDocumentProfileNoSeriesById request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetDocumentProfileNoSeriesById(request.Id);
        }

    }
    public class InsertOrUpdateFileProfileTypeHandler : IRequestHandler<InsertOrUpdateFileProfileType, FileProfileTypeModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public InsertOrUpdateFileProfileTypeHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<FileProfileTypeModel> Handle(InsertOrUpdateFileProfileType request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.InsertOrUpdateFileProfileType(request.FileProfileTypeModel);
        }

    }
    public class GetFileProfileTypeBySessionHandler : IRequestHandler<GetFileProfileTypeBySession, FileProfileTypeModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetFileProfileTypeBySessionHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<FileProfileTypeModel> Handle(GetFileProfileTypeBySession request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetFileProfileTypeBySession(request.SessionId);
        }

    }
    public class GetDocumentPermissionByRoleIDHandler : IRequestHandler<GetDocumentPermissionByRoleID, DocumentPermissionModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetDocumentPermissionByRoleIDHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentPermissionModel> Handle(GetDocumentPermissionByRoleID request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.GetDocumentPermissionByRoleID(request.Id);
        }

    }
    public class InsertFileProfileTypeAccessHandler : IRequestHandler<InsertFileProfileTypeAccess, DocumentUserRoleModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public InsertFileProfileTypeAccessHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentUserRoleModel> Handle(InsertFileProfileTypeAccess request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.InsertFileProfileTypeAccess(request.DocumentUserRole);
        }

    }
    public class GetDocumentUserRoleListHandler : IRequestHandler<GetDocumentUserRoleList, List<DocumentUserRoleModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetDocumentUserRoleListHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentUserRoleModel>> Handle(GetDocumentUserRoleList request, CancellationToken cancellationToken)
        {
            return (List<DocumentUserRoleModel>)await _fileprofileQueryRepository.GetDocumentUserRoleList(request.Id);
        }

    }
    public class DeleteDocumentUserRoleHandler : IRequestHandler<DeleteDocumentUserRole, DocumentUserRoleModel>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public DeleteDocumentUserRoleHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<DocumentUserRoleModel> Handle(DeleteDocumentUserRole request, CancellationToken cancellationToken)
        {
            return await _fileprofileQueryRepository.DeleteDocumentUserRole(request.DocumentUserRole);
        }

    }
    public class GetFileProfileSetupFormListHandler : IRequestHandler<GetFileProfileSetupFormList, List<FileProfileSetupFormModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetFileProfileSetupFormListHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<FileProfileSetupFormModel>> Handle(GetFileProfileSetupFormList request, CancellationToken cancellationToken)
        {
            return (List<FileProfileSetupFormModel>)await _fileprofileQueryRepository.GetFileProfileSetupFormList(request.Id);
        }

    }
    public class GetReserveProfileNumberSeriesHandler : IRequestHandler<GetReserveProfileNumberSeries, List<DocumentNoSeriesModel>>
    {
        private readonly IFileprofileQueryRepository _fileprofileQueryRepository;
        public GetReserveProfileNumberSeriesHandler(IFileprofileQueryRepository fileprofileQueryRepository)
        {
            _fileprofileQueryRepository = fileprofileQueryRepository;
        }
        public async Task<List<DocumentNoSeriesModel>> Handle(GetReserveProfileNumberSeries request, CancellationToken cancellationToken)
        {
            return (List<DocumentNoSeriesModel>)await _fileprofileQueryRepository.GetReserveProfileNumberSeries(request.Id, request.ProfileId);
        }

    }

    public class InsertCreateDocumentHandler : IRequestHandler<InsertCreateDocument, DocumentsUploadModel>
    {
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        public InsertCreateDocumentHandler(IDocumentsQueryRepository documentsqueryrepository)
        {
            _documentsqueryrepository = documentsqueryrepository;
        }
        public async Task<DocumentsUploadModel> Handle(InsertCreateDocument request, CancellationToken cancellationToken)
        {
            return await _documentsqueryrepository.InsertCreateDocument(request.DocumentsUploadModel);
        }

    }
    public class UpdateCreateDocumentBySessionHandler : IRequestHandler<UpdateCreateDocumentBySession, DocumentsUploadModel>
    {
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        public UpdateCreateDocumentBySessionHandler(IDocumentsQueryRepository documentsqueryrepository)
        {
            _documentsqueryrepository = documentsqueryrepository;
        }
        public async Task<DocumentsUploadModel> Handle(UpdateCreateDocumentBySession request, CancellationToken cancellationToken)
        {
            return await _documentsqueryrepository.UpdateCreateDocumentBySession(request.DocumentsUploadModel);
        }

    }
    public class InsertOrUpdateReserveProfileNumberSeriesHandler : IRequestHandler<InsertOrUpdateReserveProfileNumberSeries, DocumentNoSeriesModel>
    {
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        public InsertOrUpdateReserveProfileNumberSeriesHandler(IDocumentsQueryRepository documentsqueryrepository)
        {
            _documentsqueryrepository = documentsqueryrepository;
        }
        public async Task<DocumentNoSeriesModel> Handle(InsertOrUpdateReserveProfileNumberSeries request, CancellationToken cancellationToken)
        {
            return await _documentsqueryrepository.InsertOrUpdateReserveProfileNumberSeries(request.DocumentNoSeriesModel);
        }
        public class UpdateCreateDocumentBySessionReserveSeriesHandler : IRequestHandler<UpdateCreateDocumentBySessionReserveSeries, DocumentNoSeriesModel>
        {
            private readonly IDocumentsQueryRepository _documentsqueryrepository;
            public UpdateCreateDocumentBySessionReserveSeriesHandler(IDocumentsQueryRepository documentsqueryrepository)
            {
                _documentsqueryrepository = documentsqueryrepository;
            }
            public async Task<DocumentNoSeriesModel> Handle(UpdateCreateDocumentBySessionReserveSeries request, CancellationToken cancellationToken)
            {
                return await _documentsqueryrepository.UpdateCreateDocumentBySessionReserveSeries(request.DocumentNoSeriesModel);
            }
        }
        public class UpdateReserveNumberDescriptionFieldHandler : IRequestHandler<UpdateReserveNumberDescriptionField, DocumentNoSeriesModel>
        {
            private readonly IDocumentsQueryRepository _documentsqueryrepository;
            public UpdateReserveNumberDescriptionFieldHandler(IDocumentsQueryRepository documentsqueryrepository)
            {
                _documentsqueryrepository = documentsqueryrepository;
            }
            public async Task<DocumentNoSeriesModel> Handle(UpdateReserveNumberDescriptionField request, CancellationToken cancellationToken)
            {
                return await _documentsqueryrepository.UpdateReserveNumberDescriptionField(request.DocumentNoSeriesModel);
            }
        }

    }
}
