using Application.Queries.Base;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllAssetCatalogMasterQuery : PagedRequest, IRequest<List<View_AssetCatalogMaster>>
    {
        public string SearchString { get; set; }
        public long? CategoryId { get; set; }
        public long? SubSectionId { get; set; }
        public GetAllAssetCatalogMasterQuery(long? CategoryId, long? SubSectionId)
        {
            this.CategoryId = CategoryId;
            this.SubSectionId = SubSectionId;
        }
    }
}
