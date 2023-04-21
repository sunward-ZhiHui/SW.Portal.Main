using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class DeleteRoleCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteRoleCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
