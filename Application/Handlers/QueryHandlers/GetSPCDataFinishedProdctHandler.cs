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
    public class GetSPCDataFinishedProdctHandler : IRequestHandler<GetSPCDataFinishedProdctList, List<SPCDataFinishedProdct>>
    {
        private readonly ISPCDataFinishedProdctQueryRepository _SPCDataFinishedProdctQueryRepository;

        public GetSPCDataFinishedProdctHandler(ISPCDataFinishedProdctQueryRepository spRegistrationQueryRepository)
        {

            _SPCDataFinishedProdctQueryRepository = spRegistrationQueryRepository;
        }
        public async Task<List<SPCDataFinishedProdct>> Handle(GetSPCDataFinishedProdctList request, CancellationToken cancellationToken)
        {
            return (List<SPCDataFinishedProdct>)await _SPCDataFinishedProdctQueryRepository.GetAllAsync();

        }
    }
    public class CreateSPCDataFinishedProdctHandler : IRequestHandler<CreateSPCDataFinishedProdctQuery, SPCDataFinishedProdct>
    {
        private readonly ISPCDataFinishedProdctQueryRepository _SPCDataFinishedProdctQueryRepository;

        public CreateSPCDataFinishedProdctHandler(ISPCDataFinishedProdctQueryRepository spRegistrationQueryRepository)
        {
            _SPCDataFinishedProdctQueryRepository = spRegistrationQueryRepository;
        }

        public async Task<SPCDataFinishedProdct> Handle(CreateSPCDataFinishedProdctQuery request, CancellationToken cancellationToken)
        {
            var createdInvoiceDocument = await _SPCDataFinishedProdctQueryRepository.InsertAsync(request.SPCDataFinishedProdct);
            return createdInvoiceDocument;
        }
    }
    public class UpdateSPCDataFinishedProdctHandler : IRequestHandler<UpdateSPCDataFinishedProdctQuery, SPCDataFinishedProdct>
    {
        private readonly ISPCDataFinishedProdctQueryRepository _SPCDataFinishedProdctQueryRepository;

        public UpdateSPCDataFinishedProdctHandler(ISPCDataFinishedProdctQueryRepository spRegistrationQueryRepository)
        {
            _SPCDataFinishedProdctQueryRepository = spRegistrationQueryRepository;
        }

        public async Task<SPCDataFinishedProdct> Handle(UpdateSPCDataFinishedProdctQuery request, CancellationToken cancellationToken)
        {
            var updatedInvoiceDocument = await _SPCDataFinishedProdctQueryRepository.UpdateAsync(request.DataItem, request.ChangedItem);
            return updatedInvoiceDocument;
        }
    }
    public class DeleteSPCDataFinishedProdctHandler : IRequestHandler<DeleteSPCDataFinishedProdctQuery, bool>
    {
        private readonly ISPCDataFinishedProdctQueryRepository _SPCDataFinishedProdctQueryRepository;

        public DeleteSPCDataFinishedProdctHandler(ISPCDataFinishedProdctQueryRepository spRegistrationQueryRepository)
        {
            _SPCDataFinishedProdctQueryRepository = spRegistrationQueryRepository;
        }

        public async Task<bool> Handle(DeleteSPCDataFinishedProdctQuery request, CancellationToken cancellationToken)
        {
            return await _SPCDataFinishedProdctQueryRepository.DeleteAsync(request.ID);
        }
    }

}
