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
    public class GetAllSalesOrderMasterPricingLineQuery : View_SalesOrderMasterPricingLine, IRequest<List<View_SalesOrderMasterPricingLine>>
    {
        public long SalesOrderMasterPricingId { get; set; }
        public GetAllSalesOrderMasterPricingLineQuery(long id)
        {
            this.SalesOrderMasterPricingId = id;
        }
    }
}
