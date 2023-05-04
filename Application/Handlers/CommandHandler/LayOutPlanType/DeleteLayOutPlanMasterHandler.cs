using Application.Command.LayoutPlanType;
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

namespace Application.Handlers.CommandHandler.LayOutPlanType
{
    public class DeleteLayOutPlanMasterHandler : IRequestHandler<DeleteLayOutPlanTypeMasterCommand, String>
    {
        private readonly ILayOutPlanCommandRepository _commandRepository;
        private readonly ILayOutPlanTypeQueryRepository _queryRepository;
        public DeleteLayOutPlanMasterHandler(ILayOutPlanCommandRepository customerRepository, ILayOutPlanTypeQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }

        public async Task<string> Handle(DeleteLayOutPlanTypeMasterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryEntity = await _queryRepository.GetByIdAsync(request.Id);
                var data = new LayoutPlanType
                {
                    LayoutPlanTypeId= queryEntity.LayoutPlanTypeId,
                    Name = queryEntity.Name,
                    VersionNo= queryEntity.VersionNo,
                    AddedByUserId = queryEntity.AddedByUserId,
                    StatusCodeId = queryEntity.StatusCodeId,
                };

                await _commandRepository.DeleteAsync(data);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            return "LevelMaster information has been deleted!";
        }
    }
    
}
