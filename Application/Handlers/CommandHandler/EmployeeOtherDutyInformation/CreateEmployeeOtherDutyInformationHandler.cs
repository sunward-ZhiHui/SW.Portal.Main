using Application.Command.Departments;
using Application.Command.EmployeeOtherDutyInformations;
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
    public class CreateEmployeeOtherDutyInformationHandler : IRequestHandler<CreateEmployeeOtherDutyInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeOtherDutyInformationCommandRepository _commandRepository;
        public CreateEmployeeOtherDutyInformationHandler(IEmployeeOtherDutyInformationCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(CreateEmployeeOtherDutyInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeOtherDutyInformation>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new EmployeeOtherDutyInformationResponse
            {
                DepartmentId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
            };
            return response;
        }
    }
}
