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
    public class GetAllMemoHandler : IRequestHandler<GetAllMemoQuery, List<Memo>>
    {
        private readonly IMemoQueryRepository _queryRepository;
        public GetAllMemoHandler(IMemoQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<Memo>> Handle(GetAllMemoQuery request, CancellationToken cancellationToken)
        {
            return (List<Memo>)await _queryRepository.GetAllByAsync();
        }
    }
    public class DeleteMemoHandler : IRequestHandler<DeleteMemo, Memo>
    {
        private readonly IMemoQueryRepository _queryRepository;

        public DeleteMemoHandler(IMemoQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<Memo> Handle(DeleteMemo request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteMemo(request.Memo);
        }
    }
    public class InsertOrUpdateMemoSectionHandler : IRequestHandler<InsertOrUpdateMemo, Memo>
    {
        private readonly IMemoQueryRepository _queryRepository;
        public InsertOrUpdateMemoSectionHandler(IMemoQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<Memo> Handle(InsertOrUpdateMemo request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateMemo(request);

        }
    }
    public class UpdateMemoUserAcknowledgementHandler : IRequestHandler<UpdateMemoUserAcknowledgement, MemoUser>
    {
        private readonly IMemoQueryRepository _queryRepository;

        public UpdateMemoUserAcknowledgementHandler(IMemoQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<MemoUser> Handle(UpdateMemoUserAcknowledgement request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateMemoUserAcknowledgement(request.MemoUserId,request.IsAcknowledgement);
        }
    }
    public class GetAllMemoByUserHandler : IRequestHandler<GetAllMemoByUserQuery, List<Memo>>
    {
        private readonly IMemoQueryRepository _queryRepository;
        public GetAllMemoByUserHandler(IMemoQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<Memo>> Handle(GetAllMemoByUserQuery request, CancellationToken cancellationToken)
        {
            return (List<Memo>)await _queryRepository.GetAllByUserAsync(request.UserId);
        }
    }
    public class GetAllMemoByMemoIdQueryHandler : IRequestHandler<GetAllMemoByMemoIdQuery, List<MemoUser>>
    {
        private readonly IMemoQueryRepository _queryRepository;
        public GetAllMemoByMemoIdQueryHandler(IMemoQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<MemoUser>> Handle(GetAllMemoByMemoIdQuery request, CancellationToken cancellationToken)
        {
            return (List<MemoUser>)await _queryRepository.GetMemoUserByMemoIdync(request.MemoId);
        }
    }
}
