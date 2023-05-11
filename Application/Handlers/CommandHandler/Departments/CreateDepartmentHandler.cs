using Application.Command.Departments;
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

namespace Application.Handlers.CommandHandler.Departments
{
    public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, DepartmentResponse>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        public CreateDepartmentHandler(IDepartmentCommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }
        public async Task<DepartmentResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Department>(request);

            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            var data = await _commandRepository.AddAsync(queryEntity);
            var response = new DepartmentResponse
            {
                DepartmentId = (long)data,
                StatusCodeId = queryEntity.StatusCodeId,
                DepartmentName = queryEntity.Name,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
