using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.FmGlobals
{
    public class DeleteFmGlobalCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteFmGlobalCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteFmGlobalLineCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteFmGlobalLineCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteFmGlobalLineItemCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteFmGlobalLineItemCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
