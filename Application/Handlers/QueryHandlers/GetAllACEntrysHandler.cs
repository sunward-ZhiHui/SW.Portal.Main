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
    public class GetAllACEntrysHandler : IRequestHandler<GetAllACEntrysQuery, List<ACEntryModel>>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public GetAllACEntrysHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ACEntryModel>> Handle(GetAllACEntrysQuery request, CancellationToken cancellationToken)
        {
            return (List<ACEntryModel>)await _queryRepository.GetAllByAsync();
        }
    }
    public class GetAllNavCustomerHandler : IRequestHandler<GetAllNavCustomerQuery, List<NavCustomerModel>>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public GetAllNavCustomerHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavCustomerModel>> Handle(GetAllNavCustomerQuery request, CancellationToken cancellationToken)
        {
            return (List<NavCustomerModel>)await _queryRepository.GetNavCustomerAsync();
        }
    }
    public class InsertOrUpdateAcentryHandler : IRequestHandler<InsertOrUpdateAcentry, ACEntryModel>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public InsertOrUpdateAcentryHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ACEntryModel> Handle(InsertOrUpdateAcentry request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateAcentry(request);

        }
    }

    public class DeleteACEntryHandler : IRequestHandler<DeleteACEntry, ACEntryModel>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public DeleteACEntryHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ACEntryModel> Handle(DeleteACEntry request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteACEntry(request.ACEntryModel);
        }

    }
    public class CopyACEntryHandler : IRequestHandler<CoptyACEntry, ACEntryModel>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public CopyACEntryHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ACEntryModel> Handle(CoptyACEntry request, CancellationToken cancellationToken)
        {
            return await _queryRepository.CopyAcentry(request);
        }

    }
    public class GetAllACEntryLinesHandler : IRequestHandler<GetAllACEntryLinesQuery, List<ACEntryLinesModel>>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public GetAllACEntryLinesHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ACEntryLinesModel>> Handle(GetAllACEntryLinesQuery request, CancellationToken cancellationToken)
        {
            return (List<ACEntryLinesModel>)await _queryRepository.GetACEntryLinesByAsync(request.ACEntryId);
        }
    }
    public class DeleteACEntryLineHandler : IRequestHandler<DeleteACEntryLine, ACEntryLinesModel>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public DeleteACEntryLineHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ACEntryLinesModel> Handle(DeleteACEntryLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteACEntryLine(request.ACEntryLinesModel);
        }

    }
    public class InsertOrUpdateAcentryLineHandler : IRequestHandler<InsertOrUpdateAcentryLine, ACEntryLinesModel>
    {
        private readonly IACEntrysQueryRepository _queryRepository;
        public InsertOrUpdateAcentryLineHandler(IACEntrysQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ACEntryLinesModel> Handle(InsertOrUpdateAcentryLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateAcentryLine(request);

        }
    }
}
