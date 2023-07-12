using Application.Command.ApplicationMasterDetails;
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

namespace Application.Handlers.CommandHandler.ApplicationMasterDetails
{
    public class CreateApplicationMasterDetailHandler : IRequestHandler<CreateApplicationMasterDetailCommand, ApplicationMasterDetailResponse>
    {
        private readonly IApplicationMasterDetailCommandRepository _commandRepository;
        public CreateApplicationMasterDetailHandler(IApplicationMasterDetailCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<ApplicationMasterDetailResponse> Handle(CreateApplicationMasterDetailCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<ApplicationMasterDetail>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new ApplicationMasterDetailResponse
            {
                ApplicationMasterDetailId = (long)data,
                Value = queryEntity.Value,
                ApplicationMasterId = queryEntity.ApplicationMasterId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
