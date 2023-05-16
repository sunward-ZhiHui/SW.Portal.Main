using Application.Command.Ictmaster;
using Application.Command.LeveMasters;
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
    public class DeleteIctmasterHandler : IRequestHandler<DeleteIctmasterCommand, String>
    {
        private readonly IIctmasterCommandRepository _commandRepository;
        private readonly IIctmasterQueryRepository _queryRepository;
        public DeleteIctmasterHandler(IIctmasterCommandRepository ictmasterRepository, IIctmasterQueryRepository ictmasterQueryRepository)
        {
            _commandRepository = ictmasterRepository;
            _queryRepository = ictmasterQueryRepository;
        }

        public async Task<string> Handle(DeleteIctmasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new Ictmaster
                {
                    IctmasterId = queryEntity.IctmasterId,
                    CompanyId = queryEntity.CompanyId,
                    Name = queryEntity.Name,
                    ParentIctid = queryEntity.ParentIctid,
                    MasterType = queryEntity.MasterType,
                    Description = queryEntity.Description,
                    LayoutPlanId = queryEntity.LayoutPlanId,
                    LocationId = queryEntity.LocationId,
                    SiteId = queryEntity.SiteId,
                    ZoneId = queryEntity.ZoneId,
                    AreaId = queryEntity.AreaId,
                    SpecificAreaId = queryEntity.SpecificAreaId
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "IctMaster information has been deleted!";
        }
    }
    
}
