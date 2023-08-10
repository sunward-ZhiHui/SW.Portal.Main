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
    public class GetAllFileProfileTypesQueryHandler : IRequestHandler<GetAllFileProfileTypeDocuments, List<view_FileProfileTypeDocument>>
    {
        private readonly IFileProfileTypeDocumentQueryRepository _fileProfileTypeDocumentQueryRepository;
        public GetAllFileProfileTypesQueryHandler(IFileProfileTypeDocumentQueryRepository fileProfileTypeDocumentQueryRepository)
        {
            _fileProfileTypeDocumentQueryRepository = fileProfileTypeDocumentQueryRepository;
        }
        public async Task<List<view_FileProfileTypeDocument>> Handle(GetAllFileProfileTypeDocuments request, CancellationToken cancellationToken)
        {
            return (List<view_FileProfileTypeDocument>)await _fileProfileTypeDocumentQueryRepository.GetAllFileProfileDocumentAsync();
        }

    }
   
}
