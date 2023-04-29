using Application.Command.Departments;
using Application.Command.SubSections;
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
    public class EditSubSectionHandler : IRequestHandler<EditSubSectionCommand, SubSectionResponse>
    {
        private readonly ISubSectionCommandRepository _commandRepository;
        private readonly ISubSectionQueryRepository _queryRepository;
        public EditSubSectionHandler(ISubSectionCommandRepository customerRepository, ISubSectionQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<SubSectionResponse> Handle(EditSubSectionCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<SubSection>(request);

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
            var response = new SubSectionResponse
            {
                SubSectionId = queryrEntity.SubSectionId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                SubSectionName =queryrEntity.Name,
            };

            return response;
        }
    }
}
