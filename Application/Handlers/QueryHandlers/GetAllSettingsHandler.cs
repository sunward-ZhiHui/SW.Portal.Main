using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllSettingsHandler : IRequestHandler<GetAllSettingsQuery, List<OpenAccessUserLink>>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetAllSettingsHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<OpenAccessUserLink>> Handle(GetAllSettingsQuery request, CancellationToken cancellationToken)
        {
            return (List<OpenAccessUserLink>)await _queryRepository.GetAllAsync(request.OpenAccessUserId);
        }
    }
    public class DeleteOpenAccessUserLinkHandler : IRequestHandler<DeleteOpenAccessUserLink, OpenAccessUserLink>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public DeleteOpenAccessUserLinkHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUserLink> Handle(DeleteOpenAccessUserLink request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteOpenAccessUserLink(request.OpenAccessUserLink);
        }

    }
    public class InsertOpenAccessUserLinkHandler : IRequestHandler<InsertOpenAccessUserLink, OpenAccessUserLink>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public InsertOpenAccessUserLinkHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUserLink> Handle(InsertOpenAccessUserLink request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOpenAccessUserLink(request.OpenAccessUserLink);
        }

    }
    public class GetOpenAccessUserHandler : IRequestHandler<GetOpenAccessUser, OpenAccessUser>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetOpenAccessUserHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUser> Handle(GetOpenAccessUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetAllByAsync(request.AccessType);
        }
    }

}
