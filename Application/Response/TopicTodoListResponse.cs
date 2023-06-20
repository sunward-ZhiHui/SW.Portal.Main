using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class TopicTodoListResponse
    {
        public long ID { get; set; }
        public long TopicID { get; set; }
        public string TopicName { get; set; }
        public bool Iscompleted { get; set; }
    }
}
