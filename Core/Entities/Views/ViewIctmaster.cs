using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public  class ViewIctmaster
    {
        [Key]
        public long IctmasterId { get; set; }
        public int Index { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        public long CompanyId { get; set; }
        public long? ParentIctid { get; set; }
        public int MasterType { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }
        public string Description { get; set; }
       public long? LayoutPlanId { get; set; }
        public string VersionNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public Guid? SessionId { get; set; }
       // [Required(ErrorMessage = "Please Select Site")]
        public long? SiteId { get; set; }
       // [Required(ErrorMessage = "Please Enter Location")]
        public long LocationId { get; set; }
       // [Required(ErrorMessage = "Please Select Zone")]
        public long ZoneId { get; set; }
       // [Required(ErrorMessage = "Please Select Area")]
        public long? AreaId { get; set; }
        public long? SpecificAreaId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Area { get; set; }
        public string Location { get; set; }
        public string Site { get; set; }
        public string Zone { get; set; }
        public string ParentName { get; set; }
    }
}
