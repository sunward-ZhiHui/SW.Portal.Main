using Application.Command.designations;
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
    public class EditDesignationHandler : IRequestHandler<EditDesignationCommand, DesignationResponse>
    {
        private readonly IDesignationCommandRepository _commandRepository;
        private readonly IDesignationQueryRepository _queryRepository;
        public EditDesignationHandler(IDesignationCommandRepository customerRepository, IDesignationQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<DesignationResponse> Handle(EditDesignationCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<Designation>(request);

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
            var response = new DesignationResponse
            {
                DesignationID = queryrEntity.DesignationId,
                CompanyID = queryrEntity.CompanyId.Value,
                AddedByUserID = queryrEntity.AddedByUserId,
                StatusCodeID = queryrEntity.StatusCodeId,
                Name =queryrEntity.Name,
            };

            return response;
        }
    }
}
