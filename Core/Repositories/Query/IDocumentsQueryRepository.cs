
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
        Task<Documents> GetByDocIdAsync(long? Id); 
        Task<List<Documents>> GetByUniqueDocAsync(string Ids);
        Task<long> Delete(long? DocumentId);
        Task<byte[]> GetByteFromUrl(string Url);
        Documents GetByOneAsync(long? DocumentId);
        Task<DocumentsUploadModel> InsertCreateDocument(DocumentsUploadModel value);
        Task<Documents> UpdateDocumentAfterUpload(Documents value);
        Task<long> UpdateEmailDMS(long DocId,long ActivityId);
        Task<Documents> InsertCreateDocumentBySession(Documents value);
        Task<DocumentsUploadModel> UpdateCreateDocumentBySession(DocumentsUploadModel value);
        Task<DocumentsUploadModel> UpdateEmailDocumentBySession(DocumentsUploadModel value);        
        Task<DocumentNoSeriesModel> InsertOrUpdateReserveProfileNumberSeries(DocumentNoSeriesModel documentNoSeries);
        Task<DocumentNoSeriesModel> UpdateCreateDocumentBySessionReserveSeries(DocumentNoSeriesModel documentNoSeries);
        Task<DocumentNoSeriesModel> UpdateReserveNumberDescriptionField(DocumentNoSeriesModel documentNoSeries);

    }
}
