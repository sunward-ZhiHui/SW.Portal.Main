using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSOMPricingLineSellingMethodQuery : SalesOrderMasterPricingLineSellingMethod, IRequest<List<SalesOrderMasterPricingLineSellingMethod>>
    {
        public long SalesOrderMasterPricingLineId { get; set; }
        public string SellingMethodName { get; set; }
        public GetAllSOMPricingLineSellingMethodQuery(long id, string sellingMethodName)
        {
            this.SalesOrderMasterPricingLineId = id;
            SellingMethodName = sellingMethodName;
        }
    }
}
