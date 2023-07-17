using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetPostSalesOrderHandler : IRequestHandler<GetPostSalesOrder, List<PostSalesOrder>>
    {

        private readonly IPostSalesOrderQueryRepository _postSalesOrderQueryRepository;
        public GetPostSalesOrderHandler(IPostSalesOrderQueryRepository postSalesOrderQueryRepository)
        {
            _postSalesOrderQueryRepository = postSalesOrderQueryRepository;
        }
        public async Task<List<PostSalesOrder>> Handle(GetPostSalesOrder request, CancellationToken cancellationToken)
        {
            return (List<PostSalesOrder>)await _postSalesOrderQueryRepository.GetAllAsync();
        }
    }
    public class PostNAVSalesOrderHandler : IRequestHandler<PostNAVSalesOrderQuery,PostSalesOrder>
    {   
        private readonly ISalesOrderService _salesOrderService;
        public PostNAVSalesOrderHandler(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }
        public async Task<PostSalesOrder> Handle(PostNAVSalesOrderQuery request, CancellationToken cancellationToken)
        {
            Task lst = _salesOrderService.PostSalesOrderAsync(request);

            var response = new PostSalesOrder
            {
                // Initialize properties of the response object
            };

            await lst; // Wait for the asynchronous operation to complete

            return response;
        }
    }
}
