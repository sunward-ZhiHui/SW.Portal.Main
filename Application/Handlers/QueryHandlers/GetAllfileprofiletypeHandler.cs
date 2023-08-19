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
    
    public class GetFileDownloadHandler : IRequestHandler<GetFileDownload, DocumentsModel>
    {
        private readonly IConfiguration _configuration;
        public GetFileDownloadHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<DocumentsModel> Handle(GetFileDownload request, CancellationToken cancellationToken)
        {
            var response = new DocumentsModel
            {
                DocumentID = request.DocumentsModel.DocumentID,
                FileName = request.DocumentsModel.FileName,
            };
            if (request.DocumentsModel.FilePath != null)
            {
                var url = _configuration["DocumentsUrl:ServerUrl"] + request.DocumentsModel.FilePath;
                var webClient = new WebClient();
                {
                    response.ImageData = webClient.DownloadData(new Uri(url));
                }
            }
            return Task.FromResult(response);
        }

    }
}
