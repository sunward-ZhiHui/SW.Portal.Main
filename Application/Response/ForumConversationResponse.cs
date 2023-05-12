using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class ForumConversationResponse : BaseEntity
    {
        public long ID { get; set; }
        public long TopicID { get; set; }
        public string Message { get; set; }
        public long ParticipantId { get; set; }
        public long ReplyId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserCode { get; set; }
        public string UserEmail { get; set; }

    }
}
