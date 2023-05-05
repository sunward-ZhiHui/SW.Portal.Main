using Application.Command.LayoutPlanType;
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

namespace Application.Handlers.CommandHandler.LayOutPlanType
{
    public class CreateLayOutPlanMasterHandler: IRequestHandler<CreateLayOutPlanTypeMasterCommand,LayOutPlanTypeResponse>
    {
        private readonly ILayOutPlanCommandRepository _commandRepository;
        public CreateLayOutPlanMasterHandler(ILayOutPlanCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<LayOutPlanTypeResponse> Handle(CreateLayOutPlanTypeMasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<LayoutPlanType>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new LayOutPlanTypeResponse
            {
                LayoutPlanTypeId= queryEntity.LayoutPlanTypeId,
                SessionId = queryEntity.SessionId,
                Name = queryEntity.Name,
            };
            return response;
        }
    }
}
