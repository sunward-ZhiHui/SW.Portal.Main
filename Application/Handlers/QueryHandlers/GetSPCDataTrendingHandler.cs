using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetSPCDataTrendingHandler : IRequestHandler<GetSPCDataTrendingList, List<SPCDataTrending>>
    {
        private readonly ISPCDataTrendingQueryRepository _SPCDataTrendingQueryRepository;

        public GetSPCDataTrendingHandler(ISPCDataTrendingQueryRepository spRegistrationQueryRepository)
        {

            _SPCDataTrendingQueryRepository = spRegistrationQueryRepository;
        }
        public async Task<List<SPCDataTrending>> Handle(GetSPCDataTrendingList request, CancellationToken cancellationToken)
        {
            return (List<SPCDataTrending>)await _SPCDataTrendingQueryRepository.GetAllAsync();

        }
    }
    public class CreateSPCDataTrendingHandler : IRequestHandler<CreateSPCDataTrendingQuery, SPCDataTrending>
    {
        private readonly ISPCDataTrendingQueryRepository _SPCDataTrendingQueryRepository;

        public CreateSPCDataTrendingHandler(ISPCDataTrendingQueryRepository spRegistrationQueryRepository)
        {
            _SPCDataTrendingQueryRepository = spRegistrationQueryRepository;
        }

        public async Task<SPCDataTrending> Handle(CreateSPCDataTrendingQuery request, CancellationToken cancellationToken)
        {
            var createdInvoiceDocument = await _SPCDataTrendingQueryRepository.InsertAsync(request.SPCDataTrending);
            return createdInvoiceDocument;
        }
    }
    public class UpdateSPCDataTrendingHandler : IRequestHandler<UpdateSPCDataTrendingQuery, SPCDataTrending>
    {
        private readonly ISPCDataTrendingQueryRepository _SPCDataTrendingQueryRepository;

        public UpdateSPCDataTrendingHandler(ISPCDataTrendingQueryRepository spRegistrationQueryRepository)
        {
            _SPCDataTrendingQueryRepository = spRegistrationQueryRepository;
        }

        public async Task<SPCDataTrending> Handle(UpdateSPCDataTrendingQuery request, CancellationToken cancellationToken)
        {
            var updatedInvoiceDocument = await _SPCDataTrendingQueryRepository.UpdateAsync(request.DataItem, request.ChangedItem);
            return updatedInvoiceDocument;
        }
    }
    public class DeleteSPCDataTrendingHandler : IRequestHandler<DeleteSPCDataTrendingQuery, bool>
    {
        private readonly ISPCDataTrendingQueryRepository _SPCDataTrendingQueryRepository;

        public DeleteSPCDataTrendingHandler(ISPCDataTrendingQueryRepository spRegistrationQueryRepository)
        {
            _SPCDataTrendingQueryRepository = spRegistrationQueryRepository;
        }

        public async Task<bool> Handle(DeleteSPCDataTrendingQuery request, CancellationToken cancellationToken)
        {
            return await _SPCDataTrendingQueryRepository.DeleteAsync(request.ID);
        }
    }

}
