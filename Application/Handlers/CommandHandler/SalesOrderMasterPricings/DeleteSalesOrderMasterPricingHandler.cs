using Application.Command.Departments;
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
        private readonly ISalesOrderMasterPricingCommandRepository _commandRepository;
        public DeleteSalesOrderMasterPricingHandler(ISalesOrderMasterPricingCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteSalesOrderMasterPricingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new SalesOrderMasterPricing
                {
                    SalesOrderMasterPricingId= request.Id,
                };
                await _commandRepository.DeleteAsync(data);
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
        private readonly ISalesOrderMasterPricingLineCommandRepository _commandRepository;
        public DeleteSalesOrderMasterPricingLineHandler(ISalesOrderMasterPricingLineCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteSalesOrderMasterPricingLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new SalesOrderMasterPricingLine
                {
                    SalesOrderMasterPricingId = request.Id,
                };
                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "SalesOrderMaster Pricing has been deleted!";
        }
    }
}
