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

namespace Application.Handlers.CommandHandler.AttributeDetail
{
    public class CreateAttributeDetailsHandler : IRequestHandler<CreateAttributeDetailsCommand, AttributeDetailsResponse>
    {
        private readonly IAttributeDetailsCommandRepository _attributeDetailsRepository;
        public CreateAttributeDetailsHandler(IAttributeDetailsCommandRepository attributeDetalsRepository)
        {
            _attributeDetailsRepository = _attributeDetailsRepository;
        }
        public async Task<AttributeDetailsResponse> Handle(CreateAttributeDetailsCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<Core.Entities.AttributeDetails>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var newCustomer = await _attributeDetailsRepository.AddwithValidateAsync(customerEntity);
            var customerResponse = RoleMapper.Mapper.Map<AttributeDetailsResponse>(newCustomer);
            return customerResponse;
        }

        Task<AttributeDetailsResponse> IRequestHandler<CreateAttributeDetailsCommand, AttributeDetailsResponse>.Handle(CreateAttributeDetailsCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
