
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
    public interface IDocumentsQueryRepository : IQueryRepository<Documents>
    {
        Task<Documents> GetBySessionIdAsync(Guid? SessionId);
        Task<Documents> GetByIdAsync(long? Id);
        Task<long> Delete(long? DocumentId);
        Task<byte[]> GetByteFromUrl(string Url);
        Documents GetByOneAsync(long? DocumentId);
        Task<DocumentsUploadModel> InsertCreateDocument(DocumentsUploadModel value);
        Task<Documents> UpdateDocumentAfterUpload(Documents value);
        Task<Documents> InsertCreateDocumentBySession(Documents value);
    }
}
