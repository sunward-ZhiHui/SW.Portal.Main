using Application.Response;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.FmGlobals
{
    public class CreateFmGlobalCommand : IRequest<FmglobalResponse>
    {
        public long FmglobalId { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? ExpectedShipmentDate { get; set; }
        public string Pono { get; set; }
        public long? LocationToId { get; set; }
        public long? LocationFromId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? FmglobalStausId { get; set; }
        public CreateFmGlobalCommand()
        {
        }
    }
    public class CreateFmGlobalLineCommand : IRequest<FmglobalLineResponse>
    {
        public long FmglobalLineId { get; set; }
        public long? FmglobalId { get; set; }
        public long? PalletEntryId { get; set; }
        public string PalletNoYear { get; set; }
        public long? PalletNoAuto { get; set; }
        public string PalletNo { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? CompanyId { get; set; }
        public string TempPalletNo { get; set; }
        public CreateFmGlobalLineCommand()
        {

        }
    }
    public class CreateGlobalLineItemCommand : IRequest<FmglobalLineItemResponse>
    {
        public long FmglobalLineItemId { get; set; }
        public long? FmglobalLineId { get; set; }
        public long? ItemId { get; set; }
        public long? BatchInfoId { get; set; }
        public long? CartonPackingId { get; set; }
        public int? NoOfCarton { get; set; }
        public int? NoOfUnitsPerCarton { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? ItemBatchInfoId { get; set; }
        public CreateGlobalLineItemCommand()
        {

        }
    }
}
