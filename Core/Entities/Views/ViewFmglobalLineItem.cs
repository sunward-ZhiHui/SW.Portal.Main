using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewFmglobalLineItem
    {
        public long? FmglobalLineId { get; set; }
        public long FmglobalLineItemId { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public long? ItemId { get; set; }
        public long? BatchInfoId { get; set; }
        [Required(ErrorMessage = "Carton Packing is Required")]
        public long? CartonPackingId { get; set; }
        [Required(ErrorMessage = "No Of Carton is Required")]
        public int? NoOfCarton { get; set; }
        [Required(ErrorMessage = "No Of Units Per Carton is Required")]
        public int? NoOfUnitsPerCarton { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public string StatusCode { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string CartonPackingName { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string PackSize { get; set; }
        public string BatchNo { get; set; }
        public string BatchSize { get; set; }
        public string LocationCode { get; set; }
        public string LotNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public long? ItemBatchInfoId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PalletNo { get; set; }
        public string TempPalletNo { get; set; }
        public decimal? NavQuantity { get; set; }
        public DateTime? ExpectedShipmentDate { get; set; }
        public DateTime? FMGlobalDate { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        public string FMGlobalStatus { get; set; }
        public decimal? TotalQty { get; set; }
    }
}
