using Application.Command.designations;
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

namespace Application.Handlers.CommandHandler.Designations
{
    public class CreateDesignationHandler : IRequestHandler<CreateDesignationCommand, DesignationResponse>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        public CreateDesignationHandler(IDesignationCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<DesignationResponse> Handle(CreateDesignationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Designation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new DesignationResponse
            {
                DesignationID = data.DesignationId,
                StatusCodeID = queryEntity.StatusCodeId,
                Name = queryEntity.Name,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
