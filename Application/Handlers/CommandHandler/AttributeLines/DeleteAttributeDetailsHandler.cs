
using Application.Command.AttributeDetails;
using Core.Repositories.Command;
using Application.Commands;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.AttributeDetail
{
    public class DeleteAttributeDetailsHandler : IRequestHandler<DeleteAttributeDetailsCommand, String>
    {
        private readonly IAttributeDetailsCommandRepository _attribtuteDetailsCommandRepository;
        private readonly IAttributeDetailsQueryRepository _attributeDetailsQueryRepository;
        public DeleteAttributeDetailsHandler(IAttributeDetailsCommandRepository attribtuteDetailsCommandRepository, IAttributeDetailsQueryRepository attributeDetailsQueryRepository)
        {
            _attribtuteDetailsCommandRepository = attribtuteDetailsCommandRepository;
            _attributeDetailsQueryRepository = attributeDetailsQueryRepository;
        }

        public async Task<string> Handle(DeleteAttributeDetailsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerEntity = await _attributeDetailsQueryRepository.GetByIdAsync(request.Id);

                await _attribtuteDetailsCommandRepository.DeleteAsync(customerEntity);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Attribute information has been deleted!";
        }
    }
}
