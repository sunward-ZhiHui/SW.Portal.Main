using Application.Command.ApplicationMasterDetails;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteApplicationMasterDetailHandler : IRequestHandler<DeleteApplicationMasterDetailCommand, String>
    {
        private readonly IApplicationMasterDetailCommandRepository _commandRepository;
        private readonly IApplicationMasterDetailQueryRepository _queryRepository;
        public DeleteApplicationMasterDetailHandler(IApplicationMasterDetailCommandRepository customerRepository, IApplicationMasterDetailQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteApplicationMasterDetailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new ApplicationMasterDetail
                {
                    ApplicationMasterDetailId= request.Id,
                    ApplicationMasterId = queryEntity.ApplicationMasterID.Value,
                    Value=queryEntity.Value,
                    Description= queryEntity.Description,
                };
                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "ApplicationMaster Detail information has been deleted!";
        }
    }
}
