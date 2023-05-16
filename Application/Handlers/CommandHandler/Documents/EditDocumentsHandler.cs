using Application.Command.Document;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class EditDocumentsHandler : IRequestHandler<EditDocumentCommand, DocumentsResponse>
    {
        private readonly IDocumentsCommandRepository _commandRepository;
        private readonly IDocumentsQueryRepository _queryRepository;
        public EditDocumentsHandler(IDocumentsCommandRepository commandRepository, IDocumentsQueryRepository queryRepository)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
        }
        public async Task<DocumentsResponse> Handle(EditDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var doc = await _queryRepository.GetBySessionIdAsync(request.SessionId);
                var documents = new Documents();
                documents.DocumentId = doc.DocumentId;
                documents.FileName = doc.FileName;
                documents.DisplayName = request.DisplayName;
                documents.Extension = doc.Extension;
                documents.ContentType = doc.ContentType;
                documents.DocumentType = doc.DocumentType;
                documents.FileData = doc.FileData;
                documents.FileSize = doc.FileSize;
                documents.UploadDate = doc.UploadDate;
                documents.SessionId = doc.SessionId;
                documents.LinkId = request.LinkId;
                documents.IsSpecialFile = request.IsSpecialFile;
                documents.IsTemp = request.IsTemp;
                documents.DepartmentId = request.DepartmentId;
                documents.WikiId = request.WikiId;
                documents.CategoryId = request.CategoryId;
                documents.StatusCodeId = request.StatusCodeId;
                documents.ReferenceNumber = request.ReferenceNumber;
                documents.Description = request.Description;
                documents.AddedByUserId = request.AddedByUserId;
                documents.AddedDate = doc.AddedDate;
                documents.ModifiedByUserId = request.ModifiedByUserId;
                documents.ModifiedDate = request.ModifiedDate;
                documents.FilterProfileTypeId = request.FilterProfileTypeId;
                documents.ProfileNo = request.ProfileNo;
                documents.TableName = request.TableName;
                documents.DocumentParentId = request.DocumentParentId;
                documents.ScreenId = request.ScreenId;
                documents.IsLatest = doc.IsLatest;
                documents.ExpiryDate = request.ExpiryDate;
                documents.IsLocked = request.IsLocked;
                documents.LockedDate = request.LockedDate;
                documents.LockedByUserId = request.LockedByUserId;
                documents.IsWikiDraft = request.IsWikiDraft;
                documents.IsWikiDraftDelete = request.IsWikiDraftDelete;
                documents.IsMobileUpload = request.IsMobileUpload;
                documents.IsCompressed = request.IsCompressed;
                documents.IsHeaderImage = request.IsHeaderImage;
                documents.FileIndex = request.FileIndex;
                documents.IsVideoFile = request.IsVideoFile;
                documents.CloseDocumentId = request.CloseDocumentId;
                documents.ArchiveStatusId = request.ArchiveStatusId;
                documents.IsPublichFolder = request.IsPublichFolder;
                documents.FolderId = request.FolderId;
                documents.TaskId = request.TaskId;
                documents.IsMainTask = request.IsMainTask;
                documents.IsPrint = request.IsPrint;
                documents.IsWiki = request.IsWiki;
                documents.SubjectName = request.SubjectName;
                documents.FilePath = doc.FilePath;
                await _commandRepository.UpdateAsync(documents);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new DocumentsResponse
            {
                DocumentId = request.DocumentId,
                AddedByUserId = request.AddedByUserId,
                StatusCodeId = request.StatusCodeId,
            };
            return response;
        }
    }
}
