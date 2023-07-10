using Application.Command.SoSalesOrder;
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
    public class SoSalesOrderHandler : IRequestHandler<CreateSoSalesOrderCommand, SoSalesOrderResponse>
    {
        private readonly ISoSalesOrderCommandRepository _commandRepository;
        public SoSalesOrderHandler(ISoSalesOrderCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SoSalesOrderResponse> Handle(CreateSoSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<SoSalesOrder>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(customerEntity);
            var response = new SoSalesOrderResponse
            {

                SoCustomerId = customerEntity.SoCustomerId,
                Remark = customerEntity.Remark,
                DocumentDate = customerEntity.DocumentDate,
                OrderDate = customerEntity.OrderDate,
                PrioityDeliveryDate = customerEntity.PrioityDeliveryDate,

            };
            return response;
        }
    }
}

