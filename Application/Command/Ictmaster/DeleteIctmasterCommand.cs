using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.Ictmaster
{
    public  class DeleteIctmasterCommand : IRequest<String>
    {
    

        public Int64 Id { get; private set; }

        public DeleteIctmasterCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }

}
