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
    public class GetDMSAccessByUserHandler : IRequestHandler<GetDMSAccessByUser, OpenAccessUserLink>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetDMSAccessByUserHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUserLink> Handle(GetDMSAccessByUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetDMSAccessByUser(request.UserId);
        }
    }
    public class GetEmailAccessByUserHandler : IRequestHandler<GetEmailAccessByUser, OpenAccessUserLink>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetEmailAccessByUserHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUserLink> Handle(GetEmailAccessByUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetEmailAccessByUser(request.UserId);
        }
    }
    public class GetNotifyPAAccessByUserHandler : IRequestHandler<GetNotifyPAAccessByUser, OpenAccessUserLink>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetNotifyPAAccessByUserHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUserLink> Handle(GetNotifyPAAccessByUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetNotifyPAAccessByUser(request.UserId);
        }
    }
    public class GetEmailOtherTagAccessByUserHandler : IRequestHandler<GetEmailOtherTagAccessByUser, OpenAccessUserLink>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetEmailOtherTagAccessByUserHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<OpenAccessUserLink> Handle(GetEmailOtherTagAccessByUser request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetEmailOtherTagAccessByUser(request.UserId);
        }
    }
    
    public class GetDocumentAccessTypeEmptyHandler : IRequestHandler<GetDocumentAccessTypeQuery, List<OpenAccessUserLink>>
    {
        private readonly ISettingsQueryRepository _queryRepository;
        public GetDocumentAccessTypeEmptyHandler(ISettingsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<OpenAccessUserLink>> Handle(GetDocumentAccessTypeQuery request, CancellationToken cancellationToken)
        {
            return (List<OpenAccessUserLink>)await _queryRepository.GetDocumentAccessTypeEmptyAsync(request.AccessType);
        }
    }
}
