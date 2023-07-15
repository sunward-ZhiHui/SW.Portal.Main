using Application.Command.PostSalesOrder;
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
    public class EditPostSalesOrderHandler : IRequestHandler<EditPostSalesOrderCommand, PostSalesOrderResponse>
    {
        private readonly IPostSalesOrderCommandRepository _commandRepository;
        private readonly IPostSalesOrderQueryRepository _queryRepository;
        public EditPostSalesOrderHandler(IPostSalesOrderCommandRepository customerRepository, IPostSalesOrderQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<PostSalesOrderResponse> Handle(EditPostSalesOrderCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<PostSalesOrder>(request);

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
            var response = new PostSalesOrderResponse
            {
                PostSalesOrderID = queryrEntity.PostSalesOrderID,
               
            };

            return response;
        }
    }
}
