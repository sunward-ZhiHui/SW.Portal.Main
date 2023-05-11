using Application.Command.designations;
using Application.Command.Ictmaster;
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
    public class EditIctmasterHandler : IRequestHandler<EditIctmasterCommand, IctmasterResponse>
    {
        private readonly IIctmasterCommandRepository _commandRepository;
        private readonly IIctmasterQueryRepository _queryRepository;
        public EditIctmasterHandler(IIctmasterCommandRepository ictmasterRepository, IIctmasterQueryRepository ictmasterQueryRepository)
        {
            _commandRepository = ictmasterRepository;
            _queryRepository = ictmasterQueryRepository;
        }
        public async Task<IctmasterResponse> Handle(EditIctmasterCommand request, CancellationToken cancellationToken)
        {
            var queryEntity = RoleMapper.Mapper.Map<Ictmaster>(request);

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
            var response = new IctmasterResponse
            {
                IctmasterId = queryEntity.IctmasterId,
                CompanyId = queryEntity.CompanyId,
                Name = queryEntity.Name,
                ParentIctid = queryEntity.ParentIctid,
                MasterType = queryEntity.MasterType,
                Description = queryEntity.Description,
                LayoutPlanId = queryEntity.LayoutPlanId,
                LocationId = queryEntity.LocationId,
                ZoneId = queryEntity.ZoneId,
                AreaId = queryEntity.AreaId,
                SpecificAreaId = queryEntity.SpecificAreaId
            };

            return response;
        }
    }
   
}
