using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_FMGlobalMovePackingList
    {
        public long? FmglobalLineId { get; set; }
        public long FmglobalLineItemId { get; set; }
        public long? ItemId { get; set; }
        public long? ItemBatchInfoId { get; set; }
        public long? CartonPackingId { get; set; }
        public int? NoOfCarton { get; set; }
        public int? NoOfUnitsPerCarton { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public string PoNo { get; set; }
        public DateTime? ExpectedShipmentDate { get; set; }
        public string? ShippedBy { get; set; }
        public DateTime? FMGlobalDate { get; set; }
        public long? FmglobalStausId { get; set; }
        public string? FmglobalStatus { get; set; }
        public string? PlantCode { get; set; }
        public long? FmglobalLinePreviousId { get; set; }
        public long? TransactionAddedUserId { get; set; }
        public DateTime? TransactionAddedDate { get; set; }
        public long? TransactionModifiedByUserId { get; set; }
        public DateTime? TransactionModifiedDate { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string PackSize { get; set; }
        public string LocationCode { get; set; }
        public string BatchNo { get; set; }
        public string LotNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public decimal? QuantityOnHand { get; set; }
        public decimal? NavQuantity { get; set; }
        public string TempPalletNo { get; set; }
        public string PalletNo { get; set; }
        public string CartonPackingName { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public int? TotalQty { get; set; }
        public int? TransactionQty { get; set; }
        public int? IsHandQty { get; set; }
    }
}
