using Application.Command.Departments;
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
    public class EditDepartmentHandler : IRequestHandler<EditDepartmentCommand, DepartmentResponse>
    {
        private readonly IDepartmentCommandRepository _commandRepository;
        private readonly IDepartmentQueryRepository _queryRepository;
        public EditDepartmentHandler(IDepartmentCommandRepository customerRepository, IDepartmentQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<DepartmentResponse> Handle(EditDepartmentCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<Department>(request);

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
            var response = new DepartmentResponse
            {
                DepartmentId = queryrEntity.DepartmentId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                DepartmentName =queryrEntity.Name,
            };

            return response;
        }
    }
}
