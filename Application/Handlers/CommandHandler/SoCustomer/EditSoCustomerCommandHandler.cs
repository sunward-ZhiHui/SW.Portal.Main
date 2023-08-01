using Application.Command.SoCustomer;
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

namespace Application.Handlers.CommandHandler.SoCustomers
{
    public class EditSoCustomerCommandHandler : IRequestHandler<EditSoCustomerCommand, SoCustomerResponse>
    {
        private readonly ISoCustomerCommandRepository _customerCommandRepository;
        private readonly ISoCustomerQueryRepository _customerQueryRepository;

        public EditSoCustomerCommandHandler(ISoCustomerCommandRepository customerRepository, ISoCustomerQueryRepository customerQueryRepository)
        {
            _customerCommandRepository = customerRepository;
            _customerQueryRepository = customerQueryRepository;
        }
        public async Task<SoCustomerResponse> Handle(EditSoCustomerCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<SoCustomer>(request);

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

            var modifiedCustomer = await _customerQueryRepository.GetByIdAsync(request.SoCustomerId);
            var customerResponse = RoleMapper.Mapper.Map<SoCustomerResponse>(modifiedCustomer);

            return customerResponse;
        }
    
    }
}
