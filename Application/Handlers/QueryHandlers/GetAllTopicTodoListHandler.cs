using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
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
    public class GetAllTopicTodoListHandler : IRequestHandler<GetAllTopicToDoList, List<TopicToDoList>>
    {
        private readonly ITopicTodoListQueryRepository _topicTodoListQueryRepository;
        private readonly IQueryRepository<ForumTypes> _queryRepository;
        public GetAllTopicTodoListHandler(ITopicTodoListQueryRepository topicTodoListQueryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
        }
        public async Task<List<TopicToDoList>> Handle(GetAllTopicToDoList request, CancellationToken cancellationToken)
        {
            return (List<TopicToDoList>)await _topicTodoListQueryRepository.GetAllAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class CreateTopicTodoListHandler : IRequestHandler<CreateTopicTodoListQuery, long>
    {
        private readonly ITopicTodoListQueryRepository _topicTodoListQueryRepository;
        private readonly IQueryRepository<TopicToDoList> _queryRepository;
        public CreateTopicTodoListHandler(ITopicTodoListQueryRepository topicTodoListQueryRepository, IQueryRepository<TopicToDoList> queryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;

            _queryRepository = queryRepository;
        }

        public async Task<long> Handle(CreateTopicTodoListQuery request, CancellationToken cancellationToken)
        {

            var newlist = await _topicTodoListQueryRepository.Insert(request);
            return newlist;

        }

    }
    public class EditTopicTodoListHandler : IRequestHandler<EditTopicTodoListQuery, long>
    {
        private readonly ITopicTodoListQueryRepository _topicTodoListQueryRepository;
        private readonly IQueryRepository<TopicToDoList> _queryRepository;
        public EditTopicTodoListHandler(ITopicTodoListQueryRepository topicTodoListQueryRepository, IQueryRepository<TopicToDoList> queryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;
            _queryRepository = queryRepository;
        }

        public async Task<long> Handle(EditTopicTodoListQuery request, CancellationToken cancellationToken)
        {
            var req = await _topicTodoListQueryRepository.Update(request);
            return req;
        }
    }
    public class DeleteCompanyHandler : IRequestHandler<DeleteTopicToDoListQuery,long>
    {
        private readonly ITopicTodoListQueryRepository _topicTodoListQueryRepository;
        private readonly IQueryRepository<TopicToDoList> _queryRepository;
        public DeleteCompanyHandler(ITopicTodoListQueryRepository topicTodoListQueryRepository, IQueryRepository<TopicToDoList> queryRepository)
        {
            _topicTodoListQueryRepository = topicTodoListQueryRepository;

            _queryRepository = queryRepository;
        }

        public async Task<long> Handle(DeleteTopicToDoListQuery request, CancellationToken cancellationToken)
        {
             var req = await _topicTodoListQueryRepository.Delete(request);
              return req;
        }
    }
}