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
    public class OnReplyEmail
    {
        public long ID { get; set; }    
        public List<ViewEmployee>? Allparticipant { get; set; }
        public List<ViewEmployee>? Allparticipants { get; set; }
        public List<ViewEmployee>? ToList { get; set; } 
        public List<ViewEmployee>? CCList { get; set; }
        public List<string> SelectedCCIds { get; set; }
        public List<ViewEmployee>? _Toparticipant { get; set; }
        public IEnumerable<long> AssignccIds { get; set; }
    }
}
