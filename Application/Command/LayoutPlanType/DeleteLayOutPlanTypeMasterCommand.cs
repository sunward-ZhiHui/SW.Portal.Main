using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.LayoutPlanType
{
    public class DeleteLayOutPlanTypeMasterCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteLayOutPlanTypeMasterCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    
}
