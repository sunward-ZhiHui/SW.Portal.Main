using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllRoleHandler : IRequestHandler<GetAllRoleQuery, List<ApplicationRole>>
    {
        private readonly IRoleQueryRepository _roleQueryRepository;
        private readonly IQueryRepository<ApplicationRole> _queryRepository;
        public GetAllRoleHandler(IRoleQueryRepository roleQueryRepository, IQueryRepository<ApplicationRole> queryRepository)
        {
            _roleQueryRepository = roleQueryRepository;
            _queryRepository= queryRepository;
        }
        public async Task<List<ApplicationRole>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            //return (List<ApplicationRole>)await _queryRepository.GetListAsync();
            return (List<ApplicationRole>)await _roleQueryRepository.GetAllAsync();
        }
    }
}
