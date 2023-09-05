using Application.Command;
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

namespace Application.Handlers.CommandHandler.Attribute
{
    public class CreateAttributeHeaderHandler : IRequestHandler<CreateAttributeHeaderCommand, AttributeHeaderResponse>
    {
        private readonly IAttributeHeaderCommandRepository _attributeHeaderRepository;
        public CreateAttributeHeaderHandler(IAttributeHeaderCommandRepository attributeHeaderRepository)
        {
            _attributeHeaderRepository = attributeHeaderRepository;
        }
        public async Task<AttributeHeaderResponse> Handle(CreateAttributeHeaderCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<AttributeHeader>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCustomer = await _attributeHeaderRepository.AddwithValidateAsync(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<AttributeHeaderResponse>(newCustomer);
            return customerResponse;
        }

    }
}
