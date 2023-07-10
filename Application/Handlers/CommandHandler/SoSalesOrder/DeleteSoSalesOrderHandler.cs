using Application.Command.SoSalesOrder;
using Application.Commands;
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
    public class DeleteSoSalesOrderHandler : IRequestHandler<DeleteSoSalesOrderCommand, String>
    {
        private readonly ISoSalesOrderCommandRepository _commandRepository;
        private readonly ISoSalesOrderQueryRepository _queryRepository;
        public DeleteSoSalesOrderHandler(ISoSalesOrderCommandRepository customerRepository, ISoSalesOrderQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteSoSalesOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var PlantData = new SoSalesOrder
                {
                    SoSalesOrderId = queryEntity.SoSalesOrderId
                };

                await _commandRepository.DeleteAsync(PlantData);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Plant information has been deleted!";
        }
    }
}
