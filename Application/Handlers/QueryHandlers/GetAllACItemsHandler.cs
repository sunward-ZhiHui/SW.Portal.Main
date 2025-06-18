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
    public class GetAllACItemsHandler : IRequestHandler<GetAllACItemsQuery, List<ACItemsModel>>
    {
        private readonly IACItemsQueryRepository _queryRepository;
        public GetAllACItemsHandler(IACItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ACItemsModel>> Handle(GetAllACItemsQuery request, CancellationToken cancellationToken)
        {
            return (List<ACItemsModel>)await _queryRepository.GetAllByAsync(request.ACItemsModel);
        }
    }
    public class GetDDACItemsHandler : IRequestHandler<GetDDACItems, List<ACItemsModel>>
    {
        private readonly IACItemsQueryRepository _queryRepository;
        public GetDDACItemsHandler(IACItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ACItemsModel>> Handle(GetDDACItems request, CancellationToken cancellationToken)
        {
            return (List<ACItemsModel>)await _queryRepository.GetDDACItems();
        }
    }

    public class GetNavItemCitemListHandler : IRequestHandler<GetNavItemCitemList, List<NavItemCitemList>>
    {
        private readonly IACItemsQueryRepository _queryRepository;
        public GetNavItemCitemListHandler(IACItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavItemCitemList>> Handle(GetNavItemCitemList request, CancellationToken cancellationToken)
        {
            return (List<NavItemCitemList>)await _queryRepository.GetNavItemCitemList(request.ItemId);
        }
    }
    public class InsertOrUpdateAcitemsHandler : IRequestHandler<InsertOrUpdateAcitems, ACItemsModel>
    {
        private readonly IACItemsQueryRepository _queryRepository;
        public InsertOrUpdateAcitemsHandler(IACItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ACItemsModel> Handle(InsertOrUpdateAcitems request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateAcitems(request);

        }
    }

    public class DeleteACItemsHandler : IRequestHandler<DeleteACItems, ACItemsModel>
    {
        private readonly IACItemsQueryRepository _queryRepository;
        public DeleteACItemsHandler(IACItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ACItemsModel> Handle(DeleteACItems request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteACItems(request.ACItemsModel);
        }

    }
}
