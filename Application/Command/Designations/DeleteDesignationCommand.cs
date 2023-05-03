using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.designations
{
    public class DeleteDesignationCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteDesignationCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
