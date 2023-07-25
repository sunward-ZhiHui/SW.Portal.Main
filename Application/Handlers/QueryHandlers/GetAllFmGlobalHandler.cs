using Application.Queries;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    //Fmglobal
    public class GetAllFmGlobalHandler : IRequestHandler<GetAllFmGlobalQuery, List<ViewFmglobal>>
    {
        private readonly IFmGlobalQueryRepository _queryRepository;
        public GetAllFmGlobalHandler(IFmGlobalQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewFmglobal>> Handle(GetAllFmGlobalQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewFmglobal>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetAllFmGlobalByIdHandler : IRequestHandler<GetAllFmGlobalByIdQuery, ViewFmglobal>
    {
        private readonly IFmGlobalQueryRepository _queryRepository;
        public GetAllFmGlobalByIdHandler(IFmGlobalQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ViewFmglobal> Handle(GetAllFmGlobalByIdQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetByIdAsync(request.Id.Value);
        }
    }
    public class GetAllFmGlobalBySessionHandler : IRequestHandler<GetAllFmGlobalBySessionQuery, ViewFmglobal>
    {
        private readonly IFmGlobalQueryRepository _queryRepository;
        public GetAllFmGlobalBySessionHandler(IFmGlobalQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ViewFmglobal> Handle(GetAllFmGlobalBySessionQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetBySessionIdAsync(request.SessionId);
        }
    }

    //FmglobalLine
    public class GetAllFmGlobalLineHandler : IRequestHandler<GetAllFmGlobalLineQuery, List<ViewFmglobalLine>>
    {
        private readonly IFmGlobalLineQueryRepository _queryRepository;
        public GetAllFmGlobalLineHandler(IFmGlobalLineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewFmglobalLine>> Handle(GetAllFmGlobalLineQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewFmglobalLine>)await _queryRepository.GetAllAsync(request.Id.Value);
        }
    }
    public class GetAllFmGlobalLineBySessionHandler : IRequestHandler<GetAllFmGlobalLineBySessionQuery, ViewFmglobalLine>
    {
        private readonly IFmGlobalLineQueryRepository _queryRepository;
        public GetAllFmGlobalLineBySessionHandler(IFmGlobalLineQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ViewFmglobalLine> Handle(GetAllFmGlobalLineBySessionQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetBySessionIdAsync(request.SessionId);
        }
    }
    //FmglobalLineItem
    public class GetAllFmGlobalLineItemHandler : IRequestHandler<GetAllFmGlobalLineItemQuery, List<ViewFmglobalLineItem>>
    {
        private readonly IFmGlobalLineItemQueryRepository _queryRepository;
        public GetAllFmGlobalLineItemHandler(IFmGlobalLineItemQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewFmglobalLineItem>> Handle(GetAllFmGlobalLineItemQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewFmglobalLineItem>)await _queryRepository.GetAllAsync(request.Id.Value);
        }
    }
}
