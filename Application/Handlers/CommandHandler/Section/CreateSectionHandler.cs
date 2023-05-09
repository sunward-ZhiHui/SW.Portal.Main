using Application.Command.Departments;
using Application.Command.Sections;
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

namespace Application.Handlers.CommandHandler.Sections
{
    public class CreateSectionHandler : IRequestHandler<CreateSectionCommand, SectionResponse>
    {
        private readonly ISectionCommandRepository _commandRepository;
        public CreateSectionHandler(ISectionCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<SectionResponse> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Section>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new SectionResponse
            {
                SectionId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
                SectionName = queryEntity.Name,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
