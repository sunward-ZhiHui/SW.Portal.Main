using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeOtherDutyInformations
{
    public class DeleteEmployeeOtherDutyInformationCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteEmployeeOtherDutyInformationCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
