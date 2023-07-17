using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SalesOrderMasterPricings
{
    public class DeleteSalesOrderMasterPricingCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteSalesOrderMasterPricingCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteSalesOrderMasterPricingLineCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteSalesOrderMasterPricingLineCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
