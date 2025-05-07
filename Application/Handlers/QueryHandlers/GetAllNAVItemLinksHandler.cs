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
    public class GetAllNAVItemLinksHandler : IRequestHandler<GetAllNAVItemLinksQuery, List<NAVItemLinksModel>>
    {
        private readonly INAVItemLinksQueryRepository _queryRepository;
        public GetAllNAVItemLinksHandler(INAVItemLinksQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NAVItemLinksModel>> Handle(GetAllNAVItemLinksQuery request, CancellationToken cancellationToken)
        {
            return (List<NAVItemLinksModel>)await _queryRepository.GetAllByAsync();
        }
    }
    public class InsertOrUpdateNavitemLinksHandler : IRequestHandler<InsertOrUpdateNavitemLinks, NAVItemLinksModel>
    {
        private readonly INAVItemLinksQueryRepository _queryRepository;
        public InsertOrUpdateNavitemLinksHandler(INAVItemLinksQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NAVItemLinksModel> Handle(InsertOrUpdateNavitemLinks request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateNavitemLinks(request);

        }
    }

    public class DeleteNavitemLinksHandler : IRequestHandler<DeleteNavitemLinks, NAVItemLinksModel>
    {
        private readonly INAVItemLinksQueryRepository _queryRepository;
        public DeleteNavitemLinksHandler(INAVItemLinksQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<NAVItemLinksModel> Handle(DeleteNavitemLinks request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteNavitemLinks(request.NAVItemLinksModel);
        }

    }
}
