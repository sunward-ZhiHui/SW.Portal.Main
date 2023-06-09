using Application.Response;
using MediatR;
 

namespace Application.Commands
{
    public class EditForumTypeCommand : IRequest<ForumTypeResponse>
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
        public int? RowIndex { get; set; }
        public EditForumTypeCommand()
        {
           
        }
    }
}
