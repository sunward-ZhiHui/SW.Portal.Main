using Application.Queries.Base;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllFmGlobalQuery : PagedRequest, IRequest<List<ViewFmglobal>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllFmGlobalByIdQuery : PagedRequest, IRequest<ViewFmglobal>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetAllFmGlobalByIdQuery(long? Id)
        {
            this.Id = Id;
        }
    }

    public class GetAllFmGlobalBySessionQuery : PagedRequest, IRequest<ViewFmglobal>
    {
        public string SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetAllFmGlobalBySessionQuery(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetAllFmGlobalLineQuery : PagedRequest, IRequest<List<ViewFmglobalLine>>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetAllFmGlobalLineQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllFmGlobalLineBySessionQuery : PagedRequest, IRequest<ViewFmglobalLine>
    {
        public string SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetAllFmGlobalLineBySessionQuery(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetAllFmGlobalLineByPalletEntryNoQuery : PagedRequest, IRequest<List<ViewFmglobalLine>>
    {
        public string SearchString { get; set; }
        public long? CompanyId { get; set; }
        public GetAllFmGlobalLineByPalletEntryNoQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetAllFmGlobalLineItemQuery : PagedRequest, IRequest<List<ViewFmglobalLineItem>>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetAllFmGlobalLineItemQuery(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllFmGlobalLineItemBySessionQuery : PagedRequest, IRequest<ViewFmglobalLineItem>
    {
        public string SearchString { get; set; }
        public Guid? SessionId { get; set; }
        public GetAllFmGlobalLineItemBySessionQuery(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetFmGlobalAddressQuery : View_FMGlobalAddess, IRequest<List<View_FMGlobalAddess>>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public string BillingType { get; set; }
        public GetFmGlobalAddressQuery(long? Id, string billingType)
        {
            this.Id = Id;
            BillingType = billingType;
        }
    }
    public class CreateFmGlobalAddress : View_FMGlobalAddess, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class EditFmGlobalAddress : View_FMGlobalAddess, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteFmGlobalAddress : IRequest<String>
    {
        public long AddressID { get; private set; }
        public long FMGlobalID { get; private set; }

        public DeleteFmGlobalAddress(long AddressID, long FMGlobalID)
        {
            this.AddressID = AddressID;
            this.FMGlobalID = FMGlobalID;
        }
    }
    public class GetPalletMovementListingQuery : PagedRequest, IRequest<List<ViewFmglobalLineItem>>
    {
        public string SearchString { get; set; }
    }

}
