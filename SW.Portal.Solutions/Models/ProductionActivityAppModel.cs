using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SW.Portal.Solutions.Models
{
    public class ProductionActivityAppModel
    {
        [Key]
        public long ProductionActivityAppID { get; set; }

        public long? CompanyID { get; set; }

        public string? ProdOrderNo { get; set; }
        public string? Comment { get; set; }
        public string? TopicID { get; set; }
        public long? LocationID { get; set; }
        [NotMapped]
        public long? ICTMasterID { get; set; }
        [NotMapped]
        public string? DeropdownName { get; set; }
    }
}
