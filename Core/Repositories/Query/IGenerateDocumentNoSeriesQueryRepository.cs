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
    public interface IGenerateDocumentNoSeriesSeviceQueryRepository : IQueryRepository<DocumentNoSeriesModel>
    {
        Task<string> GenerateDocumentProfileAutoNumber(DocumentNoSeriesModel noSeriesModel);
        Task<GenerateDocumentNoSeriesModel> GenerateDocumentProfileAutoNumberAllAsync(DocumentNoSeriesModel noSeriesModel);
        Task<string> GenerateSampleDocumentNoAsync(DocumentNoSeriesModel noSeriesModel);
    }
    public interface IGenerateDocumentNoSeriesQueryRepository : IQueryRepository<DocumentNoSeriesModel>
    {
        Task<MasterListsModel> GetMasterLists(DocumentNoSeriesModel documentNoSeriesModel);
        Task<IReadOnlyList<ProfileAutoNumber>> GetProfileAutoNumber(long? Id, DocumentNoSeriesModel documentNoSeriesModel);
        Task<DocumentProfileNoSeries> UpdateDocumentProfileNoSeriesLastCreateDate(DocumentProfileNoSeries documentProfileNoSeries);
        Task<DocumentNoSeries> InsertDocumentNoSeries(DocumentNoSeries documentNoSeries);
        Task<ProfileAutoNumber> InsertProfileAutoNumber(ProfileAutoNumber profileAutoNumber);
        Task<ProfileAutoNumber> UpdateDocumentProfileNoSeriesLastNoUsed(ProfileAutoNumber profileAutoNumber);
    }
}
