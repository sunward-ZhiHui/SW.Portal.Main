using Application.Queries;
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

    public class GetAllForumCatHandler : IRequestHandler<GetAllForumCategorys, List<ForumCategorys>>
    {
       
        private readonly IQueryRepository<ForumCategorys> _queryRepository;
        public GetAllForumCatHandler(IQueryRepository<ForumCategorys> queryRepository)
        {           
            _queryRepository = queryRepository;
        }
        public async Task<List<ForumCategorys>> Handle(GetAllForumCategorys request, CancellationToken cancellationToken)
        {
            return (List<ForumCategorys>)await _queryRepository.GetListAsync();
            //return (List<ForumTypes>)await _roleQueryRepository.GetAllAsync();
        }
    }

}
