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
    public class GetDocumentProfileNoSeriesQuery : PagedRequest, IRequest<List<DocumentProfileNoSeriesModel>>
    {
    }
    public class GetDocumentProfileNoOneSeriesQuery : PagedRequest, IRequest<List<DocumentProfileNoSeriesModel>>
    {
    }
    public class GetProfileNoBySession : PagedRequest, IRequest<DocumentProfileNoSeriesModel>
    {
        public string? SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetProfileNoBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }

    public class InsertOrUpdateDocumentProfileNoSeries : PagedRequest, IRequest<DocumentProfileNoSeriesModel>
    {
        public DocumentProfileNoSeriesModel DocumentProfileNoSeriesModel { get; set; }
        public InsertOrUpdateDocumentProfileNoSeries(DocumentProfileNoSeriesModel documentProfileNoSeriesModel)
        {
            this.DocumentProfileNoSeriesModel = documentProfileNoSeriesModel;
        }
    }
    public class GetDocumentNoSeriesCount : PagedRequest, IRequest<long?>
    {
        public long? ProfileId { get; set; }
        public GetDocumentNoSeriesCount(long? profileId)
        {
            this.ProfileId = profileId;
        }
    }
    public class DeleteDocumentProfileNoSeries : DocumentProfileNoSeriesModel, IRequest<DocumentProfileNoSeriesModel>
    {
        public DocumentProfileNoSeriesModel DocumentProfileNoSeriesModel { get; set; }
        public long? UserId {  get; set; }

        public DeleteDocumentProfileNoSeries(DocumentProfileNoSeriesModel documentProfileNoSeriesModel, long? userId)
        {
            this.DocumentProfileNoSeriesModel = documentProfileNoSeriesModel;
            UserId = userId;
        }
    }
    public class GetAllDocumentNoSeriesQuery : PagedRequest, IRequest<List<DocumentNoSeries>>
    {
        public DocumentProfileNoSeriesModel DocumentProfileNoSeriesModel { get; set; }
        public GetAllDocumentNoSeriesQuery(DocumentProfileNoSeriesModel documentProfileNoSeriesModel)
        {
            this.DocumentProfileNoSeriesModel = documentProfileNoSeriesModel;
        }
    }
}
