using Core.Entities.Base;
using Core.Entities.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OnReplyEmailTopic
    {
        public long ID { get; set; }
        public List<long>? ToIds { get; set; }
        public List<long>? CcIds { get; set; }       
        public List<long>? allparticipant { get; set; }
        public List<ViewEmployeeModel>? _allparticipants { get; set; }


    }
}
