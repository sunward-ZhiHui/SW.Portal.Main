using Application.Command.Departments;
using Application.Command.EmployeeEmailInfos;
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
    public class EditEmployeeEmailInfoHandler : IRequestHandler<EditEmployeeEmailInfoCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoQueryRepository _queryRepository;
        public EditEmployeeEmailInfoHandler(IEmployeeEmailInfoCommandRepository customerRepository, IEmployeeEmailInfoQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(EditEmployeeEmailInfoCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeEmailInfo>(request);

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
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = request.EmployeeEmailInfoID,
                EmployeeID = request.EmployeeID,
            };

            return response;
        }
    }
    public class EditEmployeeEmailInfoForwardHandler : IRequestHandler<EditEmployeeEmailInfoForwardCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoForwardCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoForwardQueryRepository _queryRepository;
        public EditEmployeeEmailInfoForwardHandler(IEmployeeEmailInfoForwardCommandRepository customerRepository, IEmployeeEmailInfoForwardQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(EditEmployeeEmailInfoForwardCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoForward>(request);

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
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = 1,
            };

            return response;
        }
    }

    public class EditEmployeeEmailInfoAuthorityHandler : IRequestHandler<EditEmployeeEmailInfoAuthorityCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoAuthorityCommandRepository _commandRepository;
        private readonly IEmployeeEmailInfoAuthorityQueryRepository _queryRepository;
        public EditEmployeeEmailInfoAuthorityHandler(IEmployeeEmailInfoAuthorityCommandRepository customerRepository, IEmployeeEmailInfoAuthorityQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(EditEmployeeEmailInfoAuthorityCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoAuthority>(request);

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
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = 1,
            };

            return response;
        }
    }
}
