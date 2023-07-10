using Application.Command.SoSalesOrder;
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

namespace Application.Handlers.CommandHandler
{
    public class EditSoSalesOrderHandler : IRequestHandler<EditSoSalesOrderCommand, SoSalesOrderResponse>
    {
        private readonly ISoSalesOrderCommandRepository _commandRepository;
        private readonly ISoSalesOrderQueryRepository _queryRepository;
        public EditSoSalesOrderHandler(ISoSalesOrderCommandRepository customerRepository, ISoSalesOrderQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<SoSalesOrderResponse> Handle(EditSoSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<SoSalesOrder>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new SoSalesOrderResponse
            {
                SoSalesOrderId = queryrEntity.SoSalesOrderId,
            };

            return response;
        }
    }
}
