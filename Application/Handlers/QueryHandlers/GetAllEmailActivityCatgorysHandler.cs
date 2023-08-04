using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
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
    public class GetAllEmailActivityCatgorysHandler : IRequestHandler<GetAllEmailActivityCatgorys, List<EmailActivityCatgorys>>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        // private readonly IQueryRepository<ViewProductionEntry> _queryRepository;
        public GetAllEmailActivityCatgorysHandler(IEmailActivityCatgorysQueryRepository emailactyqueryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }
        public async Task<List<EmailActivityCatgorys>> Handle(GetAllEmailActivityCatgorys request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<EmailActivityCatgorys>)await _emailactyqueryRepository.GetAllAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }
    }
    public class CreateEmailActivityCatgorysHandlerr : IRequestHandler<CreateEmailActivityCatgorysQuery, long>
    {
        private readonly IEmailActivityCatgorysQueryRepository _emailactyqueryRepository;
        public CreateEmailActivityCatgorysHandlerr(IEmailActivityCatgorysQueryRepository emailactyqueryRepository, IQueryRepository<EmailActivityCatgorys> queryRepository)
        {
            _emailactyqueryRepository = emailactyqueryRepository;
        }

        public async Task<long> Handle(CreateEmailActivityCatgorysQuery request, CancellationToken cancellationToken)
        {
            var newlist = await _emailactyqueryRepository.Insert(request);
            return newlist;

        }

    }
}
