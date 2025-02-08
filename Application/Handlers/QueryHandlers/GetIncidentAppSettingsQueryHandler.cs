using Application.Common.Mapper;
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
    public class GetIncidentAppSettingsQueryHandler : IRequestHandler<GetAllIncidentAppSettingsQuery, List<IncidentAppSettings>>
    {
        private readonly IIncidentAppSettingsQueryRepository _QueryRepostitory;
        public GetIncidentAppSettingsQueryHandler(IIncidentAppSettingsQueryRepository QueryRepostitory)
        {
            _QueryRepostitory = QueryRepostitory;
        }
        public async Task<List<IncidentAppSettings>> Handle(GetAllIncidentAppSettingsQuery request, CancellationToken cancellationToken)
        {
            return (List<IncidentAppSettings>)await _QueryRepostitory.GetAllAsync();
        }

    }
    public class CreateEmailTopicsHandler : IRequestHandler<CreateincidentApp, long>
    {
        private readonly IIncidentAppSettingsQueryRepository _QueryRepository;

        public CreateEmailTopicsHandler(IIncidentAppSettingsQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }
        public async Task<long> Handle(CreateincidentApp request, CancellationToken cancellationToken)
        {

            var newTopics = await _QueryRepository.Insert(request);
         
            return newTopics;
        }
    }

    public class EditIncidentAppHandler : IRequestHandler<EditIncidentApp, long>
    {
        private readonly IIncidentAppSettingsQueryRepository _QueryRepository;

        public EditIncidentAppHandler(IIncidentAppSettingsQueryRepository QueryRepository)
        {
            _QueryRepository = QueryRepository;
        }
        public async Task<long> Handle(EditIncidentApp request, CancellationToken cancellationToken)
        {
            

            var newTopics = await _QueryRepository.Update(request);
           
            return newTopics;
        }
    }

}
