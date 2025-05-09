using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class NAVItemLinksModel
    {
        [Key]
        public long ItemLinkId { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? MyItemId { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? SgItemId { get; set; }
        public string? MalaysiaItemNo { get; set; }
        public string? SingaporeItemNo { get; set; }
        public string? CountryName { get; set; }
        [Required(ErrorMessage = "StatusCode is Required")]
        public int? StatusCodeID { get; set; }

        public string? StatusCode { get; set; }
        public long? AddedByUserID { get; set; }
        public string? AddedByUser { get; set; }
        public DateTime? AddedDate { get; set; }
        public string? ModifiedByUser { get; set; }
        public long? ModifiedByUserID { get; set; }
        public Guid? SessionId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
