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
    public interface IDocumentProfileNoSeriesQueryRepository
    {
        Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetDocumentNoSeriesAsync();
        Task<IReadOnlyList<DocumentProfileNoSeriesModel>> GetAllAsync();
        Task<DocumentProfileNoSeriesModel> GetProfileNoBySession(Guid? SessionId);
        Task<DocumentProfileNoSeriesModel> InsertOrUpdateDocumentProfileNoSeries(DocumentProfileNoSeriesModel value);
        DocumentProfileNoSeriesModel GetProfileNameCheckValidation(string? value, long id);
        Task<long?> GetDocumentNoSeriesCount(long? id);
        Task<DocumentProfileNoSeriesModel> DeleteDocumentProfileNoSeries(DocumentProfileNoSeriesModel documentProfileNoSeriesModel,long? UserId);
        DocumentProfileNoSeriesModel GetAbbreviationCheckValidation(string? value, string? Abbreviation, long id);
        Task<IReadOnlyList<DocumentNoSeries>> GetAllDocumentNoSeriesAsync(DocumentProfileNoSeriesModel documentProfileNoSeriesModel);

    }
}
