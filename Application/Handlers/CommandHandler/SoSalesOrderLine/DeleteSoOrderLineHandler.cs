using Application.Command.SoSalesOrderLine;
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
    public class DeleteSoOrderLineHandler : IRequestHandler<DeleteSoSalesOrderLineCommand, String>
    {
        private readonly ISoSalesOrderLineCommandRepository _commandRepository;
        private readonly ISoSalesOrderLineQueryRepository _queryRepository;
        public DeleteSoOrderLineHandler(ISoSalesOrderLineCommandRepository customerRepository, ISoSalesOrderLineQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteSoSalesOrderLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var PlantData = new SoSalesOrderLine
                {
                   SoSalesOrderLineId =queryEntity.SoSalesOrderLineId
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
