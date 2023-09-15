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
    }
    public interface IGenerateDocumentNoSeriesQueryRepository : IQueryRepository<DocumentNoSeriesModel>
    {
        Task<MasterListsModel> GetMasterLists();
        IReadOnlyList<ProfileAutoNumber> GetProfileAutoNumber(long? Id);
        DocumentProfileNoSeries UpdateDocumentProfileNoSeriesLastCreateDate(DocumentProfileNoSeries documentProfileNoSeries);
        DocumentNoSeries InsertDocumentNoSeries(DocumentNoSeries documentNoSeries);
        ProfileAutoNumber InsertProfileAutoNumber(ProfileAutoNumber profileAutoNumber);
        ProfileAutoNumber UpdateDocumentProfileNoSeriesLastNoUsed(ProfileAutoNumber profileAutoNumber);
    }
}
