using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeICTInformations
{
    public class CreateEmployeeICTInformationCommand:IRequest<EmployeeOtherDutyInformationResponse>
    {
        public long EmployeeIctinformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? SoftwareId { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public long? RoleId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsPortal { get; set; }
        public CreateEmployeeICTInformationCommand()
        {

        }
    }
    public class CreateEmployeeICTHardInformationCommand : IRequest<EmployeeOtherDutyInformationResponse>
    {
        public long EmployeeIctHardinformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? HardwareId { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Instruction { get; set; }
        public long? CompanyId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public CreateEmployeeICTHardInformationCommand()
        {

        }
    }
}
