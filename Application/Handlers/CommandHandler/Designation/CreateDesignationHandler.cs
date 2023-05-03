using Application.Command.Designation;
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

namespace Application.Handlers.CommandHandler
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
               DesignationID = queryEntity.DesignationId,
                StatusCodeID = queryEntity.StatusCodeId,
                AddedByUserID = queryEntity.AddedByUserId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}

