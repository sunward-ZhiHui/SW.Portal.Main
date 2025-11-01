using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? RoleName { get; set; }
        [NotMapped]
        public string? SoftwareName { get; set; }
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
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        public CreateEmployeeICTHardInformationCommand()
        {

        }
    }
}
