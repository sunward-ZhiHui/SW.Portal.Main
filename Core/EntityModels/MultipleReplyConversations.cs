using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class MultipleReplyConversations
    {
        public List<EmailConversations> ReplyConversation { get; set; } = new List<EmailConversations>();
        public List<Documents> Documents { get; set; } = new List<Documents>();
        public List<EmailAssignToList> AssignToList { get; set; } = new List<EmailAssignToList>();
        public List<EmailAssignToList> AssignCCList { get; set; } = new List<EmailAssignToList>();        
    }
}
