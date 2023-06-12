using Application.Common.Mapper;
using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllForumHandler : IRequestHandler<GetAllForumTypes, List<ForumTypes>>
    {
       
        private readonly IQueryRepository<ForumTypes> _queryRepository;
        public GetAllForumHandler(IQueryRepository<ForumTypes> queryRepository)
        {           
            _queryRepository= queryRepository;
        }
        public async Task<List<ForumTypes>> Handle(GetAllForumTypes request, CancellationToken cancellationToken)
        {
            return (List<ForumTypes>)await _queryRepository.GetListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }

    public class GetAllForumUsersHandler : IRequestHandler<GetAllForumTypesUsers, List<ForumTypes>>
    {

        private readonly IForumTypeQueryRepository _queryRepository;
        public GetAllForumUsersHandler(IForumTypeQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumTypes>> Handle(GetAllForumTypesUsers request, CancellationToken cancellationToken)
        {
            return (List<ForumTypes>)await _queryRepository.GetAllTypeUserAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class GetAllForumCategoryHandler : IRequestHandler<GetAllForumCategorys, List<ForumCategorys>>
    {
       
        private readonly IQueryRepository<ForumCategorys> _queryRepository;
        public GetAllForumCategoryHandler(IQueryRepository<ForumCategorys> queryRepository)
        {           
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumCategorys>> Handle(GetAllForumCategorys request, CancellationToken cancellationToken)
        {
            return (List<ForumCategorys>)await _queryRepository.GetListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class GetAllForumCategoryUsersHandler : IRequestHandler<GetAllForumCategorysUsers, List<ForumCategorys>>
    {

        private readonly IForumCategoryQueryRepository _queryRepository;
        public GetAllForumCategoryUsersHandler(IForumCategoryQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumCategorys>> Handle(GetAllForumCategorysUsers request, CancellationToken cancellationToken)
        {
            return (List<ForumCategorys>)await _queryRepository.GetAllCategoryUserAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class GetUserForumTopicListHandler : IRequestHandler<GetUserForumTopics, List<ForumTopics>>
    {

        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
        public GetUserForumTopicListHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
           _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<ForumTopics>> Handle(GetUserForumTopics request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetUserTopicList(request.UserId);
           
        }
    }
    public class GetForumTopicToHandler : IRequestHandler<GetForumTopicTo, List<ForumTopics>>
    {

        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
        public GetForumTopicToHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<ForumTopics>> Handle(GetForumTopicTo request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetTopicToList(request.UserId);

        }
    }
    public class GetForumTopicCCHandler : IRequestHandler<GetForumTopicCC, List<ForumTopics>>
    {

        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
        public GetForumTopicCCHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<ForumTopics>> Handle(GetForumTopicCC request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetTopicCCList(request.UserId);

        }
    }
    public class GetTreeListHandler : IRequestHandler<GetTreeList, List<ForumTopics>>
    {

        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
        public GetTreeListHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<ForumTopics>> Handle(GetTreeList request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetTreeTopicList(request.UserId);

        }
    }
    public class GetParticipantListHandler : IRequestHandler<GetParticipantsList, List<TopicParticipant>>
    {

        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
        public GetParticipantListHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<TopicParticipant>> Handle(GetParticipantsList request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetParticipantList(request.TopicId);
        }
    }

    public class GetByIdTopicListHandler : IRequestHandler<GetByIdTopics, List<ForumTopics>>
    {

        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
        public GetByIdTopicListHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<List<ForumTopics>> Handle(GetByIdTopics request, CancellationToken cancellationToken)
        {
            return await _forumTopicsQueryRepository.GetByIdAsync(request.ID);

        }
    }

    public class CreateForumTopicsHandler : IRequestHandler<CreateForumTopics, long>
    {
        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
       
        public CreateForumTopicsHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {           
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateForumTopics request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<ForumTopics>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newTopics = _forumTopicsQueryRepository.Insert(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return customerResponse;
        }
    }
    public class UpdateTopicDueDateHandler : IRequestHandler<UpdateTopicDueDate, long>
    {
        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;

        public UpdateTopicDueDateHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateTopicDueDate request, CancellationToken cancellationToken)
        {
            var req = await _forumTopicsQueryRepository.UpdateDueDate(request);
            return req;
        }
    }
    public class UpdateTopicClosedHandler : IRequestHandler<UpdateTopicClosed, long>
    {
        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;

        public UpdateTopicClosedHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;

        }

        public async Task<long> Handle(UpdateTopicClosed request, CancellationToken cancellationToken)
        {
            var req = await _forumTopicsQueryRepository.UpdateTopicClose(request);
            return req;
        }
    }

    public class CreateForumParticipantHandler : IRequestHandler<CreateTopicParticipant, long>
    {
        private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;

        public CreateForumParticipantHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
        {
            _forumTopicsQueryRepository = forumTopicsQueryRepository;
        }
        public async Task<long> Handle(CreateTopicParticipant request, CancellationToken cancellationToken)
        {
            var newTopics = await _forumTopicsQueryRepository.InsertParticipant(request);
            //var customerResponse = RoleMapper.Mapper.Map<long>(newTopics);
            return newTopics;
        }
    }
    public class ForumCategoryListHandler : IRequestHandler<GetListCategory, List<ForumCategorys>>
    {
        private readonly IForumTopicsQueryRepository _forumqueryRepository;
        private readonly IQueryRepository<ForumCategorys> _queryRepository;
        public ForumCategoryListHandler(IForumTopicsQueryRepository forumqueryRepository,IQueryRepository<ForumCategorys> queryRepository)
        {
            _forumqueryRepository = forumqueryRepository;
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumCategorys>> Handle(GetListCategory request, CancellationToken cancellationToken)
        {
            return (List<ForumCategorys>)await _forumqueryRepository.GetCategoryByTypeId(request.ID);
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    public class GetAllForumTopicsHandler : IRequestHandler<GetAllForumTopics, List<ForumTopics>>
    {

        private readonly IQueryRepository<ForumTopics> _queryRepository;
        public GetAllForumTopicsHandler(IQueryRepository<ForumTopics> queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumTopics>> Handle(GetAllForumTopics request, CancellationToken cancellationToken)
        {
            return (List<ForumTopics>)await _queryRepository.GetListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }
    //public class GetForumTopicListHandler : IRequestHandler<GetForumTopicList, string type>
    //{
    //    private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;

    //    public GetForumTopicListHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
    //    {
    //        _forumTopicsQueryRepository = forumTopicsQueryRepository;
    //    }
    //    public async Task<ForumTopicsResponse> Handle(GetForumTopicList request, CancellationToken cancellationToken)
    //    {
    //        //var topicList = RoleMapper.Mapper.Map<ForumTopics>(request);

    //        //if (topicList is null)
    //        //{
    //        //    throw new ApplicationException("There is a problem in mapper");
    //        //}


    //        var newTopics = await _forumTopicsQueryRepository.GetTopicListAsync(request.type);
    //        return newTopics;
    //        //var newTopics = await _forumTopicsQueryRepository.Insert(topicList);
    //        //var customerTopicResponse = RoleMapper.Mapper.Map<ForumTopicsResponse>(newTopics);
    //        //return customerTopicResponse;
    //    }
    //}

}
