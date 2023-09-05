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

namespace Application.Handlers.CommandHandler.Attribute
{
    public class EditAttributeHeaderHandler : IRequestHandler<EditAttributeHeaderCommand, AttributeHeaderResponse>
    {
        private readonly IAttributeHeaderCommandRepository _attributeHeaderCommandRepository;
        private readonly IAttributeQueryRepository _attributeHeaderTypeQueryRepository;

        public EditAttributeHeaderHandler(IAttributeHeaderCommandRepository attributeHeaderRepository, IAttributeQueryRepository attributeHeaderQueryRepository)
        {
            _attributeHeaderCommandRepository = attributeHeaderRepository;
            _attributeHeaderTypeQueryRepository = attributeHeaderQueryRepository;
        }
        public async Task<AttributeHeaderResponse> Handle(EditAttributeHeaderCommand request, CancellationToken cancellationToken)
        {
            var attributeHeader = RoleMapper.Mapper.Map<AttributeHeader>(request);

            if (attributeHeader is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _attributeHeaderCommandRepository.UpdateAsync(attributeHeader);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedCustomer = await _attributeHeaderTypeQueryRepository.GetByIdAsync(request.AttributeID);
            var customerResponse = RoleMapper.Mapper.Map<AttributeHeaderResponse>(modifiedCustomer);

            return customerResponse;
        }
    }
}
