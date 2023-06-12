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
    public class GetAllAssetCatalogMasterHandler : IRequestHandler<GetAllAssetCatalogMasterQuery, List<View_AssetCatalogMaster>>
    {
        private readonly IAssetCatalogMasterQueryRepository _queryRepository;
        public GetAllAssetCatalogMasterHandler(IAssetCatalogMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_AssetCatalogMaster>> Handle(GetAllAssetCatalogMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<View_AssetCatalogMaster>)await _queryRepository.GetAllAsync(request.CategoryId, request.SubSectionId);
        }
    }
}
