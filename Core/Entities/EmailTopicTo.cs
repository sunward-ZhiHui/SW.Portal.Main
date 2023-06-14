using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailTopicTo
    {
        public long UserID { get; set; }
        public long TopicId { get; set; }
        public string? FirstName { get; set; }        
    }
}