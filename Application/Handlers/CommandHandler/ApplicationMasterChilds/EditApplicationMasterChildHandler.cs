using Application.Command.ApplicationMasterChilds;
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
    public class EditApplicationMasterParentHandler : IRequestHandler<EditApplicationMasterChildCommand, ApplicationMasterChildResponse>
    {
        private readonly IApplicationMasterChildCommandRepository _commandRepository;
        public EditApplicationMasterParentHandler(IApplicationMasterChildCommandRepository customerRepository)
        {
            _commandRepository = customerRepository;
        }
        public async Task<ApplicationMasterChildResponse> Handle(EditApplicationMasterChildCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<ApplicationMasterChild>(request);
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
            var response = new ApplicationMasterChildResponse
            {
                ApplicationMasterChildId = queryEntity.ApplicationMasterChildId,
                Value = queryEntity.Value,
                ParentId = queryEntity.ParentId,
                ApplicationMasterParentId = queryEntity.ApplicationMasterParentId,
                Description = queryEntity.Description,
            };
            return response;
        }
    }
}
