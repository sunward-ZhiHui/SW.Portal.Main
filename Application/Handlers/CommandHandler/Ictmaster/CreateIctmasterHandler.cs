using Application.Command.Ictmaster;
using Application.Command.LeveMasters;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class CreateIctmasterHandler : IRequestHandler<CreateIctmasterCommand, IctmasterResponse>
    {
        private readonly IIctmasterCommandRepository _commandRepository;
        public CreateIctmasterHandler(IIctmasterCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<IctmasterResponse> Handle(CreateIctmasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Ictmaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new IctmasterResponse
            {
                IctmasterId = queryEntity.IctmasterId,
                CompanyId = queryEntity.CompanyId,
                Name = queryEntity.Name,
                ParentIctid = queryEntity.ParentIctid,
                MasterType = queryEntity.MasterType,
                Description = queryEntity.Description,
                LayoutPlanId = queryEntity.LayoutPlanId,
                LocationId = queryEntity.LocationId,
                ZoneId = queryEntity.ZoneId,
                AreaId = queryEntity.AreaId,
                SpecificAreaId = queryEntity.SpecificAreaId
            };
            return response;
        }
    }
   
}
