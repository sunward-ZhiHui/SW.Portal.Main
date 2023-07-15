using Application.Command.PostSalesOrder;
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
    public class DeletePostSalesOrderHandler : IRequestHandler<DeletePostSalesOrderCommand, String>
    {
        private readonly IPostSalesOrderCommandRepository _commandRepository;
        private readonly IPostSalesOrderQueryRepository _queryRepository;
        public DeletePostSalesOrderHandler(IPostSalesOrderCommandRepository customerRepository, IPostSalesOrderQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeletePostSalesOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var PostSalesOrderData = new PostSalesOrder
                {
                   PostSalesOrderID =queryEntity.PostSalesOrderID
                };

                await _commandRepository.DeleteAsync(PostSalesOrderData);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Plant information has been deleted!";
        }
    }
}
