using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllDocumentProfileNoSeriesHandler : IRequestHandler<GetDocumentProfileNoSeriesQuery, List<DocumentProfileNoSeriesModel>>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;
        public GetAllDocumentProfileNoSeriesHandler(IDocumentProfileNoSeriesQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DocumentProfileNoSeriesModel>> Handle(GetDocumentProfileNoSeriesQuery request, CancellationToken cancellationToken)
        {
            return (List<DocumentProfileNoSeriesModel>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetDocumentProfileNoSeriesHandler : IRequestHandler<GetDocumentProfileNoOneSeriesQuery, List<DocumentProfileNoSeriesModel>>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;
        public GetDocumentProfileNoSeriesHandler(IDocumentProfileNoSeriesQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DocumentProfileNoSeriesModel>> Handle(GetDocumentProfileNoOneSeriesQuery request, CancellationToken cancellationToken)
        {
            return (List<DocumentProfileNoSeriesModel>)await _queryRepository.GetDocumentNoSeriesAsync();
        }
    }
    public class GetProfileNoBySessionHandler : IRequestHandler<GetProfileNoBySession, DocumentProfileNoSeriesModel>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;
        public GetProfileNoBySessionHandler(IDocumentProfileNoSeriesQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<DocumentProfileNoSeriesModel> Handle(GetProfileNoBySession request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetProfileNoBySession(request.SesionId);
        }
    }
    public class InsertOrUpdateDocumentProfileNoSeriesHandler : IRequestHandler<InsertOrUpdateDocumentProfileNoSeries, DocumentProfileNoSeriesModel>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;
        public InsertOrUpdateDocumentProfileNoSeriesHandler(IDocumentProfileNoSeriesQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<DocumentProfileNoSeriesModel> Handle(InsertOrUpdateDocumentProfileNoSeries request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateDocumentProfileNoSeries(request.DocumentProfileNoSeriesModel);

        }
    }
    public class GetDocumentNoSeriesCountHandler : IRequestHandler<GetDocumentNoSeriesCount, long?>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;
        public GetDocumentNoSeriesCountHandler(IDocumentProfileNoSeriesQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<long?> Handle(GetDocumentNoSeriesCount request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetDocumentNoSeriesCount(request.ProfileId);
        }
    }
    public class DeleteDocumentProfileNoSeriesHandler : IRequestHandler<DeleteDocumentProfileNoSeries, DocumentProfileNoSeriesModel>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;

        public DeleteDocumentProfileNoSeriesHandler(IDocumentProfileNoSeriesQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<DocumentProfileNoSeriesModel> Handle(DeleteDocumentProfileNoSeries request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteDocumentProfileNoSeries(request.DocumentProfileNoSeriesModel);
        }
    }
    public class GetAllDocumentNoSeriesHandler : IRequestHandler<GetAllDocumentNoSeriesQuery, List<DocumentNoSeries>>
    {
        private readonly IDocumentProfileNoSeriesQueryRepository _queryRepository;
        public GetAllDocumentNoSeriesHandler(IDocumentProfileNoSeriesQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<DocumentNoSeries>> Handle(GetAllDocumentNoSeriesQuery request, CancellationToken cancellationToken)
        {
            return (List<DocumentNoSeries>)await _queryRepository.GetAllDocumentNoSeriesAsync(request.DocumentProfileNoSeriesModel);
        }
    }
}
