using Application.Command.ApplicationMasterDetails;
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
    public class EditApplicationMasterDetailHandler : IRequestHandler<EditApplicationMasterDetailCommand, ApplicationMasterDetailResponse>
    {
        private readonly IApplicationMasterDetailCommandRepository _commandRepository;
        private readonly IApplicationMasterDetailQueryRepository _queryRepository;
        public EditApplicationMasterDetailHandler(IApplicationMasterDetailCommandRepository customerRepository, IApplicationMasterDetailQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<ApplicationMasterDetailResponse> Handle(EditApplicationMasterDetailCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<ApplicationMasterDetail>(request);
            if (queryEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _commandRepository.UpdateAsync(queryEntity);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }
            var response = new ApplicationMasterDetailResponse
            {
                ApplicationMasterDetailId = queryEntity.ApplicationMasterDetailId,
                Value = queryEntity.Value,
                ApplicationMasterId = queryEntity.ApplicationMasterId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
