using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ActivityEmailTopics
    {
        [Key]
        public long ActivityEmailTopicID { get; set; }
        public string SubjectName { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? DocumentSessionId { get; set; }
        public Guid? EmailTopicSessionId { get; set; }
        public string ActivityType { get; set; }
        public long FromId { get; set; }
        public string ToIds { get; set; }
        public string CcIds { get; set; }       
        public string Tags { get; set; }
        public string Comment { get; set; }

    }
}
