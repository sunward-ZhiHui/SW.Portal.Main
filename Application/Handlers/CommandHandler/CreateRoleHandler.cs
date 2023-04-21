using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, RoleResponse>
    {
        private readonly IRoleCommandRepository _roleCommandRepository;
        public CreateRoleHandler(IRoleCommandRepository roleCommandRepository)
        {
            _roleCommandRepository = roleCommandRepository;
        }
        public async Task<RoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<ApplicationRole>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCustomer = await _roleCommandRepository.AddAsync(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<RoleResponse>(newCustomer);
            return customerResponse;
        }
    }
}
