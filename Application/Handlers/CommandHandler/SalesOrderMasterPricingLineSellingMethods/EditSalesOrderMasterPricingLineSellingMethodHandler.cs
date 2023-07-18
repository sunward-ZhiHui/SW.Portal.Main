using Application.Command.SalesOrderMasterPricingLineSellingMethods;
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
    public class EditSalesOrderMasterPricingLineSellingMethodHandler : IRequestHandler<EditSalesOrderMasterPricingLineSellingMethodCommand, SalesOrderMasterPricingLineSellingMethodResponse>
    {
        private readonly ISalesOrderMasterPricingLineSellingMethodCommandRepository _commandRepository;
        public EditSalesOrderMasterPricingLineSellingMethodHandler(ISalesOrderMasterPricingLineSellingMethodCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<SalesOrderMasterPricingLineSellingMethodResponse> Handle(EditSalesOrderMasterPricingLineSellingMethodCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<SalesOrderMasterPricingLineSellingMethod>(request);

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
            var response = new SalesOrderMasterPricingLineSellingMethodResponse
            {
                SalesOrderMasterPricingLineSellingMethodId = queryrEntity.SalesOrderMasterPricingLineSellingMethodId,
            };

            return response;
        }
    }
}
