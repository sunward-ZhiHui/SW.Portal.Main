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
        public long CompanyId { get; set; }
        public long? ParentIctid { get; set; }
        public int MasterType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? LayoutPlanId { get; set; }
        public string VersionNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? SiteId { get; set; }
        public long LocationId { get; set; }
        public long ZoneId { get; set; }
        public long? AreaId { get; set; }
        public long? SpecificAreaId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string LocationDescription { get; set; }
    }
}
