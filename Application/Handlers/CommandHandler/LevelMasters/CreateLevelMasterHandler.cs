using Application.Command.LeveMasters;
using Application.Commands;
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

namespace Application.Handlers.CommandHandler.LevelMasters
{
    public class CreateLevelMasterHandler : IRequestHandler<CreateLevelMasterCommand, LevelMasterResponse>
    {
        private readonly ILevelMasterCommandRepository _commandRepository;
        public CreateLevelMasterHandler(ILevelMasterCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<LevelMasterResponse> Handle(CreateLevelMasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<LevelMaster>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new LevelMasterResponse
            {
                LevelID = queryEntity.LevelId,
                StatusCodeID = queryEntity.StatusCodeId,
                Name = queryEntity.Name,
            };
            return response;
        }
    }
}
