using Application.Command.LayoutPlanType;
using Application.Command.LeveMasters;
using Application.Common.Mapper;
using Application.Response;
using Core.Entities;
using Core.Repositories.Command;
using Core.Repositories.Command.Base;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.CommandHandler.LayOutPlanType
{
    public class EditLayOutPlanMasterHandler : IRequestHandler<EditLayOutPlanTypeMasterCommand, LayOutPlanTypeResponse>
    {
        private readonly ILayOutPlanCommandRepository _commandRepository;
        private readonly ILayOutPlanTypeQueryRepository _queryRepository;
        public EditLayOutPlanMasterHandler(ILayOutPlanCommandRepository customerRepository, ILayOutPlanTypeQueryRepository customerQueryRepository)
        {
            _commandRepository = customerRepository;
            _queryRepository = customerQueryRepository;
        }
        public async Task<LayOutPlanTypeResponse> Handle(EditLayOutPlanTypeMasterCommand request, CancellationToken cancellationToken)
        {
            var queryrEntity = RoleMapper.Mapper.Map<LayoutPlanType>(request);

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
            var response = new LayOutPlanTypeResponse
            {
                LayoutPlanTypeId= queryrEntity.LayoutPlanTypeId,
                AddedByUserId = queryrEntity.AddedByUserId,
                StatusCodeId = queryrEntity.StatusCodeId,
                Name = queryrEntity.Name,
                VersionNo= queryrEntity.VersionNo,
            };

            return response;
        }

    }
}