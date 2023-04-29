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
    public class EditPlantHandler : IRequestHandler<EditPlantCommand, PlantResponse>
    {
        private readonly IPlantCommandRepository _commandRepository;
        private readonly IPlantQueryRepository _queryRepository;
        public EditPlantHandler(IPlantCommandRepository customerRepository, IPlantQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<PlantResponse> Handle(EditPlantCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<Plant>(request);

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
            var response = new PlantResponse
            {
                PlantID = queryrEntity.PlantID,
                CompanyID = queryrEntity.CompanyID,
                PlantCode = queryrEntity.PlantCode,
                Description = queryrEntity.Description,
            };

            return response;
        }
    }
}
