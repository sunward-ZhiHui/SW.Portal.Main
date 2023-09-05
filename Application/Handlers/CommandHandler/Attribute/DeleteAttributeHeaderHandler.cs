using Application.Command.AttributeHeader;
using Application.Commands;
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
    public class DeleteAttributeHeaderHandler : IRequestHandler<DeleteAttributeHeaderCommand, String>
    {
        private readonly IAttributeHeaderCommandRepository _attribtuteHeaderCommandRepository;
        private readonly IAttributeQueryRepository _attributeHeaderQueryRepository;
        public DeleteAttributeHeaderHandler(IAttributeHeaderCommandRepository attribtuteHeaderCommandRepository, IAttributeQueryRepository attributeHeaderQueryRepository)
        {
            _attribtuteHeaderCommandRepository = attribtuteHeaderCommandRepository;
            _attributeHeaderQueryRepository = attributeHeaderQueryRepository;
        }

        public async Task<string> Handle(DeleteAttributeHeaderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerEntity = await _attributeHeaderQueryRepository.GetByIdAsync(request.Id);

                await _attribtuteHeaderCommandRepository.DeleteAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Attribute information has been deleted!";
        }
    }
}
