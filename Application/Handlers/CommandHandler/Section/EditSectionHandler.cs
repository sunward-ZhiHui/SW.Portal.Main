using Application.Command.Departments;
using Application.Command.Sections;
using Application.Commands;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler
{
    public class EditSectionHandler : IRequestHandler<EditSectionCommand, SectionResponse>
    {
        private readonly ISectionCommandRepository _commandRepository;
        private readonly ISectionQueryRepository _queryRepository;
        public EditSectionHandler(ISectionCommandRepository customerRepository, ISectionQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<SectionResponse> Handle(EditSectionCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<Section>(request);

            if (queryrEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryrEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new SectionResponse
            {
                SectionId = queryrEntity.SectionId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                SectionName =queryrEntity.Name,
            };

            return response;
        }
    }
}
