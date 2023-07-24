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
    public class GetAllFmGlobalLineItemQuery : PagedRequest, IRequest<List<ViewFmglobalLineItem>>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetAllFmGlobalLineItemQuery(long? Id)
        {
            this.Id = Id;
        }
    }
}
