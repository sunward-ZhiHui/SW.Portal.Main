using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FinishProdcutGeneralInfoLine
    {
     [Key]
        public long FinishProductGeneralInfoLineId { get; set; }
        public long? FinishProductGeneralInfoId { get; set; }
        public long? PackTypeId { get; set; }
        public long? PackagingTypeId { get; set; }
        public long? PackSize { get; set; }
        public decimal? PerUnitQty { get; set; }
        public decimal? ShelfLife { get; set; }
        public string StorageCondition { get; set; }
        public byte[] Picture { get; set; }
        public decimal? PackQty { get; set; }
        public long? PackQtyunitId { get; set; }
        public long? PerPackId { get; set; }
        public decimal? EquvalentSmallestQty { get; set; }
        public long? EquvalentSmallestUnitId { get; set; }
        public decimal? FactorOfSmallestProductionPack { get; set; }
        public long? SalesPerPackId { get; set; }
        public long? TemparatureConditionId { get; set; }
        public decimal? Temparature { get; set; }
        public bool? IsProtectFromLight { get; set; }
        public long? RegistrationSalesId { get; set; }
        public long? StorageConditionId { get; set; }
        public int? RegistrationPackingStatusId { get; set; }
        public long? CapacityId { get; set; }
        public long? ClosureId { get; set; }
        public long? LinerId { get; set; }
    }
}
