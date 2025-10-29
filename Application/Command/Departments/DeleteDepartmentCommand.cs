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
        public long? UserId { get; set; }

        public DeleteDepartmentCommand(Int64 Id, long? userId)
        {
            this.Id = Id;
            UserId = userId;
        }
    }
}
