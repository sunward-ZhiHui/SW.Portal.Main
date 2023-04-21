using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class EditRoleHandler : IRequestHandler<EditRoleCommand, RoleResponse>
    {
        private readonly IRoleCommandRepository _customerCommandRepository;
        private readonly IRoleQueryRepository _customerQueryRepository;
        public EditRoleHandler(IRoleCommandRepository customerRepository, IRoleQueryRepository customerQueryRepository)
        {
            _customerCommandRepository = customerRepository;
            _customerQueryRepository = customerQueryRepository;
        }
        public async Task<RoleResponse> Handle(EditRoleCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<ApplicationRole>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _customerCommandRepository.UpdateAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedCustomer = await _customerQueryRepository.GetByIdAsync(request.RoleID);
            var customerResponse = RoleMapper.Mapper.Map<RoleResponse>(modifiedCustomer);

            return customerResponse;
        }
    }
}
