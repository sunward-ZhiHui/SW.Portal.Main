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
        public long? UserId { get; private set; }

        public DeleteEmployeeICTInformationCommand(Int64 Id, long? userId)
        {
            this.Id = Id;
            UserId = userId;
        }
    }
    public class DeleteEmployeeICTHardInformationCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }
        public long? UserId { get; private set; }

        public DeleteEmployeeICTHardInformationCommand(Int64 Id, long? userId)
        {
            this.Id = Id;
            UserId = userId;
        }
    }
}
