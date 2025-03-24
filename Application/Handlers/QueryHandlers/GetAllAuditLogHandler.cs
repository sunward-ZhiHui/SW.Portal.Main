using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllAuditLogHandler : IRequestHandler<GetAllAuditLogQuery, List<AuditLog>>
    {
        private readonly IAuditLogQueryRepository _queryRepository;
        public GetAllAuditLogHandler(IAuditLogQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<AuditLog>> Handle(GetAllAuditLogQuery request, CancellationToken cancellationToken)
        {
            return (List<AuditLog>)await _queryRepository.GetAuditLog(request.AuditLog);
        }
    }
}
