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

    //public class CreateForumTopicsHandler : IRequestHandler<CreateForumTopics, ForumTopicsResponse>
    //{
    //    private readonly IForumTopicsQueryRepository _forumTopicsQueryRepository;
       
    //    public CreateForumTopicsHandler(IForumTopicsQueryRepository forumTopicsQueryRepository)
    //    {           
    //        _forumTopicsQueryRepository = forumTopicsQueryRepository;
    //    }
    //    public async Task<ForumTopicsResponse> Handle(CreateForumTopics request, CancellationToken cancellationToken)
    //    {
    //        var customerEntity = RoleMapper.Mapper.Map<ForumTopics>(request);

    //        if (customerEntity is null)
    //        {
    //            throw new ApplicationException("There is a problem in mapper");
    //        }

    //        var newTopics = await _forumTopicsQueryRepository.Insert(customerEntity);
    //        var customerResponse = RoleMapper.Mapper.Map<ForumTopicsResponse>(newTopics);
    //        return customerResponse;
    //    }
    //}

}
