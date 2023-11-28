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
    public class ToDoNotesUsers
    {
        [Key]
        public long ID { get; set; }
        public long NotesHistoryID { get; set; }      
        public long UserID { get; set; }
        public long AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        
}
}
 