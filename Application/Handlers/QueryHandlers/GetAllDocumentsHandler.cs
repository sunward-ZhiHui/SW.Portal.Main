using Application.Queries;
using Core.Entities;
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
    public class GetAllDocumentsHandler : IRequestHandler<GetAllDocumentsQuery, Documents>
    {
        private readonly IDocumentsQueryRepository _queryRepository;
        public GetAllDocumentsHandler(IDocumentsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<Documents> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetByIdAsync(request.SessionId.Value);
        }
    }
}
