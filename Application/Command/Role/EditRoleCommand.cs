using Application.Response;
using MediatR;
 

namespace Application.Commands
{
    public class EditRoleCommand : IRequest<RoleResponse>
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        public EditRoleCommand()
        {
            this.ModifiedDate = DateTime.Now;
        }
    }
}
