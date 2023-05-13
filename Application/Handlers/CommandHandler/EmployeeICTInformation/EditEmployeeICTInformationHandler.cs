using Application.Command.Departments;
using Application.Command.EmployeeICTInformations;
using Application.Command.EmployeeOtherDutyInformations;
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
    public class EditEmployeeICTInformationHandler : IRequestHandler<EditEmployeeICTInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTInformationQueryRepository _queryRepository;
        public EditEmployeeICTInformationHandler(IEmployeeICTInformationCommandRepository customerRepository, IEmployeeICTInformationQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeICTInformationCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeICTInformation>(request);

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
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
    public class EditEmployeeICTHardInformationHandler : IRequestHandler<EditEmployeeICTHardInformationCommand, EmployeeOtherDutyInformationResponse>
    {
        private readonly IEmployeeICTHardInformationCommandRepository _commandRepository;
        private readonly IEmployeeICTHardInformationQueryRepository _queryRepository;
        public EditEmployeeICTHardInformationHandler(IEmployeeICTHardInformationCommandRepository customerRepository, IEmployeeICTHardInformationQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeOtherDutyInformationResponse> Handle(EditEmployeeICTHardInformationCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeICTHardInformation>(request);

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
            var response = new EmployeeOtherDutyInformationResponse
            {
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
