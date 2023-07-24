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
    public class GetAllSalesOrderMasterPricingMasterTypeQuery : PagedRequest, IRequest<List<View_SalesOrderMasterPricing>>
    {
        public string MasterType { get; set; }
        public GetAllSalesOrderMasterPricingMasterTypeQuery( string MasterType)
        {
            this.MasterType= MasterType;
        }
    }
    public class GetAllSalesOrderMasterPricingBySessionQuery : PagedRequest, IRequest<View_SalesOrderMasterPricing>
    {
        public Guid? SessionId { get; set; }
        public GetAllSalesOrderMasterPricingBySessionQuery(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    
}
