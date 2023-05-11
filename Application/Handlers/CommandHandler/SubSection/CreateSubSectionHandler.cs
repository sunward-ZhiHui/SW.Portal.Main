using Application.Command.Departments;
using Application.Command.SubSections;
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

namespace Application.Handlers.CommandHandler.SubSections
{
    public class CreateSubSectionHandler : IRequestHandler<CreateSubSectionCommand, SubSectionResponse>
    {
        private readonly ISubSectionCommandRepository _commandRepository;
        public CreateSubSectionHandler(ISubSectionCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SubSectionResponse> Handle(CreateSubSectionCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<SubSection>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new SubSectionResponse
            {
                SubSectionId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
                SubSectionName = queryEntity.Name,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
