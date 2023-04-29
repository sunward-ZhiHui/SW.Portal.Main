using Application.Commands;
using Application.Common.Mapper;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;


namespace Application.Handlers.CommandHandler
{
    public class DeletePlantHandler : IRequestHandler<DeletePlantCommand, String>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IPlantQueryRepository _queryRepository;
        public DeletePlantHandler(IPlantCommandRepository customerRepository, IPlantQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeletePlantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var PlantData = new Plant
                {
                    PlantID = queryEntity.PlantID,
                    CompanyID=queryEntity.CompanyID,
                    PlantCode= queryEntity.PlantCode,
                    Description= queryEntity.Description,
                };

                await _commandRepository.DeleteAsync(PlantData);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "Plant information has been deleted!";
        }
    }
}
