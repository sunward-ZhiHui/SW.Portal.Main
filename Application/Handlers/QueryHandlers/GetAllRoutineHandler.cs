using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllRoutineHandler : IRequestHandler<GetAllRoutine, List<ProductionActivityRoutineAppLine>>
    {
        private readonly IDynamicFormQueryRepository _dynamicFormQueryRepository;
        public GetAllRoutineHandler(IDynamicFormQueryRepository dynamicFormQueryRepository)
        {
            _dynamicFormQueryRepository = dynamicFormQueryRepository;
        }
        public async Task<List<ProductionActivityRoutineAppLine>> Handle(GetAllRoutine request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityRoutineAppLine>)await _dynamicFormQueryRepository.GetAllAsync();
        }
    }
}
