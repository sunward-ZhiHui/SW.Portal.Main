using Application.Command.SalesOrderMasterPricingLine;
using Application.Command.SalesOrderMasterPricings;
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

namespace Application.Handlers.CommandHandler.SalesOrderMasterPricings
{
    public class CreateSalesOrderMasterPricingHandler : IRequestHandler<CreateSalesOrderMasterPricingCommand, SalesOrderMasterPricingResponse>
    {
        private readonly ISalesOrderMasterPricingCommandRepository _commandRepository;
        private readonly ISalesOrderMasterPricingQueryRepository _queryRepository;
        public CreateSalesOrderMasterPricingHandler(ISalesOrderMasterPricingCommandRepository commandRepository, ISalesOrderMasterPricingQueryRepository queryRepository)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
        }
        public async Task<SalesOrderMasterPricingResponse> Handle(CreateSalesOrderMasterPricingCommand request, CancellationToken cancellationToken)
        {

            var queryEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricing>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }
            var ErrorMessage = "";
            long salesOrderMasterPricingId = -1;
            var CheckPriceValidaityDate = await _queryRepository.GetCheckPriceValidaityDateAsync(queryEntity);
            if (CheckPriceValidaityDate == null)
            {
                salesOrderMasterPricingId = (long)await _commandRepository.AddAsync(queryEntity);
                queryEntity.SalesOrderMasterPricingId = salesOrderMasterPricingId;
                if (queryEntity.MasterType == "MasterPrice")
                {
                    await _queryRepository.InsertSalesOrderMasterPricingLineAsync(queryEntity);
                }
            }
            else
            {
                ErrorMessage = "A Date Range Already Exits From This Company.Current Range From " + CheckPriceValidaityDate.PriceValidaityFrom.Date.ToString("MMM/yyyy") + " To " + CheckPriceValidaityDate.PriceValidaityTo.Date.ToString("MMM/yyyy");
            }
            var response = new SalesOrderMasterPricingResponse
            {
                SalesOrderMasterPricingId = salesOrderMasterPricingId,
                StatusCodeId = queryEntity.StatusCodeId,
                CompanyId = queryEntity.CompanyId,
                SalesPricingForId = queryEntity.SalesPricingForId,
                ErrorMessage = ErrorMessage
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
