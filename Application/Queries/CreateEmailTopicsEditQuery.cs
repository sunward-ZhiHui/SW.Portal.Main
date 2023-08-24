using Application.Queries.Base;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class CreateEmailTopicsEditQuery : EmailTopics, IRequest<long>
    {
    }
    public class CreateEmailUploadQuery : PagedRequest, IRequest<List<Documents>>
    {
        public Guid? sessionId { get; set; }
        public CreateEmailUploadQuery(Guid? id)
        {
            this.sessionId = id;
        }
    }

}
