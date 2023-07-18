using Application.Command.SalesOrderMasterPricingLineSellingMethods;
using Application.Commands;
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
    public class DeleteSalesOrderMasterPricingLineSellingMethodHandler : IRequestHandler<DeleteSalesOrderMasterPricingLineSellingMethodCommand, String>
    {
        private readonly ISalesOrderMasterPricingLineSellingMethodCommandRepository _commandRepository;
        public DeleteSalesOrderMasterPricingLineSellingMethodHandler(ISalesOrderMasterPricingLineSellingMethodCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteSalesOrderMasterPricingLineSellingMethodCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var PlantData = new SalesOrderMasterPricingLineSellingMethod
                {
                   SalesOrderMasterPricingLineSellingMethodId =request.Id
                };

                await _commandRepository.DeleteAsync(PlantData);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Selling Method information has been deleted!";
        }
    }
}
