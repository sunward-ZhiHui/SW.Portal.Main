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
    public class EditDivisionHandler : IRequestHandler<EditDivisionCommand, DivisionResponse>
    {
        private readonly IDivisionCommandRepository _commandRepository;
        private readonly IDivisionQueryRepository _queryRepository;
        public EditDivisionHandler(IDivisionCommandRepository customerRepository, IDivisionQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<DivisionResponse> Handle(EditDivisionCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<Division>(request);

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
            var response = new DivisionResponse
            {
                DivisionID = queryrEntity.DivisionId,
                CompanyId = queryrEntity.CompanyId,
                AddedByUserID = queryrEntity.AddedByUserId,
                StatusCodeID = queryrEntity.StatusCodeId,
            };

            return response;
        }
    }
}
