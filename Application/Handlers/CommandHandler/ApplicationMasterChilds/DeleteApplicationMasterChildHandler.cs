using Application.Command.ApplicationMasterChilds;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteApplicationMasterChildHandler : IRequestHandler<DeleteApplicationMasterChildCommand, String>
    {
        private readonly IApplicationMasterChildCommandRepository _commandRepository;
        private readonly IApplicationMasterChildQueryRepository _queryRepository;
        public DeleteApplicationMasterChildHandler(IApplicationMasterChildCommandRepository customerRepository, IApplicationMasterChildQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteApplicationMasterChildCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetAllByChildIDAsync(request.Id);
                if (queryEntity != null)
                {
                    var data = new ApplicationMasterChild
                    {
                        ApplicationMasterChildId = request.Id,
                        ApplicationMasterParentId = queryEntity.ApplicationMasterParentId,
                        ParentId = queryEntity.ParentId,
                        Value = queryEntity.Value,
                        Description = queryEntity.Description,
                    };
                    await _commandRepository.DeleteAsync(data);
                }
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "ApplicationMaster information has been deleted!";
        }
    }
}
