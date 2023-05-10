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
    public class CreateDivisionHandler : IRequestHandler<CreateDivisionCommand, DivisionResponse>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        public CreateDivisionHandler(IDivisionCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<DivisionResponse> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Division>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new DivisionResponse
            {
                DivisionID = (long)data,
                StatusCodeID = queryEntity.StatusCodeId,
                AddedByUserID = queryEntity.AddedByUserId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
