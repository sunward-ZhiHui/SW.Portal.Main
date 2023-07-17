using Application.Command.SalesOrderMasterPricings;
using Application.Command.SoSalesOrder;
using Application.Command.SalesOrderMasterPricingLineSellingMethods;
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
    public class CreateSalesOrderMasterPricingLineSellingMethodHandler : IRequestHandler<CreateSalesOrderMasterPricingLineSellingMethodCommand, SalesOrderMasterPricingLineSellingMethodResponse>
    {
        private readonly ISalesOrderMasterPricingLineSellingMethodCommandRepository _commandRepository;
        public CreateSalesOrderMasterPricingLineSellingMethodHandler(ISalesOrderMasterPricingLineSellingMethodCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SalesOrderMasterPricingLineSellingMethodResponse> Handle(CreateSalesOrderMasterPricingLineSellingMethodCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricingLineSellingMethod>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var id = await _commandRepository.AddAsync(customerEntity);
            var response = new SalesOrderMasterPricingLineSellingMethodResponse
            {
                SalesOrderMasterPricingLineSellingMethodId = (long)id,
                SalesOrderMasterPricingLineId = customerEntity.SalesOrderMasterPricingLineId
            };
            return response;
        }
    }
}
