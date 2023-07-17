using Application.Command.Departments;
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

namespace Application.Handlers.CommandHandler
{
    public class EditSalesOrderMasterPricingHandler : IRequestHandler<EditSalesOrderMasterPricingCommand, SalesOrderMasterPricingResponse>
    {
        private readonly ISalesOrderMasterPricingCommandRepository _commandRepository;
        public EditSalesOrderMasterPricingHandler(ISalesOrderMasterPricingCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<SalesOrderMasterPricingResponse> Handle(EditSalesOrderMasterPricingCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricing>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new SalesOrderMasterPricingResponse
            {
                SalesOrderMasterPricingId = queryEntity.SalesOrderMasterPricingId,
                StatusCodeId = queryEntity.StatusCodeId,
                CompanyId = queryEntity.CompanyId,
                SalesPricingForId = queryEntity.SalesPricingForId,
            };

            return response;
        }
    }


    public class EditSalesOrderMasterPricingLineHandler : IRequestHandler<EditSalesOrderMasterPricingLineCommand, SalesOrderMasterPricingLineResponse>
    {
        private readonly ISalesOrderMasterPricingLineCommandRepository _commandRepository;
        public EditSalesOrderMasterPricingLineHandler(ISalesOrderMasterPricingLineCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<SalesOrderMasterPricingLineResponse> Handle(EditSalesOrderMasterPricingLineCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricingLine>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new SalesOrderMasterPricingLineResponse
            {
                SalesOrderMasterPricingId = queryEntity.SalesOrderMasterPricingId,
                StatusCodeId = queryEntity.StatusCodeId,
                ItemId = queryEntity.ItemId,
                SellingMethodId = queryEntity.SellingMethodId,
            };

            return response;
        }
    }
}
