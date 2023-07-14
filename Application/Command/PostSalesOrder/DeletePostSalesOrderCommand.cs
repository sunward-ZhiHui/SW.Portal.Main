using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.PostSalesOrder
{
    public class DeletePostSalesOrderCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeletePostSalesOrderCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
   
}
