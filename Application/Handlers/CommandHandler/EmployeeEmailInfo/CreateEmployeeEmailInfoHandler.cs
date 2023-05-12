using Application.Command.Departments;
using Application.Command.EmployeeEmailInfos;
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
    public class CreateEmployeeEmailInfoHandler : IRequestHandler<CreateEmployeeEmailInfoCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoCommandRepository _commandRepository;
        public CreateEmployeeEmailInfoHandler(IEmployeeEmailInfoCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(CreateEmployeeEmailInfoCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeEmailInfo>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID= (long)data,
                EmployeeID = request.EmployeeID,
            };
            return response;
        }
    }
    public class CreateEmployeeEmailInfoForwardHandler : IRequestHandler<CreateEmployeeEmailInfoForwardCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoForwardCommandRepository _commandRepository;
        public CreateEmployeeEmailInfoForwardHandler(IEmployeeEmailInfoForwardCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(CreateEmployeeEmailInfoForwardCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoForward>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = (long)data,
            };
            return response;
        }
    }

    public class CreateEmployeeEmailInfoAuthorityHandler : IRequestHandler<CreateEmployeeEmailInfoAuthorityCommand, EmployeeEmailInfoResponse>
    {
        private readonly IEmployeeEmailInfoAuthorityCommandRepository _commandRepository;
        public CreateEmployeeEmailInfoAuthorityHandler(IEmployeeEmailInfoAuthorityCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<EmployeeEmailInfoResponse> Handle(CreateEmployeeEmailInfoAuthorityCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<EmployeeEmailInfoAuthority>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new EmployeeEmailInfoResponse
            {
                EmployeeEmailInfoID = (long)data,
            };
            return response;
        }
    }
}
