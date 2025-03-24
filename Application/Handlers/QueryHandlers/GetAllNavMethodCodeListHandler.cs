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
    public class GetAllNavMethodCodeListHandler : IRequestHandler<GetAllNavMethodCodeQuery, List<NavMethodCodeModel>>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public GetAllNavMethodCodeListHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavMethodCodeModel>> Handle(GetAllNavMethodCodeQuery request, CancellationToken cancellationToken)
        {
            return (List<NavMethodCodeModel>)await _queryRepository.GetNavMethodCodeAsync();
        }
    }
    public class InsertOrUpdateNavMethodCodeHandler : IRequestHandler<InsertOrUpdateNavMethodCode, NavMethodCodeModel>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public InsertOrUpdateNavMethodCodeHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavMethodCodeModel> Handle(InsertOrUpdateNavMethodCode request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateNavMethodCode(request);

        }
    }

    public class DeleteNavMethodCodeHandler : IRequestHandler<DeleteNavMethodCode, NavMethodCodeModel>
    {
        private readonly INavMethodCodeQueryRepository _queryRepository;
        public DeleteNavMethodCodeHandler(INavMethodCodeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<NavMethodCodeModel> Handle(DeleteNavMethodCode request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteNavMethodCode(request.NavMethodCodeModel);
        }

    }
}
