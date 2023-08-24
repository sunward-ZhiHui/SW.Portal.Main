using Application.Queries;
using Core.Entities;
using Core.Repositories.Query.Base;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class CreateEmailTopicEditQueryHandler : IRequestHandler<CreateEmailTopicsEditQuery, long>
    {
        private readonly ICreateEmailTopicEditQueryRepository _topicTodoListQueryRepository;
        public CreateEmailTopicEditQueryHandler(ICreateEmailTopicEditQueryRepository topicTodoListQueryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
        }

        public async Task<long> Handle(CreateEmailTopicsEditQuery request, CancellationToken cancellationToken)
        {
            var req = await _topicTodoListQueryRepository.Update(request);
            return req;
        }
    }
    public class CreateEmailTopicUploadQueryHandler : IRequestHandler<CreateEmailUploadQuery, List<Documents>>
    {
        private readonly ICreateEmailTopicEditQueryRepository _topicTodoListQueryRepository;
        public CreateEmailTopicUploadQueryHandler(ICreateEmailTopicEditQueryRepository topicTodoListQueryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
        }

        public async Task<List<Documents>> Handle(CreateEmailUploadQuery request, CancellationToken cancellationToken)
        {
            return (List<Documents>) await _topicTodoListQueryRepository.GetAllAsync(request.sessionId);
           
        }
    }
}
