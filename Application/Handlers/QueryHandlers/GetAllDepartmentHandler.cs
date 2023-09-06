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
    public class GetAllDepartmentHandler : IRequestHandler<GetAllDepartmentQuery, List<ViewDepartment>>
    {
        private readonly IDepartmentQueryRepository _queryRepository;
        public GetAllDepartmentHandler(IDepartmentQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewDepartment>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            return (List<ViewDepartment>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetDepartmentByDivisionHandler : IRequestHandler<GetDepartmentByDivision, List<ViewDepartment>>
    {
        private readonly IDepartmentQueryRepository _queryRepository;
        public GetDepartmentByDivisionHandler(IDepartmentQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ViewDepartment>> Handle(GetDepartmentByDivision request, CancellationToken cancellationToken)
        {
            return (List<ViewDepartment>)await _queryRepository.GetDepartmentByDivisionAsync(request.DivisionId);
        }
    }
}
