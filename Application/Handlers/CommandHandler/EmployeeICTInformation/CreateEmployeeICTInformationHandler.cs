using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
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
    public class CreateEmployeeICTInformationHandler : IRequestHandler<CreateEmployeeICTInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        public CreateEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(CreateEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeICTInformation>(request);

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
    public class CreateEmployeeICTHardInformationHandler : IRequestHandler<CreateEmployeeICTHardInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        public CreateEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(CreateEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeICTHardInformation>(request);

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
