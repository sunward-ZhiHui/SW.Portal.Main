using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.ApplicationUsers
{
    public class CreateApplicationUserCommand : IRequest<ApplicationUserResponse>
    {
        public long UserID { get; set; }
        public string? LoginID { get; set; }
        public string? LoginPassword { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public long UserId { get; set; }
        public string EmployeeNo { get; set; }
        public string UserEmail { get; set; }
        public byte AuthenticationType { get; set; }
        public long? DepartmentId { get; set; }
        public bool IsPasswordChanged { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public long? EmployeId { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public CreateApplicationUserCommand()
        {
            this.AddedDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
        }
    }
}
