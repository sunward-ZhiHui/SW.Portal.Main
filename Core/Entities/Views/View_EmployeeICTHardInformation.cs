using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_EmployeeICTHardInformation
    {
        public long EmployeeIctHardinformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? HardwareId { get; set; }
        [Required(ErrorMessage = "Login ID is Required")]
        public string LoginId { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsPortal { get; set; }
        public string EmployeeName { get; set; }
        public string HardwareName { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        public string Instruction { get; set; }
        public string PlantDescription { get; set; }
        public string PlantCode { get; set; }
        public long? CompanyId { get; set; }
        public int? CodeId { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public string StatusCode { get; set; }
    }
}
