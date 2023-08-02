using Application.Queries;
using Core.Entities;
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
    public class GetAllReportFileUploadsHandler : IRequestHandler<GetAllReportFileUploadsQuery, List<ReportDocuments>>
    {
        private readonly IReportFileUploadsQueryRepository _fileuploadqueryRepository;
        // private readonly IQueryRepository<ViewProductionEntry> _queryRepository;
        public GetAllReportFileUploadsHandler(IReportFileUploadsQueryRepository fileuploadqueryRepository)
        {
            _fileuploadqueryRepository = fileuploadqueryRepository;
        }
        public async Task<List<ReportDocuments>> Handle(GetAllReportFileUploadsQuery request, CancellationToken cancellationToken)
        {
            // return (List<AppSampling>)await _queryRepository.GetListAsync();
            return (List<ReportDocuments>)await _fileuploadqueryRepository.GetAllAsync();
            // return (List<AppSampling>)await _samplingqueryRepository.GetsamplingByStatus(request.Id);
        }

    }
}