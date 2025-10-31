using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeEmailInfos
{
    public class DeleteEmployeeEmailInfoCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }
        public long? UserId { get; private set; }
        public string? UserName { get; private set; }

        public DeleteEmployeeEmailInfoCommand(Int64 Id, long? userId, string? userName)
        {
            this.Id = Id;
            UserId = userId;
            UserName = userName;
        }
    }
    public class DeleteEmployeeEmailInfoForwardCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteEmployeeEmailInfoForwardCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteEmployeeEmailInfoAuthorityCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteEmployeeEmailInfoAuthorityCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
}
