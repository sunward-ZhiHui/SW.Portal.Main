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
    public class GetAllLoginSessionHistoryQueryHandler : IRequestHandler<GetAllLoginSessionHistoryQuery, List<LoginSessionHistory>>
    {
        private readonly ILoginSessionHistoryQueryRepository _queryRepository;
        public GetAllLoginSessionHistoryQueryHandler(ILoginSessionHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<LoginSessionHistory>> Handle(GetAllLoginSessionHistoryQuery request, CancellationToken cancellationToken)
        {
            return (List<LoginSessionHistory>)await _queryRepository.GetAllByAsync();
        }
    }

    public class InsertLoginSessionHistoryHandler : IRequestHandler<InsertLoginSessionHistory, LoginSessionHistory>
    {
        private readonly ILoginSessionHistoryQueryRepository _queryRepository;
        public InsertLoginSessionHistoryHandler(ILoginSessionHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<LoginSessionHistory> Handle(InsertLoginSessionHistory request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertLoginSessionHistory(request.LoginSessionHistory);

        }
    }
    public class UpdateLogOutActivityHandler : IRequestHandler<UpdateLogOutActivity, LoginSessionHistory>
    {
        private readonly ILoginSessionHistoryQueryRepository _queryRepository;
        public UpdateLogOutActivityHandler(ILoginSessionHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<LoginSessionHistory> Handle(UpdateLogOutActivity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateLogOutActivity(request.LoginSessionHistory);

        }
    }
    public class UpdateLastActivityHandler : IRequestHandler<UpdateLastActivity, LoginSessionHistory>
    {
        private readonly ILoginSessionHistoryQueryRepository _queryRepository;
        public UpdateLastActivityHandler(ILoginSessionHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<LoginSessionHistory> Handle(UpdateLastActivity request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateLastActivity(request.LoginSessionHistory);

        }
    }
    public class GetLoginSessionHistoryOneHandler : IRequestHandler<GetLoginSessionHistoryOne, LoginSessionHistory>
    {
        private readonly ILoginSessionHistoryQueryRepository _queryRepository;
        public GetLoginSessionHistoryOneHandler(ILoginSessionHistoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<LoginSessionHistory> Handle(GetLoginSessionHistoryOne request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetLoginSessionHistoryOne(request.SessionId, request.UserId);

        }
    }
}
