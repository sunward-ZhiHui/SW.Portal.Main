using Application.Command.SalesOrderMasterPricingLine;
using Application.Command.SalesOrderMasterPricings;
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

namespace Application.Handlers.CommandHandler.SalesOrderMasterPricings
{
    public class CreateSalesOrderMasterPricingHandler : IRequestHandler<CreateSalesOrderMasterPricingCommand, SalesOrderMasterPricingResponse>
    {
        private readonly ISalesOrderMasterPricingCommandRepository _commandRepository;
        public CreateSalesOrderMasterPricingHandler(ISalesOrderMasterPricingCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SalesOrderMasterPricingResponse> Handle(CreateSalesOrderMasterPricingCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricing>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var salesOrderMasterPricingId = await _commandRepository.AddAsync(queryEntity);
            var response = new SalesOrderMasterPricingResponse
            {
                SalesOrderMasterPricingId = (long)salesOrderMasterPricingId,
                StatusCodeId = queryEntity.StatusCodeId,
                CompanyId = queryEntity.CompanyId,
                SalesPricingForId = queryEntity.SalesPricingForId,
            };
            return response;
        }
    }



    public class CreateSalesOrderMasterPricingLineHandler : IRequestHandler<CreateSalesOrderMasterPricingLineCommand, SalesOrderMasterPricingLineResponse>
    {
        private readonly ISalesOrderMasterPricingLineCommandRepository _commandRepository;
        public CreateSalesOrderMasterPricingLineHandler(ISalesOrderMasterPricingLineCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SalesOrderMasterPricingLineResponse> Handle(CreateSalesOrderMasterPricingLineCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricingLine>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var salesOrderMasterPricingId = await _commandRepository.AddAsync(queryEntity);
            var response = new SalesOrderMasterPricingLineResponse
            {
                
                StatusCodeId = queryEntity.StatusCodeId,
                ItemId = queryEntity.ItemId,
                SellingMethodId = queryEntity.SellingMethodId,
            };
            return response;
        }
    }
}
