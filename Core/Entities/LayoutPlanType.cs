using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class LayoutPlanType
    {

        //public partial class LayoutPlanType
        //{
        //    public LayoutPlanType()
        //    {
        //        IctlayoutPlanTypes = new HashSet<IctlayoutPlanTypes>();
        //    }
        [Key]
            public long LayoutPlanTypeId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string VersionNo { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public Guid? SessionId { get; set; }
            public int? StatusCodeId { get; set; }
            public long? AddedByUserId { get; set; }
            public DateTime? AddedDate { get; set; }
            public long? ModifiedByUserId { get; set; }
            public DateTime? ModifiedDate { get; set; }

            //public virtual ApplicationUser AddedByUser { get; set; }
            //public virtual ApplicationUser ModifiedByUser { get; set; }
            //public virtual CodeMaster StatusCode { get; set; }
            //public virtual ICollection<IctlayoutPlanTypes> IctlayoutPlanTypes { get; set; }
        }
    }

