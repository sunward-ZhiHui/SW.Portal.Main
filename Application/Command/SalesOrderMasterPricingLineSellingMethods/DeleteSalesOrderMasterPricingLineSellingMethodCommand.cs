using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SalesOrderMasterPricingLineSellingMethods
{
    public class DeleteSalesOrderMasterPricingLineSellingMethodCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteSalesOrderMasterPricingLineSellingMethodCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
