using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SalesOrderMasterPricingLine
{   
    public class DeleteSalesOrderMasterPricingLineCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteSalesOrderMasterPricingLineCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
