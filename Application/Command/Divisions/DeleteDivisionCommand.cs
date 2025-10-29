using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class DeleteDivisionCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }
        public long UserId { get; private set; }
        public DeleteDivisionCommand(Int64 Id, long userId)
        {
            this.Id = Id;
            UserId = userId;
        }
    }
}
