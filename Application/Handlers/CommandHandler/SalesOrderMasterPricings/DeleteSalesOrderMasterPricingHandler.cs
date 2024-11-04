using Application.Command.Departments;
using Application.Command.SalesOrderMasterPricingLine;
using Application.Command.SalesOrderMasterPricings;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteSalesOrderMasterPricingHandler : IRequestHandler<DeleteSalesOrderMasterPricingCommand, String>
    {
        private readonly ISalesOrderMasterPricingQueryRepository _commandRepository;
        public DeleteSalesOrderMasterPricingHandler(ISalesOrderMasterPricingQueryRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteSalesOrderMasterPricingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _commandRepository.DeleteSalesOrderMasterPricing(request.Id);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "SalesOrderMaster Pricing has been deleted!";
        }
    }



    public class DeleteSalesOrderMasterPricingLineHandler : IRequestHandler<DeleteSalesOrderMasterPricingLineCommand, String>
    {
        private readonly ISalesOrderMasterPricingQueryRepository _commandRepository;
        public DeleteSalesOrderMasterPricingLineHandler(ISalesOrderMasterPricingQueryRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteSalesOrderMasterPricingLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _commandRepository.DeleteSalesOrderMasterPricingLine(request.Id);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "SalesOrderMaster Pricing has been deleted!";
        }
    }
}
