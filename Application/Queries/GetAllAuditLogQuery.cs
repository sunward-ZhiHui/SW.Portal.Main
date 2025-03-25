using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllAuditLogQuery : PagedRequest, IRequest<List<AuditLog>>
    {
        public AuditLog AuditLog { get; set; }
        public GetAllAuditLogQuery(AuditLog auditLog)
        {
            this.AuditLog = auditLog;
        }
    }
}
