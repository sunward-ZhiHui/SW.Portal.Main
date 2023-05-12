using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeEmailInfos
{
    public class EditEmployeeEmailInfoCommand: IRequest<EmployeeEmailInfoResponse>
    {
        public long EmployeeEmailInfoID { get; set; }
        public long? EmployeeID { get; set; }
        public long? SubscriptionID { get; set; }
        public long? EmailGuideID { get; set; }
        public EditEmployeeEmailInfoCommand()
        {

        }
    }
    public class EditEmployeeEmailInfoForwardCommand : IRequest<EmployeeEmailInfoResponse>
    {
        public long EmployeeEmailInfoForwardID { get; set; }
        public long? EmployeeEmailInfoID { get; set; }
        public string? EmailAddress { get; set; }
        public EditEmployeeEmailInfoForwardCommand() { }
    }
    public class EditEmployeeEmailInfoAuthorityCommand : IRequest<EmployeeEmailInfoResponse>
    {
        public long EmployeeEmailInfoAuthorityID { get; set; }
        public long? EmployeeEmailInfoID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
        public EditEmployeeEmailInfoAuthorityCommand() { }
    }
}
