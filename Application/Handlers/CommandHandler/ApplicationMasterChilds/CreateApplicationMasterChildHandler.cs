using Application.Command.ApplicationMasterChilds;
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

namespace Application.Handlers.CommandHandler.ApplicationMasterChilds
{
    public class CreateApplicationMasterChildHandler : IRequestHandler<CreateApplicationMasterChildCommand, ApplicationMasterChildResponse>
    {
        private readonly IApplicationMasterChildCommandRepository _commandRepository;
        public CreateApplicationMasterChildHandler(IApplicationMasterChildCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<ApplicationMasterChildResponse> Handle(CreateApplicationMasterChildCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<ApplicationMasterChild>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new ApplicationMasterChildResponse
            {
                ApplicationMasterChildId = (long)data,
                Value = queryEntity.Value,
                ParentId=queryEntity.ParentId,
                ApplicationMasterParentId = queryEntity.ApplicationMasterParentId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
