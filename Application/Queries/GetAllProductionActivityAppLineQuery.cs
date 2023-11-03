
using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
   
     public class GetAllProductionActivityAppLineQuery : PagedRequest, IRequest<List<ProductionActivityAppLine>>
    {
        public long? companyID { get; set; }
        public string? prodorderNo { get; set; }
        public long? LocationID { get; set; }
        public GetAllProductionActivityAppLineQuery(long? companyid, string?  prododerno ,long? locationid)
        {
            this.companyID = companyid;
            this.prodorderNo = prododerno;
            this.LocationID = locationid;
        }
    }

  
    public class GetAllProductionActivitylocQuery : PagedRequest, IRequest<List<ProductionActivityApp>>
    {
        public long? ProductionActivityAppID { get; set; }
        public GetAllProductionActivitylocQuery(long? location)
        {
            this.ProductionActivityAppID = location;

        }
    }

    public class CreateProductionActivityAppLineCommand : ProductionActivityAppLine, IRequest<long>
    {
    }
}
