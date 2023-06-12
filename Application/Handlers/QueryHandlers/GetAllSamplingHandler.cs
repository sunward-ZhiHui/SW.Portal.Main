using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllSamplingHandler  : IRequestHandler<GetAllSamplingQuery, List<AppSampling>>
    {
        private readonly IAppsamplingQueryRepository _samplingqueryRepository;
        private readonly IQueryRepository<AppSampling> _queryRepository;
        public GetAllSamplingHandler(IAppsamplingQueryRepository samplingqueryRepository)
        {
            _samplingqueryRepository = samplingqueryRepository;
        }
        public async Task<List<AppSampling>> Handle(GetAllSamplingQuery request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
             return (List<AppSampling>)await _samplingqueryRepository.GetListAsync();
           // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }
    public class GetAllSamplingLineHandler : IRequestHandler<GetAllSamplingLineQuery, List<AppSamplingLine>>
    {
        private readonly IAppsamplingQueryRepository _samplingqueryRepository;
        private readonly IQueryRepository<AppSamplingLine> _queryRepository;
        public GetAllSamplingLineHandler(IAppsamplingQueryRepository samplingqueryRepository)
        {
            _samplingqueryRepository = samplingqueryRepository;
        }
        public async Task<List<AppSamplingLine>> Handle(GetAllSamplingLineQuery request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<AppSamplingLine>)await _samplingqueryRepository.GetAllLineAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }

}
