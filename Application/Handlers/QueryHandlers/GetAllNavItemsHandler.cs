using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllNavItemsHandler : IRequestHandler<GetAllNavItemsQuery, List<View_NavItems>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_NavItems>> Handle(GetAllNavItemsQuery request, CancellationToken cancellationToken)
        {
            return (List<View_NavItems>)await _queryRepository.GetAsyncList();
        }
    }
    public class GetAllNavItemsItemSerialNoHandler : IRequestHandler<GetAllNavItemsItemSerialNoQuery, View_NavItems>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsItemSerialNoHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<View_NavItems> Handle(GetAllNavItemsItemSerialNoQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetByItemSerialNoAsync(request.ItemSerialNo);
        }
    }
}
