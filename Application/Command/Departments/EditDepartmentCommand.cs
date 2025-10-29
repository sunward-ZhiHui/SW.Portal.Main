using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.Departments
{
    public class EditDepartmentCommand : IRequest<DepartmentResponse>
    {
        public long DepartmentId { get; set; }
        public long? CompanyId { get; set; }
        public long? Hodid { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? HeadCount { get; set; }
        public long? DivisionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ProfileCode { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? DivisionName { get; set; }
        public EditDepartmentCommand()
        {
            this.ModifiedDate = DateTime.Now;
            this.ModifiedByUserId = 1;
        }
    }
}
