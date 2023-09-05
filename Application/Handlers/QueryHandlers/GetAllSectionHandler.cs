using Application.Queries;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllSectionHandler : IRequestHandler<GetAllSectionQuery, List<ViewSection>>
    {
        private readonly ISectionQueryRepository _queryRepository;
        public GetAllSectionHandler(ISectionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewSection>> Handle(GetAllSectionQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewSection>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetSectionByDepartmentHandler : IRequestHandler<GetSectionByDepartment, List<ViewSection>>
    {
        private readonly ISectionQueryRepository _queryRepository;
        public  GetSectionByDepartmentHandler(ISectionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewSection>> Handle(GetSectionByDepartment request, CancellationToken cancellationToken)
        {
            return (List<ViewSection>)await _queryRepository.GetSectionByDepartment(request.Id);
        }
    }
}
