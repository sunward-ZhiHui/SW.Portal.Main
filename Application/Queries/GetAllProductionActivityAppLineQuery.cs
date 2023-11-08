
using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
   
     public class GetAllProductionActivityAppLineQuery : PagedRequest, IRequest<List<ProductActivityAppModel>>
    {
        public long? CompanyID { get; set; }
        public string? ProdorderNo { get; set; }
        public long? LocationID { get; set; }
        public long? UserId { get; set; }
        public GetAllProductionActivityAppLineQuery(long? CompanyID, string? prodorderNo, long? userId, long? locationID)
        {
            this.CompanyID = CompanyID;
            this.ProdorderNo = prodorderNo;
            this.LocationID = locationID;
            this.UserId = userId;
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
