using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeEmailInfos
{
    public class CreateEmployeeEmailInfoCommand : IRequest<EmployeeEmailInfoResponse>
    {
        public long EmployeeEmailInfoID { get; set; }
        public long? EmployeeID { get; set; }
        public long? SubscriptionID { get; set; }
        public long? EmailGuideID { get; set; }
        [NotMapped]
        public long? AddedByUserId { get; set; }
        [NotMapped]
        public DateTime? AddedDate { get; set; }
        [NotMapped]
        public long? ModifiedByUserId { get; set; }
        [NotMapped]
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? EmailGuide { get; set; }
        [NotMapped]
        public string? Subscription { get; set; }
        public CreateEmployeeEmailInfoCommand() { }
    }
    public class CreateEmployeeEmailInfoForwardCommand : IRequest<EmployeeEmailInfoResponse>
    {
        public long EmployeeEmailInfoForwardID { get; set; }
        public long? EmployeeEmailInfoID { get; set; }
        public string EmailAddress { get; set; }
        [NotMapped]
        public long? AddedByUserId { get; set; }
        [NotMapped]
        public DateTime? AddedDate { get; set; }
        [NotMapped]
        public long? ModifiedByUserId { get; set; }
        [NotMapped]
        public DateTime? ModifiedDate { get; set; }
        public CreateEmployeeEmailInfoForwardCommand() { }
    }
    public class CreateEmployeeEmailInfoAuthorityCommand : IRequest<EmployeeEmailInfoResponse>
    {
        public long EmployeeEmailInfoAuthorityID { get; set; }
        public long? EmployeeEmailInfoID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
        public CreateEmployeeEmailInfoAuthorityCommand() { }
    }
}
