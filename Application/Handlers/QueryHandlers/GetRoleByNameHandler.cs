using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetRoleByNameHandler : IRequestHandler<GetRoleByNameQuery, ApplicationRole>
    {
        private readonly IMediator _mediator;
        private readonly IRoleQueryRepository _roleQueryRepository;
        public GetRoleByNameHandler(IMediator mediator, IRoleQueryRepository roleQueryRepository)
        {
            _mediator = mediator;
            _roleQueryRepository = roleQueryRepository;
        }
        public async Task<ApplicationRole> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
            //var customers = await _mediator.Send(new GetRoleByNameQuery(request.Name.ToLower()));
            var customers = await _roleQueryRepository.GetCustomerByEmail(request.Name);
            return customers;
        }
    }
}
