using Core.Entities.Base;
using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionActivityApp:BaseEntity
    {
        [Key]
        public long ProductionActivityAppID { get; set; }
       
        public long? CompanyID { get; set; }
       
        public string? ProdOrderNo { get; set;}
        public string? Comment { get; set;}
        public string? TopicID { get; set;}
        public long? LocationID { get; set;}
        [NotMapped]
        public long? ICTMasterID { get; set;}
        [NotMapped]
        public string? SiteName { get; set;}
        [NotMapped]
        public string? ZoneName { get; set;}
        [NotMapped]
        public long? Companyid { get; set;}
        [NotMapped]
        public string? DeropdownName { get; set; }
        [NotMapped]
        public long? NavprodOrderLineId { get;set; }
        [NotMapped]
        public string? RePlanRefNo { get; set; }
        [NotMapped]
        public string? BatchNo { get;set; }
        [NotMapped]
        public string? Description { get; set; }
        [NotMapped]
        public string? prodOrderNo { get; set; }
    }
}
