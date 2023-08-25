using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CCFAInformation:BaseEntity
    {
        [Key]
        public long CCFAInformationID { get; set; }
        public bool? IsInternalChanges  { get; set; }
        public bool? IsAuthorityDirectedChanges  { get; set; }
        public long? PIC { get; set; }
        public long? PIA { get;set; }
        public long? DepartmentID { get; set; }
        public bool? IsSiteTransfer { get; set; }
        public bool? IsProduct { get; set; }
        public bool? IsEquipment { get; set; }
        public bool? IsComposition { get; set; }
        public bool? IsFacility { get; set; }
        public bool? IsLayout { get; set; }
        public bool? IsDocument { get; set; }
        public bool? IsProcess { get; set; }
        public bool? IsControlParameter { get; set; }
        public bool? IsBatchSize { get; set; }
        public bool? IsHoldingTime { get; set; }
        public bool? IsRawMeterial { get; set; }
        public bool? IsArtwork { get; set; }
        public bool? IsPackagingMaterial { get; set; }
        public bool? IsVendor { get; set; }
        public bool? IsShelfLife { get; set; }
        public bool? IsRegulatory { get; set; }
        public bool? IsReTestPeriod { get; set; }
        public string?  Others { get; set; }

        public string? DescriptionOfProposedChange { set; get; }
        public string? Description { get; set; }
        public string? Justification { get; set;}
        public string? ProposedImplementationAction { get; set; }
        public string? RelatedDeviation { get; set; }

    }
}
