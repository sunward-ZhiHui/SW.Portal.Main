using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Notes
    {
        [Key]
        public long NotesId { get; set; }
        public string Notes1 { get; set; }
        public Guid? SessionId { get; set; }
        public long? DocumentId { get; set; }
        public string Link { get; set; }
        public int StatusCodeId { get; set; }
        public long AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsPersonalNotes { get; set; }
    }
}
