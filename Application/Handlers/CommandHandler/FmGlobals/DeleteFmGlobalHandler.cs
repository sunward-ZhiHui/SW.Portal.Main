using Application.Command.FmGlobals;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteFmGlobalHandler : IRequestHandler<DeleteFmGlobalCommand, String>
    {
        private readonly IFmGlobalCommandRepository _commandRepository;
        public DeleteFmGlobalHandler(IFmGlobalCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteFmGlobalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new Fmglobal
                {
                    FmglobalId = request.Id,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "FM Global information has been deleted!";
        }
    }

    public class DeleteFmGlobalLineHandler : IRequestHandler<DeleteFmGlobalLineCommand, String>
    {
        private readonly IFmGlobalLineCommandRepository _commandRepository;
        public DeleteFmGlobalLineHandler(IFmGlobalLineCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteFmGlobalLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new FmglobalLine
                {
                    FmglobalLineId = request.Id,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "FM Global Line information has been deleted!";
        }
    }
    public class DeleteFmGlobalLineItemHandler : IRequestHandler<DeleteFmGlobalLineItemCommand, String>
    {
        private readonly IFmGlobalLineItemCommandRepository _commandRepository;
        public DeleteFmGlobalLineItemHandler(IFmGlobalLineItemCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }

        public async Task<string> Handle(DeleteFmGlobalLineItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new FmglobalLineItem
                {
                    FmglobalLineItemId = request.Id,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "FM Global Line Item information has been deleted!";
        }
    }
}
