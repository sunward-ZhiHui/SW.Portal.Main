using Application.Queries;
using Application.Queries.Base;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllFbOutputCartonsHandler : IRequestHandler<GetAllFbOutputCartonsQuery, List<FbOutputCartons>>
    {

        private readonly IFbOutputCartonsQueryRepository  _CartonsqueryRepository;
        public GetAllFbOutputCartonsHandler(IFbOutputCartonsQueryRepository CartonsqueryRepository)
        {
            _CartonsqueryRepository = CartonsqueryRepository;
        }
        public async Task<List<FbOutputCartons>> Handle(GetAllFbOutputCartonsQuery request, CancellationToken cancellationToken)
        {
            return (List<FbOutputCartons>)await _CartonsqueryRepository.GetAllAsync();
        }
    }
    public class GetAllDispensedmeterialHandler : IRequestHandler<GetAllDispensedMeterialQuery, List<DispensedMeterial>>
    {

        private readonly IFbOutputCartonsQueryRepository _CartonsqueryRepository;
        public GetAllDispensedmeterialHandler(IFbOutputCartonsQueryRepository CartonsqueryRepository)
        {
            _CartonsqueryRepository = CartonsqueryRepository;
        }
        public async Task<List<DispensedMeterial>> Handle(GetAllDispensedMeterialQuery request, CancellationToken cancellationToken)
        {
            return (List<DispensedMeterial>)await _CartonsqueryRepository.GetAllDispensedMeterialAsync();
        }
    }

    public class CreateFbOutputCartonsHandler : IRequestHandler<CreateFbOutputCartonsQuery, long>
    {
        private readonly IFbOutputCartonsQueryRepository _CartonsqueryRepository;
        public CreateFbOutputCartonsHandler(IFbOutputCartonsQueryRepository CartonsqueryRepository)
        {
            _CartonsqueryRepository = CartonsqueryRepository;
        }

        public async Task<long> Handle(CreateFbOutputCartonsQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _CartonsqueryRepository.Insert(request);
            return newlist;

        }

    }

    public class CreateDispensedMeterialHandler : IRequestHandler<CreateDispensedMeterialQuery, long>
    {
        private readonly IFbOutputCartonsQueryRepository _CartonsqueryRepository;
        public CreateDispensedMeterialHandler(IFbOutputCartonsQueryRepository CartonsqueryRepository)
        {
            _CartonsqueryRepository = CartonsqueryRepository;
        }

        public async Task<long> Handle(CreateDispensedMeterialQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _CartonsqueryRepository.DispensedMeterialInsert(request);
            return newlist;

        }

    }
    public class EditFbOutputCartonsHandler : IRequestHandler<EditFbOutputCartonsQuery, long>
    {
        private readonly IFbOutputCartonsQueryRepository _CartonsqueryRepository;
        public EditFbOutputCartonsHandler(IFbOutputCartonsQueryRepository CartonsqueryRepository)
        {
            _CartonsqueryRepository = CartonsqueryRepository;
        }

        public async Task<long> Handle(EditFbOutputCartonsQuery request, CancellationToken cancellationToken)
        {
            var req = await _CartonsqueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteFbOutputCartonsHandler : IRequestHandler<DeleteFbOutputCartonsQuery, long>
    {
        private readonly IFbOutputCartonsQueryRepository _CartonsqueryRepository;

        public DeleteFbOutputCartonsHandler(IFbOutputCartonsQueryRepository CartonsqueryRepository)
        {
            _CartonsqueryRepository = CartonsqueryRepository;
        }

        public async Task<long> Handle(DeleteFbOutputCartonsQuery request, CancellationToken cancellationToken)
        {
            var req = await _CartonsqueryRepository.Delete(request.ID);
            return req;
        }
    }
}
