using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class TempSalesPackInformationReportModel:BaseModel
    {
        [Key]
        public long? FinishProductGeneralInfoID { get; set; }
        public long? TempSalesPackInformationID { get; set; }
        public string? ManufacturingSite { get; set; }
        public string? RegisterCountry { get; set; }
        public string? ProductionRegistrationHolderName { get; set; }
        public string? ProductName { get; set; }
        public string? PrhspecificProductName { get; set; }
        public string? ProductOwner { get; set; }
        public string? PackType { get; set; }
        public string? PackagingType { get; set; }

        public decimal? SmallestPackQty { get; set; }

        public string? SmallestQtyUnit { get; set; }

        public string? SmallestPerPack { get; set; }
        public string? PackingUnitsPerPack { get; set; }
        public decimal? RegistrationFactor { get; set; }

        public string? RegistrationPerPack { get; set; }


        public long? FinishProductGeneralInfoLineID { get; set; }
    }
}
