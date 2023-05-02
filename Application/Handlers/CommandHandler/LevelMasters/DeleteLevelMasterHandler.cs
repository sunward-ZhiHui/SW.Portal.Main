using Application.Command.LeveMasters;
using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeleteLevelMasterHandler : IRequestHandler<DeleteLevelMasterCommand, String>
    {
        private readonly ILevelMasterCommandRepository _commandRepository;
        private readonly ILevelMasterQueryRepository _queryRepository;
        public DeleteLevelMasterHandler(ILevelMasterCommandRepository customerRepository, ILevelMasterQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteLevelMasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new LevelMaster
                {
                    LevelId= queryEntity.LevelID,
                    CompanyId = queryEntity.CompanyID,
                    Name=queryEntity.Name,
                    AddedByUserId= queryEntity.AddedByUserID,
                    StatusCodeId= queryEntity.StatusCodeID.Value,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "LevelMaster information has been deleted!";
        }
    }
}
