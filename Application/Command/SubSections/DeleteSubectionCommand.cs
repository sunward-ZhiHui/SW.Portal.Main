using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SubSections
{
    public class DeleteSubSectionCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteSubSectionCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
