using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllAttributeDetailsHandler : IRequestHandler<GetAllAttributeDetailsQuery, List<AttributeDetails>>
    {
        private readonly IQueryRepository<AttributeDetails> _queryRepository;
        private readonly IAttributeDetailsQueryRepository _attrubutequeryRepository;
        public GetAllAttributeDetailsHandler(IQueryRepository<AttributeDetails> queryRepository, IAttributeDetailsQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeDetails>> Handle(GetAllAttributeDetailsQuery request, CancellationToken cancellationToken)
        {
            return (List<AttributeDetails>)await _attrubutequeryRepository.GetAllAsync();
        }
    }

    public class GetAllAttributeLoadHandler : IRequestHandler<GetAllAttributeLoadQuery, List<AttributeDetails>>
    {

        private readonly IAttributeDetailsQueryRepository _attrubutequeryRepository;
        public GetAllAttributeLoadHandler(IAttributeDetailsQueryRepository attrubutequeryRepository)
        {

            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeDetails>> Handle(GetAllAttributeLoadQuery request, CancellationToken cancellationToken)
        {
            return (List<AttributeDetails>)await _attrubutequeryRepository.LoadAttributelst(request.ID);

        }
    }

    public class CreateAttributeDetailsHandler : IRequestHandler<CreateAttributeDetails, long>
    {

        private readonly IQueryRepository<AttributeDetails> _queryRepository;
        private readonly IAttributeDetailsQueryRepository _attrubutequeryRepository;
        public CreateAttributeDetailsHandler(IQueryRepository<AttributeDetails> queryRepository, IAttributeDetailsQueryRepository attrubutequeryRepository)
        {
            _queryRepository = queryRepository;
            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<long> Handle(CreateAttributeDetails request, CancellationToken cancellationToken)
        {
            return (long)await _attrubutequeryRepository.Insert(request);

        }
    }
    public class EditAttributeDetailsHandler : IRequestHandler<EditAttributeDetails, long>
    {
        private readonly IAttributeDetailsQueryRepository _conversationQueryRepository;

        public EditAttributeDetailsHandler(IAttributeDetailsQueryRepository conversationQueryRepository)
        {
            _conversationQueryRepository = conversationQueryRepository;

        }

        public async Task<long> Handle(EditAttributeDetails request, CancellationToken cancellationToken)
        {
            var req = await _conversationQueryRepository.UpdateAsync(request);
            return req;
        }
    }

    public class DeleteAttributeDetailsHandler : IRequestHandler<DeleteAttributeDetails, long>
    {
        private readonly IAttributeDetailsQueryRepository _QueryRepository;

        public DeleteAttributeDetailsHandler(IAttributeDetailsQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<long> Handle(DeleteAttributeDetails request, CancellationToken cancellationToken)
        {
            var req = await _QueryRepository.Delete(request.AttributeDetailsID);
            return req;
        }
    }
    public class GetAttributeGroupCheckBoxListHandler : IRequestHandler<GetAttributeGroupCheckBoxList, List<AttributeGroupCheckBox>>
    {

        private readonly IAttributeDetailsQueryRepository _attrubutequeryRepository;
        public GetAttributeGroupCheckBoxListHandler(IAttributeDetailsQueryRepository attrubutequeryRepository)
        {

            _attrubutequeryRepository = attrubutequeryRepository;
        }
        public async Task<List<AttributeGroupCheckBox>> Handle(GetAttributeGroupCheckBoxList request, CancellationToken cancellationToken)
        {
            return (List<AttributeGroupCheckBox>)await _attrubutequeryRepository.GetAttributeGroupCheckBoxList(request.ID);

        }
    }
    public class DeleteAttributeGroupCheckBoxHandler : IRequestHandler<DeleteAttributeGroupCheckBox, AttributeGroupCheckBox>
    {
        private readonly IAttributeDetailsQueryRepository _QueryRepository;

        public DeleteAttributeGroupCheckBoxHandler(IAttributeDetailsQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<AttributeGroupCheckBox> Handle(DeleteAttributeGroupCheckBox request, CancellationToken cancellationToken)
        {
            return await _QueryRepository.DeleteAttributeGroupCheckBox(request.AttributeGroupCheckBox);

        }
    }
    public class InsertOrUpdateAttributeGroupCheckBoxHandler : IRequestHandler<InsertOrUpdateAttributeGroupCheckBox, AttributeGroupCheckBox>
    {
        private readonly IAttributeDetailsQueryRepository _QueryRepository;

        public InsertOrUpdateAttributeGroupCheckBoxHandler(IAttributeDetailsQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }

        public async Task<AttributeGroupCheckBox> Handle(InsertOrUpdateAttributeGroupCheckBox request, CancellationToken cancellationToken)
        {
            return await _QueryRepository.InsertOrUpdateAttributeGroupCheckBox(request);

        }
    }
}

