using Application.Command.SoSalesOrder;
using Application.Command.SoSalesOrderLine;
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
    public class CreateSoSalesOrderLineHandler : IRequestHandler<CreateSoSalesOrderLineCommand, SoSalesOrderLineResponse>
    {
        private readonly ISoSalesOrderLineCommandRepository _commandRepository;
        public CreateSoSalesOrderLineHandler(ISoSalesOrderLineCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SoSalesOrderLineResponse> Handle(CreateSoSalesOrderLineCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<SoSalesOrderLine>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(customerEntity);
            var response = new SoSalesOrderLineResponse
            {

                SoSalesOrderLineId = customerEntity.SoSalesOrderLineId
            };
            return response;
        }
    }
}
