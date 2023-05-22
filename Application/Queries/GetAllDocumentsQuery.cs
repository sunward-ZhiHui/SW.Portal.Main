using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllDocumentsQuery : PagedRequest, IRequest<Documents>
    {
        public Guid? SessionId { get; set; }
        public GetAllDocumentsQuery(Guid? SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class DeleteDoc : Documents, IRequest<long>
    {
        public long DocumentId { get; private set; }
        public DeleteDoc(long DocumentId)
        {
            this.DocumentId = DocumentId;
        }
    }

}
