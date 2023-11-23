using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class view_ActivityEmailSubjects
    {
        public long TopicId { get; set; }
        public int? ID { get; set; }
        public string? TopicName { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? AddedByUserID { get; set; }
        public string? FirstName { get; set; }

        public string? AddedByUser { get; set; }
        public string? LastName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
