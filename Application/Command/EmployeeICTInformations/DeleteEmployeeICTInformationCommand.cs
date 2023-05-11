using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeICTInformations
{
    public class DeleteEmployeeICTInformationCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteEmployeeICTInformationCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteEmployeeICTHardInformationCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteEmployeeICTHardInformationCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
