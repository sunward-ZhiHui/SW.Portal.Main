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
    public class CreateReportFileHandler : IRequestHandler<CreateReportFileQuery, long>
    {
        private readonly IReportFileUploadsQueryRepository _ReportFileQueryRepository;
        public CreateReportFileHandler(IReportFileUploadsQueryRepository ReportFileQueryRepository)
        {
            _ReportFileQueryRepository = ReportFileQueryRepository;
        }

        public async Task<long> Handle(CreateReportFileQuery request, CancellationToken cancellationToken)
        {
            string BaseDirectory = System.AppContext.BaseDirectory;
            var reportFolder = Path.Combine(BaseDirectory, "Reports");
            File.WriteAllBytes(Path.Combine(reportFolder, request.FileName + ".repx"), request.FileContent);
            var newlist = await _ReportFileQueryRepository.Insert(request);
            return newlist;

        }

    }
    public class EditReportFileHandler : IRequestHandler<EditReportFileQuery, long>
    {
        private readonly IReportFileUploadsQueryRepository _reportFileQueryRepository;
        public EditReportFileHandler(IReportFileUploadsQueryRepository reportFileQueryRepository)
        {
            _reportFileQueryRepository = reportFileQueryRepository;
        }

        public async Task<long> Handle(EditReportFileQuery request, CancellationToken cancellationToken)
        {
            
            
            if(request.FileName != null)
            {
                string BaseDirectory = System.AppContext.BaseDirectory;
                var reportFolder = Path.Combine(BaseDirectory, "Reports");
                File.Delete(Path.Combine(reportFolder, request.ActFileName + ".repx"));
            
              File.WriteAllBytes(Path.Combine(reportFolder, request.FileName + ".repx"), request.FileContent);
            }  
           
                request.FileName = request.ActFileName;
                
            

            var req = await _reportFileQueryRepository.Update(request);

            return req;
            
        }
    }
    public class DeleteReportDocumentHandler : IRequestHandler<DeleteReportdocumentQuery, long>
    {
        private readonly IReportFileUploadsQueryRepository _reportdocumentQueryRepository;

        public DeleteReportDocumentHandler(IReportFileUploadsQueryRepository reportdocumentQueryRepository)
        {
            _reportdocumentQueryRepository = reportdocumentQueryRepository;
        }

        public async Task<long> Handle(DeleteReportdocumentQuery request, CancellationToken cancellationToken)
        {
            string BaseDirectory = System.AppContext.BaseDirectory;
            var reportFolder = Path.Combine(BaseDirectory, "Reports");
            if (File.Exists(Path.Combine(reportFolder, request.ReportName + ".repx")))
            {
                File.Delete(Path.Combine(reportFolder, request.ReportName + ".repx"));
            }
           
             var req = await _reportdocumentQueryRepository.Delete(request.ReportDocumentID);
            return req;
        }
    }
}