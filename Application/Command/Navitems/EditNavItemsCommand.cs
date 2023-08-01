using Application.Response;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class EditNavItemsCommand : IRequest<NavitemsResponse>
    {
        public long ItemId { get; set; }
        public long? CompanyId { get; set; }
        public string No { get; set; }
        public string RelatedItemNo { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string ItemType { get; set; }
        public decimal? Inventory { get; set; }
        public string InternalRef { get; set; }
        public string ItemRegistration { get; set; }
        public string ExpirationCalculation { get; set; }
        public string BatchNos { get; set; }
        public string ProductionRecipeNo { get; set; }
        public bool? Qcenabled { get; set; }
        public string SafetyLeadTime { get; set; }
        public string ProductionBomno { get; set; }
        public string RoutingNo { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? UnitPrice { get; set; }
        public string VendorNo { get; set; }
        public string VendorItemNo { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemTrackingCode { get; set; }
        public string Qclocation { get; set; }
        public string Company { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public long? LastSyncBy { get; set; }
        public long? CategoryId { get; set; }
        public bool? Steroid { get; set; }
        public string ShelfLife { get; set; }
        public string Quota { get; set; }
        public int? PackSize { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string PackUom { get; set; }
        public long? GenericCodeId { get; set; }
        public bool? IsDifferentAcuom { get; set; }
        public decimal? PackQty { get; set; }
        public string PurchaseUom { get; set; }
        public long? ReplenishmentMethodId { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsPortal { get; set; }
        public string ItemSerialNo { get; set; }
        public long? PackSizeId { get; set; }
        public long? SupplyToId { get; set; }
        public long? UomId { get; set; }
        public EditNavItemsCommand()
        {
        }
    }

    public class CreateNavProductionInformationCommand : IRequest<NavProductionInformationResponse>
    {
        public long NavProductionInformationId { get; set; }
        public long? ItemId { get; set; }
        public int? NoOfBUOMInOneCarton { get; set; }
        public CreateNavProductionInformationCommand()
        {
        }
    }
    public class EditNavProductionInformationCommand : IRequest<NavProductionInformationResponse>
    {
        public long NavProductionInformationId { get; set; }
        public long? ItemId { get; set; }
        public int? NoOfBUOMInOneCarton { get; set; }
        public EditNavProductionInformationCommand()
        {
        }
    }
    public class CreateNavCrossReferenceCommand : IRequest<NavCrossReferenceResponse>
    {
        public long NavCrossReferenceId { get; set; }
        public long? ItemId { get; set; }
        public string? TypeOfCompany { get; set; }
        public long? CompanyId { get; set; }
        public long? NavVendorId { get; set; }
        public long? NavCustomerId { get; set; }
        public string? CrossReferenceNo { get; set; }
        public CreateNavCrossReferenceCommand()
        {
        }
    }
    public class EditNavCrossReferenceCommand : IRequest<NavCrossReferenceResponse>
    {
        public long NavCrossReferenceId { get; set; }
        public long? ItemId { get; set; }
        public string? TypeOfCompany { get; set; }
        public long? CompanyId { get; set; }
        public long? NavVendorId { get; set; }
        public long? NavCustomerId { get; set; }
        public string? CrossReferenceNo { get; set; }
        public EditNavCrossReferenceCommand()
        {
        }
    }
    public class DeleteNavItemsCommand : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteNavItemsCommand(Int64 Id)
        {
            this.Id = Id;
        }
    }
    public class DeleteNavCrossReference : IRequest<String>
    {
        public Int64 Id { get; private set; }

        public DeleteNavCrossReference(Int64 Id)
        {
            this.Id = Id;
        }
    }
    
}
