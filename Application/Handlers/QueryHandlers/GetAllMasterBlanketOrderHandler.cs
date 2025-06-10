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
    public class GetAllMasterBlanketOrderHandler : IRequestHandler<GetAllMasterBlanketOrderQuery, List<MasterBlanketOrderModel>>
    {
        private readonly IMasterBlanketOrderQueryRepository _queryRepository;
        public GetAllMasterBlanketOrderHandler(IMasterBlanketOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<MasterBlanketOrderModel>> Handle(GetAllMasterBlanketOrderQuery request, CancellationToken cancellationToken)
        {
            return (List<MasterBlanketOrderModel>)await _queryRepository.GetAllByAsync();
        }
    }
    public class InsertOrUpdateMasterBlanketOrderHandler : IRequestHandler<InsertOrUpdateMasterBlanketOrder, MasterBlanketOrderModel>
    {
        private readonly IMasterBlanketOrderQueryRepository _queryRepository;
        public InsertOrUpdateMasterBlanketOrderHandler(IMasterBlanketOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<MasterBlanketOrderModel> Handle(InsertOrUpdateMasterBlanketOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateMasterBlanketOrder(request);

        }
    }
    public class DeleteMasterBlanketOrderHandler : IRequestHandler<DeleteMasterBlanketOrder, MasterBlanketOrderModel>
    {
        private readonly IMasterBlanketOrderQueryRepository _queryRepository;
        public DeleteMasterBlanketOrderHandler(IMasterBlanketOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<MasterBlanketOrderModel> Handle(DeleteMasterBlanketOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteMasterBlanketOrder(request.MasterBlanketOrderModel);
        }

    }
}
