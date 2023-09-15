using Application.Command.AttributeDetails;
using Application.Command.AttributeHeader;
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

namespace Application.Handlers.CommandHandler.AttributeDetail
{
    public class EditAttributeDetailsHandler : IRequestHandler<EditAttributeDetailsCommand, AttributeDetailsResponse>
    {
        private readonly IAttributeDetailsCommandRepository _attributeDetailsCommandRepository;
        private readonly IAttributeDetailsQueryRepository _attributeDetailsTypeQueryRepository;

        public EditAttributeDetailsHandler(IAttributeDetailsCommandRepository attributeDetailsRepository, IAttributeDetailsQueryRepository attributeDetailsQueryRepository)
        {
            _attributeDetailsCommandRepository = attributeDetailsRepository;
            _attributeDetailsTypeQueryRepository = attributeDetailsQueryRepository;
        }
        public async Task<AttributeDetailsResponse> Handle(EditAttributeDetailsCommand request, CancellationToken cancellationToken)
        {
            var attributeDetails = RoleMapper.Mapper.Map<Core.Entities.AttributeDetails>(request);

            if (attributeDetails is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _attributeDetailsCommandRepository.UpdateAsync(attributeDetails);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedCustomer = await _attributeDetailsTypeQueryRepository.GetByIdAsync(request.AttributeID);
            var customerResponse = RoleMapper.Mapper.Map<AttributeDetailsResponse>(modifiedCustomer);

            return customerResponse;
        }
    }
}
