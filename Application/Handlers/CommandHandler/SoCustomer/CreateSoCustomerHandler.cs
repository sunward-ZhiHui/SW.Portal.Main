using Application.Command.SoCustomer;
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

namespace Application.Handlers.CommandHandler.SoCustomers
{
    public class CreateSoCustomerHandler : IRequestHandler<CreateSoCustomerCommand, SoCustomerResponse>
    {
        private readonly ISoCustomerCommandRepository _socustomerCommandRepository;
        public CreateSoCustomerHandler(ISoCustomerCommandRepository customerCommandRepository)
        {
            _socustomerCommandRepository = customerCommandRepository;
        }
        public async Task<SoCustomerResponse> Handle(CreateSoCustomerCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<SoCustomer>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCustomer = await _socustomerCommandRepository.AddwithValidateAsync(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<SoCustomerResponse>(newCustomer);
            return customerResponse;
        }

    }
    
}
