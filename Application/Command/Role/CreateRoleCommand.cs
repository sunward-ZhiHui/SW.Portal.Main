using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CreateRoleCommand : IRequest<RoleResponse>
    {
        public long RoleID { get; set; }
         
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeID { get; set; }

        public long? AddedByUserID { get; set; }
        public CreateRoleCommand()
        {
            this.AddedDate = DateTime.Now;
            this.SessionId = Guid.NewGuid();
        }
    }
}
