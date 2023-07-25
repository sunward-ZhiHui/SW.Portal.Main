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
        private readonly ISalesOrderMasterPricingLineSellingMethodCommandRepository _SellingMethodRepository;
        public EditSalesOrderMasterPricingLineHandler(ISalesOrderMasterPricingLineCommandRepository customerRepository, ISalesOrderMasterPricingLineSellingMethodCommandRepository sellingMethodRepository)
        {
            _commandRepository = customerRepository;
            _SellingMethodRepository = sellingMethodRepository;
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
                if (request.SalesOrderMasterPricingLineSellingMethods != null && request.SalesOrderMasterPricingLineSellingMethods.Count > 0)
                {
                    request.SalesOrderMasterPricingLineSellingMethods.ForEach(s =>
                    {
                        var employeeReportTo = new SalesOrderMasterPricingLineSellingMethod();
                        employeeReportTo.SalesOrderMasterPricingLineId = request.SalesOrderMasterPricingLineId;
                        employeeReportTo.TierPrice = s.TierPrice;
                        employeeReportTo.TierFromQty = s.TierFromQty;
                        employeeReportTo.TierToQty = s.TierToQty;
                        employeeReportTo.BounsPrice = s.BounsPrice;
                        employeeReportTo.BounsFocQty = s.BounsFocQty;
                        if (s.SalesOrderMasterPricingLineSellingMethodId > 0)
                        {
                            employeeReportTo.SalesOrderMasterPricingLineSellingMethodId = s.SalesOrderMasterPricingLineSellingMethodId;
                            _SellingMethodRepository.UpdateAsync(employeeReportTo);
                        }
                        else
                        {
                            _SellingMethodRepository.AddAsync(employeeReportTo);
                        }
                    });
                }
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
