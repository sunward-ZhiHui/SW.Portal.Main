using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.EntityModel;
using Core.Repositories.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.Plants
{
    public class CreatePlantHandler : IRequestHandler<CreatePlantCommand, PlantResponse>
    {
        private readonly IPlantCommandRepository _commandRepository;
        public CreatePlantHandler(IPlantCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<PlantResponse> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
        {
            var customerEntity = RoleMapper.Mapper.Map<Plant>(request);

            if (customerEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var plantData = await _commandRepository.AddAsync(customerEntity);
            var response = new PlantResponse
            {
                PlantID = (long)plantData,
                CompanyID = customerEntity.CompanyID,
                PlantCode = customerEntity.PlantCode,
                Description = customerEntity.Description,
            };
            return response;
        }
    }
}
