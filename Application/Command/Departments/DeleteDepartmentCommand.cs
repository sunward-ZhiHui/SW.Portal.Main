using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.Departments
{
    public class DeleteDepartmentCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteDepartmentCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
