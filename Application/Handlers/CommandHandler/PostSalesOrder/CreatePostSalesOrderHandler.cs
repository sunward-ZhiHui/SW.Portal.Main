using Application.Command.PostSalesOrder;
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
    public class CreatePostSalesOrderHandler : IRequestHandler<CreatePostSalesOrderCommand, PostSalesOrderResponse>
    {
        private readonly IPostSalesOrderCommandRepository _commandRepository;
        public CreatePostSalesOrderHandler(IPostSalesOrderCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<PostSalesOrderResponse> Handle(CreatePostSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<PostSalesOrder>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var PostSalesOrderData = await _commandRepository.AddAsync(customerEntity);
            var response = new PostSalesOrderResponse
            {

                PostSalesOrderID = customerEntity.PostSalesOrderID
            };
            return response;
        }
    }
}
